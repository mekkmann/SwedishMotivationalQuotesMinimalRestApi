namespace Api.Tests;

using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Text;
using Models;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

public class Tests
{

    // TODO: Seed database when testing

    HttpClient _client;

    public Tests()
    {
        var app = new WebApplicationFactory<Program>();
        _client = app.CreateClient();
    }

    [Fact]
    public async Task Sanity_Check()
    {

        var response = await _client.GetStringAsync("/");

        Assert.Equal("If you can see this, it works!", response);
    }

    [Fact]
    public async Task Posting_Quote_Adds_To_Db()
    {

        var payload = JsonConvert.SerializeObject(new Quote{
            Id = 1,
            Author = "Dennis Gabor",
            Text = "Det bästa sättet att förutspå framtiden är genom att skapa den.",
            Secret = "qwerty"
        });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        var postResponse = await _client.PostAsync("/quotes", content);
        
        // checks statuscode of postResponse
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

        var getResponse = await _client.GetAsync("/quotes");

        // checks statuscode of getResponse
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var jsonGetResponse = getResponse.Content.ReadFromJsonAsync<QuoteDTO[]>();
        
        // checks that JSON returned from getResponse is not empty
        jsonGetResponse.Result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Get_All_Quotes()
    {

        // TODO : Here is where having it seeded would be nice
        
        var getResponse = await _client.GetAsync("/quotes");

        // checks statuscode of getResponse
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var jsonGetResponse = getResponse.Content.ReadFromJsonAsync<QuoteDTO[]>();

        //checks length of JSON returned from getResponse
        jsonGetResponse.Result.Should().HaveCount(0);

    }

    [Fact]
    public async Task Can_Get_With_Id()
    {
        var payload = JsonConvert.SerializeObject(new Quote{
            Id = 1,
            Author = "Dennis Gabor",
            Text = "Det bästa sättet att förutspå framtiden är genom att skapa den.",
            Secret = "qwerty"
        });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        await _client.PostAsync("/quotes", content);

        var getResponse = await _client.GetAsync("/quotes/1");

        // checks statuscode of getResponse
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }
    [Fact]
    public async Task Get_With_Non_Existing_Id_Returns_404()
    {
        var payload = JsonConvert.SerializeObject(new Quote{
            Id = 1,
            Author = "Dennis Gabor",
            Text = "Det bästa sättet att förutspå framtiden är genom att skapa den.",
            Secret = "qwerty"
        });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        await _client.PostAsync("/quotes", content);

        var getResponse = await _client.GetAsync("/quotes/999");

        // checks statuscode of getResponse
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Can_Search_For_Quotes_With_Full_Author_Name()
    {
        var payload = JsonConvert.SerializeObject(new Quote{
            Id = 1,
            Author = "Dennis Gabor",
            Text = "Det bästa sättet att förutspå framtiden är genom att skapa den.",
            Secret = "qwerty"
        });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        await _client.PostAsync("/quotes", content);

        var getResponse = await _client.GetAsync("/quotes/search/Dennis%20Gabor");

        // checks statuscode of getResponse
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task Can_Search_For_Quotes_With_Partial_Author_Name()
    {
        var payload = JsonConvert.SerializeObject(new Quote{
            Id = 1,
            Author = "Dennis Gabor",
            Text = "Det bästa sättet att förutspå framtiden är genom att skapa den.",
            Secret = "qwerty"
        });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        await _client.PostAsync("/quotes", content);

        var getResponse = await _client.GetAsync("/quotes/search/gab");
        // checks statuscode of getResponse
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task Search_For_Quotes_With_Non_Existing_Author_Returns_404()
    {
        var payload = JsonConvert.SerializeObject(new Quote{
            Id = 1,
            Author = "Dennis Gabor",
            Text = "Det bästa sättet att förutspå framtiden är genom att skapa den.",
            Secret = "qwerty"
        });
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        await _client.PostAsync("/quotes", content);

        var getResponse = await _client.GetAsync("/quotes/search/gabbagool");

        // checks statuscode of getResponse
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

}