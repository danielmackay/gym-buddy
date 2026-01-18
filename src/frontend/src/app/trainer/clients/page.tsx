import { ClientList } from "@/features/trainer/components/ClientList";
import { Button } from "@/components/ui/button";
import Link from "next/link";
import { UserPlus } from "lucide-react";

/**
 * Client List Page
 * Displays all clients for the current trainer
 */
export default function ClientsPage() {
  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Clients</h1>
          <p className="text-muted-foreground">
            Manage your client accounts
          </p>
        </div>
        <Link href="/trainer/clients/new">
          <Button>
            <UserPlus className="mr-2 h-4 w-4" />
            New Client
          </Button>
        </Link>
      </div>

      <ClientList />
    </div>
  );
}
