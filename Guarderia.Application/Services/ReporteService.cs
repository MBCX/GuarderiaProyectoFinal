using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Application.Interfaces;
using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;

namespace Guarderia.Application.Services
{
    public class ReporteService : IReporteService
    {
        private readonly IAsistenciaRepository _asistenciaRepository;
        private readonly IConsumoMenuRepository _consumoMenuRepository;
        private readonly IComidaRepository _comidaRepository;
        private readonly ICargoMensualRepository _cargoMensualRepository;
        private readonly IAlergiaRepository _alergiaRepository;
        private readonly INinoRepository _ninoRepository;

        public ReporteService(
            IAsistenciaRepository asistenciaRepository,
            IConsumoMenuRepository consumoMenuRepository,
            IComidaRepository comidaRepository,
            ICargoMensualRepository cargoMensualRepository,
            IAlergiaRepository alergiaRepository,
            INinoRepository ninoRepository)
        {
            _asistenciaRepository = asistenciaRepository;
            _consumoMenuRepository = consumoMenuRepository;
            _comidaRepository = comidaRepository;
            _cargoMensualRepository = cargoMensualRepository;
            _alergiaRepository = alergiaRepository;
            _ninoRepository = ninoRepository;
        }

        public async Task<List<Asistencia>> GenerarReporteAsistenciaDiariaAsync(DateTime fecha)
        {
            return await _asistenciaRepository.ObtenerPorFechaAsync(fecha);
        }

        public async Task<List<Asistencia>> GenerarReporteAsistenciaMensualAsync(int mes, int año)
        {
            return await _asistenciaRepository.ObtenerPorMesYAñoAsync(mes, año);
        }

        public async Task<Dictionary<int, decimal>> GenerarReportePorcentajeAsistenciaAsync(int mes, int año)
        {
            var ninosActivos = await _ninoRepository.ObtenerActivosAsync();
            var porcentajes = new Dictionary<int, decimal>();

            foreach (var nino in ninosActivos)
            {
                var porcentaje = await _asistenciaRepository.CalcularPorcentajeAsistenciaAsync(nino.Id, mes, año);
                porcentajes[nino.Id] = porcentaje;
            }

            return porcentajes;
        }

        public async Task<List<ConsumoMenu>> GenerarReporteConsumoMenuDiarioAsync(DateTime fecha)
        {
            return await _consumoMenuRepository.ObtenerPorFechaAsync(fecha);
        }

        public async Task<List<ConsumoMenu>> GenerarReporteConsumoMenuMensualAsync(int mes, int año)
        {
            var todosLosConsumos = await _consumoMenuRepository.ObtenerTodosAsync();
            return todosLosConsumos
                .Where(c => c.Fecha.Month == mes && c.Fecha.Year == año)
                .OrderBy(c => c.Fecha)
                .ThenBy(c => c.Nino?.Nombre)
                .ToList();
        }

        public async Task<List<Comida>> GenerarReporteConsumoComidasAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var todasLasComidas = await _comidaRepository.ObtenerTodasAsync();
            return todasLasComidas
                .Where(c => c.Fecha.Date >= fechaInicio.Date && c.Fecha.Date <= fechaFin.Date)
                .OrderBy(c => c.Fecha)
                .ThenBy(c => c.Nino?.Nombre)
                .ToList();
        }

        public async Task<List<CargoMensual>> GenerarReporteCargosAsync(int mes, int año)
        {
            return await _cargoMensualRepository.ObtenerPorMesYAñoAsync(mes, año);
        }

        public async Task<List<CargoMensual>> GenerarReportePendientesPagoAsync()
        {
            return await _cargoMensualRepository.ObtenerPendientesAsync();
        }

        public async Task<decimal> GenerarReporteIngresosMensualesAsync(int mes, int año)
        {
            var cargos = await _cargoMensualRepository.ObtenerPorMesYAñoAsync(mes, año);
            return cargos.Where(c => c.Estado == "Pagado").Sum(c => c.TotalCargo);
        }

