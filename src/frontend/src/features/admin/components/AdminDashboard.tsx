"use client";

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import Link from "next/link";
import { UserPlus, Users } from "lucide-react";

/**
 * Admin Dashboard Component
 * Displays overview and quick actions for admin users
 */
export function AdminDashboard() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Admin Dashboard</h1>
        <p className="text-muted-foreground">
          Manage trainers and oversee the gym system
        </p>
      </div>

      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Users className="h-5 w-5" />
              Manage Trainers
            </CardTitle>
            <CardDescription>
              View, create, and manage trainer accounts
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="flex flex-col gap-2">
              <Link href="/admin/trainers">
                <Button variant="outline" className="w-full">
                  View All Trainers
                </Button>
              </Link>
              <Link href="/admin/trainers/new">
                <Button className="w-full">
                  <UserPlus className="mr-2 h-4 w-4" />
                  Create New Trainer
                </Button>
              </Link>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>System Overview</CardTitle>
            <CardDescription>
              Quick statistics and system status
            </CardDescription>
          </CardHeader>
          <CardContent>
            <p className="text-sm text-muted-foreground">
              More features coming soon...
            </p>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
