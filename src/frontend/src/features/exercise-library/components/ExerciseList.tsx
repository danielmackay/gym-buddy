"use client";

import { useRouter } from "next/navigation";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Edit, Dumbbell } from "lucide-react";
import { useExercises } from "../hooks/useExercises";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import type { ExerciseType, MuscleGroup } from "@/lib/types/exercise";
import { MuscleGroupBadge } from "./MuscleGroupBadge";

interface ExerciseListProps {
  muscleGroup?: MuscleGroup;
  type?: ExerciseType;
}

export function ExerciseList({ muscleGroup, type }: ExerciseListProps) {
  const router = useRouter();
  const { data: exercises, isLoading, error } = useExercises({
    muscleGroup,
    type,
  });

  if (isLoading) {
    return <LoadingSpinner text="Loading exercises..." />;
  }

  if (error) {
    return (
      <Card>
        <CardContent className="flex min-h-[200px] items-center justify-center py-8">
          <div className="text-center">
            <p className="text-muted-foreground">
              Failed to load exercises. Please try again.
            </p>
          </div>
        </CardContent>
      </Card>
    );
  }

  if (!exercises || exercises.length === 0) {
    return (
      <Card>
        <CardContent className="flex min-h-[200px] items-center justify-center py-8">
          <div className="text-center">
            <Dumbbell className="mx-auto h-12 w-12 text-muted-foreground" />
            <h3 className="mt-4 text-lg font-semibold">No exercises found</h3>
            <p className="text-muted-foreground">
              {muscleGroup || type
                ? "Try adjusting your filters"
                : "Get started by creating your first exercise"}
            </p>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      {exercises.map((exercise) => (
        <Card key={exercise.id} className="hover:shadow-md transition-shadow">
          <CardHeader className="flex flex-row items-start justify-between space-y-0">
            <div className="space-y-1">
              <CardTitle className="text-lg">{exercise.name}</CardTitle>
              <Badge variant={exercise.type === 1 ? "default" : "secondary"}>
                {exercise.type === 1 ? "Reps & Weight" : "Time-Based"}
              </Badge>
            </div>
            <Button
              variant="ghost"
              size="icon"
              onClick={() => router.push(`/trainer/exercises/${exercise.id}`)}
            >
              <Edit className="h-4 w-4" />
            </Button>
          </CardHeader>
          <CardContent>
            {exercise.description && (
              <p className="mb-3 text-sm text-muted-foreground line-clamp-2">
                {exercise.description}
              </p>
            )}
            <div className="flex flex-wrap gap-1">
              {exercise.muscleGroups.map((mg) => (
                <MuscleGroupBadge key={mg} muscleGroup={mg} size="sm" />
              ))}
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}
