"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { Navigation } from "@/components/Navigation";
import { useUserStore } from "@/lib/stores/user-store";
import { UserRole } from "@/lib/types/user";
import { LoadingSpinner } from "@/components/LoadingSpinner";

export default function TrainerLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const router = useRouter();
  const { currentUser } = useUserStore();

  useEffect(() => {
    // Redirect if no user selected or user is not a trainer
    if (!currentUser) {
      router.push("/");
    } else if (!currentUser.roles.includes(UserRole.Trainer)) {
      // Redirect to appropriate page based on role
      if (currentUser.roles.includes(UserRole.Admin)) {
        router.push("/admin");
      } else if (currentUser.roles.includes(UserRole.Client)) {
        router.push("/client");
      } else {
        router.push("/");
      }
    }
  }, [currentUser, router]);

  // Show loading while checking authentication
  if (!currentUser || !currentUser.roles.includes(UserRole.Trainer)) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <LoadingSpinner size="lg" text="Checking authentication..." />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-background">
      <Navigation />
      <main className="container mx-auto px-4 py-8">
        {children}
      </main>
    </div>
  );
}
