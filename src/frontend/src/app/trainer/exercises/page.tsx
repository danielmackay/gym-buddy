"use client";

import { Suspense } from "react";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Plus } from "lucide-react";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import { ExerciseList } from "@/features/exercise-library/components/ExerciseList";
import { ExerciseFilter } from "@/features/exercise-library/components/ExerciseFilter";
import { useState } from "react";
import type { ExerciseType, MuscleGroup } from "@/lib/types/exercise";

export default function ExercisesPage() {
  const router = useRouter();
  const [selectedMuscleGroup, setSelectedMuscleGroup] =
    useState<MuscleGroup | undefined>(undefined);
  const [selectedType, setSelectedType] = useState<ExerciseType | undefined>(
    undefined
  );

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Exercise Library</h1>
          <p className="text-muted-foreground">
            Create and manage your exercise library
          </p>
        </div>
        <Button onClick={() => router.push("/trainer/exercises/new")}>
          <Plus className="mr-2 h-4 w-4" />
          New Exercise
        </Button>
      </div>

      <ExerciseFilter
        selectedMuscleGroup={selectedMuscleGroup}
        selectedType={selectedType}
        onMuscleGroupChange={setSelectedMuscleGroup}
        onTypeChange={setSelectedType}
      />

      <Suspense fallback={<LoadingSpinner text="Loading exercises..." />}>
        <ExerciseList
          muscleGroup={selectedMuscleGroup}
          type={selectedType}
        />
      </Suspense>
    </div>
  );
}
