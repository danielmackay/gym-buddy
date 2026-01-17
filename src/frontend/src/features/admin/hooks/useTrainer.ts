"use client";

import { useQuery } from "@tanstack/react-query";
import { usersApi } from "@/lib/api/users";

/**
 * Hook to fetch a single user by ID
 * Uses TanStack Query for caching
 */
export function useTrainer(id: string | undefined) {
  return useQuery({
    queryKey: ["user", id],
    queryFn: () => usersApi.getUser(id!),
    enabled: !!id, // Only fetch if id is provided
    staleTime: 60 * 1000, // Consider data fresh for 1 minute
  });
}
