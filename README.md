# Expense Tracker Web API

A REST API and simple UI for managing expense categories and recording expenses, with a bar chart showing total expenses by category.

## Setup Instructions

### Prerequisites

- .NET 8 SDK
- SQL Server (e.g. SQL Server Express) with the instance name matching the connection string, or update the connection string to match your environment.

### Steps

1. **Clone or open the solution**
   - Open the folder in Visual Studio or from command line.

2. **Connection string**
   - The default connection string is in `ExpenseTrackerAPI/appsettings.json`:
   ```json
   "DefaultConnection": "Server=XIT096\\SQLEXPRESS;Initial Catalog=ExpenseTracker_Db;Integrated Security=True;MultipleActiveResultSets=true;TrustServerCertificate=true;"
   ```
   - Change `Server=XIT096\\SQLEXPRESS` to your SQL Server instance (e.g. `Server=.\SQLEXPRESS` or `Server=(localdb)\mssqllocaldb`) if needed.

3. **Restore and run**
   ```bash
   cd ExpenseTrackerAPI
   dotnet restore
   dotnet run
   ```
   - On first run, the application applies EF Core migrations and creates the database `ExpenseTracker_Db` and tables if they do not exist.

4. **Access the application**
   - API base URL: `https://localhost:7xxx` or `http://localhost:5xxx` (see console output for the actual port).
   - **Dashboard** (chart): `https://localhost:7xxx/` or `index.html`
   - **Category Management**: `https://localhost:7xxx/categories.html`
   - **Expense Management**: `https://localhost:7xxx/expenses.html`
   - **Swagger UI**: `https://localhost:7xxx/swagger`

---

## Assumptions

- **Authentication**: No authentication or authorization; the API is open. Suitable for local or trusted environment use.
- **Date handling**: “Expense date cannot be in the future” is enforced in UTC date comparison (same calendar day in UTC). For local-time boundaries, the server time zone could be considered in a future iteration.
- **Category name**: Updating a category name only updates the `Category` entity; existing expenses keep their `CategoryId` and display the new name via the relation (no data migration needed).
- **Delete category**: Deleting a category is disallowed if any expense references it; the API returns 400 with a clear message.
- **Amount**: Stored as `decimal(18,2)`; validation requires amount &gt; 0.
- **UI**: Dashboard (chart), Category Management (full CRUD), and Expense Management (add, list, delete) pages in `wwwroot`; all use the REST API.

---

## API Endpoint List

Base path for all endpoints: `/api`.

### Category

| Method | Endpoint             | Description                    |
|--------|----------------------|--------------------------------|
| POST   | `/categories`        | Create category                |
| GET    | `/categories`        | Get all categories             |
| GET    | `/categories/{id}`   | Get category by id             |
| PUT    | `/categories/{id}`   | Update category                |
| DELETE | `/categories/{id}`   | Delete category (fails if has expenses) |

### Expense

| Method | Endpoint                      | Description                          |
|--------|-------------------------------|--------------------------------------|
| POST   | `/expenses`                   | Add expense                          |
| GET    | `/expenses`                   | Get all expenses                     |
| GET    | `/expenses/{id}`              | Get expense by id                    |
| GET    | `/expenses/category/{categoryId}` | Get expenses by category         |
| GET    | `/expenses/summary?categoryId={id}` | Get total by category (for chart; `categoryId` optional) |
| DELETE | `/expenses/{id}`              | Delete expense                       |

### Request/Response Examples

- **Create category**  
  `POST /api/categories`  
  Body: `{ "name": "Food" }`

- **Create expense**  
  `POST /api/expenses`  
  Body: `{ "categoryId": 1, "amount": 25.50, "expenseDate": "2025-02-10" }`

- **Chart data**  
  `GET /api/expenses/summary`  
  Returns: `[{ "categoryId": 1, "categoryName": "Food", "totalAmount": 100.00 }, ...]`

---

## Test Cases

Suggested test scenarios to validate behaviour:

1. **Category**
   - Create category with valid name → 201, returns category with Id.
   - Get all categories → 200, list of categories.
   - Get category by id (existing) → 200; (non-existing) → 404.
   - Update category name → 200; existing expenses still link to category and show new name.
   - Delete category with no expenses → 204.
   - Delete category that has expenses → 400, message that category has linked expenses.

2. **Expense**
   - Add expense with valid categoryId, amount &gt; 0, date today or past → 201.
   - Add expense with amount ≤ 0 → 400.
   - Add expense with future date → 400.
   - Add expense with non-existing categoryId → 400.
   - Get all expenses / get by id / get by category → 200 with expected data.
   - Delete expense (existing) → 204; (non-existing) → 404.

3. **Validation**
   - Create category with empty or missing name → 400.
   - Create expense with missing required fields → 400.

4. **UI**
   - **Dashboard** (`/` or `index.html`): Bar chart and expense table; category filter; uses GET /categories and GET /expenses.
   - **Categories** (`/categories.html`): Create, list, edit, delete categories; validation and “category has expenses” error on delete.
   - **Expenses** (`/expenses.html`): Add expense (category, amount, date), list, delete; amount &gt; 0 and date not in future validated.

---

## Project Structure (Layered Architecture)

- **Controllers**: `CategoriesController`, `ExpensesController` — HTTP API and routing.
- **Services**: `CategoryService`, `ExpenseService` — business rules and validation.
- **Repositories**: `CategoryRepository`, `ExpenseRepository` — data access (EF Core).
- **Models**: `Category`, `Expense` — domain entities.
- **DTOs**: Request/response DTOs under `DTOs/`.
- **Data**: `ApplicationDbContext` and EF Core configuration.
- **Migrations**: Under `Migrations/` for database schema.
- **wwwroot**: Static UI — `index.html` (dashboard with chart and table), `categories.html` (category CRUD), `expenses.html` (expense add/list/delete), `styles.css` (shared styles).

---

## Database Schema

- **Categories**: `Id` (PK), `Name` (required, max 200).
- **Expenses**: `Id` (PK), `CategoryId` (FK to Categories, Restrict on delete), `Amount` (decimal 18,2), `ExpenseDate` (datetime2).

Schema is created and updated via EF Core migrations (e.g. `InitialCreate`). Run the app once to apply migrations, or run `dotnet ef database update --project ExpenseTrackerAPI` from the solution directory.
