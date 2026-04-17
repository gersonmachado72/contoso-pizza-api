using Microsoft.EntityFrameworkCore;
using ContosoPizza.Data;
using ContosoPizza.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();

// 🔥 FORÇAR POSTGRESQL - IGNORAR SQLITE
var connectionString = "Host=dpg-d7drr967r5hc73d6bpe0-a.ohio-postgres.render.com;Port=5432;Database=contoso_pizza_db;Username=contoso_pizza_db_user;Password=ZWlW2qi9kWyphx4IOON94nEKI77Xi3B1;SSL Mode=Require;Trust Server Certificate=true;";

Console.WriteLine("=== USANDO POSTGRESQL FIXO ===");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<PedidoService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    Console.WriteLine("✅ Banco PostgreSQL criado/verificado");
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
