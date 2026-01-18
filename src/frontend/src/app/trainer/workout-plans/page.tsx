"use client";

import { Suspense } from "react";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Plus } from "lucide-react";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import { WorkoutPlanList } from "@/features/workout-plans/components/WorkoutPlanList";

export default function WorkoutPlansPage() {
  const router = useRouter();

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Workout Plans</h1>
          <p className="text-muted-foreground">
            Create and manage workout plans for your clients
          </p>
        </div>
        <Button onClick={() => router.push("/trainer/workout-plans/new")}>
          <Plus className="mr-2 h-4 w-4" />
          New Workout Plan
        </Button>
      </div>

      <Suspense fallback={<LoadingSpinner text="Loading workout plans..." />}>
        <WorkoutPlanList />
      </Suspense>
    </div>
  );
}
