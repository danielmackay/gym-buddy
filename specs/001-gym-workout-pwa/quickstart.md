# Quickstart Guide: Gym Workout Tracking PWA

**Feature**: 001-gym-workout-pwa  
**Date**: Sat Jan 17 2026

## Overview

This guide provides step-by-step instructions for implementing the Gym Workout Tracking PWA, including both backend API endpoints and frontend Next.js application.

---

## Prerequisites

- .NET 10 SDK installed
- Node.js 20+ and npm installed
- Docker Desktop running (for Aspire SQL Server)
- Git repository cloned
- IDE: VS Code, Visual Studio, or Rider

---

## Phase 1: Backend Implementation

### Step 1: Create Backend Feature Slices

Create three new feature slices using the VSA pattern following the existing Heroes/Teams structure.

#### 1.1 Users Feature Slice

```bash
cd src/api/src/WebApi/Features
mkdir -p Users/Endpoints Users/Validators
```

Create the following files in `Features/Users/`:

**UsersFeature.cs**:
```csharp
namespace GymBuddy.Api.Features.Users;

public class UsersFeature : Group
{
    public UsersFeature()
    {
        Configure("users", ep =>
        {
            ep.Description(x => x
                .WithTags("Users")
                .ProducesProblemDetails(401, 403, 404, 500));
        });
    }
}
```

**Endpoints to create**:
- `CreateTrainer.cs` - POST /api/users/trainers
- `CreateClient.cs` - POST /api/users/clients
- `ListTrainers.cs` - GET /api/users/trainers
- `ListClients.cs` - GET /api/users/clients (with trainerId query param)
- `GetUser.cs` - GET /api/users/{id}
- `UpdateUser.cs` - PUT /api/users/{id}

**Validators to create**:
- `CreateTrainerValidator.cs`
- `CreateClientValidator.cs`
- `UpdateUserValidator.cs`

#### 1.2 Exercises Feature Slice

```bash
mkdir -p Exercises/Endpoints Exercises/Validators
```

**ExercisesFeature.cs** + endpoints for create, list, get, update exercises

#### 1.3 WorkoutPlans Feature Slice

```bash
mkdir -p WorkoutPlans/Endpoints WorkoutPlans/Validators
```

**WorkoutPlansFeature.cs** + endpoints for workout plan management

### Step 2: Create Admin User Seeder

