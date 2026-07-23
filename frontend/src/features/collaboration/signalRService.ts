import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from "@microsoft/signalr";
import { tokenStore } from "../../services/tokenStore";
import { useCollaborationStore } from "./collaborationStore";
import type { CodeOperation, CursorPosition, FileChangedMessage, PresenceUpdate, ResyncRequiredMessage } from "./types";

type Handler<T> = (payload: T) => void;
const HUB_URL = import.meta.env.VITE_SIGNALR_URL ?? "/hubs/collaboration";

class SignalRService {
  private connection?: HubConnection;
  private projectId?: string;
  private fileId?: string;
  private heartbeat?: number;
  private operationQueues = new Map<string, Promise<void>>();
  private operationListeners = new Set<Handler<CodeOperation>>();
  private changedListeners = new Set<Handler<FileChangedMessage>>();
  private resyncListeners = new Set<Handler<ResyncRequiredMessage>>();

  async connect() {
    if (this.connection?.state === HubConnectionState.Connected || this.connection?.state === HubConnectionState.Connecting) return;
    this.connection = new HubConnectionBuilder().withUrl(HUB_URL, { accessTokenFactory: () => tokenStore.get() ?? "" })
      .withAutomaticReconnect([0, 1000, 2000, 5000, 10000, 30000])
      .configureLogging(import.meta.env.DEV ? LogLevel.Information : LogLevel.Warning).build();
    this.registerHandlers(this.connection);
    try { await this.connection.start(); useCollaborationStore.getState().setConnectionState("connected"); await this.rejoin(); this.startHeartbeat(); }
    catch (error) { useCollaborationStore.getState().setConnectionState("failed"); throw error; }
  }

  async disconnect() {
    this.stopHeartbeat(); const connection = this.connection; this.connection = undefined; this.projectId = undefined; this.fileId = undefined;
    useCollaborationStore.getState().clearFileState(); if (connection) await connection.stop(); useCollaborationStore.getState().setConnectionState("disconnected");
  }
  async joinProject(projectId: string) { this.projectId = projectId; await this.ensureConnected(); await this.connection!.invoke("JoinProject", projectId); }
  async leaveProject(projectId: string) { if (this.isConnected()) await this.connection!.invoke("LeaveProject", projectId); if (this.projectId === projectId) this.projectId = undefined; }
  async joinFile(fileId: string, version: number) {
    if (this.fileId && this.fileId !== fileId && this.isConnected()) await this.connection!.invoke("LeaveFile", this.fileId);
    this.fileId = fileId; useCollaborationStore.getState().clearFileState(); useCollaborationStore.getState().setLiveVersion(fileId, version);
    await this.ensureConnected(); await this.connection!.invoke("JoinFile", fileId);
  }
  async leaveFile(fileId: string) { if (this.isConnected()) await this.connection!.invoke("LeaveFile", fileId); if (this.fileId === fileId) this.fileId = undefined; useCollaborationStore.getState().clearFileState(); }

  sendOperation(operation: Omit<CodeOperation, "userId" | "clientVersion" | "baseVersion" | "timestamp">) {
    const previous = this.operationQueues.get(operation.fileId) ?? Promise.resolve();
    const next = previous.then(async () => {
      if (!this.isConnected()) return;
      const baseVersion = useCollaborationStore.getState().liveVersions[operation.fileId] ?? 0;
      const serverVersion = await this.connection!.invoke<number>("SendCodeOperation", { ...operation, userId: "00000000-0000-0000-0000-000000000000", clientVersion: baseVersion + 1, baseVersion, timestamp: new Date().toISOString() });
      if (serverVersion >= 0) useCollaborationStore.getState().setLiveVersion(operation.fileId, serverVersion);
    }).catch((error) => console.error("Code operation failed", error));
    this.operationQueues.set(operation.fileId, next);
  }
  updateCursor(position: CursorPosition) { if (this.isConnected()) void this.connection!.send("UpdateCursor", position).catch(() => undefined); }
  startTyping(fileId: string) { if (this.isConnected()) void this.connection!.send("StartTyping", fileId).catch(() => undefined); }
  stopTyping(fileId: string) { if (this.isConnected()) void this.connection!.send("StopTyping", fileId).catch(() => undefined); }
  notifyFileChanged(fileId: string, version: number, concurrencyToken: string) {
    useCollaborationStore.getState().setLiveVersion(fileId, version);
    if (this.isConnected()) void this.connection!.send("NotifyFileChanged", fileId, version, concurrencyToken).catch(() => undefined);
  }
  onOperation(handler: Handler<CodeOperation>) { this.operationListeners.add(handler); return () => { this.operationListeners.delete(handler); }; }
  onFileChanged(handler: Handler<FileChangedMessage>) { this.changedListeners.add(handler); return () => { this.changedListeners.delete(handler); }; }
  onResync(handler: Handler<ResyncRequiredMessage>) { this.resyncListeners.add(handler); return () => { this.resyncListeners.delete(handler); }; }

  private registerHandlers(connection: HubConnection) {
    connection.onreconnecting(() => { useCollaborationStore.getState().setConnectionState("reconnecting"); this.stopHeartbeat(); });
    connection.onreconnected(async () => { useCollaborationStore.getState().setConnectionState("connected"); await this.rejoin(); this.startHeartbeat(); });
    connection.onclose(() => { useCollaborationStore.getState().setConnectionState("disconnected"); this.stopHeartbeat(); });
    connection.on("PresenceUpdated", (message: PresenceUpdate) => useCollaborationStore.getState().setPresence(message.users));
    connection.on("CursorUpdated", (userId: string, position: CursorPosition) => useCollaborationStore.getState().setCursor(userId, position));
    connection.on("TypingStarted", (_fileId: string, userId: string) => useCollaborationStore.getState().setTyping(userId, true));
    connection.on("TypingStopped", (_fileId: string, userId: string) => useCollaborationStore.getState().setTyping(userId, false));
    connection.on("CodeOperationReceived", (operation: CodeOperation) => { useCollaborationStore.getState().setLiveVersion(operation.fileId, operation.clientVersion); this.operationListeners.forEach((handler) => handler(operation)); });
    connection.on("FileChanged", (message: FileChangedMessage) => { useCollaborationStore.getState().setLiveVersion(message.fileId, message.versionNumber); this.changedListeners.forEach((handler) => handler(message)); });
    connection.on("ResyncRequired", (message: ResyncRequiredMessage) => { useCollaborationStore.getState().setLiveVersion(message.fileId, message.serverVersion); this.resyncListeners.forEach((handler) => handler(message)); });
  }
  private async ensureConnected() { if (!this.isConnected()) await this.connect(); }
  private isConnected() { return this.connection?.state === HubConnectionState.Connected; }
  private async rejoin() { if (!this.isConnected()) return; if (this.projectId) await this.connection!.invoke("JoinProject", this.projectId); if (this.fileId) await this.connection!.invoke("JoinFile", this.fileId); }
  private startHeartbeat() { this.stopHeartbeat(); this.heartbeat = window.setInterval(() => { if (this.isConnected()) void this.connection!.send("Heartbeat").catch(() => undefined); }, 20_000); }
  private stopHeartbeat() { if (this.heartbeat) window.clearInterval(this.heartbeat); this.heartbeat = undefined; }
}
export const signalRService = new SignalRService();
