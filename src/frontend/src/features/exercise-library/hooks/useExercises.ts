import { useQuery } from "@tanstack/react-query";
import { exercisesApi } from "@/lib/api/exercises";
import type { ExerciseType, MuscleGroup } from "@/lib/types/exercise";

interface UseExercisesParams {
  muscleGroup?: MuscleGroup;
  type?: ExerciseType;
}

export function useExercises(params?: UseExercisesParams) {
  return useQuery({
    queryKey: ["exercises", params],
    queryFn: () => exercisesApi.listExercises(params),
  });
}
