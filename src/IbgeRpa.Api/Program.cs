using IbgeRpa.Infrastructure.Data;
using IbgeRpa.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var dbPath = builder.Configuration["Database:Path"] ?? "./ibge.db";

builder.Services.AddInfrastructure($"Data Source={dbPath}");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "IBGE RPA API", Version = "v1" }));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    db.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();