"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { usersApi } from "@/lib/api/users";
import type { UpdateUserRequest } from "@/lib/types/user";

/**
 * Hook to update an existing client
 * Automatically invalidates client queries after successful update
 */
export function useUpdateClient(clientId: string, trainerId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: UpdateUserRequest) =>
      usersApi.updateUser(clientId, data),
    onSuccess: () => {
      // Invalidate both the specific client and the clients list
      queryClient.invalidateQueries({ queryKey: ["client", clientId] });
      queryClient.invalidateQueries({ queryKey: ["clients", trainerId] });
    },
  });
}
