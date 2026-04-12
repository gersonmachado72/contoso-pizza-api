using ContosoPizza.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adicionar suporte a views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();  // Serve arquivos estáticos (css, js, images)
app.UseRouting();

app.MapControllers();      // Rotas da API (/pizza, etc.)
app.MapControllerRoute(    // Rotas das páginas
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
