# Phase 0: Research & Architecture Decisions

**Feature**: Gym Workout Tracking PWA  
**Date**: Sat Jan 17 2026  
**Status**: Complete

## Overview

This document consolidates architectural decisions and best practices research for building a mobile-first PWA with Next.js 16 that connects to the existing .NET backend API. All decisions align with the Constitution principles and leverage existing infrastructure.

---

## 1. Frontend Framework & PWA Strategy

### Decision: Next.js 16 with next-pwa

**Rationale**:
- Next.js 16 (React 19) provides server-side rendering, App Router, and excellent mobile performance
- `next-pwa` plugin generates service workers automatically for offline capability
- Vercel deployment (per constitution) provides optimal CDN and edge network for mobile users
- Built-in TypeScript support matches backend type safety approach

**Alternatives Considered**:
- **Vite + React**: Lighter but lacks SSR benefits for initial mobile load performance
- **Create React App**: Deprecated, not recommended for new projects
- **Remix**: Good but less mature PWA ecosystem compared to Next.js

**Implementation Details**:
- Use App Router (not Pages Router) for better performance and cleaner code organization
- Configure `next-pwa` for workbox-based service worker generation
- Implement offline-first strategy for viewing cached data (read-only)
- Target Lighthouse PWA score 90+ per constitution requirement

