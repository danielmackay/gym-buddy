"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";
import { usersApi } from "@/lib/api/users";
import type { CreateTrainerRequest } from "@/lib/types/user";

/**
 * Hook to create a new trainer
 * Automatically invalidates trainers query after successful creation
 */
export function useCreateTrainer() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateTrainerRequest) => usersApi.createTrainer(data),
    onSuccess: () => {
      // Invalidate trainers query to trigger refetch
      queryClient.invalidateQueries({ queryKey: ["trainers"] });
    },
  });
}
