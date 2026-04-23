using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using ContosoPizza.Data;
using ContosoPizza.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();

// 🔥 CONFIGURAÇÃO DO BANCO DE DADOS
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    Console.WriteLine("=== USANDO POSTGRESQL ===");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(databaseUrl));
}
else
{
    Console.WriteLine("=== USANDO SQLITE LOCAL ===");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=contosopizza.db"));
}

// 🔥 REGISTRAR SERVIÇOS
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<CloudinaryService>();

// 🔥 AUTENTICAÇÃO
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

// 🔥 MIGRAÇÕES E DADOS INICIAIS
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    
    // Inicializar PizzaService com o DbContext
    PizzaService.Initialize(db);
    
    // Verificar se há pizzas, se não houver, adicionar as padrão
    if (!db.Pizzas.Any())
    {
        Console.WriteLine("⚠️ Nenhuma pizza encontrada. Adicionando pizzas padrão...");
        db.Pizzas.AddRange(PizzaService.GetPizzasPadrao());
        db.SaveChanges();
        Console.WriteLine("✅ Pizzas padrão adicionadas!");
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

// Garantir que a coluna ImageUrl existe em todas as execuções
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.ExecuteSqlRaw("ALTER TABLE Pizzas ADD COLUMN ImageUrl TEXT;");
    }
    catch { /* Coluna já existe, ignorar erro */ }
}
