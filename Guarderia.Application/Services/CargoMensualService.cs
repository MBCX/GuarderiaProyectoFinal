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
    public class CargoMensualService : ICargoMensualService
    {
        private readonly ICargoMensualRepository _cargoMensualRepository;
        private readonly INinoRepository _ninoRepository;
        private readonly IAsistenciaRepository _asistenciaRepository;
        private readonly IConsumoMenuRepository _consumoMenuRepository;
        private readonly ICostoFijoMensualRepository _costoFijoRepository;

        public CargoMensualService(
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

        public async Task<CargoMensual> ObtenerPorIdAsync(int id)
        {
            return await _cargoMensualRepository.ObtenerPorIdAsync(id);
        }

        public async Task<List<CargoMensual>> ObtenerPorNiñoAsync(int ninoId)
        {
            return await _cargoMensualRepository.ObtenerPorNinoIdAsync(ninoId);
        }

        public async Task<CargoMensual> ObtenerPorNiñoYMesAsync(int ninoId, int mes, int año)
        {
            return await _cargoMensualRepository.ObtenerPorNinoYMesAsync(ninoId, mes, año);
        }

        public async Task<List<CargoMensual>> ObtenerPorMesAsync(int mes, int año)
        {
            return await _cargoMensualRepository.ObtenerPorMesYAñoAsync(mes, año);
        }

        public async Task<List<CargoMensual>> ObtenerPendientesAsync()
        {
            return await _cargoMensualRepository.ObtenerPendientesAsync();
        }

        public async Task<List<CargoMensual>> ObtenerPorResponsableAsync(int responsablePagoId)
        {
            return await _cargoMensualRepository.ObtenerPorResponsableAsync(responsablePagoId);
        }

        public async Task<int> GenerarCargoMensualAsync(int ninoId, int mes, int año)
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

            if (await YaExisteCargoAsync(ninoId, mes, año))
            {
                throw new InvalidOperationException($"Ya existe un cargo para el niño en {mes:D2}/{año}");
            }

            var cargo = await CalcularCargoMensualAsync(ninoId, mes, año);
            await _cargoMensualRepository.GenerarAsync(cargo);

            return cargo.Id;
        }

        public async Task<CargoMensual> CalcularCargoMensualAsync(int ninoId, int mes, int año)
        {
            var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId);
            if (nino == null)
            {
                throw new ArgumentException("El niño especificado no existe");
            }

            // Obtener costo fijo mensual vigente
            var fechaReferencia = new DateTime(año, mes, 1);
            var costoFijo = await _costoFijoRepository.ObtenerMontoVigenteAsync(fechaReferencia);
            var costoComidas = await _consumoMenuRepository.CalcularCostoMenusMensualAsync(ninoId, mes, año);

            var cargo = new CargoMensual
            {
                NinoId = ninoId,
                ResponsablePagoId = nino.ResponsablePagoId.Value,
                Mes = mes,
                Año = año,
                CostoFijo = costoFijo,
                CostoComidas = costoComidas,
                TotalCargo = costoFijo + costoComidas,
                FechaGeneracion = DateTime.Now,
                Estado = "Pendiente",
                Nino = nino
            };

            return cargo;
        }

        public async Task ActualizarCargoAsync(CargoMensual cargo)
        {
            if (cargo == null)
            {
                throw new ArgumentException("El cargo no puede ser nulo");
            }

            var cargoExistente = await _cargoMensualRepository.ObtenerPorIdAsync(cargo.Id);
            if (cargoExistente == null)
            {
                throw new ArgumentException("El cargo especificado no existe");
            }

            cargo.TotalCargo = cargo.CostoFijo + cargo.CostoComidas;

            await _cargoMensualRepository.ActualizarAsync(cargo);
        }

        public async Task MarcarComoPagadoAsync(int cargoId, DateTime fechaPago)
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
                throw new ArgumentException("La fecha de pago no puede ser anterior a la fecha de generación del cargo");
            }

            await _cargoMensualRepository.MarcarComoPagadoAsync(cargoId, fechaPago);
        }

        public async Task<decimal> ObtenerTotalPendientePorResponsableAsync(int responsablePagoId)
        {
            return await _cargoMensualRepository.ObtenerTotalPendientePorResponsableAsync(responsablePagoId);
        }

        public async Task<bool> YaExisteCargoAsync(int ninoId, int mes, int año)
        {
            var cargo = await _cargoMensualRepository.ObtenerPorNinoYMesAsync(ninoId, mes, año);
            return cargo != null;
        }

        public async Task RegenerarCargosDelMesAsync(int mes, int año)
        {
            var ninosActivos = await _ninoRepository.ObtenerActivosAsync();
            var ninosConResponsable = ninosActivos.Where(n => n.ResponsablePagoId.HasValue).ToList();

            foreach (var nino in ninosConResponsable)
            {
                try
                {
                    if (!await YaExisteCargoAsync(nino.Id, mes, año))
                    {
                        await GenerarCargoMensualAsync(nino.Id, mes, año);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error generando cargo para niño {nino.Id}: {ex.Message}");
                }
            }
        }

        public async Task<List<CargoMensual>> GenerarReporteCargosAsync(int mes, int año)
        {
            return await ObtenerPorMesAsync(mes, año);
        }
    }
}