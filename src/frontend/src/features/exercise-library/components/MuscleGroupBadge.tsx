import { Badge } from "@/components/ui/badge";
import { MuscleGroup } from "@/lib/types/exercise";
import { cn } from "@/lib/utils/cn";

const MUSCLE_GROUP_LABELS: Record<MuscleGroup, string> = {
  [MuscleGroup.Chest]: "Chest",
  [MuscleGroup.Back]: "Back",
  [MuscleGroup.Shoulders]: "Shoulders",
  [MuscleGroup.Biceps]: "Biceps",
  [MuscleGroup.Triceps]: "Triceps",
  [MuscleGroup.Forearms]: "Forearms",
  [MuscleGroup.Quadriceps]: "Quads",
  [MuscleGroup.Hamstrings]: "Hamstrings",
  [MuscleGroup.Glutes]: "Glutes",
  [MuscleGroup.Calves]: "Calves",
  [MuscleGroup.Abs]: "Abs",
  [MuscleGroup.Obliques]: "Obliques",
};

const MUSCLE_GROUP_COLORS: Record<MuscleGroup, string> = {
  [MuscleGroup.Chest]: "bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200",
  [MuscleGroup.Back]: "bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200",
  [MuscleGroup.Shoulders]: "bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200",
  [MuscleGroup.Biceps]: "bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200",
  [MuscleGroup.Triceps]: "bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200",
  [MuscleGroup.Forearms]: "bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200",
  [MuscleGroup.Quadriceps]: "bg-pink-100 text-pink-800 dark:bg-pink-900 dark:text-pink-200",
  [MuscleGroup.Hamstrings]: "bg-indigo-100 text-indigo-800 dark:bg-indigo-900 dark:text-indigo-200",
  [MuscleGroup.Glutes]: "bg-cyan-100 text-cyan-800 dark:bg-cyan-900 dark:text-cyan-200",
  [MuscleGroup.Calves]: "bg-teal-100 text-teal-800 dark:bg-teal-900 dark:text-teal-200",
  [MuscleGroup.Abs]: "bg-amber-100 text-amber-800 dark:bg-amber-900 dark:text-amber-200",
  [MuscleGroup.Obliques]: "bg-lime-100 text-lime-800 dark:bg-lime-900 dark:text-lime-200",
};

interface MuscleGroupBadgeProps {
  muscleGroup: MuscleGroup;
  size?: "sm" | "default";
}

export function MuscleGroupBadge({
  muscleGroup,
  size = "default",
}: MuscleGroupBadgeProps) {
  return (
    <Badge
      variant="outline"
      className={cn(
        MUSCLE_GROUP_COLORS[muscleGroup],
        size === "sm" && "text-xs px-2 py-0"
      )}
    >
      {MUSCLE_GROUP_LABELS[muscleGroup]}
    </Badge>
  );
}
