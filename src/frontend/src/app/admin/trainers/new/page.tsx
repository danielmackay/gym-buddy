"use client";

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { TrainerForm } from "@/features/admin/components/TrainerForm";

export default function NewTrainerPage() {
  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Create New Trainer</h1>
        <p className="text-muted-foreground">
          Add a new trainer to the system
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Trainer Information</CardTitle>
          <CardDescription>
            Enter the trainer's name and email address
          </CardDescription>
        </CardHeader>
        <CardContent>
          <TrainerForm />
        </CardContent>
      </Card>
    </div>
  );
}
