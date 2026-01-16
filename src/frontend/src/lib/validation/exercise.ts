import { z } from "zod";
import { ExerciseType, MuscleGroup } from "../types/exercise";

// Validation schema for creating an exercise
export const createExerciseSchema = z.object({
  name: z
    .string()
    .min(1, "Name is required")
    .max(100, "Name must be 100 characters or less"),
  type: z.nativeEnum(ExerciseType, {
    message: "Invalid exercise type",
  }),
  muscleGroups: z
    .array(z.nativeEnum(MuscleGroup))
    .min(1, "At least one muscle group is required"),
  description: z
    .string()
    .max(500, "Description must be 500 characters or less")
    .optional(),
});

// Validation schema for updating an exercise
export const updateExerciseSchema = z.object({
  name: z
    .string()
    .min(1, "Name is required")
    .max(100, "Name must be 100 characters or less"),
  description: z
    .string()
    .max(500, "Description must be 500 characters or less")
    .optional(),
  muscleGroups: z
    .array(z.nativeEnum(MuscleGroup))
    .min(1, "At least one muscle group is required"),
});

export type CreateExerciseFormData = z.infer<typeof createExerciseSchema>;
export type UpdateExerciseFormData = z.infer<typeof updateExerciseSchema>;
