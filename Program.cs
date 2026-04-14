using Microsoft.EntityFrameworkCore;
using ContosoPizza.Data;
using ContosoPizza.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Configurar Timezone do Brasil
try
{
    TimeZoneInfo.TryFindSystemTimeZoneById("America/Sao_Paulo", out var tz);
    if (tz == null)
        tz = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao configurar timezone: {ex.Message}");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();

// Configurar o PostgreSQL
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrEmpty(databaseUrl))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=contosopizza.db"));
}
else
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var username = userInfo[0];
    var password = userInfo[1];
    var database = uri.AbsolutePath.TrimStart('/');
    var connectionString = $"Host={uri.Host};Port={uri.Port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}

// Registrar serviços (Scoped = uma instância por requisição HTTP)
builder.Services.AddScoped<PizzaService>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<CloudinaryService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllers();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
