"use client";

import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { X } from "lucide-react";
import { ExerciseType, MuscleGroup } from "@/lib/types/exercise";

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

const EXERCISE_TYPE_OPTIONS = [
  { value: ExerciseType.RepsAndWeight, label: "Reps & Weight" },
  { value: ExerciseType.TimeBased, label: "Time-Based" },
];

interface ExerciseFilterProps {
  selectedMuscleGroup?: MuscleGroup;
  selectedType?: ExerciseType;
  onMuscleGroupChange: (muscleGroup?: MuscleGroup) => void;
  onTypeChange: (type?: ExerciseType) => void;
}

export function ExerciseFilter({
  selectedMuscleGroup,
  selectedType,
  onMuscleGroupChange,
  onTypeChange,
}: ExerciseFilterProps) {
  const hasFilters = selectedMuscleGroup !== undefined || selectedType !== undefined;

  const handleClearFilters = () => {
    onMuscleGroupChange(undefined);
    onTypeChange(undefined);
  };

  return (
    <Card>
      <CardContent className="pt-6">
        <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div className="flex flex-col gap-4 sm:flex-row sm:items-center">
            <div className="flex flex-col gap-2 sm:flex-row sm:items-center">
              <label className="text-sm font-medium">Muscle Group:</label>
              <Select
                value={selectedMuscleGroup?.toString() ?? "all"}
                onValueChange={(value) =>
                  onMuscleGroupChange(
                    value === "all" ? undefined : parseInt(value) as MuscleGroup
                  )
                }
              >
                <SelectTrigger className="w-full sm:w-[180px]">
                  <SelectValue placeholder="All Muscle Groups" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Muscle Groups</SelectItem>
                  {MUSCLE_GROUP_OPTIONS.map((option) => (
                    <SelectItem key={option.value} value={option.value.toString()}>
                      {option.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="flex flex-col gap-2 sm:flex-row sm:items-center">
              <label className="text-sm font-medium">Type:</label>
              <Select
                value={selectedType?.toString() ?? "all"}
                onValueChange={(value) =>
                  onTypeChange(
                    value === "all" ? undefined : parseInt(value) as ExerciseType
                  )
                }
              >
                <SelectTrigger className="w-full sm:w-[180px]">
                  <SelectValue placeholder="All Types" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Types</SelectItem>
                  {EXERCISE_TYPE_OPTIONS.map((option) => (
                    <SelectItem key={option.value} value={option.value.toString()}>
                      {option.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          {hasFilters && (
            <Button
              variant="outline"
              size="sm"
              onClick={handleClearFilters}
              className="w-full sm:w-auto"
            >
              <X className="mr-2 h-4 w-4" />
              Clear Filters
            </Button>
          )}
        </div>
      </CardContent>
    </Card>
  );
}
