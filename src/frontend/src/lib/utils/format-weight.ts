import type { Weight } from "../types/workout-plan";
import { WeightUnit } from "../types/workout-plan";

/**
 * Format weight for display
 * @param weight Weight object with value and unit
 * @param targetUnit Optional target unit for conversion (defaults to weight's unit)
 * @returns Formatted weight string with unit
 */
export function formatWeight(
  weight: Weight,
  targetUnit?: "kg" | "lbs"
): string {
  const unit = targetUnit || (weight.unit === WeightUnit.Kilograms ? "kg" : "lbs");
  
  let displayValue = weight.value;
  
  // Convert if needed
  if (weight.unit === WeightUnit.Kilograms && unit === "lbs") {
    displayValue = weight.value * 2.20462;
  } else if (weight.unit === WeightUnit.Pounds && unit === "kg") {
    displayValue = weight.value / 2.20462;
  }
  
  return `${displayValue.toFixed(1)} ${unit}`;
}

/**
 * Convert weight from pounds to kilograms
 */
export function poundsToKilograms(pounds: number): Weight {
  return { value: pounds / 2.20462, unit: WeightUnit.Kilograms };
}

/**
 * Convert weight from kilograms to pounds
 */
export function kilogramsToPounds(kg: number): Weight {
  return { value: kg * 2.20462, unit: WeightUnit.Pounds };
}
