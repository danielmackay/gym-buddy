import { useMutation, useQueryClient } from "@tanstack/react-query";
import { workoutPlansApi } from "@/lib/api/workout-plans";
import { toast } from "sonner";

/**
 * Hook to remove an exercise from a workout plan
 */
export function useRemoveExercise() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      workoutPlanId,
      exerciseId,
    }: {
      workoutPlanId: string;
      exerciseId: string;
    }) => workoutPlansApi.removeExercise(workoutPlanId, exerciseId),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["workout-plans"] });
      queryClient.invalidateQueries({
        queryKey: ["workout-plans", variables.workoutPlanId],
      });
      toast.success("Exercise removed from workout plan");
    },
    onError: (error: any) => {
      toast.error(error?.message || "Failed to remove exercise");
    },
  });
}
