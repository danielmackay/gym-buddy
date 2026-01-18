import { useMutation, useQueryClient } from "@tanstack/react-query";
import { workoutPlansApi } from "@/lib/api/workout-plans";

interface AssignPlanParams {
  clientId: string;
  workoutPlanId: string;
}

/**
 * Hook for assigning a workout plan to a client
 */
export function useAssignPlan() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ clientId, workoutPlanId }: AssignPlanParams) =>
      workoutPlansApi.assignPlanToClient(clientId, workoutPlanId),
    onSuccess: () => {
      // Invalidate relevant queries to refetch updated data
      queryClient.invalidateQueries({ queryKey: ["workoutPlans"] });
      queryClient.invalidateQueries({ queryKey: ["clients"] });
      queryClient.invalidateQueries({ queryKey: ["users"] });
    },
  });
}
