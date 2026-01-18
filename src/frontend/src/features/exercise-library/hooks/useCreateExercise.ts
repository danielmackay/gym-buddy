import { useMutation, useQueryClient } from "@tanstack/react-query";
import { exercisesApi } from "@/lib/api/exercises";
import type { CreateExerciseRequest } from "@/lib/types/exercise";

export function useCreateExercise() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateExerciseRequest) =>
      exercisesApi.createExercise(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["exercises"] });
    },
  });
}
