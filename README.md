# JobQuest.Web
JobSeeker Portal with .NET and Entity Framework Core

## Setting Up the JobQuest Database

1. Clone the repository.
2. Configure the `appsettings.json` file with your database connection details.
3. Run the following commands to apply migrations and set up the database:

   ```bash
   dotnet ef database update
