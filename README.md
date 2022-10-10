# SwedishMotivationalQuotesMinimalRestApi
A minimal RESTful web api for motivational quotes in Swedish

## About this repo
This repo contains a .NET Minimal RESTful web API, using EF InMemoryDatabase for simplicity.



## Try it out
### To run the api locally
1. Clone the repo
2. Open a terminal and, from the folder containing the cloned repo, cd into _SwedishMotivationalQuotesMinimalRestApi/SwedishMotivationalQuoteApi_ 
3. In the same terminal, run: dotnet restore
4. In the same terminal, run: dotnet run
5. Open up a browser and go to http://localhost:5001. Here, you should get a message saying "If you can see this, it works!"
6. In the browser, you can now go to http://localhost:5001/swagger to be redirected to the Swagger GUI.

PS. Database is not seeded so I recommend starting with POSTing a new quote. After that, everything should run smoothly for you.

### To test the api
1. Follow step 1-3 in the section above
2. In the same terminal, run: cd ..
3. In the same terminal, run: dotnet restore
4. In the same terminal, run: dotnet test
