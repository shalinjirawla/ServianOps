# NSwag API Client Generation

This folder contains the configuration and scripts to automatically generate fully typed Angular HTTP API clients (Services and DTOs) from your .NET Core Backend, just like ABP Boilerplate.

## How It Works

1. **`service.config.nswag`**: This is the configuration file for NSwag Studio. It defines the swagger URL (`https://localhost:7224/swagger/v1/swagger.json`) and specifies that we want an Angular TypeScript client using RxJS and `HttpClient`.
2. **`refresh.bat`**: A simple bash script that calls the local NSwag CLI (installed in `node_modules`) to generate the services.

## Prerequisites

1. Your backend **must be running**. Start the .NET project and make sure `https://localhost:7224/swagger/v1/swagger.json` is accessible.
2. The `nswag` package must be installed in your frontend: `npm install nswag --save-dev`.

## How to generate services

Whenever you create a new Controller or change an existing one in the backend:

1. Re-run your Backend API.
2. Open a terminal in the frontend folder.
3. Run:
   ```bash
   npm run generate-api
   ```
   Or manually double-click/execute `angular/nswag/refresh.bat`.

All services and DTOs will be placed in a single, robust file:
`src/app/core/api/service-proxies.ts`

## Troubleshooting

- **Error: "Failed to load document from URL"**
  - **Solution**: The backend is not running. Start the backend project first.
- **Error: "nswag is not recognized as an internal or external command"**
  - **Solution**: Run `npm install` in the frontend directory to ensure NSwag is available via `npx`.