**References**:
- [Next.js 16 Documentation](https://nextjs.org/docs)
- [next-pwa GitHub](https://github.com/shadowwalker/next-pwa)
- [PWA Best Practices - web.dev](https://web.dev/progressive-web-apps/)

---

## 2. State Management Strategy

### Decision: Zustand + TanStack Query (React Query)

**Rationale**:
- **Zustand** for client-side state (current user selection, UI state): Lightweight, minimal boilerplate, TypeScript-friendly
- **TanStack Query** for server state (API data): Automatic caching, background refetching, optimistic updates, error handling
- Separation of concerns: server state vs client state
- Both libraries have excellent mobile performance and small bundle sizes

**Alternatives Considered**:
- **Redux Toolkit**: Overkill for this scope, more boilerplate, larger bundle size
- **Jotai/Recoil**: Good but TanStack Query already handles most state needs
- **Context API only**: No caching or refetching strategies, would need manual implementation

**Implementation Pattern**:
```typescript
// Zustand for client state
const useUserStore = create((set) => ({
  currentUser: null,
  setCurrentUser: (user) => set({ currentUser: user }),
}));

// TanStack Query for server state
const useTrainers = () => {
  return useQuery({
    queryKey: ['trainers'],
    queryFn: () => api.users.listTrainers(),
  });
};
```

**References**:
- [Zustand Documentation](https://github.com/pmndrs/zustand)
- [TanStack Query Documentation](https://tanstack.com/query/latest)

---

## 3. Form Handling & Validation

### Decision: React Hook Form + Zod

**Rationale**:
- **React Hook Form**: Minimal re-renders, excellent performance on mobile, small bundle size
- **Zod**: Type-safe schema validation that matches backend FluentValidation patterns
- Integration allows sharing validation rules between client and server
- Native support for showing backend validation errors per requirement

**Alternatives Considered**:
- **Formik**: Larger bundle, more re-renders, older architecture
- **React Final Form**: Good but less active maintenance
- **Manual forms**: Error-prone, lots of boilerplate

**Error Handling Strategy**:
- Display FluentValidation errors from backend API responses
- Display Domain errors (from ErrorOr pattern) with field-specific messages
- Generic error message for unexpected errors per user requirement
- Toast notifications for success/error feedback

**Implementation Pattern**:
```typescript
const schema = z.object({
  name: z.string().min(1).max(100),
  email: z.string().email().max(256),
});

const { register, handleSubmit, setError, formState: { errors } } = useForm({
  resolver: zodResolver(schema),
});

// Handle backend errors
if (response.errors) {
  response.errors.forEach(err => {
    if (err.code === 'Validation.Email') {
      setError('email', { message: err.description });
    }
  });
}
```

**References**:
- [React Hook Form Documentation](https://react-hook-form.com/)
- [Zod Documentation](https://zod.dev/)

---

## 4. UI Component Library

### Decision: Shadcn/ui + Tailwind CSS

**Rationale**:
- **Shadcn/ui**: Copy-paste components built on Radix UI primitives, full customization, excellent accessibility
- **Tailwind CSS**: Mobile-first by default, utility-first approach, small production bundle
- Radix UI provides touch-friendly primitives (44x44px minimum) per constitution
- No runtime dependency - components are copied into project for full control

**Alternatives Considered**:
- **Material UI**: Too opinionated, larger bundle size, not mobile-first by default
- **Chakra UI**: Good but runtime CSS-in-JS impacts mobile performance
- **Headless UI**: Good but less comprehensive than Shadcn/ui ecosystem

**Mobile-First Strategy**:
- Start with mobile layouts (320px minimum width)
- Use Tailwind breakpoints (`sm:`, `md:`, `lg:`) for progressive enhancement
- Touch targets minimum 44x44px (Radix UI default)
- Bottom navigation for mobile, sidebar for desktop

**Key Components Needed**:
- Button, Input, Form, Card, Select, Badge, Toast, Dialog, Sheet (mobile drawer)
- All components include proper ARIA labels and keyboard navigation

**References**:
- [Shadcn/ui Documentation](https://ui.shadcn.com/)
- [Radix UI Primitives](https://www.radix-ui.com/)
- [Tailwind CSS Documentation](https://tailwindcss.com/)

---

## 5. Backend API Integration

### Decision: FastEndpoints + ErrorOr Pattern Integration

**Rationale**:
- Backend already uses FastEndpoints with ErrorOr pattern
- All validation errors use FluentValidation
- Domain errors use ErrorOr<T> pattern with structured error codes
- Frontend must parse and display these error structures appropriately

**Error Response Format**:
```csharp
// FluentValidation error
{
  "errors": {
    "Name": ["Name is required", "Name must be between 1 and 100 characters"],
    "Email": ["Email is invalid"]
  }
}

// Domain error (ErrorOr)
{
  "errors": [{
    "code": "User.AlreadyHasRole",
    "description": "User already has this role",
    "type": "Validation"
  }]
}
```

**Frontend API Client Strategy**:
- Typed API client using TypeScript interfaces matching C# DTOs
- TanStack Query for all GET requests with automatic caching
- Mutations for POST/PUT/DELETE with optimistic updates where appropriate
- Error interceptor to parse FluentValidation and ErrorOr responses

**Implementation**:
```typescript
// API client base
const apiClient = {
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
      throw new ApiError(error);
    }

    return response.json();
  },
};
```

**References**:
- [FastEndpoints Documentation](https://fast-endpoints.com/)
- [ErrorOr GitHub](https://github.com/amantinband/error-or)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)

---

## 6. Offline PWA Strategy

### Decision: Cache-First for Read Operations, Online-Only for Writes

**Rationale**:
- Users need to view workout plans offline (per FR-019)
- Write operations require backend validation and domain logic
- Service worker caches API responses and static assets
- Background sync for failed requests (future enhancement)

**Caching Strategy**:
- **Static assets** (JS, CSS, images): Cache-first with stale-while-revalidate
- **API GET requests**: Network-first with cache fallback
- **API POST/PUT/DELETE**: Network-only (no offline writes in v1)
- **User selection state**: localStorage for persistence

**Service Worker Configuration** (next-pwa):
```javascript
// next.config.js
const withPWA = require('next-pwa')({
  dest: 'public',
  disable: process.env.NODE_ENV === 'development',
  register: true,
  skipWaiting: true,
  runtimeCaching: [
    {
      urlPattern: /^https:\/\/api\.gymbuddy\.com\/api\/.*/i,
      handler: 'NetworkFirst',
      options: {
        cacheName: 'api-cache',
        expiration: {
          maxEntries: 100,
          maxAgeSeconds: 60 * 60 * 24, // 24 hours
        },
      },
    },
  ],
});
```

**Limitations** (per spec assumptions):
- Offline support limited to viewing cached data
- No offline editing or creation
- Network connectivity required for all write operations

**References**:
- [Workbox Strategies](https://developer.chrome.com/docs/workbox/modules/workbox-strategies/)
- [PWA Offline Cookbook](https://web.dev/offline-cookbook/)

---

## 7. User Authentication Approach (No-Auth MVP)

### Decision: Simple User Selection with localStorage

**Rationale**:
- Per specification: "Authentication will be added later"
- Pre-configured admin user exists in backend
- Trainers and clients created via API calls
- Simple user selection dropdown to switch between users
- Store selected user in localStorage and Zustand

**Implementation**:
```typescript
// User selection component
const UserSelector = () => {
  const { currentUser, setCurrentUser } = useUserStore();
  const { data: users } = useQuery(['users'], () => api.users.listAll());

  return (
    <Select value={currentUser?.id} onChange={(id) => {
      const user = users.find(u => u.id === id);
      setCurrentUser(user);
      localStorage.setItem('currentUser', JSON.stringify(user));
    }}>
      {users?.map(user => (
        <option key={user.id} value={user.id}>
          {user.name} ({user.roles.join(', ')})
        </option>
      ))}
    </Select>
  );
};
```

**Security Considerations**:
- This is NOT secure - only for MVP demonstration
- All users can access all data (no role enforcement on frontend)
- Backend endpoints should not enforce authorization yet (per spec)
- Future enhancement: Auth0 integration per constitution

**References**:
- Constitution Section: "Deployment & Infrastructure" mentions Auth0 for future

---

## 8. Mobile-First Design System

### Decision: Design Tokens + Tailwind + 320px Minimum

**Rationale**:
- Constitution requires mobile-first with 320px minimum width
- Touch targets minimum 44x44px
- Thumb-reachable zones prioritized
- Progressive enhancement for larger screens

**Breakpoints** (Tailwind default):
- `sm`: 640px (large phones, small tablets)
- `md`: 768px (tablets)
- `lg`: 1024px (desktop)
- `xl`: 1280px (large desktop)

**Touch Target Guidelines**:
- Buttons: minimum 44x44px
- Form inputs: minimum 44px height
- List items: minimum 44px tap area
- Navigation items: minimum 44px

**Layout Patterns**:
- **Mobile**: Single column, bottom navigation, full-width cards
- **Tablet**: Two columns where appropriate, sidebar navigation option
- **Desktop**: Multi-column grids, persistent sidebar, more information density

**Performance Budget**:
- Initial bundle: < 200KB gzipped
- First Contentful Paint: < 1.5s on 3G
- Time to Interactive: < 3s on 3G
- Lighthouse Performance: 90+
- Lighthouse PWA: 90+

**References**:
- [Mobile-First Design - web.dev](https://web.dev/mobile-first/)
- [Touch Target Size](https://web.dev/accessible-tap-targets/)

---

## 9. Backend Feature Slice Organization

### Decision: 3 New Feature Slices (Users, Exercises, WorkoutPlans)

**Rationale**:
- Follows existing pattern from Heroes and Teams features
- Each slice is self-contained with endpoints, validators, and feature registration
- Uses existing Domain entities (no new domain models needed)
- FastEndpoints auto-discovery eliminates manual registration

**Slice Structure** (example: Users):
```
Features/Users/
├── Endpoints/
│   ├── CreateTrainer.cs       // POST /api/users/trainers
│   ├── CreateClient.cs         // POST /api/users/clients
│   ├── ListTrainers.cs         // GET /api/users/trainers
│   ├── ListClients.cs          // GET /api/users/clients
│   ├── GetUser.cs              // GET /api/users/{id}
│   └── UpdateUser.cs           // PUT /api/users/{id}
├── Validators/
│   ├── CreateTrainerValidator.cs
│   ├── CreateClientValidator.cs
│   └── UpdateUserValidator.cs
└── UsersFeature.cs             // Feature group registration
```

**Endpoint Pattern**:
```csharp
public class CreateTrainer : Endpoint<CreateTrainerRequest, CreateTrainerResponse>
{
    private readonly ApplicationDbContext _db;

    public CreateTrainer(ApplicationDbContext db) => _db = db;

    public override void Configure()
    {
        Post("/api/users/trainers");
        Group<UsersFeature>();
        Summary(s => s.Summary = "Create a new trainer user");
    }

    public override async Task HandleAsync(
        CreateTrainerRequest req,
        CancellationToken ct)
    {
        var user = User.Create(req.Name, req.Email);
        var addRoleResult = user.AddRole(UserRole.Trainer);

        if (addRoleResult.IsError)
        {
            await SendErrorsAsync(addRoleResult.Errors);
            return;
        }

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        await SendAsync(new CreateTrainerResponse { Id = user.Id.Value }, 201, ct);
    }
}
```

**References**:
- [FastEndpoints VSA Guide](https://fast-endpoints.com/docs/vertical-slice-architecture)
- Existing code: `src/api/src/WebApi/Features/Heroes/`

---

## 10. Database Seeding Strategy

### Decision: AdminUserSeeder in MigrationService

**Rationale**:
- Per FR-001: "System MUST come pre-configured with one admin user"
- Aspire automatically runs MigrationService on startup
- Check for existing admin before seeding (idempotent)
- Use Bogus for generating realistic seed data

**Implementation**:
```csharp
public class AdminUserSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        // Check if admin exists
        if (await db.Users.AnyAsync(u => u.Roles.Contains(UserRole.Admin)))
            return;

        // Create pre-configured admin
        var admin = User.Create("Admin User", "admin@gymbuddy.com");
        admin.AddRole(UserRole.Admin);

        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}
```

**Seed Data Scope**:
- 1 pre-configured admin user
- Optional: Sample exercises for demo purposes
- Optional: Sample trainers and clients for development

**References**:
- [EF Core Data Seeding](https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding)
- [Bogus GitHub](https://github.com/bchavez/Bogus)

---

## 11. Error Handling & User Feedback

### Decision: Toast Notifications + Inline Form Errors

**Rationale**:
- Per requirement: "Any errors from fluent validation or the domain should be shown to the user"
- Per requirement: "Other errors should show a generic error message"
- Mobile-friendly toast notifications for global feedback
- Inline errors for form field validation

**Error Categories**:
1. **FluentValidation errors**: Display per-field with red text below input
2. **Domain errors** (ErrorOr): Display as form-level error or toast
3. **Network errors**: Generic "Something went wrong" toast
4. **Unexpected errors**: Generic error with error boundary fallback

**Implementation**:
```typescript
// Toast for general errors
toast.error("Failed to create trainer. Please try again.");

// Inline form errors
<Input 
  {...register('email')}
  error={errors.email?.message}
/>
{errors.email && (
  <p className="text-sm text-red-500">{errors.email.message}</p>
)}

// Error boundary for catastrophic failures
<ErrorBoundary fallback={<GenericErrorPage />}>
  <App />
</ErrorBoundary>
```

**Toast Library**: Use Shadcn/ui Toast component (built on Radix UI Toast)

**References**:
- [React Hook Form Error Handling](https://react-hook-form.com/api/useform/seterror)
- [Radix UI Toast](https://www.radix-ui.com/docs/primitives/components/toast)

---

## 12. Development & Deployment Workflow

### Decision: Aspire for Backend, Vercel for Frontend

**Rationale**:
- Backend already uses Aspire for local development (per constitution)
- Vercel provides optimal Next.js deployment with edge network
- Separate deployment allows frontend/backend to scale independently

**Local Development**:
```bash
# Run entire stack (backend + frontend)
aspire run  # Starts Aspire dashboard, API, database, and frontend

# Or run individually:
# Backend (from tools/AppHost/)
dotnet run  # Starts Aspire dashboard, API, database

# Frontend (from src/frontend/)
npm run dev  # Starts Next.js dev server on http://localhost:3000
```

**Environment Variables**:
```bash
# Frontend .env.local
NEXT_PUBLIC_API_URL=https://localhost:7255
```

**Production Deployment**:
- **Backend**: Azure Container App (per constitution)
- **Frontend**: Vercel with custom domain
- **Database**: Azure SQL Database (per constitution)
- **Monitoring**: Application Insights + OpenTelemetry

**References**:
- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Vercel Deployment](https://nextjs.org/docs/deployment)

---

## Summary of Key Decisions

| Area | Decision | Rationale |
|------|----------|-----------|
| Frontend Framework | Next.js 16 + next-pwa | SSR performance, PWA support, Vercel deployment |
| State Management | Zustand + TanStack Query | Separation of client/server state, minimal boilerplate |
| Forms & Validation | React Hook Form + Zod | Performance, type safety, backend error integration |
| UI Components | Shadcn/ui + Tailwind | Mobile-first, accessibility, customization |
| Backend Architecture | FastEndpoints + VSA | Follows existing pattern, constitution compliance |
| Error Handling | Toast + Inline Errors | User-friendly, matches backend error patterns |
| Offline Strategy | Cache-first for reads | PWA requirement, limited to viewing |
| Authentication | User selection (no auth) | MVP approach per spec, Auth0 future |
| Deployment | Vercel + Azure | Constitution-specified infrastructure |

---

**Status**: ✅ Research complete - all architectural decisions documented and justified.

**Next Phase**: Phase 1 - Data Model & API Contracts
