"use client";

import { useState, useEffect } from "react";
import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
  type DragEndEvent,
} from "@dnd-kit/core";
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  useSortable,
  verticalListSortingStrategy,
} from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { GripVertical, Trash2, Dumbbell } from "lucide-react";
import { useRemoveExercise } from "../hooks/useRemoveExercise";
import { useReorderExercises } from "../hooks/useReorderExercises";
import type { PlannedExercise } from "@/lib/types/workout-plan";
import { ExerciseType } from "@/lib/types/exercise";

interface PlanExerciseListProps {
  workoutPlanId: string;
  exercises: PlannedExercise[];
}

export function PlanExerciseList({
  workoutPlanId,
  exercises,
}: PlanExerciseListProps) {
  const [localExercises, setLocalExercises] = useState(exercises);
  const removeExercise = useRemoveExercise();
  const reorderExercises = useReorderExercises();

  // Sync local state when exercises prop changes (e.g., after adding new exercise)
  useEffect(() => {
    setLocalExercises(exercises);
  }, [exercises]);

  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

  const handleDragEnd = async (event: DragEndEvent) => {
    const { active, over } = event;

    if (over && active.id !== over.id) {
      const oldIndex = localExercises.findIndex((ex) => ex.id === active.id);
      const newIndex = localExercises.findIndex((ex) => ex.id === over.id);

      const newOrder = arrayMove(localExercises, oldIndex, newIndex);
      setLocalExercises(newOrder);

      // Send reorder request to backend
      await reorderExercises.mutateAsync({
        workoutPlanId,
        data: { exerciseIds: newOrder.map((ex) => ex.exerciseId) },
      });
    }
  };

  const handleRemoveExercise = async (exerciseId: string) => {
    await removeExercise.mutateAsync({
      workoutPlanId,
      exerciseId,
    });
  };

  if (exercises.length === 0) {
    return (
      <Card>
        <CardContent className="flex min-h-[200px] items-center justify-center py-8">
          <div className="text-center">
            <Dumbbell className="mx-auto h-12 w-12 text-muted-foreground" />
            <h3 className="mt-4 text-lg font-semibold">No exercises yet</h3>
            <p className="text-muted-foreground">
              Add exercises to this workout plan
            </p>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <DndContext
      sensors={sensors}
      collisionDetection={closestCenter}
      onDragEnd={handleDragEnd}
    >
      <SortableContext
        items={localExercises.map((ex) => ex.id)}
        strategy={verticalListSortingStrategy}
      >
        <div className="space-y-2">
          {localExercises.map((exercise) => (
            <SortableExerciseItem
              key={exercise.id}
              exercise={exercise}
              onRemove={handleRemoveExercise}
            />
          ))}
        </div>
      </SortableContext>
    </DndContext>
  );
}

interface SortableExerciseItemProps {
  exercise: PlannedExercise;
  onRemove: (exerciseId: string) => void;
}

function SortableExerciseItem({
  exercise,
  onRemove,
}: SortableExerciseItemProps) {
  const {
    attributes,
    listeners,
    setNodeRef,
    transform,
    transition,
    isDragging,
  } = useSortable({ id: exercise.id });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : 1,
  };

  const isRepsAndWeight = exercise.exerciseType === ExerciseType.RepsAndWeight;
  const isTimeBased = exercise.exerciseType === ExerciseType.TimeBased;

  return (
    <Card ref={setNodeRef} style={style} className="touch-none">
      <CardContent className="flex items-center gap-4 py-4">
        {/* Drag Handle */}
        <button
          type="button"
          className="cursor-grab active:cursor-grabbing touch-none"
          {...attributes}
          {...listeners}
        >
          <GripVertical className="h-5 w-5 text-muted-foreground" />
        </button>

        {/* Exercise Info */}
        <div className="flex-1 space-y-1">
          <div className="flex items-center gap-2">
            <span className="text-sm font-medium text-muted-foreground">
              {exercise.order}.
            </span>
            <h4 className="font-semibold">{exercise.exerciseName}</h4>
            <Badge variant="secondary" className="ml-auto">
              {exercise.sets} sets
            </Badge>
          </div>

          <div className="flex items-center gap-4 text-sm text-muted-foreground">
            {isRepsAndWeight && (
              <>
                <span>{exercise.reps} reps</span>
                {exercise.weight && (
                  <span>
                    {exercise.weight.value}{" "}
                    {exercise.weight.unit === 1 ? "kg" : "lbs"}
                  </span>
                )}
              </>
            )}
            {isTimeBased && exercise.duration && (
              <span>{exercise.duration.seconds}s</span>
            )}
          </div>
        </div>

        {/* Delete Button */}
        <Button
          variant="ghost"
          size="icon"
          onClick={() => onRemove(exercise.exerciseId)}
          className="flex-shrink-0"
        >
          <Trash2 className="h-4 w-4 text-destructive" />
        </Button>
      </CardContent>
    </Card>
  );
}
