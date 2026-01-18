"use client";

import { use } from "react";
import { useRouter } from "next/navigation";
import { ArrowLeft } from "lucide-react";
import { Button } from "@/components/ui/button";
import { ExerciseForm } from "@/features/exercise-library/components/ExerciseForm";
import { useExercise } from "@/features/exercise-library/hooks/useExercise";
import { useUpdateExercise } from "@/features/exercise-library/hooks/useUpdateExercise";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import { toast } from "sonner";
import type { UpdateExerciseFormData } from "@/lib/validation/exercise";

export default function ExerciseDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const resolvedParams = use(params);
  const router = useRouter();
  const { data: exercise, isLoading } = useExercise(resolvedParams.id);
  const updateExercise = useUpdateExercise();

  const handleSubmit = async (data: UpdateExerciseFormData) => {
    try {
      await updateExercise.mutateAsync({
        id: resolvedParams.id,
        data,
      });
      toast.success("Exercise updated successfully!");
      router.push("/trainer/exercises");
    } catch (error) {
      toast.error("Failed to update exercise. Please try again.");
    }
  };

  if (isLoading) {
    return (
      <div className="flex min-h-[400px] items-center justify-center">
        <LoadingSpinner text="Loading exercise..." />
      </div>
    );
  }

  if (!exercise) {
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
          <h1 className="text-3xl font-bold tracking-tight">Exercise Not Found</h1>
          <p className="text-muted-foreground">
            The exercise you're looking for doesn't exist.
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
        <h1 className="text-3xl font-bold tracking-tight">Edit Exercise</h1>
        <p className="text-muted-foreground">
          Update exercise details
        </p>
      </div>

      <ExerciseForm
        exercise={exercise}
        onSubmit={handleSubmit}
        isSubmitting={updateExercise.isPending}
        isEdit
      />
    </div>
  );
}
