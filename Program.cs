using Microsoft.EntityFrameworkCore;
using ContosoPizza.Data;
using ContosoPizza.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();

// 🔴 IMPORTANTE: Usar PostgreSQL do Render
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrEmpty(databaseUrl))
{
    // Fallback para SQLite se não tiver DATABASE_URL
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=contosopizza.db"));
}
else
{
    // Converter DATABASE_URL para connection string do PostgreSQL
    // Formato: postgresql://usuario:senha@host:porta/database
    var uri = new Uri(databaseUrl);
    var connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true";
    
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));
}

// Cloudinary
builder.Services.AddScoped<CloudinaryService>();

var app = builder.Build();

// Criar banco de dados e tabelas
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
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
