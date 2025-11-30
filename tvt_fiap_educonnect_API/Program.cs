using EduConnect_API.Data;
using EduConnect_API.Data.Seed;
using EduConnect_API.Repositories;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services;
using EduConnect_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// 1. FORÇAR CARREGAMENTO DOS ARQUIVOS DE CONFIGURAÇÃO
// ======================================================
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// ======================================================
// 2. CONFIGURAR SQL SERVER (CONNECTION STRING)
// ======================================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ======================================================
// 3. INJETAR SERVICES E REPOSITORIES
// ======================================================
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Registrar Seeder
builder.Services.AddScoped<DatabaseSeeder>();

// ======================================================
// 4. ADICIONAR CONTROLLERS
// ======================================================
builder.Services.AddControllers();

// ======================================================
// 5. CONFIGURAR SWAGGER
// ======================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ======================================================
// 6. EXECUTAR SEED AO INICIAR
// ======================================================
await using (var scope = app.Services.CreateAsyncScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

// ======================================================
// 7. PIPELINE HTTP
// ======================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

// ======================================================
// 8. RODAR A API
// ======================================================
app.Run();