using Microsoft.EntityFrameworkCore;
using Models;
using Data;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<QuoteDb>(opt => opt.UseInMemoryDatabase("QuoteList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using (var scope = app.Services.CreateScope())
    using (var dbContext = scope.ServiceProvider.GetRequiredService<QuoteDb>())
    {
        try
        {
            // NOTE Using EnsureCreated is not recommended for relational db if one plans to use EF Migrations
            dbContext.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            // TODO Log error here
            throw;
        }
    }
}

app.MapFallback(() => Results.Redirect("/swagger"));

app.MapGet("/", () => "If you can see this, it works!");

app.MapGet("/quotes", async (QuoteDb db) =>
    await db.Quotes.Select(q => new QuoteDTO(q))
                    .ToListAsync());

app.MapGet("/quotes/search/{author}", async (QuoteDb db, string author) =>
{
    var quoteList = await db.Quotes.Where(q => q.Author.ToLower().Contains(author.ToLower()) == true)
                    .Select(q => new QuoteDTO(q))
                     .ToListAsync();

    if (quoteList.Count == 0) return Results.NotFound("No quotes with that author was found.");

    return Results.Ok(quoteList);
});

app.MapGet("/quotes/{id}", async (int id, QuoteDb db) =>
    await db.Quotes.FindAsync(id)
        is Quote quote
            ? Results.Ok(new QuoteDTO(quote))
            : Results.NotFound());

app.MapPost("/quotes", async (QuoteDTO quoteDTO, QuoteDb db) =>
{
    if (String.IsNullOrWhiteSpace(quoteDTO.Author) || String.IsNullOrWhiteSpace(quoteDTO.Text))
    {
        return Results.BadRequest();
    }

    var quote = new Quote
    {
        Author = quoteDTO.Author,
        Text = quoteDTO.Text,
    };

    db.Quotes.Add(quote);
    await db.SaveChangesAsync();

    return Results.Created($"/quotes/{quote.Id}", new QuoteDTO(quote));
});

app.MapPut("/quotes/{id}", async (int id, QuoteDTO quoteDTO, QuoteDb db) =>
{
    if (await db.Quotes.FindAsync(id) is not Quote quote)
    {
        return Results.NotFound();  
    }

    quote.Author = quoteDTO.Author;
    quote.Text = quoteDTO.Text;

    await db.SaveChangesAsync();

    return Results.NoContent();

});

app.MapDelete("/quotes/{id}", async (int id, QuoteDb db) =>
{
    if (await db.Quotes.FindAsync(id) is Quote quote)
    {
        db.Quotes.Remove(quote);
        await db.SaveChangesAsync();
        
        return Results.Ok(new QuoteDTO(quote));
    }

    return Results.NotFound();
});

app.Run();

// Partial class is for XUnit to be able to access program.cs and let us write tests against it
public partial class Program {}


namespace Models
{
    public class Quote
    {
        [Key]
        public int Id { get; set; }
        public string? Author { get; set; }
        public string? Text { get; set; }
        public string? Secret { get; set; } // Only to show functionality of DTO, effectively hiding the secret to users
    }

    public class QuoteDTO
    {
        public int Id { get; set; }
        public string? Author { get; set; }
        public string? Text { get; set; }

        public QuoteDTO() { }
        public QuoteDTO(Quote quote) =>
            (Id, Author, Text) = (quote.Id, quote.Author, quote.Text);
    }

    public class SeedData
    {
        public static QuoteDTO[] Quotes()
        {
            var id = 1;

            var quotes = new QuoteDTO[]
                {
                    new QuoteDTO
                    {
                        Id = id++,
                        Author = "Dennis Gabor",
                        Text = "Det bästa sättet att förutspå framtiden är att skapa den.",
                        
                    },
                    new QuoteDTO
                    {
                        Id = id++,
                        Author = "Mahatma Gandhi",
                        Text = "Lev som om du skulle dö imorgon, lär som att du skulle leva förevigt.",
                        
                    },
                    new QuoteDTO
                    {
                        Id = id++,
                        Author = "Pablo Picasso",
                        Text = "Handling är den grundläggande nyckeln till framgång.",
                        
                    }
                };
            return quotes;
        }
    }

};

namespace Data
{
    class QuoteDb : DbContext
    {
        public QuoteDb(DbContextOptions<QuoteDb> options)
            : base(options) { }

        // public DbSet<Quote> Quotes => Set<Quote>();
        public DbSet<Quote> Quotes {get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Quote>().HasData(SeedData.Quotes());
        }

    }
};