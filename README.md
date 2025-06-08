# Book Management API

Book Management System built with .NET 8, Entity Framework Core, and SQLite using controllers.

## Technologies

- ASP.NET Core 8 (Controllers)
- Entity Framework Core 8
- SQLite (can be replaced with SQL Server)
- Swagger / OpenAPI
- xUnit for Unit Testing
- Moq (for mocking in tests)

## Setup Instructions

1. Clone the repository:

   ```bash
   git clone https://github.com/tcheletc/Book-Management-API.git
   cd Book-Management-API
   ```

2. Restore dependencies:
 
	```bash
	dotnet restore
	```

3. Create the database:
 
	```bash
	cd BookApi
	dotnet ef database update
	```

4. Run the app:
 
	```bash
	dotnet run
	```
	The app should be available at: http://localhost:5188/swagger

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

## Validation Rules

- All fields are required: Title, Author, PublicationDate, Price
- PublicationDate:
	- Cannot be the default date
	- Cannot be in the future
- Case-insensitive search by title or author
- Sorting: books are sorted first by author, then by title

## Assumptions

- The app assumes valid input for creating and updating books
- Invalid input returns proper 400 BadRequest responses
- The database is initially empty

## Development Decisions

- SQLite is used for simplicity and portability
- Controllers used instead of Minimal API
- Swagger is enabled by default
- Created a custom validation attribute for PublicationDate
- Layered architecture: Controllers → Services → DbContext

## Future Improvements

- Add authentication/authorization (e.g. JWT)
- Add full filtering and sorting support
- Add full integration tests
- Add Docker support

## Migrations

To add a new migration:

```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## Tests

- CRUD operations tested via BooksControllerTests
- BookServiceTests include:
	- Valid and invalid queries
	- Edge cases like empty search, pagination beyond data range
- Custom annotation [NotDefaultOrFutureDate] tested for all edge cases

To run the tests:
```bash
cd BookApi.Tests
dotnet test
```