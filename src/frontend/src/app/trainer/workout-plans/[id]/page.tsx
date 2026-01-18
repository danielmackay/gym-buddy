"use client";

import { use, useState } from "react";
import { useRouter } from "next/navigation";
import { ArrowLeft, Plus } from "lucide-react";
import { Button } from "@/components/ui/button";
import { WorkoutPlanForm } from "@/features/workout-plans/components/WorkoutPlanForm";
import { PlanExerciseList } from "@/features/workout-plans/components/PlanExerciseList";
import { AddExerciseModal } from "@/features/workout-plans/components/AddExerciseModal";
import { useWorkoutPlan } from "@/features/workout-plans/hooks/useWorkoutPlan";
import { useUpdateWorkoutPlan } from "@/features/workout-plans/hooks/useUpdateWorkoutPlan";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import type { CreateWorkoutPlanRequest } from "@/lib/types/workout-plan";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";

export default function WorkoutPlanDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const resolvedParams = use(params);
  const router = useRouter();
  const { data: workoutPlan, isLoading } = useWorkoutPlan(resolvedParams.id);
  const updateWorkoutPlan = useUpdateWorkoutPlan();
  const [isAddExerciseModalOpen, setIsAddExerciseModalOpen] = useState(false);

  const handleUpdatePlan = async (data: CreateWorkoutPlanRequest) => {
    await updateWorkoutPlan.mutateAsync({
      id: resolvedParams.id,
      data,
    });
  };

  if (isLoading) {
    return (
      <div className="flex min-h-[400px] items-center justify-center">
        <LoadingSpinner text="Loading workout plan..." />
      </div>
    );
  }

  if (!workoutPlan) {
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
            Workout Plan Not Found
          </h1>
          <p className="text-muted-foreground">
            The workout plan you're looking for doesn't exist.
          </p>
        </div>
      </div>
    );
  }

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
          {workoutPlan.name}
        </h1>
        <p className="text-muted-foreground">Manage workout plan details and exercises</p>
      </div>

      {/* Plan Details */}
      <Card>
        <CardHeader>
          <CardTitle>Plan Details</CardTitle>
          <CardDescription>Update the workout plan name and description</CardDescription>
        </CardHeader>
        <CardContent>
          <WorkoutPlanForm
            workoutPlan={workoutPlan}
            onSubmit={handleUpdatePlan}
            isSubmitting={updateWorkoutPlan.isPending}
            isEdit
          />
        </CardContent>
      </Card>

      <Separator />

      {/* Exercises */}
      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <div>
            <h2 className="text-2xl font-bold tracking-tight">Exercises</h2>
            <p className="text-muted-foreground">
              Add, remove, and reorder exercises in this workout plan
            </p>
          </div>
          <Button onClick={() => setIsAddExerciseModalOpen(true)}>
            <Plus className="mr-2 h-4 w-4" />
            Add Exercise
          </Button>
        </div>

        <PlanExerciseList
          workoutPlanId={resolvedParams.id}
          exercises={workoutPlan.exercises}
        />
      </div>

      <AddExerciseModal
        workoutPlanId={resolvedParams.id}
        isOpen={isAddExerciseModalOpen}
        onClose={() => setIsAddExerciseModalOpen(false)}
      />
    </div>
  );
}
