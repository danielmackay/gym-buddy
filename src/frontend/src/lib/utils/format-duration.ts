import type { Duration } from "../types/workout-plan";

/**
 * Format duration from seconds to human-readable format
 * Examples: "1:30", "0:45", "2:00"
 */
export function formatDuration(duration: Duration): string {
  const minutes = Math.floor(duration.seconds / 60);
  const seconds = duration.seconds % 60;
  return `${minutes}:${seconds.toString().padStart(2, "0")}`;
}

/**
 * Parse duration string (MM:SS) to seconds
 * Examples: "1:30" -> 90, "0:45" -> 45
 */
export function parseDuration(durationString: string): Duration {
  const [minutes, seconds] = durationString.split(":").map(Number);
  return { seconds: minutes * 60 + seconds };
}
