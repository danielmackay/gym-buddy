import { z } from "zod";
import { WeightUnit } from "@/lib/types/workout-plan";

// Weight validation (matches backend: value + unit)
const weightSchema = z.object({
  value: z.number().positive("Weight must be positive"),
  unit: z.nativeEnum(WeightUnit),
});

// Duration validation
const durationSchema = z.object({
  seconds: z.number().int().positive("Duration must be positive"),
});

// Validation schema for creating a workout plan
export const createWorkoutPlanSchema = z.object({
  name: z
    .string()
    .min(1, "Name is required")
    .max(100, "Name must be 100 characters or less"),
  description: z
    .string()
    .max(500, "Description must be 500 characters or less")
    .optional(),
});

// Validation schema for adding an exercise to a plan
export const addExerciseToPlanSchema = z.object({
  exerciseId: z.string().min(1, "Exercise is required"),
  sets: z.number().int().min(1, "Sets must be at least 1"),
  reps: z.number().int().min(1, "Reps must be at least 1").optional(),
  weight: weightSchema.optional(),
  duration: durationSchema.optional(),
});

// Validation schema for reordering exercises in a plan
export const reorderExercisesSchema = z.object({
  exerciseIds: z
    .array(z.string().uuid())
    .min(1, "At least one exercise ID is required"),
});

export type CreateWorkoutPlanFormData = z.infer<
  typeof createWorkoutPlanSchema
>;
export type AddExerciseToPlanFormData = z.infer<
  typeof addExerciseToPlanSchema
>;
export type ReorderExercisesFormData = z.infer<
  typeof reorderExercisesSchema
>;
