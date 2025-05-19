using Truestory.Frontend.Components;
using Truestory.Frontend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();
builder.Services.AddSingleton<ProductApiService>();
builder.Services.AddHttpClient("TruestoryApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("TruestoryApi:BaseUrl") ?? "https://localhost:5001");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>();

app.Run();
