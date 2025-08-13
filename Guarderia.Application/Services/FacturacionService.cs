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
            decimal totalDescuentos = 0;

            try
            {
                // 1. Descuento por hermanos en la guardería
                var descuentoHermanos = await CalcularDescuentoPorHermanosAsync(ninoId);
                totalDescuentos += descuentoHermanos;

                // 3. Descuento por promociones especiales
                var descuentoPromociones = await CalcularDescuentoPorPromocionesAsync(ninoId, mes, año);
                totalDescuentos += descuentoPromociones;

                // 4. Descuento por antigüedad en la guardería
                var descuentoAntiguedad = await CalcularDescuentoPorAntiguedadAsync(ninoId);
                totalDescuentos += descuentoAntiguedad;

                return totalDescuentos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                // En caso de error, devolver 0 para no afectar la facturación
                return 0;
            }
        }

        private async Task<decimal> CalcularDescuentoPorHermanosAsync(int ninoId)
        {
            if (_ninoRepository == null)
            {
                return 0;
            }

            try
            {
                var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId);
                if (nino?.ResponsablePago == null)
                {
                    return 0;
                }

                // Obtener todos los niños activos del mismo responsable de pago
                var hermanos = await _ninoRepository.ObtenerPorResponsablePagoAsync(
                    (int)nino?.ResponsablePagoId
                );
                var hermanosActivos = hermanos.Where(h => h.Activo && h.Id != ninoId).Count();

                // Aplicar descuento escalonado
                decimal porcentajeDescuento = hermanosActivos switch
                {
                    1 => 0.10m,  // 10% descuento por 1 hermano
                    2 => 0.15m,  // 15% descuento por 2 hermanos
                    3 => 0.20m,  // 20% descuento por 3 hermanos
                    >= 4 => 0.25m, // 25% descuento por 4 o más hermanos
                    _ => 0m
                };

                if (porcentajeDescuento > 0)
                {
                    var costoFijo = await CalcularCostoFijoAsync(DateTime.Now.Month, DateTime.Now.Year);
                    return costoFijo * porcentajeDescuento;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private async Task<decimal> CalcularDescuentoPorPromocionesAsync(int ninoId, int mes, int año)
        {
            await Task.CompletedTask;

            // Descuentos por promociones especiales según la fecha
            DateTime fechaReferencia = new DateTime(año, mes, 1);
            decimal descuentoPromocional = 0;

            // Promoción de inicio de año (Enero-Febrero)
            if (mes == 1 || mes == 2)
            {
                decimal costoFijo = await CalcularCostoFijoAsync(mes, año);
                descuentoPromocional = costoFijo * 0.05m; // 5% descuento
            }

            // Promoción de mitad de año (Junio-Julio)
            if (mes == 6 || mes == 7)
            {
                decimal costoFijo = await CalcularCostoFijoAsync(mes, año);
                descuentoPromocional = costoFijo * 0.03m; // 3% descuento
            }

            // Promoción navideña (Diciembre)
            if (mes == 12)
            {
                decimal costoFijo = await CalcularCostoFijoAsync(mes, año);
                descuentoPromocional = costoFijo * 0.08m; // 8% descuento
            }

            return descuentoPromocional;
        }

        private async Task<decimal> CalcularDescuentoPorAntiguedadAsync(int ninoId)
        {
            if (_ninoRepository == null) return 0;

            try
            {
                var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId);
                if (nino == null)
                {
                    return 0;
                }

                // Calcular años de antigüedad
                var tiempoEnGuarderia = DateTime.Now - nino.FechaIngreso;
                var añosAntiguedad = (int)(tiempoEnGuarderia.TotalDays / 365.25);

                // Aplicar descuento por antigüedad
                decimal porcentajeDescuento = añosAntiguedad switch
                {
                    >= 3 => 0.15m, // 15% descuento por 3 o más años
                    2 => 0.10m,    // 10% descuento por 2 años
                    1 => 0.05m,    // 5% descuento por 1 año
                    _ => 0m
                };

                if (porcentajeDescuento > 0)
                {
                    var costoFijo = await CalcularCostoFijoAsync(
                        DateTime.Now.Month,
                        DateTime.Now.Year
                    );
                    return costoFijo * porcentajeDescuento;
                }

                return 0;
            }
            catch
            {
                return 0;
            }
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