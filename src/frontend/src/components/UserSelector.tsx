"use client";

import { useUserStore } from "@/lib/stores/user-store";
import { User, UserRole } from "@/lib/types/user";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";

interface UserSelectorProps {
  users: User[];
  onUserSelect?: (user: User) => void;
}

/**
 * User selector component for no-auth user switching
 * Allows selecting between Admin, Trainer, and Client users
 */
export function UserSelector({ users, onUserSelect }: UserSelectorProps) {
  const { currentUser, setCurrentUser } = useUserStore();

  const handleSelectUser = (userId: string) => {
    const user = users.find((u) => u.id === userId);
    if (user) {
      setCurrentUser(user);
      onUserSelect?.(user);
    }
  };

  const getUserRoleLabel = (roles: UserRole[]): string => {
    if (roles.includes(UserRole.Admin)) return "Admin";
    if (roles.includes(UserRole.Trainer)) return "Trainer";
    if (roles.includes(UserRole.Client)) return "Client";
    return "User";
  };

  if (users.length === 0) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>No Users Found</CardTitle>
          <CardDescription>
            There are no users in the system. Please seed the database first.
          </CardDescription>
        </CardHeader>
      </Card>
    );
  }

  return (
    <Card className="w-full max-w-md">
      <CardHeader>
        <CardTitle>Select User</CardTitle>
        <CardDescription>
          Choose a user to continue (authentication will be added later)
        </CardDescription>
      </CardHeader>
      <CardContent>
        <Select
          value={currentUser?.id}
          onValueChange={handleSelectUser}
        >
          <SelectTrigger className="w-full">
            <SelectValue placeholder="Select a user..." />
          </SelectTrigger>
          <SelectContent>
            {users.map((user) => (
              <SelectItem key={user.id} value={user.id}>
                {user.name} ({getUserRoleLabel(user.roles)}) - {user.email}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </CardContent>
    </Card>
  );
}
