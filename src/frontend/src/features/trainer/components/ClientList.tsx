"use client";

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import { useClients } from "../hooks/useClients";
import { useUserStore } from "@/lib/stores/user-store";
import Link from "next/link";
import { Mail, User } from "lucide-react";
import type { User as UserType } from "@/lib/types/user";

/**
 * ClientList Component
 * Displays a list of all clients for the current trainer
 */
export function ClientList() {
  const { currentUser } = useUserStore();
  const trainerId = currentUser?.id;
  const { data: clients, isLoading, error } = useClients(trainerId || "");

  // Show loading if no user is selected yet
  if (!currentUser || !trainerId) {
    return (
      <div className="rounded-md bg-yellow-50 p-4 text-yellow-800 dark:bg-yellow-900/20 dark:text-yellow-400">
        <p className="font-semibold">No trainer selected</p>
        <p className="text-sm">
          Please select a trainer from the user selection page to view clients.
        </p>
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-8">
        <LoadingSpinner />
      </div>
    );
  }

  if (error) {
    return (
      <div className="rounded-md bg-destructive/15 p-4 text-destructive">
        <p className="font-semibold">Error loading clients</p>
        <p className="text-sm">
          {error instanceof Error ? error.message : "Unknown error occurred"}
        </p>
      </div>
    );
  }

  if (!clients || clients.length === 0) {
    return (
      <Card>
        <CardContent className="pt-6">
          <div className="text-center text-muted-foreground">
            <User className="mx-auto h-12 w-12 opacity-50" />
            <p className="mt-2">No clients found</p>
            <p className="text-sm">Create your first client to get started</p>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      {clients.map((client: UserType) => (
        <Link key={client.id} href={`/trainer/clients/${client.id}`}>
          <Card className="h-full transition-shadow hover:shadow-md">
            <CardHeader>
              <div className="flex items-start justify-between">
                <div>
                  <CardTitle className="text-lg">{client.name}</CardTitle>
                  <CardDescription className="flex items-center gap-1 mt-1">
                    <Mail className="h-3 w-3" />
                    {client.email}
                  </CardDescription>
                </div>
                <Badge variant="secondary">Client</Badge>
              </div>
            </CardHeader>
            <CardContent>
              <div className="text-sm text-muted-foreground">
                {client.assignedWorkoutPlanIds.length === 0 ? (
                  <p>No workout plans assigned</p>
                ) : (
                  <p>{client.assignedWorkoutPlanIds.length} workout plan(s) assigned</p>
                )}
              </div>
            </CardContent>
          </Card>
        </Link>
      ))}
    </div>
  );
}
