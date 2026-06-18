using Microsoft.EntityFrameworkCore;
// Substitua pelo namespace real do seu DbContext
using NickTax.Infrastructure.Context;

namespace NickTax.API.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                Console.WriteLine("--> Aplicando Migrations pendentes no banco de dados...");
                context.Database.Migrate();
                Console.WriteLine("--> Migrations aplicadas com sucesso!");
            }
            else
            {
                Console.WriteLine("--> O banco de dados já está atualizado. Nenhuma migration pendente.");
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"--> Erro crítico ao aplicar as migrations: {ex.Message}");
            Console.ResetColor();
            throw;
        }
    }
}