Edit `src/api/tools/MigrationService/DbInitializer.cs` (or create if it doesn't exist):

```csharp
public static async Task SeedAdminUser(ApplicationDbContext db)
{
    if (await db.Users.AnyAsync(u => u.Roles.Contains(UserRole.Admin)))
        return;

    var admin = User.Create("Admin User", "admin@gymbuddy.com");
    admin.AddRole(UserRole.Admin);
    
    db.Users.Add(admin);
    await db.SaveChangesAsync();
}
```

### Step 3: Update Database Context

Verify `ApplicationDbContext` includes:
- `DbSet<User> Users`
- `DbSet<Exercise> Exercises`
- `DbSet<WorkoutPlan> WorkoutPlans`

### Step 4: Create and Run Migration

```bash
cd src/api/src/WebApi
dotnet ef migrations add AddGymWorkoutFeatures --output-dir Common/Database/Migrations
```

### Step 5: Test Backend

```bash
cd src/api/tools/AppHost
dotnet run
```

- Aspire Dashboard opens at https://localhost:17255
- API available at https://localhost:7255/swagger
- Verify admin user seeded in database
- Test endpoints via Swagger UI

---

## Phase 2: Frontend Implementation

### Step 1: Initialize Next.js 16 Application

```bash
cd src
npx create-next-app@latest frontend --typescript --tailwind --app --no-src-dir
cd frontend
```

Answer prompts:
- TypeScript: Yes
- ESLint: Yes
- Tailwind CSS: Yes
- `/src` directory: No (use root-level directories)
- App Router: Yes
- Import alias: @/*

### Step 2: Install Dependencies

```bash
npm install zustand @tanstack/react-query react-hook-form zod @hookform/resolvers
npm install date-fns lucide-react
npm install -D @types/node
```

### Step 3: Setup PWA

```bash
npm install next-pwa
```

Update `next.config.js`:
```javascript
const withPWA = require('next-pwa')({
  dest: 'public',
  disable: process.env.NODE_ENV === 'development',
  register: true,
  skipWaiting: true,
});

module.exports = withPWA({
  reactStrictMode: true,
  env: {
    NEXT_PUBLIC_API_URL: process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7255/api',
  },
});
```

Create `public/manifest.json`:
```json
{
  "name": "Gym Buddy",
  "short_name": "Gym Buddy",
  "description": "Workout tracking application",
  "start_url": "/",
  "display": "standalone",
  "background_color": "#ffffff",
  "theme_color": "#000000",
  "icons": [
    {
      "src": "/icon-192.png",
      "sizes": "192x192",
      "type": "image/png"
    },
    {
      "src": "/icon-512.png",
      "sizes": "512x512",
      "type": "image/png"
    }
  ]
}
```

### Step 4: Setup Shadcn/ui

```bash
npx shadcn-ui@latest init
```

Answer prompts:
- Style: Default
- Base color: Slate
- CSS variables: Yes

Install required components:
```bash
npx shadcn-ui@latest add button input form card select badge toast
```

### Step 5: Create Directory Structure

```bash
mkdir -p src/app/admin/trainers
mkdir -p src/app/trainer/{clients,exercises,workout-plans}
mkdir -p src/features/{admin,trainer,exercise-library,workout-plans}
mkdir -p src/lib/{api,stores,validation,utils}
mkdir -p src/components/ui
```

### Step 6: Setup API Client

Create `src/lib/api/client.ts`:
```typescript
const API_URL = process.env.NEXT_PUBLIC_API_URL;

export class ApiError extends Error {
  constructor(public response: any, public statusCode: number) {
    super('API Error');
  }
}

export const apiClient = {
  async request<T>(endpoint: string, options?: RequestInit): Promise<T> {
    const response = await fetch(`${API_URL}${endpoint}`, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...options?.headers,
      },
    });

    if (!response.ok) {
      const error = await response.json();
      throw new ApiError(error, response.status);
    }

    return response.json();
  },
};
```

### Step 7: Setup TanStack Query

Create `src/app/providers.tsx`:
```typescript
'use client';

import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useState } from 'react';

export function Providers({ children }: { children: React.ReactNode }) {
  const [queryClient] = useState(() => new QueryClient({
    defaultOptions: {
      queries: {
        staleTime: 60 * 1000,
      },
    },
  }));

  return (
    <QueryClientProvider client={queryClient}>
      {children}
    </QueryClientProvider>
  );
}
```

Update `src/app/layout.tsx` to use providers.

### Step 8: Create Zustand Store

Create `src/lib/stores/user-store.ts`:
```typescript
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface User {
  id: string;
  name: string;
  email: string;
  roles: number[];
}

interface UserStore {
  currentUser: User | null;
  setCurrentUser: (user: User | null) => void;
}

export const useUserStore = create<UserStore>()(
  persist(
    (set) => ({
      currentUser: null,
      setCurrentUser: (user) => set({ currentUser: user }),
    }),
    {
      name: 'gym-buddy-user',
    }
  )
);
```

### Step 9: Create TypeScript Models

Create files in `src/lib/types/`:
- `user.ts` - User models (see data-model.md)
- `exercise.ts` - Exercise models
- `workout-plan.ts` - Workout plan models
- `errors.ts` - Error models

### Step 10: Create API Service Layer

Create files in `src/lib/api/`:
- `users.ts` - User CRUD operations
- `exercises.ts` - Exercise CRUD operations
- `workout-plans.ts` - Workout plan CRUD operations

Example `users.ts`:
```typescript
import { apiClient } from './client';
import type { User, CreateTrainerRequest, CreateTrainerResponse } from '../types/user';

export const usersApi = {
  listTrainers: () => apiClient.request<User[]>('/users/trainers'),
  createTrainer: (data: CreateTrainerRequest) =>
    apiClient.request<CreateTrainerResponse>('/users/trainers', {
      method: 'POST',
      body: JSON.stringify(data),
    }),
  // ... other endpoints
};
```

### Step 11: Create Feature Components

For each feature (admin, trainer, exercise-library, workout-plans), create:
- Components (list, form, dashboard)
- Custom hooks using TanStack Query
- Type definitions

Example hook pattern:
```typescript
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { usersApi } from '@/lib/api/users';

export function useTrainers() {
  return useQuery({
    queryKey: ['trainers'],
    queryFn: usersApi.listTrainers,
  });
}

export function useCreateTrainer() {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: usersApi.createTrainer,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['trainers'] });
    },
  });
}
```

### Step 12: Create Pages

Create pages in `src/app/`:
- `app/page.tsx` - User selection screen
- `app/admin/page.tsx` - Admin dashboard
- `app/admin/trainers/page.tsx` - Trainer list
- `app/trainer/page.tsx` - Trainer dashboard
- `app/trainer/clients/page.tsx` - Client list
- `app/trainer/exercises/page.tsx` - Exercise library
- `app/trainer/workout-plans/page.tsx` - Workout plans

### Step 13: Run Frontend

```bash
npm run dev
```

Frontend available at http://localhost:3000

---

## Phase 3: Testing & Validation

### Backend Testing

1. Test via Swagger UI at https://localhost:7255/swagger
2. Verify admin user exists
3. Create trainer, then client
4. Create exercise and workout plan
5. Assign plan to client

### Frontend Testing

1. Select admin user from user selector
2. Navigate to trainer management
3. Create a new trainer
4. Switch to trainer user
5. Create client, exercises, and workout plan
6. Assign plan to client

### PWA Testing

1. Build for production: `npm run build`
2. Start production server: `npm start`
3. Open Chrome DevTools > Application > Service Workers
4. Verify service worker registered
5. Go offline and verify cached data accessible
6. Test "Add to Home Screen" functionality

---

## Environment Variables

### Backend (.NET)

Set in Aspire configuration or User Secrets:
- `ConnectionStrings:DefaultConnection` - Set by Aspire automatically

### Frontend (Next.js)

Create `.env.local`:
```bash
NEXT_PUBLIC_API_URL=https://localhost:7255/api
```

For production:
```bash
NEXT_PUBLIC_API_URL=https://api.gymbuddy.com/api
```

---

## Deployment

### Backend

```bash
cd src/api/tools/AppHost
dotnet publish -c Release
```

Deploy to Azure Container App (per constitution)

### Frontend

```bash
cd src/frontend
npm run build
```

Deploy to Vercel:
```bash
vercel --prod
```

---

## Troubleshooting

### Backend Issues

- **Database connection fails**: Ensure Docker is running for Aspire SQL Server
- **Migration errors**: Delete migrations folder and recreate
- **Swagger not loading**: Check HTTPS certificate trust

### Frontend Issues

- **API calls fail with CORS**: Configure CORS in backend Program.cs
- **Service worker not registering**: Check HTTPS in production
- **Build fails**: Verify all TypeScript types are correct

---

## Next Steps

After basic implementation:

1. Add error handling and loading states
2. Implement optimistic updates with TanStack Query
3. Add form validation with Zod
4. Improve mobile UI/UX
5. Add E2E tests with Playwright
6. Configure Auth0 for real authentication
7. Add client-facing features (sessions, history)

---

## Resources

- [Feature Specification](./spec.md)
- [Data Model](./data-model.md)
- [API Contracts](./contracts/)
- [Research Decisions](./research.md)
- [Constitution](./../.specify/memory/constitution.md)
