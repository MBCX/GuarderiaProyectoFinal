using Guarderia.Application.Common.Interfaces;
using Guarderia.Domain.Interfaces;
using Guarderia.Infraestructure.Data;
using Guarderia.Infraestructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Guarderia.Infraestructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // Configuración de Entity Framework
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            // Registro de repositorios principales
            services.AddScoped<INinoRepository, NinoRepository>();
            services.AddScoped<IAlergiaRepository, AlergiaRepository>();
            services.AddScoped<IIngredienteRepository, IngredienteRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IAsistenciaRepository, AsistenciaRepository>();
            services.AddScoped<IComidaRepository, ComidaRepository>();
            services.AddScoped<IResponsablePagoRepository, ResponsablePagoRepository>();
            services.AddScoped<IPersonaAutorizadaRepository, PersonaAutorizadaRepository>();

            // Registro de repositorios de relaciones y entidades adicionales
            services.AddScoped<ICargoMensualRepository, CargoMensualRepository>();
            services.AddScoped<IConsumoDiarioRepository, ConsumoDiarioRepository>();
            services.AddScoped<IConsumoMenuRepository, ConsumoMenuRepository>();
            services.AddScoped<IPlatoRepository, PlatoRepository>();
            services.AddScoped<ICostoFijoMensualRepository, CostoFijoMensualRepository>();
            services.AddScoped<IPlatoIngredienteRepository, PlatoIngredienteRepository>();
            services.AddScoped<IMenuPlatoRepository, MenuPlatoRepository>();
            services.AddScoped<INinoPersonaAutorizadaRepository, NinoPersonaAutorizadaRepository>();

            return services;
        }

        public static IServiceCollection AddInfrastructureServicesWithInMemoryDatabase(
            this IServiceCollection services
        )
        {
            // Configuración para testing con base de datos en memoria
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("GuarderiaTestDb"));
            
            // Registro de repositorios principales
            services.AddScoped<INinoRepository, NinoRepository>();
            services.AddScoped<IAlergiaRepository, AlergiaRepository>();
            services.AddScoped<IIngredienteRepository, IngredienteRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IAsistenciaRepository, AsistenciaRepository>();
            services.AddScoped<IComidaRepository, ComidaRepository>();
            services.AddScoped<IResponsablePagoRepository, ResponsablePagoRepository>();
            services.AddScoped<IPersonaAutorizadaRepository, PersonaAutorizadaRepository>();

            // Registro de repositorios de relaciones y entidades adicionales
            services.AddScoped<ICargoMensualRepository, CargoMensualRepository>();
            services.AddScoped<IConsumoDiarioRepository, ConsumoDiarioRepository>();
            services.AddScoped<IConsumoMenuRepository, ConsumoMenuRepository>();
            services.AddScoped<IPlatoRepository, PlatoRepository>();
            services.AddScoped<ICostoFijoMensualRepository, CostoFijoMensualRepository>();
            services.AddScoped<IPlatoIngredienteRepository, PlatoIngredienteRepository>();
            services.AddScoped<IMenuPlatoRepository, MenuPlatoRepository>();
            services.AddScoped<INinoPersonaAutorizadaRepository, NinoPersonaAutorizadaRepository>();

            return services;
        }
    }
}
