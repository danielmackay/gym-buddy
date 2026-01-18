import { useMutation, useQueryClient } from "@tanstack/react-query";
import { exercisesApi } from "@/lib/api/exercises";
import type { UpdateExerciseRequest } from "@/lib/types/exercise";

interface UpdateExerciseParams {
  id: string;
  data: UpdateExerciseRequest;
}

export function useUpdateExercise() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: UpdateExerciseParams) =>
      exercisesApi.updateExercise(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["exercises"] });
      queryClient.invalidateQueries({
        queryKey: ["exercise", variables.id],
      });
    },
  });
}
