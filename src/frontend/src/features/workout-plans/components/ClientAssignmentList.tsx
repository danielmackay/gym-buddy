"use client";

import { useState } from "react";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { UserPlus, X } from "lucide-react";
import { useClients } from "@/features/trainer/hooks/useClients";
import { useAssignPlan } from "../hooks/useAssignPlan";
import { useUnassignPlan } from "../hooks/useUnassignPlan";
import { toast } from "sonner";

interface ClientAssignmentListProps {
  workoutPlanId: string;
  assignedClientIds: string[];
}

/**
 * Component for managing client assignments to a workout plan
 * Shows list of assigned clients with ability to assign/unassign
 */
export function ClientAssignmentList({
  workoutPlanId,
  assignedClientIds,
}: ClientAssignmentListProps) {
  const [selectedClientId, setSelectedClientId] = useState<string>("");

  const { data: clients = [], isLoading } = useClients();
  const assignMutation = useAssignPlan();
  const unassignMutation = useUnassignPlan();

  // Filter clients to only show those assigned to this plan
  const assignedClients = clients.filter((client) =>
    assignedClientIds.includes(client.id)
  );

  // Filter clients to only show those NOT assigned to this plan
  const unassignedClients = clients.filter(
    (client) => !assignedClientIds.includes(client.id)
  );

  const handleAssign = async () => {
    if (!selectedClientId) {
      toast.error("Please select a client to assign");
      return;
    }

    try {
      await assignMutation.mutateAsync({
        workoutPlanId,
        clientId: selectedClientId,
      });
      toast.success("Client assigned successfully");
      setSelectedClientId("");
    } catch (error) {
      toast.error("Failed to assign client");
    }
  };

  const handleUnassign = async (clientId: string) => {
    try {
      await unassignMutation.mutateAsync({ workoutPlanId, clientId });
      toast.success("Client unassigned successfully");
    } catch (error) {
      toast.error("Failed to unassign client");
    }
  };

  if (isLoading) {
    return <div>Loading clients...</div>;
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Assigned Clients</CardTitle>
        <CardDescription>
          Manage which clients have access to this workout plan
        </CardDescription>
      </CardHeader>
      <CardContent className="space-y-4">
        {/* Assign new client */}
        <div className="flex gap-2">
          <Select value={selectedClientId} onValueChange={setSelectedClientId}>
            <SelectTrigger className="flex-1">
              <SelectValue placeholder="Select a client to assign" />
            </SelectTrigger>
            <SelectContent>
              {unassignedClients.length === 0 ? (
                <div className="p-2 text-sm text-muted-foreground">
                  No unassigned clients available
                </div>
              ) : (
                unassignedClients.map((client) => (
                  <SelectItem key={client.id} value={client.id}>
                    {client.name} ({client.email})
                  </SelectItem>
                ))
              )}
            </SelectContent>
          </Select>
          <Button
            onClick={handleAssign}
            disabled={!selectedClientId || assignMutation.isPending}
            size="icon"
          >
            <UserPlus className="h-4 w-4" />
          </Button>
        </div>

        {/* List of assigned clients */}
        {assignedClients.length === 0 ? (
          <div className="text-sm text-muted-foreground py-4 text-center">
            No clients assigned to this plan yet
          </div>
        ) : (
          <div className="space-y-2">
            {assignedClients.map((client) => (
              <div
                key={client.id}
                className="flex items-center justify-between p-3 border rounded-lg"
              >
                <div className="flex-1">
                  <p className="font-medium">{client.name}</p>
                  <p className="text-sm text-muted-foreground">{client.email}</p>
                </div>
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={() => handleUnassign(client.id)}
                  disabled={unassignMutation.isPending}
                >
                  <X className="h-4 w-4" />
                </Button>
              </div>
            ))}
          </div>
        )}
      </CardContent>
    </Card>
  );
}
