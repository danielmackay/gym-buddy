import { Button } from "@/components/ui/button";
import { TrainerList } from "@/features/admin/components/TrainerList";
import { UserPlus } from "lucide-react";
import Link from "next/link";

export default function TrainersPage() {
  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Trainers</h1>
          <p className="text-muted-foreground">
            Manage trainer accounts and permissions
          </p>
        </div>
        <Link href="/admin/trainers/new">
          <Button>
            <UserPlus className="mr-2 h-4 w-4" />
            Add Trainer
          </Button>
        </Link>
      </div>

      <TrainerList />
    </div>
  );
}
