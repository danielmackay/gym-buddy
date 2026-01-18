// Workout Plan types matching backend Domain models
import type { ExerciseType } from "./exercise";

export enum WeightUnit {
  Kilograms = 1,
  Pounds = 2,
}

export interface Weight {
  value: number;
  unit: WeightUnit;
}

export interface Duration {
  seconds: number;
}

export interface PlannedExercise {
  id: string; // PlannedExerciseId.Value
  exerciseId: string;
  exerciseName: string;
  exerciseType: ExerciseType;
  sets: number;
  reps?: number; // Required for RepsAndWeight exercises
  weight?: Weight; // Optional for RepsAndWeight exercises
  duration?: Duration; // Required for TimeBased exercises
  order: number;
}

export interface WorkoutPlan {
  id: string; // WorkoutPlanId.Value
  name: string;
  description?: string;
  trainerId: string;
  exercises: PlannedExercise[];
}

// Request DTOs
export interface CreateWorkoutPlanRequest {
  name: string;
  description?: string;
}

export interface AddExerciseToPlanRequest {
  exerciseId: string;
  sets: number;
  reps?: number;
  weight?: Weight;
  duration?: Duration;
}

export interface UpdatePlannedExerciseRequest {
  sets: number;
  reps?: number;
  weight?: Weight;
  duration?: Duration;
}

export interface ReorderExercisesRequest {
  exerciseIds: string[]; // New order of PlannedExercise IDs
}

export interface AssignPlanToClientRequest {
  clientId: string;
}

// Response DTOs
export interface CreateWorkoutPlanResponse {
  id: string;
}
