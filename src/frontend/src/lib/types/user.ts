// User types matching backend Domain models

export enum UserRole {
  Admin = 1,
  Trainer = 2,
  Client = 3,
}

export interface User {
  id: string; // UserId.Value (Guid as string)
  name: string;
  email: string;
  roles: UserRole[];
  trainerId?: string; // UserId.Value (Guid as string), optional for trainers
  assignedWorkoutPlanIds: string[]; // WorkoutPlanId.Value[]
}

// Request DTOs
export interface CreateTrainerRequest {
  name: string;
  email: string;
}

export interface CreateClientRequest {
  name: string;
  email: string;
  trainerId: string; // Required for clients
}

export interface UpdateUserRequest {
  name: string;
  email: string;
}

// Response DTOs
export interface CreateTrainerResponse {
  id: string;
}

export interface CreateClientResponse {
  id: string;
}
