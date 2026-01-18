import { useMutation, useQueryClient } from "@tanstack/react-query";
import { workoutPlansApi } from "@/lib/api/workout-plans";

interface UnassignPlanParams {
  workoutPlanId: string;
  clientId: string;
}

/**
 * Hook for unassigning a workout plan from a client
 */
export function useUnassignPlan() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ workoutPlanId, clientId }: UnassignPlanParams) =>
      workoutPlansApi.unassignPlanFromClient(workoutPlanId, clientId),
    onSuccess: () => {
      // Invalidate relevant queries to refetch updated data
      queryClient.invalidateQueries({ queryKey: ["workoutPlans"] });
      queryClient.invalidateQueries({ queryKey: ["clients"] });
      queryClient.invalidateQueries({ queryKey: ["users"] });
    },
  });
}
