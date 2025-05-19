using Truestory.Common.Validators;
using Truestory.Frontend.Components;
using Truestory.Frontend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Make the session cookie essential
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ProductApiService>();
builder.Services.AddScoped<FlashMessageService>();
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

app.UseSession();
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>();

app.Run();
