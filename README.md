# I know it's not a RESTful api yet, it's in development :)

# SwedishMotivationalQuotesMinimalRestApi
A minimal RESTful web api for motivational quotes in Swedish

## About this repo
This repo contains a .NET Minimal RESTful web API, using EF InMemoryDatabase for simplicity.

The InMemoryDatabase is seeded with 3 quotes for ease of use.


## Try it out
### To run the api locally
1. Clone the repo
2. Open a terminal and, from the folder containing the cloned repo, cd into _SwedishMotivationalQuotesMinimalRestApi_ 
3. In the same terminal, run: dotnet restore
4. In the same terminal, run: cd SwedishMotivationalQuoteApi
5. In the same terminal, run: dotnet run
6. Open up a browser and go to http://localhost:5001. Here, you should get a message saying "If you can see this, it works!"
7. In the browser, you can now go to http://localhost:5001/swagger to be redirected to the Swagger GUI.


### To test the api
1. Follow step 1-3 in the section above
2. In the same terminal, run: dotnet test
