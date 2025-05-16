using Microsoft.EntityFrameworkCore;
using Truestory.WebApi.Database;
using Truestory.WebApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TruestoryDbContext>(options => options.UseInMemoryDatabase("TruestoryInMemoryDb"));
builder.Services.AddHttpClient("ExternalApiClient", client =>
{
    client.BaseAddress = builder.Configuration.GetValue<Uri>("ExternalApi:BaseUrl");
    client.Timeout = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("ExternalApi:Timeout"));
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

var app = builder.Build();

app.UseHttpsRedirection();
app.MapGet("/", () => "Welcome to the Truestory API!");
app.MapProductApiEndpoints();

app.Run();
