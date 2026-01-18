"use client";

import { useQuery } from "@tanstack/react-query";
import { usersApi } from "@/lib/api/users";

/**
 * Hook to fetch a single client by ID
 * Uses TanStack Query for caching and automatic refetching
 */
export function useClient(clientId: string) {
  return useQuery({
    queryKey: ["client", clientId],
    queryFn: () => usersApi.getUser(clientId),
    staleTime: 60 * 1000, // Consider data fresh for 1 minute
    enabled: !!clientId, // Only fetch if clientId is provided
  });
}
