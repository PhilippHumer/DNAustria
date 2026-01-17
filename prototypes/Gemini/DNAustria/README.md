# DNAustria - University Event Management System

A world-class web application for managing university events, contacts, and organizations.

## Features

- **Event Management**: Create, update, delete, and view events.
- **Contact Management**: Manage contacts associated with events.
- **Organization Management**: Manage organizations hosting events.
- **LLM Import**: Import events from unstructured text using AI (Mocked).
- **Address Reuse**: Intelligent address management to avoid duplicates.

## Tech Stack

- **Backend**: .NET 8 Web API, Entity Framework Core, PostgreSQL
- **Frontend**: Angular 19, Angular Material, Tailwind-like Utility Classes
- **Database**: PostgreSQL 17 (Docker)

## Prerequisites

- .NET 8 SDK
- Node.js (v18+)
- Docker & Docker Compose

## Getting Started

### 1. Start the Database

```bash
docker-compose up -d
```

### 2. Run the Backend

```bash
cd backend/DNAustria.API
dotnet run
```
The API will be available at `http://localhost:5088`.

### 3. Run the Frontend

```bash
cd frontend
npm install
npm start
```
The application will be available at `http://localhost:4200`.

## Project Structure

- `backend/`: .NET Web API solution
- `frontend/`: Angular application
- `docker-compose.yml`: Database configuration

## LLM Import Feature

To test the LLM import:
1. Go to "Events" -> "Create Event".
2. Click "Import from Text".
3. Paste a text like "Tech Conference next Friday at 10am in Vienna".
4. Click "Import".
5. The form will be populated with extracted data.
