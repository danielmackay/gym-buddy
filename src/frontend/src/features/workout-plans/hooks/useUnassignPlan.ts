import { useMutation, useQueryClient } from "@tanstack/react-query";
import { workoutPlansApi } from "@/lib/api/workout-plans";

interface UnassignPlanParams {
  clientId: string;
  workoutPlanId: string;
}

/**
 * Hook for unassigning a workout plan from a client
 */
export function useUnassignPlan() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ clientId, workoutPlanId }: UnassignPlanParams) =>
      workoutPlansApi.unassignPlanFromClient(clientId, workoutPlanId),
    onSuccess: () => {
      // Invalidate relevant queries to refetch updated data
      queryClient.invalidateQueries({ queryKey: ["workoutPlans"] });
      queryClient.invalidateQueries({ queryKey: ["clients"] });
      queryClient.invalidateQueries({ queryKey: ["users"] });
    },
  });
}
