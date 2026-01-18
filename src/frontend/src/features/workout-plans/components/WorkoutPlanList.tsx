"use client";

import { useRouter } from "next/navigation";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Edit, Dumbbell, Calendar } from "lucide-react";
import { useWorkoutPlans } from "../hooks/useWorkoutPlans";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import type { WorkoutPlan } from "@/lib/types/workout-plan";

export function WorkoutPlanList() {
  const router = useRouter();
  const { data: workoutPlans, isLoading, error } = useWorkoutPlans();

  if (isLoading) {
    return <LoadingSpinner text="Loading workout plans..." />;
  }

  if (error) {
    return (
      <Card>
        <CardContent className="flex min-h-[200px] items-center justify-center py-8">
          <div className="text-center">
            <p className="text-muted-foreground">
              Failed to load workout plans. Please try again.
            </p>
          </div>
        </CardContent>
      </Card>
    );
  }

  if (!workoutPlans || workoutPlans.length === 0) {
    return (
      <Card>
        <CardContent className="flex min-h-[200px] items-center justify-center py-8">
          <div className="text-center">
            <Calendar className="mx-auto h-12 w-12 text-muted-foreground" />
            <h3 className="mt-4 text-lg font-semibold">No workout plans found</h3>
            <p className="text-muted-foreground">
              Get started by creating your first workout plan
            </p>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      {workoutPlans.map((plan) => (
        <WorkoutPlanCard key={plan.id} plan={plan} router={router} />
      ))}
    </div>
  );
}

interface WorkoutPlanCardProps {
  plan: WorkoutPlan;
  router: ReturnType<typeof useRouter>;
}

function WorkoutPlanCard({ plan, router }: WorkoutPlanCardProps) {
  const exerciseCount = plan.exercises.length;

  return (
    <Card className="flex flex-col">
      <CardHeader>
        <CardTitle className="line-clamp-1">{plan.name}</CardTitle>
        <CardDescription className="line-clamp-2">
          {plan.description || "No description"}
        </CardDescription>
      </CardHeader>
      <CardContent className="flex-1 space-y-4">
        <div className="flex items-center text-sm text-muted-foreground">
          <Dumbbell className="mr-2 h-4 w-4" />
          <span>
            {exerciseCount} {exerciseCount === 1 ? "exercise" : "exercises"}
          </span>
        </div>

        <Button
          className="w-full"
          variant="outline"
          onClick={() => router.push(`/trainer/workout-plans/${plan.id}`)}
        >
          <Edit className="mr-2 h-4 w-4" />
          View & Edit
        </Button>
      </CardContent>
    </Card>
  );
}
