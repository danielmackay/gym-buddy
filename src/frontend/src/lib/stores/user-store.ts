import { create } from "zustand";
import { persist } from "zustand/middleware";
import type { User } from "../types/user";

interface UserStore {
  currentUser: User | null;
  setCurrentUser: (user: User | null) => void;
  clearCurrentUser: () => void;
}

/**
 * Zustand store for managing current user state
 * Persisted to localStorage for session continuity
 */
export const useUserStore = create<UserStore>()(
  persist(
    (set) => ({
      currentUser: null,
      setCurrentUser: (user) => set({ currentUser: user }),
      clearCurrentUser: () => set({ currentUser: null }),
    }),
    {
      name: "gym-buddy-user-storage", // localStorage key
    },
  ),
);
