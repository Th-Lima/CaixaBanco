using CaixaBanco.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            logger.LogInformation("Aplicando migrations pendentes ao banco de dados...");
            context.Database.Migrate();
            logger.LogInformation("Migrations aplicadas com sucesso.");
        }
        else
        {
            if (!context.Database.CanConnect() || !context.Database.GetService<IRelationalDatabaseCreator>().Exists())
            {
                logger.LogInformation("Criando banco de dados a partir do modelo (EnsureCreated)...");
                context.Database.EnsureCreated();
                logger.LogInformation("Banco criado com EnsureCreated.");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao inicializar o banco de dados.");
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
