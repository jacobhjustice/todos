# Todo Sample
## What?
- A bare-bones todo api
- An example of my current preferred dotnet architecture

## Overview
- Todos.API: The entrypoint into the API
  - Currently only holds controllers
  - If any other entrypoint like a queue were to be added, it should show up here as well
  - Main responsibility is handing off requests to the logic layer, and translating the results into a response object
- Todos.API.Logic: The logic layer of the API
  - Responsible for orchestrating validations and data layer changes
  - Note: A handler should only ever write to its own data layer repository, but can call a different handler if needed
- Todos.DTOs: The Request/Response structures that a consuming client should expect to send/receive
- Todos.Repositories: The data layer of the application
  - Note: Serves as a wrapper around EntityFramework to limit data operations from taking place all over the app
- Todos.Models: The underlying data models of the API
  - Note: These should never be returned directly to the client since the client does not have a "contract" with the database model directly
  - Note: EF migrations are generated here and should be run to create the expected database state
- Todos.Validations: The set of FluentValidations used by the logic layer
- Todos.Utils: The set of utility methods and custom boilerplate code the API uses 
  - Note: Nothing in this project should be Todo domain specific
  - Note: These would ideally be moved to NuGet packages eventually
  - 
## Setup
1) Modify appsettings.json to provide a connection string to local sql server
2) Run the app (it will automatically run migrations and generate the database)
3) Optional: Install Insomnia and pull this repo through the Insomnia client for API debugging