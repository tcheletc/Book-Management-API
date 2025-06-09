using BookApi.Data;
using BookApi.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Configure services for the application.
/// </summary>

// Register the database context using SQLite
builder.Services.AddDbContext<BookContext>(options =>
    options.UseSqlite("Data Source=books.db"));

// Add controller support
builder.Services.AddControllers();

// Add support for minimal API endpoint exploration (for Swagger)
builder.Services.AddEndpointsApiExplorer();

// Add Swagger generation with XML comments for documentation
builder.Services.AddSwaggerGen(options =>
{
    // Load XML documentation file
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// Register the book service for dependency injection
builder.Services.AddScoped<IBookService, BookService>();

var app = builder.Build();

/// <summary>
/// Configure the HTTP request pipeline.
/// </summary>

// Enable Swagger only in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use HTTPS redirection only in non-development environments
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Enable authorization middleware (even if no policies are used yet)
app.UseAuthorization();

// Map controller routes
app.MapControllers();

// Set the application to listen on port 8081 (HTTP)
app.Urls.Add("http://*:8081");

/// <summary>
/// Automatically apply any pending EF Core migrations on startup.
/// This ensures the database is created and up to date.
/// </summary>
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookContext>();
    db.Database.Migrate(); // Create and apply migrations
}

// Run the application
app.Run();
