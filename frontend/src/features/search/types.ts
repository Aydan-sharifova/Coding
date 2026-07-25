export type SearchResultType = "Project" | "File" | "User" | "Task";
export interface SearchResult {
  type: SearchResultType;
  id: string;
  title: string;
  subtitle: string;
  projectId?: string;
  matchedText: string;
  navigationUrl: string;
  rank: number;
}
export interface SearchGroup { type: SearchResultType; items: SearchResult[]; hasMore: boolean; }
export interface SearchResponse { query: string; page: number; pageSize: number; groups: SearchGroup[]; }
