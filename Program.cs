using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using ContosoPizza.Data;
using ContosoPizza.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();

// 🔥 CONFIGURAÇÃO DO BANCO DE DADOS (Corrigida)
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    Console.WriteLine("=== USANDO POSTGRESQL ===");
    
    // Converter DATABASE_URL do formato URL para formato Npgsql (chave=valor)
    // Exemplo de entrada: postgresql://usuario:senha@host:porta/database?sslmode=require
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var username = userInfo[0];
    var password = userInfo[1];
    var database = uri.AbsolutePath.TrimStart('/');
    
    // Formato correto para Npgsql
    var connectionString = $"Host={uri.Host};Port={uri.Port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
    
    Console.WriteLine($"✅ Conectando ao PostgreSQL em: {uri.Host}");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    Console.WriteLine("=== USANDO SQLITE LOCAL ===");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=contosopizza.db"));
}

// Registrar serviços
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<CloudinaryService>();

// Autenticação
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
    PizzaService.Initialize(db);
    Console.WriteLine("✅ Banco de dados verificado/criado");
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

// Se não houver pizzas, adicionar as padrão
var pizzaCount = db.Pizzas.Count();
if (pizzaCount == 0)
{
    Console.WriteLine("⚠️ Nenhuma pizza encontrada. Adicionando pizzas padrão...");
    db.Pizzas.AddRange(PizzaService.GetPizzasPadrao());
    db.SaveChanges();
    Console.WriteLine($"✅ {db.Pizzas.Count()} pizzas adicionadas!");
}
