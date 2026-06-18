using Microsoft.EntityFrameworkCore;
using NickTax.Domain.Entities;

namespace NickTax.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UsuarioEntity> Usuarios { get; set; }
        public DbSet<EmpresaEntity> Empresas { get; set; }
        public DbSet<NotaFiscalEntity> NotasFiscais { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName()?.ToLower());
            }
        }
    }
}