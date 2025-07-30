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
    public class ResponsablePagoService : IResponsablePagoService
    {
        private readonly IResponsablePagoRepository _responsablePagoRepository;
        private readonly ICargoMensualRepository _cargoMensualRepository;

        public ResponsablePagoService(
            IResponsablePagoRepository responsablePagoRepository,
            ICargoMensualRepository cargoMensualRepository = null)
        {
            _responsablePagoRepository = responsablePagoRepository;
            _cargoMensualRepository = cargoMensualRepository;
        }

        public async Task<ResponsablePago> ObtenerPorIdAsync(int id)
        {
            return await _responsablePagoRepository.ObtenerPorIdAsync(id);
        }

        public async Task<ResponsablePago> ObtenerPorCedulaAsync(string cedula)
        {
            return await _responsablePagoRepository.ObtenerPorCedulaAsync(cedula);
        }

        public async Task<List<ResponsablePago>> ObtenerTodosAsync()
        {
            return await _responsablePagoRepository.ObtenerTodosAsync();
        }

        public async Task<ResponsablePago> ObtenerPorNiñoAsync(int ninoId)
        {
            var responsables = await _responsablePagoRepository.ObtenerPorNinoIdAsync(ninoId);
            return responsables.FirstOrDefault();
        }

        public async Task<int> RegistrarResponsablePagoAsync(ResponsablePago responsablePago)
        {
            // Validaciones
            if (!await ValidarDatosObligatoriosAsync(responsablePago))
            {
                throw new ArgumentException("Faltan datos obligatorios del responsable de pago");
            }

            if (await _responsablePagoRepository.ExistePorCedulaAsync(responsablePago.Cedula))
            {
                throw new InvalidOperationException("Ya existe un responsable de pago con esa cédula");
            }

            if (!string.IsNullOrWhiteSpace(responsablePago.CuentaCorriente))
            {
                if (!await ValidarCuentaCorrienteAsync(responsablePago.CuentaCorriente))
                {
                    throw new ArgumentException("El formato de la cuenta corriente no es válido");
                }
            }

            // Limpiar y normalizar datos
            responsablePago.Cedula = responsablePago.Cedula.Trim();
            responsablePago.Nombre = responsablePago.Nombre.Trim();

            if (!string.IsNullOrWhiteSpace(responsablePago.Direccion))
            {
                responsablePago.Direccion = responsablePago.Direccion.Trim();
            }

            if (!string.IsNullOrWhiteSpace(responsablePago.Telefono))
            {
                responsablePago.Telefono = responsablePago.Telefono.Trim();
            }

            if (!string.IsNullOrWhiteSpace(responsablePago.CuentaCorriente))
            {
                responsablePago.CuentaCorriente = responsablePago.CuentaCorriente.Trim();
            }

            await _responsablePagoRepository.AgregarAsync(responsablePago);
            return responsablePago.Id;
        }

        public async Task ActualizarResponsablePagoAsync(ResponsablePago responsablePago)
        {
            if (responsablePago == null)
            {
                throw new ArgumentException("El responsable de pago no puede ser nulo");
            }

            var responsableExistente = await _responsablePagoRepository.ObtenerPorIdAsync(responsablePago.Id);
            if (responsableExistente == null)
            {
                throw new ArgumentException("El responsable de pago especificado no existe");
            }

            if (!await ValidarDatosObligatoriosAsync(responsablePago))
            {
                throw new ArgumentException("Faltan datos obligatorios del responsable de pago");
            }

            // Verificar cédula unica si cambio
            if (responsableExistente.Cedula != responsablePago.Cedula)
            {
                if (await _responsablePagoRepository.ExistePorCedulaAsync(responsablePago.Cedula))
                {
                    throw new InvalidOperationException("Ya existe un responsable de pago con esa cédula");
                }
            }

            if (!string.IsNullOrWhiteSpace(responsablePago.CuentaCorriente))
            {
                if (!await ValidarCuentaCorrienteAsync(responsablePago.CuentaCorriente))
                {
                    throw new ArgumentException("El formato de la cuenta corriente no es válido");
                }
            }

            // Limpiar datos
            responsablePago.Cedula = responsablePago.Cedula.Trim();
            responsablePago.Nombre = responsablePago.Nombre.Trim();

            if (!string.IsNullOrWhiteSpace(responsablePago.Direccion))
            {
                responsablePago.Direccion = responsablePago.Direccion.Trim();
            }

            if (!string.IsNullOrWhiteSpace(responsablePago.Telefono))
            {
                responsablePago.Telefono = responsablePago.Telefono.Trim();
            }

            if (!string.IsNullOrWhiteSpace(responsablePago.CuentaCorriente))
            {
                responsablePago.CuentaCorriente = responsablePago.CuentaCorriente.Trim();
            }

            await _responsablePagoRepository.ActualizarAsync(responsablePago);
        }

        public async Task AsignarANiñoAsync(int responsablePagoId, int ninoId)
        {
            var responsable = await _responsablePagoRepository.ObtenerPorIdAsync(responsablePagoId);
            if (responsable == null)
            {
                throw new ArgumentException("El responsable de pago especificado no existe");
            }

            // TODO: Crear desde el NinoService
        }

        public async Task<bool> ValidarDatosObligatoriosAsync(ResponsablePago responsable)
        {
            if (responsable == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(responsable.Cedula))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(responsable.Nombre))
            {
                return false;
            }

            // Validar formato básico de cédula (solo números y guiones)
            var cedulaLimpia = responsable.Cedula.Replace("-", "").Replace(" ", "");
            if (cedulaLimpia.Length < 8 || !cedulaLimpia.All(char.IsDigit))
            {
                return false;
            }

            await Task.CompletedTask;
            return true;
        }

        public async Task<bool> ValidarCuentaCorrienteAsync(string cuentaCorriente)
        {
            if (string.IsNullOrWhiteSpace(cuentaCorriente))
            {
                // La cuenta corriente es opcional
                return true;
            }

            // Formato esperado: números y guiones, longitud mínima
            var cuentaLimpia = cuentaCorriente.Replace("-", "").Replace(" ", "");

            // Debe tener al menos 10 dígitos y máximo 20
            if (cuentaLimpia.Length < 10 || cuentaLimpia.Length > 20)
            {
                return false;
            }

            if (!cuentaLimpia.All(char.IsDigit))
            {
                return false;
            }

            await Task.CompletedTask;
            return true;
        }

        public async Task<decimal> ObtenerTotalPendienteAsync(int responsablePagoId)
        {
            if (_cargoMensualRepository == null)
            {
                return 0; // Sin acceso al repositorio de cargos
            }

            return await _cargoMensualRepository.ObtenerTotalPendientePorResponsableAsync(responsablePagoId);
        }

        public async Task EliminarResponsablePagoAsync(int id)
        {
            var responsable = await _responsablePagoRepository.ObtenerPorIdAsync(id);
            if (responsable == null)
            {
                throw new ArgumentException("El responsable de pago especificado no existe");
            }

            if (responsable.NinosAPagar != null && responsable.NinosAPagar.Any())
            {
                throw new InvalidOperationException(
                    "No se puede eliminar el responsable porque tiene niños asignados. " +
                    "Debe reasignar los niños a otro responsable primero.");
            }

            if (_cargoMensualRepository != null)
            {
                var totalPendiente = await _cargoMensualRepository.ObtenerTotalPendientePorResponsableAsync(id);
                if (totalPendiente > 0)
                {
                    throw new InvalidOperationException(
                        $"No se puede eliminar el responsable porque tiene cargos pendientes por un total de ${totalPendiente:F2}. " +
                        "Debe liquidar todos los pagos pendientes primero.");
                }
            }

            await _responsablePagoRepository.EliminarAsync(id);
        }
    }
}