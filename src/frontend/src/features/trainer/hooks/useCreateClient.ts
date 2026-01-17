"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { usersApi } from "@/lib/api/users";
import type { CreateClientRequest } from "@/lib/types/user";

/**
 * Hook to create a new client
 * Automatically invalidates clients query after successful creation
 */
export function useCreateClient(trainerId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateClientRequest) => usersApi.createClient(data),
    onSuccess: () => {
      // Invalidate clients query to trigger refetch
      queryClient.invalidateQueries({ queryKey: ["clients", trainerId] });
    },
  });
}
