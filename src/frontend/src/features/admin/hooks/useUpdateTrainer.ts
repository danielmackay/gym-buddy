"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { usersApi } from "@/lib/api/users";
import type { UpdateUserRequest } from "@/lib/types/user";

/**
 * Hook to update an existing user
 * Automatically invalidates relevant queries after successful update
 */
export function useUpdateTrainer() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateUserRequest }) =>
      usersApi.updateUser(id, data),
    onSuccess: (updatedUser) => {
      // Invalidate trainers list
      queryClient.invalidateQueries({ queryKey: ["trainers"] });
      // Invalidate specific user query
      queryClient.invalidateQueries({ queryKey: ["user", updatedUser.id] });
    },
  });
}
