import { z } from "zod";
import { WeightUnit, ExerciseType } from "@/lib/types/workout-plan";

// Weight validation (matches backend: value + unit)
const weightSchema = z.object({
  value: z.number().nonnegative("Weight cannot be negative"),
  unit: z.nativeEnum(WeightUnit),
}).refine(
  (data) => data.unit === WeightUnit.Kilograms || data.unit === WeightUnit.Pounds,
  { message: "Weight unit must be valid (1 = Kilograms, 2 = Pounds)", path: ["unit"] }
);

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
// Note: This validates at form level. Backend will enforce stricter rules per exercise type.
export const addExerciseToPlanSchema = z.object({
  type: z.nativeEnum(ExerciseType),
  exerciseId: z.string().min(1, "Exercise is required"),
  sets: z.number().int().min(1, "Sets must be at least 1"),
  reps: z.number().int().min(1, "Reps must be at least 1").optional(),
  weight: weightSchema.optional(),
  duration: durationSchema.optional(),
}).refine(
  (data) => {
    // For RepsAndWeight, reps is required
    if (data.type === ExerciseType.RepsAndWeight && !data.reps) {
      return false;
    }
    // For TimeBased, duration is required
    if (data.type === ExerciseType.TimeBased && !data.duration) {
      return false;
    }
    return true;
  },
  {
    message: "RepsAndWeight exercises require reps, TimeBased exercises require duration",
  }
);

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
