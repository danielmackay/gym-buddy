import { z } from "zod";

// Validation schema for creating a trainer
export const createTrainerSchema = z.object({
  name: z
    .string()
    .min(1, "Name is required")
    .max(100, "Name must be 100 characters or less"),
  email: z
    .string()
    .email("Invalid email format")
    .max(256, "Email must be 256 characters or less"),
});

// Validation schema for creating a client
export const createClientSchema = z.object({
  name: z
    .string()
    .min(1, "Name is required")
    .max(100, "Name must be 100 characters or less"),
  email: z
    .string()
    .email("Invalid email format")
    .max(256, "Email must be 256 characters or less"),
  trainerId: z.string().uuid("Trainer ID must be a valid UUID"),
});

// Validation schema for updating a user
export const updateUserSchema = z.object({
  name: z
    .string()
    .min(1, "Name is required")
    .max(100, "Name must be 100 characters or less"),
  email: z
    .string()
    .email("Invalid email format")
    .max(256, "Email must be 256 characters or less"),
});

export type CreateTrainerFormData = z.infer<typeof createTrainerSchema>;
export type CreateClientFormData = z.infer<typeof createClientSchema>;
export type UpdateUserFormData = z.infer<typeof updateUserSchema>;
