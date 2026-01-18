"use client";

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import { ClientForm } from "@/features/trainer/components/ClientForm";
import { useClient } from "@/features/trainer/hooks/useClient";
import { Mail, User as UserIcon } from "lucide-react";
import { use, useState } from "react";

export default function ClientDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const resolvedParams = use(params);
  return <ClientDetailPageContent id={resolvedParams.id} />;
}

function ClientDetailPageContent({ id }: { id: string }) {
  const { data: client, isLoading, error } = useClient(id);
  const [isEditing, setIsEditing] = useState(false);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-8">
        <LoadingSpinner />
      </div>
    );
  }

  if (error || !client) {
    return (
      <div className="rounded-md bg-destructive/15 p-4 text-destructive">
        <p className="font-semibold">Error loading client</p>
        <p className="text-sm">
          {error instanceof Error ? error.message : "Client not found"}
        </p>
      </div>
    );
  }

  if (isEditing) {
    return (
      <div className="max-w-2xl mx-auto space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Edit Client</h1>
            <p className="text-muted-foreground">
              Update client information
            </p>
          </div>
          <Button variant="outline" onClick={() => setIsEditing(false)}>
            Cancel
          </Button>
        </div>

        <Card>
          <CardHeader>
            <CardTitle>Client Information</CardTitle>
            <CardDescription>
              Update the client's name and email address
            </CardDescription>
          </CardHeader>
          <CardContent>
            <ClientForm client={client} />
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Client Details</h1>
          <p className="text-muted-foreground">
            View and manage client information
          </p>
        </div>
        <Button onClick={() => setIsEditing(true)}>
          Edit Client
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <UserIcon className="h-5 w-5" />
            {client.name}
          </CardTitle>
          <CardDescription className="flex items-center gap-1">
            <Mail className="h-3 w-3" />
            {client.email}
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div>
            <h3 className="font-semibold mb-2">Roles</h3>
            <div className="flex gap-1">
              {client.roles.map((role) => (
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
                <dd className="text-muted-foreground font-mono">{client.id}</dd>
              </div>
              <div className="flex">
                <dt className="font-medium w-32">Email:</dt>
                <dd className="text-muted-foreground">{client.email}</dd>
              </div>
              {client.trainerId && (
                <div className="flex">
                  <dt className="font-medium w-32">Trainer ID:</dt>
                  <dd className="text-muted-foreground font-mono">{client.trainerId}</dd>
                </div>
              )}
            </dl>
          </div>

          <div>
            <h3 className="font-semibold mb-2">Workout Plans</h3>
            {client.assignedWorkoutPlanIds.length === 0 ? (
              <p className="text-sm text-muted-foreground">
                No workout plans assigned yet
              </p>
            ) : (
              <div className="space-y-1">
                <p className="text-sm text-muted-foreground">
                  {client.assignedWorkoutPlanIds.length} plan(s) assigned
                </p>
              </div>
            )}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
