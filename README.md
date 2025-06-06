# Book Management API

Book Management System built with .NET 8, Entity Framework Core, and SQLite using Minimal APIs.

## Technologies

- .NET 8
- Entity Framework Core
- SQLite
- Swagger

## Setup Instructions

1. Clone the repository:

   ```bash
   git clone https://github.com/tcheletc/book-api.git
   cd book-api
   ```

2. Restore dependencies:
 
	```bash
	dotnet restore
	```

3. Create the database:
 
	```bash
	dotnet ef database update
	```

4. Run the app:
 
	```bash
	dotnet run
	```

## API Endpoints

| Method | Endpoint               | Description               |
| ------ | ---------------------- | ------------------------- |
| GET    | `/api/books`           | Get all books (paginated) |
| GET    | `/api/books/{id}`      | Get book by ID            |
| POST   | `/api/books`           | Create new book           |
| PUT    | `/api/books/{id}`      | Update existing book      |
| DELETE | `/api/books/{id}`      | Delete book               |
| GET    | `/api/books/search?q=` | Search by title or author |

### Example: Search Books

```bash
GET /api/books/search?q=Rowling
```

### Pagination Parameters

```bash
GET /api/books?page=1&pageSize=0
```

## Assumptions

- SQLite is used for simplicity and portability.
- No authentication or authorization implemented.
- Minimal API used instead of traditional controllers.

## Development Decisions

- Entity Framework Core with code-first approach
- Minimal API chosen for cleaner and simpler API structure
- Swagger is enabled by default

## Future Improvements

- Add authentication/authorization (e.g. JWT)
- Add sorting and filtering by additional fields
- Add unit and integration tests
- Support SQL Server
- Add Docker support

## Migrations

To add a new migration:

```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```
