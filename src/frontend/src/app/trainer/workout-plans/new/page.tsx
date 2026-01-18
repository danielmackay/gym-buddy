"use client";

import { useRouter } from "next/navigation";
import { ArrowLeft } from "lucide-react";
import { Button } from "@/components/ui/button";
import { WorkoutPlanForm } from "@/features/workout-plans/components/WorkoutPlanForm";
import { useCreateWorkoutPlan } from "@/features/workout-plans/hooks/useCreateWorkoutPlan";
import type { CreateWorkoutPlanRequest } from "@/lib/types/workout-plan";

export default function NewWorkoutPlanPage() {
  const router = useRouter();
  const createWorkoutPlan = useCreateWorkoutPlan();

  const handleSubmit = async (data: CreateWorkoutPlanRequest) => {
    const result = await createWorkoutPlan.mutateAsync(data);
    // Navigate to the new workout plan's detail page to add exercises
    router.push(`/trainer/workout-plans/${result.id}`);
  };

  return (
    <div className="space-y-6">
      <div>
        <Button
          variant="ghost"
          onClick={() => router.back()}
          className="mb-4"
        >
          <ArrowLeft className="mr-2 h-4 w-4" />
          Back
        </Button>
        <h1 className="text-3xl font-bold tracking-tight">
          Create New Workout Plan
        </h1>
        <p className="text-muted-foreground">
          Create a workout plan and add exercises
        </p>
      </div>

      <WorkoutPlanForm
        onSubmit={handleSubmit}
        isSubmitting={createWorkoutPlan.isPending}
      />
    </div>
  );
}
