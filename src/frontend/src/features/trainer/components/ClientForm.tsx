"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { createClientSchema } from "@/lib/validation/user";
import type { CreateClientRequest, User } from "@/lib/types/user";
import { useCreateClient } from "../hooks/useCreateClient";
import { useUpdateClient } from "../hooks/useUpdateClient";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import { ApiError } from "@/lib/types/errors";
import { Loader2 } from "lucide-react";
import { useUserStore } from "@/lib/stores/user-store";

interface ClientFormProps {
  client?: User; // If provided, form is in edit mode
}

/**
 * ClientForm Component
 * Reusable form for creating and updating clients
 * Uses React Hook Form with Zod validation
 */
export function ClientForm({ client }: ClientFormProps) {
  const router = useRouter();
  const { currentUser } = useUserStore();
  const trainerId = currentUser?.id || "";
  
  const createMutation = useCreateClient(trainerId);
  const updateMutation = useUpdateClient(client?.id || "", trainerId);
  
  const isEditMode = !!client;
  const isLoading = createMutation.isPending || updateMutation.isPending;

  const form = useForm<CreateClientRequest>({
    resolver: zodResolver(createClientSchema),
    defaultValues: {
      name: client?.name || "",
      email: client?.email || "",
      trainerId: trainerId,
    },
  });

  async function onSubmit(data: CreateClientRequest) {
    console.log("Form submitted with data:", data);
    console.log("Trainer ID:", trainerId);
    
    if (!trainerId) {
      toast.error("No trainer selected. Please select a trainer user first.");
      return;
    }
    
    try {
      if (isEditMode && client) {
        // Update existing client
        await updateMutation.mutateAsync(data);
        toast.success("Client updated successfully");
        router.push(`/trainer/clients/${client.id}`);
      } else {
        // Create new client
        console.log("Creating client with data:", data);
        const response = await createMutation.mutateAsync(data);
        console.log("Client created successfully:", response);
        toast.success("Client created successfully");
        router.push(`/trainer/clients/${response.id}`);
      }
    } catch (error) {
      console.error("Error in onSubmit:", error);
      if (error instanceof ApiError) {
        // Handle FluentValidation errors
        const validationErrors = error.getValidationErrors();
        if (validationErrors) {
          console.log("Validation errors:", validationErrors);
          Object.entries(validationErrors).forEach(([field, messages]) => {
            form.setError(field.toLowerCase() as "name" | "email", {
              type: "server",
              message: messages[0],
            });
          });
        }
        
        // Handle Domain errors
        const domainErrors = error.getDomainErrors();
        if (domainErrors) {
          console.log("Domain errors:", domainErrors);
          toast.error(domainErrors[0]?.description || "An error occurred");
        }
      } else {
        console.error("Unexpected error:", error);
        toast.error("An unexpected error occurred");
      }
    }
  }

  return (
    <Form {...form}>
      {!trainerId && (
        <div className="rounded-md bg-yellow-500/15 p-4 text-yellow-700 dark:text-yellow-400 mb-4">
          <p className="font-semibold">Warning: No trainer selected</p>
          <p className="text-sm">
            Please select a trainer user from the user selector at the top of the page.
          </p>
        </div>
      )}
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        <FormField
          control={form.control}
          name="name"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Name</FormLabel>
              <FormControl>
                <Input
                  placeholder="Jane Smith"
                  {...field}
                  disabled={isLoading}
                />
              </FormControl>
              <FormDescription>
                The client's full name
              </FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name="email"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Email</FormLabel>
              <FormControl>
                <Input
                  type="email"
                  placeholder="jane.smith@example.com"
                  {...field}
                  disabled={isLoading}
                />
              </FormControl>
              <FormDescription>
                The client's email address
              </FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />

        <div className="flex gap-2">
          <Button type="submit" disabled={isLoading}>
            {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            {isEditMode ? "Update Client" : "Create Client"}
          </Button>
          <Button
            type="button"
            variant="outline"
            onClick={() => router.back()}
            disabled={isLoading}
          >
            Cancel
          </Button>
        </div>
      </form>
    </Form>
  );
}
