"use client";

import { useState, useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
} from "@/components/ui/command";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { Button } from "@/components/ui/button";
import { Check, ChevronsUpDown } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { LoadingSpinner } from "@/components/LoadingSpinner";
import { useExercises } from "@/features/exercise-library/hooks/useExercises";
import { useAddExercise } from "../hooks/useAddExercise";
import { addExerciseToPlanSchema } from "@/lib/validation/workout-plan";
import { ExerciseType } from "@/lib/types/exercise";
import { WeightUnit } from "@/lib/types/workout-plan";
import type { AddExerciseToPlanFormData } from "@/lib/validation/workout-plan";
import { cn } from "@/lib/utils";

interface AddExerciseModalProps {
  workoutPlanId: string;
  isOpen: boolean;
  onClose: () => void;
}

export function AddExerciseModal({
  workoutPlanId,
  isOpen,
  onClose,
}: AddExerciseModalProps) {
  const { data: exercises, isLoading: isLoadingExercises } = useExercises();
  const addExercise = useAddExercise();
  const [selectedExerciseId, setSelectedExerciseId] = useState<string>("");
  const [open, setOpen] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    setValue,
    watch,
    formState: { errors },
  } = useForm<AddExerciseToPlanFormData>({
    resolver: zodResolver(addExerciseToPlanSchema),
    defaultValues: {
      sets: 3,
      weight: {
        value: 0,
        unit: WeightUnit.Kilograms, // Default to Kilograms (1)
      },
      duration: {
        seconds: 60,
      },
    },
  });

  const selectedExercise = exercises?.find((ex) => ex.id === selectedExerciseId);
  const isRepsAndWeight = selectedExercise?.type === ExerciseType.RepsAndWeight;
  const isTimeBased = selectedExercise?.type === ExerciseType.TimeBased;

  // Reset form when modal closes
  useEffect(() => {
    if (!isOpen) {
      reset();
      setSelectedExerciseId("");
    }
  }, [isOpen, reset]);

  const onSubmit = async (data: AddExerciseToPlanFormData) => {
    try {
      // Clean up data based on exercise type
      const payload: any = {
        exerciseId: data.exerciseId,
        sets: data.sets,
      };

      if (isRepsAndWeight) {
        payload.reps = data.reps;
        if (data.weight && data.weight.value > 0) {
          // Ensure unit is a valid number (1 or 2)
          payload.weight = {
            value: data.weight.value,
            unit: data.weight.unit ?? WeightUnit.Kilograms,
          };
        }
      }

      if (isTimeBased && data.duration) {
        payload.duration = data.duration;
      }

      await addExercise.mutateAsync({
        workoutPlanId,
        data: payload,
      });

      onClose();
    } catch (error) {
      // Error toast is handled by the hook
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle>Add Exercise</DialogTitle>
          <DialogDescription>
            Select an exercise from the library and configure sets, reps, and
            weight.
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {/* Exercise Selection */}
          <div className="space-y-2">
            <Label htmlFor="exerciseId">Exercise *</Label>
            {isLoadingExercises ? (
              <div className="flex items-center justify-center py-4">
                <LoadingSpinner text="Loading exercises..." />
              </div>
            ) : (
              <Popover open={open} onOpenChange={setOpen}>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    role="combobox"
                    aria-expanded={open}
                    className="w-full justify-between"
                  >
                    {selectedExerciseId
                      ? exercises?.find((ex) => ex.id === selectedExerciseId)?.name
                      : "Select an exercise"}
                    <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50" />
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-[--radix-popover-trigger-width] p-0">
                  <Command>
                    <CommandInput placeholder="Search exercises..." />
                    <CommandEmpty>No exercise found.</CommandEmpty>
                    <CommandGroup heading="Reps & Weight">
                      {exercises
                        ?.filter((ex) => ex.type === ExerciseType.RepsAndWeight)
                        .map((exercise) => (
                          <CommandItem
                            key={exercise.id}
                            value={exercise.name}
                            onSelect={() => {
                              setSelectedExerciseId(exercise.id);
                              setValue("exerciseId", exercise.id);
                              setOpen(false);
                            }}
                          >
                            <Check
                              className={cn(
                                "mr-2 h-4 w-4",
                                selectedExerciseId === exercise.id
                                  ? "opacity-100"
                                  : "opacity-0"
                              )}
                            />
                            {exercise.name}
                          </CommandItem>
                        ))}
                    </CommandGroup>
                    <CommandGroup heading="Time-Based">
                      {exercises
                        ?.filter((ex) => ex.type === ExerciseType.TimeBased)
                        .map((exercise) => (
                          <CommandItem
                            key={exercise.id}
                            value={exercise.name}
                            onSelect={() => {
                              setSelectedExerciseId(exercise.id);
                              setValue("exerciseId", exercise.id);
                              setOpen(false);
                            }}
                          >
                            <Check
                              className={cn(
                                "mr-2 h-4 w-4",
                                selectedExerciseId === exercise.id
                                  ? "opacity-100"
                                  : "opacity-0"
                              )}
                            />
                            {exercise.name}
                          </CommandItem>
                        ))}
                    </CommandGroup>
                  </Command>
                </PopoverContent>
              </Popover>
            )}
            {errors.exerciseId && (
              <p className="text-sm text-destructive">
                {errors.exerciseId.message}
              </p>
            )}
          </div>

          {/* Sets */}
          <div className="space-y-2">
            <Label htmlFor="sets">Sets *</Label>
            <Input
              id="sets"
              type="number"
              min="1"
              {...register("sets", { valueAsNumber: true })}
            />
            {errors.sets && (
              <p className="text-sm text-destructive">{errors.sets.message}</p>
            )}
          </div>

          {/* Reps & Weight (for RepsAndWeight exercises) */}
          {isRepsAndWeight && (
            <>
              <div className="space-y-2">
                <Label htmlFor="reps">Reps *</Label>
                <Input
                  id="reps"
                  type="number"
                  min="1"
                  {...register("reps", { valueAsNumber: true })}
                />
                {errors.reps && (
                  <p className="text-sm text-destructive">
                    {errors.reps.message}
                  </p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="weight">Weight (optional)</Label>
                <div className="flex gap-2">
                  <Input
                    id="weight"
                    type="number"
                    min="0"
                    step="0.5"
                    placeholder="0"
                    {...register("weight.value", { valueAsNumber: true })}
                    className="flex-1"
                  />
                  <Select
                    value={watch("weight.unit")?.toString() ?? WeightUnit.Kilograms.toString()}
                    onValueChange={(value) =>
                      setValue("weight.unit", parseInt(value) as WeightUnit)
                    }
                  >
                    <SelectTrigger className="w-[100px]">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value={WeightUnit.Kilograms.toString()}>
                        kg
                      </SelectItem>
                      <SelectItem value={WeightUnit.Pounds.toString()}>
                        lbs
                      </SelectItem>
                    </SelectContent>
                  </Select>
                </div>
                {errors.weight?.value && (
                  <p className="text-sm text-destructive">
                    {errors.weight.value.message}
                  </p>
                )}
              </div>
            </>
          )}

          {/* Duration (for TimeBased exercises) */}
          {isTimeBased && (
            <div className="space-y-2">
              <Label htmlFor="duration">Duration (seconds) *</Label>
              <Input
                id="duration"
                type="number"
                min="1"
                placeholder="60"
                {...register("duration.seconds", { valueAsNumber: true })}
              />
              {errors.duration?.seconds && (
                <p className="text-sm text-destructive">
                  {errors.duration.seconds.message}
                </p>
              )}
            </div>
          )}

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={onClose}
              disabled={addExercise.isPending}
            >
              Cancel
            </Button>
            <Button
              type="submit"
              disabled={addExercise.isPending || !selectedExerciseId}
            >
              {addExercise.isPending ? "Adding..." : "Add Exercise"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
