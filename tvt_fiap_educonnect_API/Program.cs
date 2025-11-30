using EduConnect_API.Data;
using EduConnect_API.Repositories;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services;
using EduConnect_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------
// 1. CONFIGURAR DB (SQL SERVER)
// -----------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// -----------------------------------
// 2. INJETAR SERVICES E REPOSITORIES
// -----------------------------------
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// -----------------------------------
// 3. ADD CONTROLLERS (obrigatório para sua arquitetura)
// -----------------------------------
builder.Services.AddControllers();

// -----------------------------------
// 4. Configurar Swagger/OpenAPI
// -----------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// -----------------------------------
// 5. PIPELINE
// -----------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// -----------------------------------
// 6. MAPEAR CONTROLLERS
// -----------------------------------
app.MapControllers();


// -----------------------------------
// 7. RODAR A API
// -----------------------------------
app.Run();
