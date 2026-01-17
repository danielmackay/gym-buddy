"use client";

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import { useTrainers } from "../hooks/useTrainers";
import Link from "next/link";
import { Mail, User } from "lucide-react";
import type { User as UserType } from "@/lib/types/user";

/**
 * TrainerList Component
 * Displays a list of all trainers with their basic information
 */
export function TrainerList() {
  const { data: trainers, isLoading, error } = useTrainers();

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
        <p className="font-semibold">Error loading trainers</p>
        <p className="text-sm">
          {error instanceof Error ? error.message : "Unknown error occurred"}
        </p>
      </div>
    );
  }

  if (!trainers || trainers.length === 0) {
    return (
      <Card>
        <CardContent className="pt-6">
          <div className="text-center text-muted-foreground">
            <User className="mx-auto h-12 w-12 opacity-50" />
            <p className="mt-2">No trainers found</p>
            <p className="text-sm">Create your first trainer to get started</p>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      {trainers.map((trainer: UserType) => (
        <Card key={trainer.id}>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <User className="h-5 w-5" />
              {trainer.name}
            </CardTitle>
            <CardDescription className="flex items-center gap-1">
              <Mail className="h-3 w-3" />
              {trainer.email}
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="flex flex-col gap-2">
              <div className="flex gap-1">
                {trainer.roles.map((role) => (
                  <Badge key={role} variant="secondary">
                    {role === 1 ? "Admin" : role === 2 ? "Trainer" : "Client"}
                  </Badge>
                ))}
              </div>
              <Link href={`/admin/trainers/${trainer.id}`}>
                <Button variant="outline" size="sm" className="w-full">
                  View Details
                </Button>
              </Link>
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}
