using Microsoft.EntityFrameworkCore;
using ContosoPizza.Data;
using ContosoPizza.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();

// Configurar PostgreSQL
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrEmpty(databaseUrl))
{
    Console.WriteLine("⚠️ DATABASE_URL não encontrada. Usando SQLite local.");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=contosopizza.db"));
}
else
{
    Console.WriteLine($"✅ Conectando ao PostgreSQL");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(databaseUrl));
}

// Registrar serviços (Scoped = uma instância por requisição)
builder.Services.AddScoped<PedidoService>();

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
