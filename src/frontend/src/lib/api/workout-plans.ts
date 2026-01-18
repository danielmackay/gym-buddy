import { apiClient } from "./client";
import type {
  WorkoutPlan,
  CreateWorkoutPlanRequest,
  CreateWorkoutPlanResponse,
  AddExerciseToPlanRequest,
  ReorderExercisesRequest,
} from "../types/workout-plan";

export const workoutPlansApi = {
  /**
   * Create a new workout plan
   */
  createWorkoutPlan: (data: CreateWorkoutPlanRequest) =>
    apiClient.request<CreateWorkoutPlanResponse>("/workout-plans", {
      method: "POST",
      body: JSON.stringify(data),
    }),

  /**
   * List all workout plans for the authenticated trainer
   */
  listWorkoutPlans: () => apiClient.request<WorkoutPlan[]>("/workout-plans"),

  /**
   * Get a single workout plan by ID
   */
  getWorkoutPlan: (id: string) =>
    apiClient.request<WorkoutPlan>(`/workout-plans/${id}`),

  /**
   * Update a workout plan's name and description
   */
  updateWorkoutPlan: (id: string, data: CreateWorkoutPlanRequest) =>
    apiClient.request<void>(`/workout-plans/${id}`, {
      method: "PUT",
      body: JSON.stringify({ id, ...data }),
    }),

  /**
   * Add an exercise to a workout plan
   */
  addExercise: (workoutPlanId: string, data: AddExerciseToPlanRequest) =>
    apiClient.request<void>(`/workout-plans/${workoutPlanId}/exercises`, {
      method: "POST",
      body: JSON.stringify({ workoutPlanId, ...data }),
    }),

  /**
   * Remove an exercise from a workout plan
   */
  removeExercise: (workoutPlanId: string, exerciseId: string) =>
    apiClient.request<void>(
      `/workout-plans/${workoutPlanId}/exercises/${exerciseId}`,
      {
        method: "DELETE",
      }
    ),

  /**
   * Reorder exercises in a workout plan
   */
  reorderExercises: (workoutPlanId: string, data: ReorderExercisesRequest) =>
    apiClient.request<void>(`/workout-plans/${workoutPlanId}/exercises/reorder`, {
      method: "PUT",
      body: JSON.stringify({ workoutPlanId, ...data }),
    }),

  /**
   * Assign a workout plan to a client
   * TODO: Implement when backend endpoint is created in Phase 7
   */
  assignPlanToClient: (workoutPlanId: string, clientId: string) =>
    apiClient.request<void>(`/workout-plans/${workoutPlanId}/clients/${clientId}`, {
      method: "POST",
    }),

  /**
   * Unassign a workout plan from a client
   * TODO: Implement when backend endpoint is created in Phase 7
   */
  unassignPlanFromClient: (workoutPlanId: string, clientId: string) =>
    apiClient.request<void>(`/workout-plans/${workoutPlanId}/clients/${clientId}`, {
      method: "DELETE",
    }),
};
