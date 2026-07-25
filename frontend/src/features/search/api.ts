import { apiClient } from "../../services/apiClient";
import type { SearchResponse, SearchResultType } from "./types";

export interface SearchParameters {
  query: string;
  type?: SearchResultType;
  projectId?: string;
  page?: number;
  pageSize?: number;
}

export const searchApi = {
  search: (parameters: SearchParameters, signal?: AbortSignal) => {
    const query = new URLSearchParams({
      query: parameters.query,
      page: String(parameters.page ?? 1),
      pageSize: String(parameters.pageSize ?? 5),
    });
    if (parameters.type) query.set("type", parameters.type);
    if (parameters.projectId) query.set("projectId", parameters.projectId);
    return apiClient.get<SearchResponse>(`/search?${query}`, { signal });
  },
};
