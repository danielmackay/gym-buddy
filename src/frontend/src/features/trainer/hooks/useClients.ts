"use client";

import { useQuery } from "@tanstack/react-query";
import { usersApi } from "@/lib/api/users";

/**
 * Hook to fetch clients for a specific trainer
 * Uses TanStack Query for caching and automatic refetching
 */
export function useClients(trainerId: string) {
  return useQuery({
    queryKey: ["clients", trainerId],
    queryFn: () => usersApi.listClients(trainerId),
    staleTime: 60 * 1000, // Consider data fresh for 1 minute
    enabled: !!trainerId, // Only fetch if trainerId is provided
  });
}
