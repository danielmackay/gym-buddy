"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useUserStore } from "@/lib/stores/user-store";
import { UserRole } from "@/lib/types/user";
import { cn } from "@/lib/utils";
import { Home, Users, Dumbbell, ClipboardList, LogOut } from "lucide-react";
import { Button } from "@/components/ui/button";

/**
 * Navigation component with role-based menu items
 * Shows different navigation based on user role
 */
export function Navigation() {
  const pathname = usePathname();
  const { currentUser, clearCurrentUser } = useUserStore();

  if (!currentUser) return null;

  const isAdmin = currentUser.roles.includes(UserRole.Admin);
  const isTrainer = currentUser.roles.includes(UserRole.Trainer);

  const adminLinks = [
    { href: "/admin", label: "Dashboard", icon: Home },
    { href: "/admin/trainers", label: "Trainers", icon: Users },
  ];

  const trainerLinks = [
    { href: "/trainer", label: "Dashboard", icon: Home },
    { href: "/trainer/clients", label: "Clients", icon: Users },
    { href: "/trainer/exercises", label: "Exercises", icon: Dumbbell },
    { href: "/trainer/workout-plans", label: "Workout Plans", icon: ClipboardList },
  ];

  const links = isAdmin ? adminLinks : isTrainer ? trainerLinks : [];

  return (
    <nav className="border-b bg-background">
      <div className="container mx-auto flex h-16 items-center justify-between px-4">
        <div className="flex items-center gap-6">
          <Link href="/" className="text-lg font-bold">
            Gym Buddy
          </Link>
          <div className="hidden items-center gap-1 md:flex">
            {links.map((link) => {
              const Icon = link.icon;
              return (
                <Link
                  key={link.href}
                  href={link.href}
                  className={cn(
                    "flex items-center gap-2 rounded-md px-3 py-2 text-sm font-medium transition-colors hover:bg-accent hover:text-accent-foreground",
                    pathname === link.href
                      ? "bg-accent text-accent-foreground"
                      : "text-muted-foreground",
                  )}
                >
                  <Icon className="h-4 w-4" />
                  {link.label}
                </Link>
              );
            })}
          </div>
        </div>
        <div className="flex items-center gap-4">
          <span className="hidden text-sm text-muted-foreground sm:inline">
            {currentUser.name}
          </span>
          <Button
            variant="ghost"
            size="sm"
            onClick={() => {
              clearCurrentUser();
              window.location.href = "/";
            }}
          >
            <LogOut className="h-4 w-4" />
            <span className="ml-2 hidden sm:inline">Logout</span>
          </Button>
        </div>
      </div>
    </nav>
  );
}
