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
    public class FacturacionService : IFacturacionService
    {
        private readonly ICargoMensualRepository _cargoMensualRepository;
        private readonly INinoRepository _ninoRepository;
        private readonly IAsistenciaRepository _asistenciaRepository;
        private readonly IConsumoMenuRepository _consumoMenuRepository;
        private readonly ICostoFijoMensualRepository _costoFijoRepository;

        public FacturacionService(
            ICargoMensualRepository cargoMensualRepository,
            INinoRepository ninoRepository,
            IAsistenciaRepository asistenciaRepository,
            IConsumoMenuRepository consumoMenuRepository,
            ICostoFijoMensualRepository costoFijoRepository)
        {
            _cargoMensualRepository = cargoMensualRepository;
            _ninoRepository = ninoRepository;
            _asistenciaRepository = asistenciaRepository;
            _consumoMenuRepository = consumoMenuRepository;
            _costoFijoRepository = costoFijoRepository;
        }

        public async Task<CargoMensual> GenerarCargoMensualAsync(int ninoId, int mes, int año)
        {
            var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId);
            if (nino == null)
            {
                throw new ArgumentException("El niño especificado no existe");
            }

            if (nino.ResponsablePagoId == null)
            {
                throw new InvalidOperationException("El niño no tiene un responsable de pago asignado");
            }

            var cargoExistente = await _cargoMensualRepository.ObtenerPorNinoYMesAsync(ninoId, mes, año);
            if (cargoExistente != null)
            {
                throw new InvalidOperationException($"Ya existe un cargo para el niño en {mes:D2}/{año}");
            }

            // Calcular costos
            var costoFijo = await CalcularCostoFijoAsync(mes, año);
            var costoMenus = await CalcularCostoMenusAsync(ninoId, mes, año);
            var descuentos = await CalcularDescuentosAsync(ninoId, mes, año);

            var totalCargo = costoFijo + costoMenus - descuentos;

            var cargo = new CargoMensual
            {
                NinoId = ninoId,
                ResponsablePagoId = nino.ResponsablePagoId.Value,
                Mes = mes,
                Año = año,
                CostoFijo = costoFijo,
                CostoComidas = costoMenus,
                TotalCargo = totalCargo,
                FechaGeneracion = DateTime.Now,
                Estado = "Pendiente",
                Nino = nino
            };

            await _cargoMensualRepository.GenerarAsync(cargo);
            return cargo;
        }

        public async Task GenerarCargosMasivosMensualAsync(int mes, int año)
        {
            var ninosActivos = await _ninoRepository.ObtenerActivosAsync();
            var ninosConResponsable = ninosActivos.Where(n => n.ResponsablePagoId.HasValue).ToList();

            var errores = new List<string>();

            foreach (var nino in ninosConResponsable)
            {
                try
                {
                    await GenerarCargoMensualAsync(nino.Id, mes, año);
                }
                catch (InvalidOperationException)
                {
                    // El cargo ya existe, continuar con el siguiente
                    continue;
                }
                catch (Exception ex)
                {
                    errores.Add($"Error generando cargo para {nino.Nombre} (ID: {nino.Id}): {ex.Message}");
                }
            }

            if (errores.Any())
            {
                throw new AggregateException($"Se encontraron {errores.Count} errores durante la generación masiva",
                    errores.Select(e => new Exception(e)));
            }
        }

        public async Task<CargoMensual> RecalcularCargoAsync(int cargoId)
        {
            var cargo = await _cargoMensualRepository.ObtenerPorIdAsync(cargoId);
            if (cargo == null)
            {
                throw new ArgumentException("El cargo especificado no existe");
            }

            // Recalcular costos
            var costoFijo = await CalcularCostoFijoAsync(cargo.Mes, cargo.Año);
            var costoMenus = await CalcularCostoMenusAsync(cargo.NinoId, cargo.Mes, cargo.Año);
            var descuentos = await CalcularDescuentosAsync(cargo.NinoId, cargo.Mes, cargo.Año);

            cargo.CostoFijo = costoFijo;
            cargo.CostoComidas = costoMenus;
            cargo.TotalCargo = costoFijo + costoMenus - descuentos;

            await _cargoMensualRepository.ActualizarAsync(cargo);
            return cargo;
        }

        public async Task MarcarComoPagadoAsync(int cargoId, DateTime fechaPago, string metodoPago)
        {
            var cargo = await _cargoMensualRepository.ObtenerPorIdAsync(cargoId);
            if (cargo == null)
            {
                throw new ArgumentException("El cargo especificado no existe");
            }

            if (cargo.Estado == "Pagado")
            {
                throw new InvalidOperationException("El cargo ya está marcado como pagado");
            }

            if (fechaPago < cargo.FechaGeneracion.Date)
            {
                throw new ArgumentException("La fecha de pago no puede ser anterior a la fecha de generación");
            }

            cargo.FechaPago = fechaPago;
            cargo.Estado = "Pagado";

            await _cargoMensualRepository.ActualizarAsync(cargo);
        }

        public async Task MarcarComoPendienteAsync(int cargoId, string motivo)
        {
            var cargo = await _cargoMensualRepository.ObtenerPorIdAsync(cargoId);
            if (cargo == null)
            {
                throw new ArgumentException("El cargo especificado no existe");
            }

            cargo.FechaPago = null;
            cargo.Estado = "Pendiente";

            await _cargoMensualRepository.ActualizarAsync(cargo);
        }

        public async Task<decimal> CalcularTotalPendienteAsync(int responsablePagoId)
        {
            return await _cargoMensualRepository.ObtenerTotalPendientePorResponsableAsync(responsablePagoId);
        }

        public async Task<decimal> CalcularCostoAsistenciaAsync(int ninoId, int mes, int año)
        {
            var diasAsistencia = await _asistenciaRepository.ContarAsistenciasMensualesAsync(ninoId, mes, año);
            var costoFijo = await CalcularCostoFijoAsync(mes, año);

            // Calcular días hábiles del mes (aproximación: 22 días por mes)
            var diasHabilesDelMes = CalcularDiasHabilesDelMes(mes, año);

            if (diasHabilesDelMes == 0)
                return costoFijo;

            // Costo proporcional por días de asistencia
            return (costoFijo / diasHabilesDelMes) * diasAsistencia;
        }

        public async Task<decimal> CalcularCostoMenusAsync(int ninoId, int mes, int año)
        {
            return await _consumoMenuRepository.CalcularCostoMenusMensualAsync(ninoId, mes, año);
        }

        public async Task<decimal> CalcularCostoFijoAsync(int mes, int año)
        {
            var fechaReferencia = new DateTime(año, mes, 1);
            return await _costoFijoRepository.ObtenerMontoVigenteAsync(fechaReferencia);
        }

        public async Task<decimal> CalcularDescuentosAsync(int ninoId, int mes, int año)
        {
            // TODO: implementar descuentos por:
            // - Hermanos en la guardería
            // - Problemas económicos documentados
            // - Promociones especiales
            // - etc.

            await Task.CompletedTask;
            return 0;
        }

        public async Task<List<CargoMensual>> GenerarEstadoCuentaAsync(int responsablePagoId, int año)
        {
            var cargos = await _cargoMensualRepository.ObtenerPorResponsableAsync(responsablePagoId);
            return cargos.Where(c => c.Año == año).OrderBy(c => c.Mes).ToList();
        }

        public async Task<decimal> CalcularIngresosTotalesAsync(int mes, int año)
        {
            var cargos = await _cargoMensualRepository.ObtenerPorMesYAñoAsync(mes, año);
            return cargos.Where(c => c.Estado == "Pagado").Sum(c => c.TotalCargo);
        }

        public async Task<List<CargoMensual>> ObtenerFacturacionPendienteAsync()
        {
            return await _cargoMensualRepository.ObtenerPendientesAsync();
        }

        public async Task<Dictionary<int, decimal>> GenerarResumenFacturacionAsync(int año)
        {
            var resumen = new Dictionary<int, decimal>();

            for (int mes = 1; mes <= 12; mes++)
            {
                var ingresosMes = await CalcularIngresosTotalesAsync(mes, año);
                resumen[mes] = ingresosMes;
            }

            return resumen;
        }

        private int CalcularDiasHabilesDelMes(int mes, int año)
        {
            var primerDia = new DateTime(año, mes, 1);
            var ultimoDia = primerDia.AddMonths(1).AddDays(-1);

            int diasHabiles = 0;
            var fechaActual = primerDia;

            while (fechaActual <= ultimoDia)
            {
                // Lunes a Viernes son dias habiles
                if (
                    fechaActual.DayOfWeek != DayOfWeek.Saturday &&
                    fechaActual.DayOfWeek != DayOfWeek.Sunday
                )
                {
                    diasHabiles++;
                }
                fechaActual = fechaActual.AddDays(1);
            }

            return diasHabiles;
        }
    }
}