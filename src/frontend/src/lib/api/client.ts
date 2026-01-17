import { ApiError, type ApiErrorResponse } from "../types/errors";
import { API_URL } from "./config";

/**
 * Base API client for making HTTP requests
 * Handles error responses from FluentValidation and Domain (ErrorOr) errors
 */
export class ApiClient {
  private baseUrl: string;

  constructor(baseUrl: string = API_URL) {
    this.baseUrl = baseUrl;
  }

  async request<T>(
    endpoint: string,
    options?: RequestInit,
  ): Promise<T> {
    const url = `${this.baseUrl}${endpoint}`;

    const response = await fetch(url, {
      ...options,
      headers: {
        "Content-Type": options?.method === "GET" ? "" : "application/json",
        ...options?.headers,
      },
    });

    // Handle successful responses
    if (response.ok) {
      // For 204 No Content, return empty object
      if (response.status === 204) {
        return {} as T;
      }

      return response.json();
    }

    // Handle error responses
    let errorData: ApiErrorResponse = {};

    try {
      errorData = await response.json();
    } catch {
      // If response body is not JSON, use status text
      errorData = {
        title: response.statusText || "Unknown error",
        status: response.status,
      };
    }

    throw new ApiError(errorData, response.status);
  }

  async get<T>(endpoint: string): Promise<T> {
    return this.request<T>(endpoint, { method: "GET" });
  }

  async post<T>(endpoint: string, data?: unknown): Promise<T> {
    return this.request<T>(endpoint, {
      method: "POST",
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  async put<T>(endpoint: string, data?: unknown): Promise<T> {
    return this.request<T>(endpoint, {
      method: "PUT",
      body: data ? JSON.stringify(data) : undefined,
    });
  }

  async delete<T>(endpoint: string): Promise<T> {
    return this.request<T>(endpoint, { method: "DELETE" });
  }
}

// Export a singleton instance
export const apiClient = new ApiClient();
