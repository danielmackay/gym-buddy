import { useQuery } from "@tanstack/react-query";
import { exercisesApi } from "@/lib/api/exercises";

export function useExercise(id: string) {
  return useQuery({
    queryKey: ["exercise", id],
    queryFn: () => exercisesApi.getExercise(id),
    enabled: !!id,
  });
}
