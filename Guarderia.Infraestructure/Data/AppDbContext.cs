using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Guarderia.Infraestructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Alergia> Alergias { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<CargoMensual> CargoMensuales { get; set; }
        public DbSet<Comida> Comidas { get; set; }
        public DbSet<ConsumoDiario> ConsumoDiarios { get; set; }
        public DbSet<ConsumoMenu> ConsumosMenu { get; set; }
        public DbSet<CostoFijoMensual> CostoFijoMensuals { get; set; }
        public DbSet<FamiliarOConocido> FamiliarOConocidos { get; set; }
        public DbSet<Ingrediente> Ingredientes { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuPlato> MenuPlatos { get; set; }
        public DbSet<Nino> Ninos { get; set; }
        public DbSet<NinoPersonaAutorizada> NinoPersonaAutorizadas { get; set; }
        public DbSet<PersonaAutorizada> PersonaAutorizadas { get; set; }
        public DbSet<Plato> Platos { get; set; }
        public DbSet<PlatoIngrediente> PlatoIngredientes { get; set; }
        public DbSet<ResponsablePago> ResponsablePagos { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Nino
            modelBuilder.Entity<Nino>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NumeroMatricula).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FechaNacimiento).IsRequired();
                entity.Property(e => e.FechaIngreso).IsRequired();
                entity.Property(e => e.Activo).IsRequired();

                entity.HasOne(e => e.ResponsablePago)
                    .WithMany(r => r.NinosAPagar)
                    .HasForeignKey(e => e.ResponsablePagoId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configuración de ResponsablePago
            modelBuilder.Entity<ResponsablePago>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Cedula).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Direccion).HasMaxLength(200);
                entity.Property(e => e.Telefono).HasMaxLength(20);
                entity.Property(e => e.CuentaCorriente).HasMaxLength(30);

                entity.HasIndex(e => e.Cedula).IsUnique();
            });

            // Configuración de PersonaAutorizada
            modelBuilder.Entity<PersonaAutorizada>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Cedula).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Direccion).HasMaxLength(200);
                entity.Property(e => e.Telefono).HasMaxLength(20);
                entity.Property(e => e.Relacion).HasMaxLength(50);

                entity.HasIndex(e => e.Cedula).IsUnique();
            });

            // Configuración de NinoPersonaAutorizada (relación muchos a muchos)
            modelBuilder.Entity<NinoPersonaAutorizada>(entity =>
            {
                entity.HasKey(e => new { e.NinoId, e.PersonaAutorizadaId });

                entity.Property(e => e.FechaAutorizacion).IsRequired();
                entity.Property(e => e.Activa).IsRequired();
                entity.Property(e => e.Observaciones).HasMaxLength(500);

                entity.HasOne(e => e.Nino)
                    .WithMany(n => n.PersonasAutorizadas)
                    .HasForeignKey(e => e.NinoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.PersonaAutorizada)
                    .WithMany(p => p.NinosAutorizados)
                    .HasForeignKey(e => e.PersonaAutorizadaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de Ingrediente
            modelBuilder.Entity<Ingrediente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.EsAlergeno).IsRequired();

                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            // Configuración de Plato
            modelBuilder.Entity<Plato>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.TipoPlato).HasMaxLength(50);

                entity.HasIndex(e => e.Nombre).IsUnique();
            });

            // Configuración de PlatoIngrediente (relación muchos a muchos)
            modelBuilder.Entity<PlatoIngrediente>(entity =>
            {
                entity.HasKey(e => new { e.PlatoId, e.IngredienteId });

                entity.Property(e => e.Cantidad).HasMaxLength(50);
                entity.Property(e => e.EsAlergeno).IsRequired();

                entity.HasOne(e => e.Plato)
                    .WithMany(p => p.Ingredientes)
                    .HasForeignKey(e => e.PlatoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Ingrediente)
                    .WithMany(i => i.Platos)
                    .HasForeignKey(e => e.IngredienteId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de Menu
            modelBuilder.Entity<Menu>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.Precio).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.FechaCreacion).IsRequired();
                entity.Property(e => e.Activo).IsRequired();
            });

            // Configuración de MenuPlato (relación muchos a muchos)
            modelBuilder.Entity<MenuPlato>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.PlatoId });

                entity.Property(e => e.Orden).IsRequired();
                entity.Property(e => e.EsPlatoPrincipal).IsRequired();

                entity.HasOne(e => e.Menu)
                    .WithMany(m => m.Platos)
                    .HasForeignKey(e => e.Id)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Plato)
                    .WithMany(p => p.Menus)
                    .HasForeignKey(e => e.PlatoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de Alergia
            modelBuilder.Entity<Alergia>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property("NinoId").IsRequired();
                entity.Property("IngredienteId").IsRequired();

                // Relación con Nino (necesaria pero no definida en la entidad)
                entity.HasOne<Nino>()
                    .WithMany(n => n.Alergias)
                    .HasForeignKey("NinoId")
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Ingrediente)
                    .WithMany(i => i.Alergias)
                    .HasForeignKey("IngredienteId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de Asistencia
            modelBuilder.Entity<Asistencia>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Fecha).IsRequired();
                entity.Property(e => e.Asistio).IsRequired();

                entity.HasOne(e => e.Nino)
                    .WithMany(n => n.Asistencias)
                    .HasForeignKey(e => e.NinoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.NinoId, e.Fecha }).IsUnique();
            });

            // Configuración de Comida
            modelBuilder.Entity<Comida>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Fecha).IsRequired();
                entity.Property(e => e.Tipo).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Costo).HasColumnType("decimal(10,2)").IsRequired();

                entity.HasOne(e => e.Nino)
                    .WithMany(n => n.Comidas)
                    .HasForeignKey(e => e.NinoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de ConsumoMenu
            modelBuilder.Entity<ConsumoMenu>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Fecha).IsRequired();
                entity.Property(e => e.CostoReal).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.Observaciones).HasMaxLength(500);

                entity.HasOne(e => e.Nino)
                    .WithMany(n => n.ConsumosMenu)
                    .HasForeignKey(e => e.NinoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Menu)
                    .WithMany(m => m.Consumos)
                    .HasForeignKey(e => e.MenuId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.NinoId, e.Fecha }).IsUnique();
            });

            // Configuración de ConsumoDiario
            modelBuilder.Entity<ConsumoDiario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Fecha).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(200);
                entity.Property(e => e.Monto).HasColumnType("decimal(10,2)").IsRequired();
            });

            // Configuración de CostoFijoMensual
            modelBuilder.Entity<CostoFijoMensual>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Monto).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.FechaVigenciaDesde).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(200);
                entity.Property(e => e.Activo).IsRequired();
            });

            // Configuración de CargoMensual
            modelBuilder.Entity<CargoMensual>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Mes).IsRequired();
                entity.Property(e => e.Año).IsRequired();
                entity.Property(e => e.CostoFijo).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.CostoComidas).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.TotalCargo).HasColumnType("decimal(10,2)").IsRequired();
                entity.Property(e => e.FechaGeneracion).IsRequired();
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(20);

                entity.HasOne(e => e.Nino)
                    .WithMany(n => n.CargosMensuales)
                    .HasForeignKey(e => e.NinoId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ResponsablePago)
                    .WithMany()
                    .HasForeignKey(e => e.ResponsablePagoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.NinoId, e.Mes, e.Año }).IsUnique();
            });

            // Configuración de FamiliarOConocido
            modelBuilder.Entity<FamiliarOConocido>(entity =>
            {
                entity.HasKey("Id");
                entity.Property("Id").ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Cedula).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Direccion).HasMaxLength(200);
            });
        }
    }
}
