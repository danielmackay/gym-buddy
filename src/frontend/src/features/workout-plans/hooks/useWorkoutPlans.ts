import { useQuery } from "@tanstack/react-query";
import { workoutPlansApi } from "@/lib/api/workout-plans";

/**
 * Hook to fetch all workout plans for the authenticated trainer
 */
export function useWorkoutPlans() {
  return useQuery({
    queryKey: ["workout-plans"],
    queryFn: () => workoutPlansApi.listWorkoutPlans(),
  });
}
