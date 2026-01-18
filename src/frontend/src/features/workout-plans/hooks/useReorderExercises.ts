import { useMutation, useQueryClient } from "@tanstack/react-query";
import { workoutPlansApi } from "@/lib/api/workout-plans";
import type { ReorderExercisesRequest } from "@/lib/types/workout-plan";
import { toast } from "sonner";

/**
 * Hook to reorder exercises in a workout plan
 */
export function useReorderExercises() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      workoutPlanId,
      data,
    }: {
      workoutPlanId: string;
      data: ReorderExercisesRequest;
    }) => workoutPlansApi.reorderExercises(workoutPlanId, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["workout-plans"] });
      queryClient.invalidateQueries({
        queryKey: ["workout-plans", variables.workoutPlanId],
      });
      toast.success("Exercises reordered successfully");
    },
    onError: (error: any) => {
      toast.error(error?.message || "Failed to reorder exercises");
    },
  });
}
