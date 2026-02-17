# OnlineJobPortal (.NET 8 MVC)

## Tech Stack
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core (Code First)
- SQL Server
- ASP.NET Core Identity with Roles (Admin, Employer, JobSeeker)
- Bootstrap 5 + Chart.js
- Repository Pattern + ViewModels

## Project Structure
- `Controllers/`
- `Models/`
- `ViewModels/`
- `Data/`
- `Repositories/`
- `Services/`
- `Middleware/`
- `Views/`
- `wwwroot/uploads`

## Step-by-step Setup
1. Install .NET 8 SDK and SQL Server.
2. Update connection string in `appsettings.json`.
3. Restore packages:
   ```bash
   dotnet restore
   ```
4. Create migration and update database:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
5. Run app:
   ```bash
   dotnet run
   ```
6. Login with seeded admin:
   - `admin@onlinejobportal.com`
   - `Admin@12345`

## Security Implemented
- Role-based authorization
- Anti-forgery token on forms
- Password policy enforcement
- Duplicate application prevention (unique index + check)
- File type/size validation for resumes and logos
- HTTPS redirection
- Global exception middleware

## Deployment (IIS)
1. Install .NET Hosting Bundle on server.
2. Publish:
   ```bash
   dotnet publish -c Release -o ./publish
   ```
3. Create IIS site pointing to `publish` folder.
4. Set App Pool to `No Managed Code`.
5. Configure SQL connection string in `appsettings.Production.json`.

## Notes
- Forgot Password UI is scaffolded; wire SMTP/email sender for production reset flow.
- For blob/object storage, replace local upload strategy with cloud storage provider.
