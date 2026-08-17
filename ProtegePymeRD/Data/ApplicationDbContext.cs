using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProtegePymeRD.Models;

namespace ProtegePymeRD.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Empresa> Empresas { get; set; }

        public DbSet<Diagnostico> Diagnosticos { get; set; }

        public DbSet<Respaldo> Respaldos { get; set; }

        public DbSet<CuentaCritica> CuentasCriticas { get; set; }

        public DbSet<AlertaSeguridad> AlertasSeguridad { get; set; }

        public DbSet<CapacitacionSeguridad> CapacitacionesSeguridad { get; set; }

        public DbSet<PlanContinuidadDigital> PlanesContinuidadDigital { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Empresa>()
                .HasIndex(empresa => empresa.Rnc)
                .IsUnique();

            builder.Entity<Diagnostico>()
                .HasOne(diagnostico => diagnostico.Empresa)
                .WithMany()
                .HasForeignKey(diagnostico => diagnostico.EmpresaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Respaldo>()
                .HasOne(respaldo => respaldo.Empresa)
                .WithMany()
                .HasForeignKey(respaldo => respaldo.EmpresaId)
                .OnDelete(DeleteBehavior.Cascade);  
        }
    }
}