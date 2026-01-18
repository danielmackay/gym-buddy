"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { createExerciseSchema, updateExerciseSchema, type CreateExerciseFormData, type UpdateExerciseFormData } from "@/lib/validation/exercise";
import { ExerciseType, MuscleGroup, type Exercise } from "@/lib/types/exercise";

const MUSCLE_GROUP_OPTIONS = [
  { value: MuscleGroup.Chest, label: "Chest" },
  { value: MuscleGroup.Back, label: "Back" },
  { value: MuscleGroup.Shoulders, label: "Shoulders" },
  { value: MuscleGroup.Biceps, label: "Biceps" },
  { value: MuscleGroup.Triceps, label: "Triceps" },
  { value: MuscleGroup.Forearms, label: "Forearms" },
  { value: MuscleGroup.Quadriceps, label: "Quadriceps" },
  { value: MuscleGroup.Hamstrings, label: "Hamstrings" },
  { value: MuscleGroup.Glutes, label: "Glutes" },
  { value: MuscleGroup.Calves, label: "Calves" },
  { value: MuscleGroup.Abs, label: "Abs" },
  { value: MuscleGroup.Obliques, label: "Obliques" },
];

interface ExerciseFormProps {
  exercise?: Exercise;
  onSubmit: (data: CreateExerciseFormData | UpdateExerciseFormData) => Promise<void>;
  isSubmitting?: boolean;
  isEdit?: boolean;
}

export function ExerciseForm({
  exercise,
  onSubmit,
  isSubmitting = false,
  isEdit = false,
}: ExerciseFormProps) {
  const form = useForm<CreateExerciseFormData | UpdateExerciseFormData>({
    resolver: zodResolver(isEdit ? updateExerciseSchema : createExerciseSchema),
    defaultValues: exercise
      ? {
          name: exercise.name,
          description: exercise.description ?? "",
          muscleGroups: exercise.muscleGroups,
          ...(isEdit ? {} : { type: exercise.type }),
        }
      : {
          name: "",
          description: "",
          type: ExerciseType.RepsAndWeight,
          muscleGroups: [],
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
                  <FormLabel>Exercise Name *</FormLabel>
                  <FormControl>
                    <Input placeholder="e.g., Bench Press" {...field} />
                  </FormControl>
                  <FormDescription>
                    The name of the exercise (max 100 characters)
                  </FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            {!isEdit && (
              <FormField
                control={form.control}
                name="type"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Exercise Type *</FormLabel>
                    <Select
                      onValueChange={(value) => field.onChange(parseInt(value))}
                      defaultValue={field.value?.toString()}
                    >
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Select exercise type" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value={ExerciseType.RepsAndWeight.toString()}>
                          Reps & Weight
                        </SelectItem>
                        <SelectItem value={ExerciseType.TimeBased.toString()}>
                          Time-Based
                        </SelectItem>
                      </SelectContent>
                    </Select>
                    <FormDescription>
                      Choose whether this exercise is measured by reps/weight or time
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />
            )}

            <FormField
              control={form.control}
              name="muscleGroups"
              render={() => (
                <FormItem>
                  <div className="mb-4">
                    <FormLabel>Muscle Groups *</FormLabel>
                    <FormDescription>
                      Select at least one muscle group targeted by this exercise
                    </FormDescription>
                  </div>
                  <div className="grid grid-cols-2 gap-3 sm:grid-cols-3">
                    {MUSCLE_GROUP_OPTIONS.map((option) => (
                      <FormField
                        key={option.value}
                        control={form.control}
                        name="muscleGroups"
                        render={({ field }) => {
                          return (
                            <FormItem
                              key={option.value}
                              className="flex flex-row items-start space-x-3 space-y-0"
                            >
                              <FormControl>
                                <Checkbox
                                  checked={field.value?.includes(option.value)}
                                  onCheckedChange={(checked) => {
                                    return checked
                                      ? field.onChange([...field.value, option.value])
                                      : field.onChange(
                                          field.value?.filter(
                                            (value) => value !== option.value
                                          )
                                        );
                                  }}
                                />
                              </FormControl>
                              <FormLabel className="font-normal cursor-pointer">
                                {option.label}
                              </FormLabel>
                            </FormItem>
                          );
                        }}
                      />
                    ))}
                  </div>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Description</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="Enter a detailed description of the exercise..."
                      className="min-h-[100px]"
                      {...field}
                    />
                  </FormControl>
                  <FormDescription>
                    Optional description (max 500 characters)
                  </FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />
          </CardContent>
        </Card>

        <div className="flex gap-4">
          <Button type="submit" disabled={isSubmitting}>
            {isSubmitting
              ? isEdit
                ? "Updating..."
                : "Creating..."
              : isEdit
              ? "Update Exercise"
              : "Create Exercise"}
          </Button>
        </div>
      </form>
    </Form>
  );
}
