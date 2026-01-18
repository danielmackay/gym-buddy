import { useMutation, useQueryClient } from "@tanstack/react-query";
import { workoutPlansApi } from "@/lib/api/workout-plans";
import type { AddExerciseToPlanRequest } from "@/lib/types/workout-plan";
import { toast } from "sonner";

/**
 * Hook to add an exercise to a workout plan
 */
export function useAddExercise() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      workoutPlanId,
      data,
    }: {
      workoutPlanId: string;
      data: AddExerciseToPlanRequest;
    }) => workoutPlansApi.addExercise(workoutPlanId, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["workout-plans"] });
      queryClient.invalidateQueries({
        queryKey: ["workout-plans", variables.workoutPlanId],
      });
      toast.success("Exercise added to workout plan");
    },
    onError: (error: any) => {
      toast.error(error?.message || "Failed to add exercise");
    },
  });
}
