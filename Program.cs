using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using ContosoPizza.Data;
using ContosoPizza.Services;
using ContosoPizza.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();

// Configurar PostgreSQL
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    Console.WriteLine("=== USANDO POSTGRESQL ===");
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var username = userInfo[0];
    var password = userInfo[1];
    var database = uri.AbsolutePath.TrimStart('/');
    var connectionString = $"Host={uri.Host};Port={uri.Port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
    
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    Console.WriteLine("=== USANDO SQLITE LOCAL ===");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=contosopizza.db"));
}

builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<PizzaService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Home/Logout";
        options.AccessDeniedPath = "/Home/AcessoNegado";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Inicializar banco de dados
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    
    if (!db.Pizzas.Any())
    {
        Console.WriteLine("⚠️ Adicionando pizzas padrão...");
        var pizzasPadrao = new List<Pizza>
        {
            new Pizza { Name = "Margherita", Price = 9.99M, IsVegetarian = true },
            new Pizza { Name = "Pepperoni", Price = 12.99M, IsVegetarian = false },
            new Pizza { Name = "Quattro Formaggi", Price = 14.99M, IsVegetarian = true },
            new Pizza { Name = "Calabresa", Price = 11.99M, IsVegetarian = false },
            new Pizza { Name = "Portuguesa", Price = 13.99M, IsVegetarian = false },
            new Pizza { Name = "Frango com Catupiry", Price = 14.99M, IsVegetarian = false },
            new Pizza { Name = "Vegetariana Especial", Price = 15.99M, IsVegetarian = true },
            new Pizza { Name = "Napolitana", Price = 12.99M, IsVegetarian = true },
            new Pizza { Name = "Mexicana", Price = 13.99M, IsVegetarian = false },
            new Pizza { Name = "Chocolate", Price = 18.99M, IsVegetarian = true }
        };
        db.Pizzas.AddRange(pizzasPadrao);
        db.SaveChanges();
        Console.WriteLine($"✅ {db.Pizzas.Count()} pizzas adicionadas!");
    }
    else
    {
        Console.WriteLine($"✅ Banco já contém {db.Pizzas.Count()} pizzas.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
