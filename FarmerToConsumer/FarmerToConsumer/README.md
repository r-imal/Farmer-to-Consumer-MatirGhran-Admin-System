# MatirGhran Admin Part

This folder contains the admin-focused portion of the project with the supporting code needed to compile and run the admin dashboard as a standalone preview.

Included:
- Admin controllers: dashboard, agent approval/assignment, employee/user management, payments
- Admin views and shared admin layout/sidebar/message partials
- Required data, entity, repo, and shared projects
- Required static assets without `bin`, `obj`, `.vs`, logs, or zip artifacts

Build from this folder:

```powershell
dotnet build FarmerToConsumer.Web.sln
```

Run the preview without login:

```powershell
cd FarmerToConsumer.Web
dotnet run --urls http://localhost:36474
```

Main admin route:

```text
http://localhost:36474/
```
