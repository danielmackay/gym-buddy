"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import {
  createWorkoutPlanSchema,
  type CreateWorkoutPlanFormData,
} from "@/lib/validation/workout-plan";
import type { WorkoutPlan } from "@/lib/types/workout-plan";

interface WorkoutPlanFormProps {
  workoutPlan?: WorkoutPlan;
  onSubmit: (data: CreateWorkoutPlanFormData) => Promise<void>;
  isSubmitting?: boolean;
  isEdit?: boolean;
}

export function WorkoutPlanForm({
  workoutPlan,
  onSubmit,
  isSubmitting = false,
  isEdit = false,
}: WorkoutPlanFormProps) {
  const form = useForm<CreateWorkoutPlanFormData>({
    resolver: zodResolver(createWorkoutPlanSchema),
    defaultValues: workoutPlan
      ? {
          name: workoutPlan.name,
          description: workoutPlan.description ?? "",
        }
      : {
          name: "",
          description: "",
        },
  });

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        <Card>
          <CardContent className="pt-6 space-y-4">
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Name *</FormLabel>
                  <FormControl>
                    <Input
                      placeholder="e.g., Full Body Workout"
                      {...field}
                      disabled={isSubmitting}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Description (Optional)</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="Describe the workout plan..."
                      rows={4}
                      {...field}
                      disabled={isSubmitting}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
          </CardContent>
        </Card>

        <div className="flex justify-end gap-4">
          <Button type="submit" disabled={isSubmitting}>
            {isSubmitting
              ? "Saving..."
              : isEdit
              ? "Update Plan"
              : "Create Plan"}
          </Button>
        </div>
      </form>
    </Form>
  );
}
