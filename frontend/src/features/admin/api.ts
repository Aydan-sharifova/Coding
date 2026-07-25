import { apiClient } from "../../services/apiClient";
export interface Page<T>{items:T[];total:number;page:number;pageSize:number}
export interface AdminUser{id:string;displayName:string;userName:string;email:string;isSuspended:boolean;roles:string[];createdAt:string;lastSeen:string}
export interface AdminUserDetails extends AdminUser{firstName:string;lastName:string;bio?:string;avatarUrl?:string;suspensionReason?:string;projectCount:number}
export interface AdminProject{id:string;name:string;ownerName:string;isPublic:boolean;memberCount:number;taskCount:number;createdAt:string}
export interface PlatformStats{totalUsers:number;activeUsers30Days:number;suspendedUsers:number;totalProjects:number;projects30Days:number;activity30Days:number}
export const adminApi={
 stats:()=>apiClient.get<PlatformStats>("/admin/statistics"),
 users:(search:string,page:number)=>apiClient.get<Page<AdminUser>>(`/admin/users?search=${encodeURIComponent(search)}&page=${page}&pageSize=20`),
 user:(id:string)=>apiClient.get<AdminUserDetails>(`/admin/users/${id}`),
 suspension:(id:string,suspended:boolean,reason?:string)=>apiClient.put<void>(`/admin/users/${id}/suspension`,{suspended,reason}),
 role:(id:string,role:string,enabled:boolean)=>apiClient.put<void>(`/admin/users/${id}/roles/${role}`,{enabled}),
 projects:(search:string,page:number)=>apiClient.get<Page<AdminProject>>(`/admin/projects?search=${encodeURIComponent(search)}&page=${page}&pageSize=20`),
 deleteProject:(id:string,reason:string)=>apiClient.delete<void>(`/admin/projects/${id}`,{reason}),
};
