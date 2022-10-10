using Microsoft.EntityFrameworkCore;
using Models;
using Data;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<QuoteDb>(opt => opt.UseInMemoryDatabase("QuoteList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();



var app = builder.Build();

app.MapGet("/", () => "If you can see this, it works!");

app.MapGet("/quotes", async (QuoteDb db) =>
    await db.Quotes.Select(q => new QuoteDTO(q))
                    .ToListAsync());

app.MapGet("/quotes/search/{author}", async (QuoteDb db, string author) =>
    await db.Quotes.Where(q => q.Author.ToLower().Contains(author.ToLower()) == true)
                    .Select(q => new QuoteDTO(q))
                     .ToListAsync());

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

};

namespace Data
{
    class QuoteDb : DbContext
    {
        public QuoteDb(DbContextOptions<QuoteDb> options)
            : base(options) { }

        public DbSet<Quote> Quotes => Set<Quote>();

    }
};