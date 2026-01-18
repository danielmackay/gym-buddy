"use client";

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import { TrainerForm } from "@/features/admin/components/TrainerForm";
import { useTrainer } from "@/features/admin/hooks/useTrainer";
import { Mail, User as UserIcon } from "lucide-react";
import { useState } from "react";

export default async function TrainerDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  
  return <TrainerDetailPageContent id={id} />;
}

function TrainerDetailPageContent({ id }: { id: string }) {
  const { data: trainer, isLoading, error } = useTrainer(id);
  const [isEditing, setIsEditing] = useState(false);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-8">
        <LoadingSpinner />
      </div>
    );
  }

  if (error || !trainer) {
    return (
      <div className="rounded-md bg-destructive/15 p-4 text-destructive">
        <p className="font-semibold">Error loading trainer</p>
        <p className="text-sm">
          {error instanceof Error ? error.message : "Trainer not found"}
        </p>
      </div>
    );
  }

  if (isEditing) {
    return (
      <div className="max-w-2xl mx-auto space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Edit Trainer</h1>
            <p className="text-muted-foreground">
              Update trainer information
            </p>
          </div>
          <Button variant="outline" onClick={() => setIsEditing(false)}>
            Cancel
          </Button>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>Trainer Information</CardTitle>
            <CardDescription>
              Update the trainer's name and email address
            </CardDescription>
          </CardHeader>
          <CardContent>
            <TrainerForm trainer={trainer} />
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Trainer Details</h1>
          <p className="text-muted-foreground">
            View and manage trainer information
          </p>
        </div>
        <Button onClick={() => setIsEditing(true)}>
          Edit Trainer
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <UserIcon className="h-5 w-5" />
            {trainer.name}
          </CardTitle>
          <CardDescription className="flex items-center gap-1">
            <Mail className="h-3 w-3" />
            {trainer.email}
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div>
            <h3 className="font-semibold mb-2">Roles</h3>
            <div className="flex gap-1">
              {trainer.roles.map((role) => (
                <Badge key={role} variant="secondary">
                  {role === 1 ? "Admin" : role === 2 ? "Trainer" : "Client"}
                </Badge>
              ))}
            </div>
          </div>

          <div>
            <h3 className="font-semibold mb-2">Details</h3>
            <dl className="grid gap-2 text-sm">
              <div className="flex">
                <dt className="font-medium w-32">ID:</dt>
                <dd className="text-muted-foreground font-mono">{trainer.id}</dd>
              </div>
              <div className="flex">
                <dt className="font-medium w-32">Email:</dt>
                <dd className="text-muted-foreground">{trainer.email}</dd>
              </div>
            </dl>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
