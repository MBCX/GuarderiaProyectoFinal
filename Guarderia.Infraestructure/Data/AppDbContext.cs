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
        public DbSet<CostoFijoMensual> CostoFijoMensuals { get; set; }
        public DbSet<FamiliarOConocido> FamiliarOConocidos { get; set; }
        public DbSet<Ingrediente> Ingredientes { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuPlato> MenuPlatos { get; set; }
        public DbSet<Nino> Ninos { get; set; }
        public DbSet<NinoPersonaAutorizada> NinoPersonaAutorizadas { get; set; }
        public DbSet<PersonaAutorizada> PersonaAutorizadas { get; set; }
        public DbSet<Plato> Platos{ get; set; }
        public DbSet<PlatoIngrediente> PlatoIngredientes { get; set; }
        public DbSet<ResponsablePago> ResponsablePagos { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