        public async Task<List<Nino>> GenerarReporteNiñosConAlergiasAsync()
        {
            return await _alergiaRepository.GetNinosConAlergiasAsync();
        }

        public async Task<Dictionary<string, int>> GenerarReporteEstadisticasAlergiasAsync()
        {
            var estadisticas = new Dictionary<string, int>();
            var ninosConAlergias = await _alergiaRepository.GetNinosConAlergiasAsync();

            foreach (var nino in ninosConAlergias)
            {
                var alergias = await _alergiaRepository.GetIngredientesAlergenosPorNinoAsync(nino.Id);
                foreach (var alergia in alergias)
                {
                    if (estadisticas.ContainsKey(alergia))
                    {
                        estadisticas[alergia]++;
                    }
                    else
                    {
                        estadisticas[alergia] = 1;
                    }
                }
            }

            return estadisticas.OrderByDescending(e => e.Value).ToDictionary(e => e.Key, e => e.Value);
        }

        public async Task<Dictionary<string, object>> GenerarDashboardEstadisticasAsync()
        {
            var estadisticas = new Dictionary<string, object>();

            try
            {
                // Estadísticas generales
                var ninosActivos = await _ninoRepository.ContarActivosAsync();
                estadisticas["TotalNinosActivos"] = ninosActivos;

                // Asistencias del día actual
                var asistenciasHoy = await _asistenciaRepository.ObtenerPorFechaAsync(DateTime.Today);
                estadisticas["AsistenciasHoy"] = asistenciasHoy.Count(a => a.Asistio);
                estadisticas["TotalRegistrosHoy"] = asistenciasHoy.Count;

                // Consumos del día actual
                var consumosHoy = await _consumoMenuRepository.ObtenerPorFechaAsync(DateTime.Today);
                estadisticas["ConsumosMenuHoy"] = consumosHoy.Count;

                // Cargos pendientes
                var cargosPendientes = await _cargoMensualRepository.ObtenerPendientesAsync();
                estadisticas["CargosPendientes"] = cargosPendientes.Count;
                estadisticas["MontoTotalPendiente"] = cargosPendientes.Sum(c => c.TotalCargo);

                // Niños con alergias
                var ninosConAlergias = await _alergiaRepository.GetNinosConAlergiasAsync();
                estadisticas["NinosConAlergias"] = ninosConAlergias.Count;

                // Ingresos del mes actual
                var mesActual = DateTime.Now.Month;
                var añoActual = DateTime.Now.Year;
                var ingresosMesActual = await GenerarReporteIngresosMensualesAsync(mesActual, añoActual);
                estadisticas["IngresosMesActual"] = ingresosMesActual;

                // Porcentaje de asistencia promedio del mes
                var porcentajesAsistencia = await GenerarReportePorcentajeAsistenciaAsync(mesActual, añoActual);
                var promedioAsistencia = porcentajesAsistencia.Values.Any() ? porcentajesAsistencia.Values.Average() : 0;
                estadisticas["PromedioAsistenciaMes"] = Math.Round(promedioAsistencia, 2);

                estadisticas["FechaGeneracion"] = DateTime.Now;
                estadisticas["Estado"] = "Exitoso";
            }
            catch (Exception ex)
            {
                estadisticas["Estado"] = "Error";
                estadisticas["MensajeError"] = ex.Message;
                estadisticas["FechaGeneracion"] = DateTime.Now;
            }

            return estadisticas;
        }

        public async Task<List<Nino>> GenerarReporteNiñosActivosAsync()
        {
            return await _ninoRepository.ObtenerActivosAsync();
        }

        public async Task<List<Nino>> GenerarReporteNiñosInactivosAsync()
        {
            return await _ninoRepository.ObtenerInactivosAsync();
        }
    }
}