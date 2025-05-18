using Microsoft.EntityFrameworkCore;
using Truestory.WebApi.Database;
using Truestory.WebApi.Endpoints;
using Truestory.WebApi.Middlewares;
using Truestory.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddDbContext<TruestoryDbContext>(options => options.UseInMemoryDatabase("TruestoryInMemoryDb"));
builder.Services.AddScoped<ProductExternalApiService>();
builder.Services.AddHttpClient("ExternalApiClient", client =>
{
    client.BaseAddress = builder.Configuration.GetValue<Uri>("ExternalApi:BaseUrl");
    client.Timeout = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("ExternalApi:Timeout"));
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddHostedService<ProductDbSeederService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Truestory API v1");
    });
}

app.UseHttpsRedirection();
app.UseMiddleware<InvalidJsonHandler>();
app.MapGet("/", () => "Welcome to the Truestory API!");
app.MapProductApiEndpoints();
app.MapFallbackEndpoints();

app.Run();
