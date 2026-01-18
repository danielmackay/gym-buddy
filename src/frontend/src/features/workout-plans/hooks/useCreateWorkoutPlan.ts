import { useMutation, useQueryClient } from "@tanstack/react-query";
import { workoutPlansApi } from "@/lib/api/workout-plans";
import type { CreateWorkoutPlanRequest } from "@/lib/types/workout-plan";
import { toast } from "sonner";

/**
 * Hook to create a new workout plan
 */
export function useCreateWorkoutPlan() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateWorkoutPlanRequest) =>
      workoutPlansApi.createWorkoutPlan(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["workout-plans"] });
      toast.success("Workout plan created successfully");
    },
    onError: (error: any) => {
      toast.error(error?.message || "Failed to create workout plan");
    },
  });
}
