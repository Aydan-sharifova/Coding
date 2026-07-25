import { useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { searchApi } from "./api";
import type { SearchResultType } from "./types";

export function useGlobalSearch(query: string, type?: SearchResultType, projectId?: string) {
  const [debounced, setDebounced] = useState("");
  useEffect(() => {
    const timer = window.setTimeout(() => setDebounced(query.trim()), 300);
    return () => window.clearTimeout(timer);
  }, [query]);
  return useQuery({
    queryKey: ["global-search", debounced, type, projectId],
    queryFn: ({ signal }) => searchApi.search({ query: debounced, type, projectId }, signal),
    enabled: debounced.length >= 2,
    staleTime: 15_000,
  });
}
