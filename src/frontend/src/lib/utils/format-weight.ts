import type { Weight } from "../types/workout-plan";

/**
 * Format weight for display
 * @param weight Weight object in kilograms
 * @param unit Target unit for display (kg or lbs)
 * @returns Formatted weight string with unit
 */
export function formatWeight(
  weight: Weight,
  unit: "kg" | "lbs" = "kg",
): string {
  if (unit === "lbs") {
    const pounds = weight.kilograms * 2.20462;
    return `${pounds.toFixed(1)} lbs`;
  }
  return `${weight.kilograms} kg`;
}

/**
 * Convert weight from pounds to kilograms
 */
export function poundsToKilograms(pounds: number): Weight {
  return { kilograms: pounds / 2.20462 };
}

/**
 * Convert weight from kilograms to pounds
 */
export function kilogramsToPounds(weight: Weight): number {
  return weight.kilograms * 2.20462;
}
