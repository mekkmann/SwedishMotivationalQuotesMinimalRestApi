namespace Api.Tests;

using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Text;
using Models;
using Data;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

public class Tests
{
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
        
        // checks that JSON returned from getResponse has a length of 4
        jsonGetResponse.Result.Should().HaveCount(4);
    }

    [Fact]
    public async Task Get_All_Quotes()
    {
        var items = SeedData.Quotes();

        var getResponse = await _client.GetAsync("/quotes");

        // checks statuscode of getResponse
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var jsonGetResponse = await getResponse.Content.ReadFromJsonAsync<QuoteDTO[]>();
        
        jsonGetResponse.Should().BeEquivalentTo(items);
    }

    [Fact]
    public async Task Can_Get_With_Id()
    {
        var getResponse = await _client.GetAsync("/quotes/3");

        // checks statuscode of getResponse
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }
    [Fact]
    public async Task Get_With_Non_Existing_Id_Returns_404()
    {
        var getResponse = await _client.GetAsync("/quotes/999");

        // checks statuscode of getResponse
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Can_Search_For_Quotes_With_Full_Author_Name()
    {
        var getResponse = await _client.GetAsync("/quotes/search/Dennis%20Gabor");

        // checks statuscode of getResponse
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task Can_Search_For_Quotes_With_Partial_Author_Name()
    {
        var getResponse = await _client.GetAsync("/quotes/search/gab");
        
        // checks statuscode of getResponse
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task Search_For_Quotes_With_Non_Existing_Author_Returns_404()
    {
        var getResponse = await _client.GetAsync("/quotes/search/gabbagool");

        // checks statuscode of getResponse
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Put_Updates_Quote()
    {
        var payload2 = JsonConvert.SerializeObject(new Quote{
            Id = 1,
            Author = "Dennis Panjuta",
            Text = "Vägen till framgång",
            Secret = "qwerty"
        });
        var content2 = new StringContent(payload2, Encoding.UTF8, "application/json");

        var putResponse = await _client.PutAsync("/quotes/1", content2);

        // checks statuscode of putResponse
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        var jsonGetResponse = await _client.GetFromJsonAsync<QuoteDTO>("/quotes/1");

        // checks that the update changed the author
        Assert.Equal("Dennis Panjuta", jsonGetResponse?.Author);

        // checks that the update changed the text
        Assert.Equal("Vägen till framgång", jsonGetResponse?.Text);
    }

    [Fact]
    public async Task Delete_Removes_From_Db_And_Returns_OK()
    {
    
        var deleteResponse = await _client.DeleteAsync("/quotes/1");

        // checks statuscode of deleteResponse
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync("/quotes");
        var jsonGetResponse = getResponse.Content.ReadFromJsonAsync<QuoteDTO[]>();

        // checks that the quote was removed
        foreach ( QuoteDTO quote in jsonGetResponse.Result){
            quote.Id.Should().NotBe(1);
        }
    }

}