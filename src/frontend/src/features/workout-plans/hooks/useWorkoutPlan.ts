import { useQuery } from "@tanstack/react-query";
import { workoutPlansApi } from "@/lib/api/workout-plans";

/**
 * Hook to fetch a single workout plan by ID
 */
export function useWorkoutPlan(id: string) {
  return useQuery({
    queryKey: ["workout-plans", id],
    queryFn: () => workoutPlansApi.getWorkoutPlan(id),
    enabled: !!id, // Only fetch if ID is provided
  });
}
