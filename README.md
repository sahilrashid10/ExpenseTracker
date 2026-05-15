# Expense Tracker API

RESTful ASP.NET Core Web API plus Razor frontend for tracking employee expenses, applying categories, and analyzing spending patterns.

## Features

- Create, read, update, and delete expenses
- Razor dashboard, create form, and edit form
- JSON file persistence with automatic load and save behavior
- Seeded sample values on startup for the local app
- Filtering by category, date range, and title search
- Sorting by date or amount
- Pagination controls in the dashboard
- Expense summary endpoint with total, category totals, and monthly totals
- Global exception middleware
- Custom `ExpenseNotFoundException`
- Async service and repository operations

## Data Model

- `Id` - `Guid`
- `Title` - required
- `Amount` - required, greater than zero
- `Category` - `Food`, `Travel`, `Bills`, `Shopping`, `Other`
- `ExpenseDate` - `DateTime`
- `PaymentMode` - `Cash`, `Card`, `UPI`
- `Notes` - optional
- `CreatedAt` - auto-generated

## Endpoints

- `GET /expenses`
- `GET /expenses/{id}`
- `POST /expenses`
- `PUT /expenses/{id}`
- `DELETE /expenses/{id}`
- `GET /expenses/summary`
- `GET /` Razor dashboard
- `GET /expenses/create` Razor create page
- `GET /expenses/edit/{id}` Razor edit page

## Run

```bash
dotnet restore
dotnet test
dotnet run --project ExpenseTracker.Api
```

## Test Coverage

- Add valid expense returns `201 Created`
- Add invalid expense returns `400 Bad Request`
- Get all expenses returns `200 OK`
- Get missing expense returns `404 Not Found`
- Delete expense returns `204 No Content`
- Filter by category returns `200 OK`
- Summary returns total expense calculation
- Dashboard supports search, filtering, sorting, and pagination

## Copilot Utilization Report

- Used Copilot to scaffold the layered architecture and data contracts.
- Used Copilot to draft the JSON-backed repository, exception middleware, and controller actions.
- Used Copilot to generate the integration test suite and README structure, then adjusted the implementation to match the assignment rules.