"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useQuery } from "@tanstack/react-query";
import { useUserStore } from "@/lib/stores/user-store";
import { UserSelector } from "@/components/UserSelector";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import { ErrorBoundary } from "@/components/ErrorBoundary";
import { apiClient } from "@/lib/api/client";
import { User, UserRole } from "@/lib/types/user";

/**
 * Home page - User selection screen
 * Allows user to select which account to use (no auth MVP)
 */
export default function Home() {
  const router = useRouter();
  const { currentUser } = useUserStore();

  // Fetch all users for selection
  const { data: users, isLoading, error } = useQuery<User[]>({
    queryKey: ["users"],
    queryFn: async () => {
      // For now, return empty array since we don't have the API endpoint yet
      // This will be replaced with actual API call once backend is ready
      return [];
    },
  });

  // Redirect if user is already selected
  useEffect(() => {
    if (currentUser) {
      if (currentUser.roles.includes(UserRole.Admin)) {
        router.push("/admin");
      } else if (currentUser.roles.includes(UserRole.Trainer)) {
        router.push("/trainer");
      } else if (currentUser.roles.includes(UserRole.Client)) {
        router.push("/client");
      }
    }
  }, [currentUser, router]);

  const handleUserSelect = (user: User) => {
    // Redirect based on role
    if (user.roles.includes(UserRole.Admin)) {
      router.push("/admin");
    } else if (user.roles.includes(UserRole.Trainer)) {
      router.push("/trainer");
    } else if (user.roles.includes(UserRole.Client)) {
      router.push("/client");
    }
  };

  if (isLoading) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <LoadingSpinner size="lg" text="Loading users..." />
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="max-w-md text-center">
          <h1 className="mb-4 text-2xl font-bold">Error Loading Users</h1>
          <p className="mb-4 text-muted-foreground">
            Unable to load users. Please ensure the backend API is running.
          </p>
          <p className="text-sm text-muted-foreground">
            Error: {error instanceof Error ? error.message : "Unknown error"}
          </p>
        </div>
      </div>
    );
  }

  return (
    <ErrorBoundary>
      <div className="flex min-h-screen flex-col items-center justify-center bg-background p-4">
        <div className="mb-8 text-center">
          <h1 className="mb-2 text-4xl font-bold">Gym Buddy</h1>
          <p className="text-muted-foreground">
            Workout tracking and management system
          </p>
        </div>
        <UserSelector users={users || []} onUserSelect={handleUserSelect} />
        <div className="mt-8 max-w-md text-center text-sm text-muted-foreground">
          <p>
            This is a development build without authentication. Select a user to
            continue.
          </p>
          <p className="mt-2">
            Make sure the backend API is running at{" "}
            <code className="rounded bg-muted px-1 py-0.5">
              {process.env.NEXT_PUBLIC_API_URL || "https://localhost:7255/api"}
            </code>
          </p>
        </div>
      </div>
    </ErrorBoundary>
  );
}
