import { apiClient } from "./client";
import type {
  User,
  CreateTrainerRequest,
  CreateTrainerResponse,
  CreateClientRequest,
  CreateClientResponse,
  UpdateUserRequest,
} from "../types/user";

/**
 * Users API service
 * Handles all user-related API calls (trainers, clients, and user management)
 */
export const usersApi = {
  /**
   * List all trainers
   */
  async listTrainers(): Promise<User[]> {
    return apiClient.get<User[]>("/users/trainers");
  },

  /**
   * Create a new trainer
   */
  async createTrainer(
    data: CreateTrainerRequest,
  ): Promise<CreateTrainerResponse> {
    return apiClient.post<CreateTrainerResponse>("/users/trainers", data);
  },

  /**
   * List clients for a specific trainer
   */
  async listClients(trainerId: string): Promise<User[]> {
    return apiClient.get<User[]>(`/users/clients?trainerId=${trainerId}`);
  },

  /**
   * Create a new client
   */
  async createClient(data: CreateClientRequest): Promise<CreateClientResponse> {
    return apiClient.post<CreateClientResponse>("/users/clients", data);
  },

  /**
   * Get a single user by ID
   */
  async getUser(id: string): Promise<User> {
    return apiClient.get<User>(`/users/${id}`);
  },

  /**
   * Update an existing user
   */
  async updateUser(id: string, data: UpdateUserRequest): Promise<User> {
    return apiClient.put<User>(`/users/${id}`, data);
  },
};
