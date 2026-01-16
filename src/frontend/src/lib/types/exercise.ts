// Exercise types matching backend Domain models

export enum ExerciseType {
  RepsAndWeight = 1,
  TimeBased = 2,
}

export enum MuscleGroup {
  Chest = 1,
  Back = 2,
  Shoulders = 3,
  Biceps = 4,
  Triceps = 5,
  Forearms = 6,
  Quadriceps = 7,
  Hamstrings = 8,
  Glutes = 9,
  Calves = 10,
  Abs = 11,
  Obliques = 12,
}

export interface Exercise {
  id: string; // ExerciseId.Value
  name: string;
  description?: string;
  type: ExerciseType;
  muscleGroups: MuscleGroup[];
}

// Request DTOs
export interface CreateExerciseRequest {
  name: string;
  type: ExerciseType;
  muscleGroups: MuscleGroup[];
  description?: string;
}

export interface UpdateExerciseRequest {
  name: string;
  description?: string;
  muscleGroups: MuscleGroup[];
}

// Response DTOs
export interface CreateExerciseResponse {
  id: string;
}
