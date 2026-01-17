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
import { createTrainerSchema } from "@/lib/validation/user";
import type { CreateTrainerRequest, User } from "@/lib/types/user";
import { useCreateTrainer } from "../hooks/useCreateTrainer";
import { useUpdateTrainer } from "../hooks/useUpdateTrainer";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import { ApiError } from "@/lib/types/errors";
import { Loader2 } from "lucide-react";

interface TrainerFormProps {
  trainer?: User; // If provided, form is in edit mode
}

/**
 * TrainerForm Component
 * Reusable form for creating and updating trainers
 * Uses React Hook Form with Zod validation
 */
export function TrainerForm({ trainer }: TrainerFormProps) {
  const router = useRouter();
  const createMutation = useCreateTrainer();
  const updateMutation = useUpdateTrainer();
  
  const isEditMode = !!trainer;
  const isLoading = createMutation.isPending || updateMutation.isPending;

  const form = useForm<CreateTrainerRequest>({
    resolver: zodResolver(createTrainerSchema),
    defaultValues: {
      name: trainer?.name || "",
      email: trainer?.email || "",
    },
  });

  async function onSubmit(data: CreateTrainerRequest) {
    try {
      if (isEditMode && trainer) {
        // Update existing trainer
        await updateMutation.mutateAsync({ id: trainer.id, data });
        toast.success("Trainer updated successfully");
        router.push(`/admin/trainers/${trainer.id}`);
      } else {
        // Create new trainer
        const response = await createMutation.mutateAsync(data);
        toast.success("Trainer created successfully");
        router.push(`/admin/trainers/${response.id}`);
      }
    } catch (error) {
      if (error instanceof ApiError) {
        // Handle FluentValidation errors
        const validationErrors = error.getValidationErrors();
        if (validationErrors) {
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
          toast.error(domainErrors[0]?.description || "An error occurred");
        }
      } else {
        toast.error("An unexpected error occurred");
      }
    }
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        <FormField
          control={form.control}
          name="name"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Name</FormLabel>
              <FormControl>
                <Input
                  placeholder="John Doe"
                  {...field}
                  disabled={isLoading}
                />
              </FormControl>
              <FormDescription>
                The trainer's full name
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
                  placeholder="john.doe@example.com"
                  {...field}
                  disabled={isLoading}
                />
              </FormControl>
              <FormDescription>
                The trainer's email address
              </FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />

        <div className="flex gap-2">
          <Button type="submit" disabled={isLoading}>
            {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            {isEditMode ? "Update Trainer" : "Create Trainer"}
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
