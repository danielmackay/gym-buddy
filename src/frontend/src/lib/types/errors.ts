// API error types matching backend FluentValidation and ErrorOr patterns

export interface ValidationError {
  [field: string]: string[]; // FluentValidation format
}

export interface DomainError {
  code: string;
  description: string;
  type: "Validation" | "NotFound" | "Conflict" | "Failure";
}

export interface ApiErrorResponse {
  errors?: ValidationError | DomainError[];
  title?: string;
  status?: number;
}

/**
 * Custom error class for API errors
 * Handles both FluentValidation errors and Domain (ErrorOr) errors
 */
export class ApiError extends Error {
  constructor(
    public response: ApiErrorResponse,
    public statusCode: number,
  ) {
    super(response.title || "API Error");
    this.name = "ApiError";
  }

  isValidationError(): boolean {
    return (
      !!this.response.errors && !Array.isArray(this.response.errors)
    );
  }

  isDomainError(): boolean {
    return Array.isArray(this.response.errors);
  }

  getValidationErrors(): ValidationError | null {
    if (this.isValidationError()) {
      return this.response.errors as ValidationError;
    }
    return null;
  }

  getDomainErrors(): DomainError[] | null {
    if (this.isDomainError()) {
      return this.response.errors as DomainError[];
    }
    return null;
  }

  /**
   * Get a user-friendly error message
   * Returns the first validation error or domain error description
   * Falls back to generic message for other errors
   */
  getUserMessage(): string {
    if (this.isValidationError()) {
      const errors = this.getValidationErrors();
      if (errors) {
        const firstField = Object.keys(errors)[0];
        return errors[firstField][0];
      }
    }

    if (this.isDomainError()) {
      const errors = this.getDomainErrors();
      if (errors && errors.length > 0) {
        return errors[0].description;
      }
    }

    return this.response.title || "An error occurred. Please try again.";
  }
}
