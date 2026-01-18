"use client";

import { useQuery } from "@tanstack/react-query";
import { usersApi } from "@/lib/api/users";

/**
 * Hook to fetch all trainers
 * Uses TanStack Query for caching and automatic refetching
 */
export function useTrainers() {
  return useQuery({
    queryKey: ["trainers"],
    queryFn: () => usersApi.listTrainers(),
    staleTime: 60 * 1000, // Consider data fresh for 1 minute
  });
}
