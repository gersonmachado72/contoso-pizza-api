using Microsoft.EntityFrameworkCore;
using ContosoPizza.Data;
using ContosoPizza.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();

// 🔥 CONFIGURAÇÃO ESPECÍFICA PARA O RENDER
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Ambiente de produção (Render) - Usar PostgreSQL
    Console.WriteLine("=== AMBIENTE DE PRODUÇÃO - USANDO POSTGRESQL ===");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(databaseUrl));
}
else
{
    // Ambiente de desenvolvimento (local) - Usar SQLite
    Console.WriteLine("=== AMBIENTE DE DESENVOLVIMENTO - USANDO SQLITE ===");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=contosopizza.db"));
}

builder.Services.AddScoped<PedidoService>();

var app = builder.Build();

// Criar banco de dados e tabelas
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    Console.WriteLine("✅ Banco de dados verificado/criado");
}

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
