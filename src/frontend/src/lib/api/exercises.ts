import { apiClient } from "./client";
import type {
  Exercise,
  CreateExerciseRequest,
  CreateExerciseResponse,
  UpdateExerciseRequest,
  ExerciseType,
  MuscleGroup,
} from "../types/exercise";

/**
 * Exercises API service
 * Handles all exercise-related API calls (CRUD operations for exercise library)
 */
export const exercisesApi = {
  /**
   * List all exercises with optional filtering
   */
  async listExercises(params?: {
    muscleGroup?: MuscleGroup;
    type?: ExerciseType;
  }): Promise<Exercise[]> {
    const searchParams = new URLSearchParams();
    if (params?.muscleGroup !== undefined) {
      searchParams.append("muscleGroup", params.muscleGroup.toString());
    }
    if (params?.type !== undefined) {
      searchParams.append("type", params.type.toString());
    }

    const queryString = searchParams.toString();
    const url = queryString ? `/exercises?${queryString}` : "/exercises";

    return apiClient.get<Exercise[]>(url);
  },

  /**
   * Create a new exercise
   */
  async createExercise(
    data: CreateExerciseRequest
  ): Promise<CreateExerciseResponse> {
    return apiClient.post<CreateExerciseResponse>("/exercises", data);
  },

  /**
   * Get a single exercise by ID
   */
  async getExercise(id: string): Promise<Exercise> {
    return apiClient.get<Exercise>(`/exercises/${id}`);
  },

  /**
   * Update an existing exercise
   */
  async updateExercise(
    id: string,
    data: UpdateExerciseRequest
  ): Promise<void> {
    return apiClient.put<void>(`/exercises/${id}`, data);
  },
};
