"use client";

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import Link from "next/link";
import { UserPlus, Users, Dumbbell, ClipboardList } from "lucide-react";

/**
 * Trainer Dashboard Component
 * Displays overview and quick actions for trainer users
 */
export function TrainerDashboard() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Trainer Dashboard</h1>
        <p className="text-muted-foreground">
          Manage your clients, exercises, and workout plans
        </p>
      </div>

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Users className="h-5 w-5" />
              Manage Clients
            </CardTitle>
            <CardDescription>
              View, create, and manage your client accounts
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="flex flex-col gap-2">
              <Link href="/trainer/clients">
                <Button variant="outline" className="w-full">
                  View All Clients
                </Button>
              </Link>
              <Link href="/trainer/clients/new">
                <Button className="w-full">
                  <UserPlus className="mr-2 h-4 w-4" />
                  Create New Client
                </Button>
              </Link>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Dumbbell className="h-5 w-5" />
              Exercise Library
            </CardTitle>
            <CardDescription>
              Manage your exercise database
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Link href="/trainer/exercises">
              <Button variant="outline" className="w-full">
                Browse Exercises
              </Button>
            </Link>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <ClipboardList className="h-5 w-5" />
              Workout Plans
            </CardTitle>
            <CardDescription>
              Create and assign workout plans
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Link href="/trainer/workout-plans">
              <Button variant="outline" className="w-full">
                View Workout Plans
              </Button>
            </Link>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
