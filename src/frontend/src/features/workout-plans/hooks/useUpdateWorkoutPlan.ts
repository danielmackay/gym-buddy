import { useMutation, useQueryClient } from "@tanstack/react-query";
import { workoutPlansApi } from "@/lib/api/workout-plans";
import type { CreateWorkoutPlanRequest } from "@/lib/types/workout-plan";
import { toast } from "sonner";

/**
 * Hook to update an existing workout plan
 */
export function useUpdateWorkoutPlan() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: CreateWorkoutPlanRequest }) =>
      workoutPlansApi.updateWorkoutPlan(id, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["workout-plans"] });
      queryClient.invalidateQueries({ queryKey: ["workout-plans", variables.id] });
      toast.success("Workout plan updated successfully");
    },
    onError: (error: any) => {
      toast.error(error?.message || "Failed to update workout plan");
    },
  });
}
