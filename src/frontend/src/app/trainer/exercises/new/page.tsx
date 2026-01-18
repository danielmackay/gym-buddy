"use client";

import { useRouter } from "next/navigation";
import { ArrowLeft } from "lucide-react";
import { Button } from "@/components/ui/button";
import { ExerciseForm } from "@/features/exercise-library/components/ExerciseForm";
import { useCreateExercise } from "@/features/exercise-library/hooks/useCreateExercise";
import { toast } from "sonner";
import type { CreateExerciseFormData } from "@/lib/validation/exercise";

export default function NewExercisePage() {
  const router = useRouter();
  const createExercise = useCreateExercise();

  const handleSubmit = async (data: CreateExerciseFormData) => {
    // Ensure we have the type field (should always be present for create)
    if (!("type" in data)) {
      toast.error("Exercise type is required");
      return;
    }

    try {
      await createExercise.mutateAsync(data);
      toast.success("Exercise created successfully!");
      router.push("/trainer/exercises");
    } catch (error) {
      toast.error("Failed to create exercise. Please try again.");
    }
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
        <h1 className="text-3xl font-bold tracking-tight">Create New Exercise</h1>
        <p className="text-muted-foreground">
          Add a new exercise to your library
        </p>
      </div>

      <ExerciseForm
        onSubmit={handleSubmit as any}
        isSubmitting={createExercise.isPending}
      />
    </div>
  );
}
