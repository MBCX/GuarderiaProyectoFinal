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
    public class NinoService : INinoService
    {
        private readonly INinoRepository _ninoRepository;
        private readonly IResponsablePagoRepository _responsablePagoRepository;
        private readonly IAlergiaRepository _alergiaRepository;
        private readonly IValidacionService _validacionService;

        public NinoService(
            INinoRepository ninoRepository,
            IResponsablePagoRepository responsablePagoRepository,
            IAlergiaRepository alergiaRepository,
            IValidacionService validacionService = null)
        {
            _ninoRepository = ninoRepository;
            _responsablePagoRepository = responsablePagoRepository;
            _alergiaRepository = alergiaRepository;
            _validacionService = validacionService;
        }

        public async Task<Nino> ObtenerNinoPorIdAsync(int id)
        {
            return await _ninoRepository.ObtenerPorIdAsync(id);
        }

        public async Task<Nino> ObtenerPorMatriculaAsync(string numeroMatricula)
        {
            return await _ninoRepository.ObtenerPorMatriculaAsync(numeroMatricula);
        }

        public async Task<List<Nino>> ObtenerTodosLosNinosAsync()
        {
            return await _ninoRepository.ObtenerTodosAsync();
        }

        public async Task<List<Nino>> ObtenerNinosActivosAsync()
        {
            return await _ninoRepository.ObtenerActivosAsync();
        }

        public async Task<List<Nino>> ObtenerNinosInactivosAsync()
        {
            return await _ninoRepository.ObtenerInactivosAsync();
        }

        public async Task<List<Nino>> ObtenerPorResponsablePagoAsync(int responsablePagoId)
        {
            return await _ninoRepository.ObtenerPorResponsablePagoAsync(responsablePagoId);
        }

        public async Task<int> RegistrarNinoAsync(Nino nino, int responsablePagoId)
        {
            // Validar datos obligatorios
            if (!await ValidarDatosObligatoriosAsync(nino))
            {
                throw new ArgumentException("Faltan campos obligatorios para el registro del nino");
            }

            // Validar que el responsable de pago existe
            var responsablePago = await _responsablePagoRepository.ObtenerPorIdAsync(responsablePagoId);
            if (responsablePago == null)
            {
                throw new ArgumentException("El responsable de pago especificado no existe");
            }

            // Generar número de matrícula si no se proporciona
            if (string.IsNullOrWhiteSpace(nino.NumeroMatricula))
            {
                nino.NumeroMatricula = await GenerarNumeroMatriculaAsync();
            }
            else
            {
                // Validar que la matrícula no existe
                if (await ExisteMatriculaAsync(nino.NumeroMatricula))
                {
                    throw new ArgumentException("Ya existe un nino con ese número de matrícula");
                }
            }

            // Establecer valores por defecto
            nino.FechaIngreso = DateTime.Now;
            nino.Activo = true;
            nino.ResponsablePagoId = responsablePagoId;

            await _ninoRepository.AgregarAsync(nino);
            return nino.Id;
        }

        public async Task ActualizarNinoAsync(Nino nino)
        {
            if (!await ValidarDatosObligatoriosAsync(nino))
            {
                throw new ArgumentException("Faltan campos obligatorios para actualizar el nino");
            }

            // Validar que existe
            var ninoExistente = await _ninoRepository.ObtenerPorIdAsync(nino.Id);
            if (ninoExistente == null)
            {
                throw new ArgumentException("El nino especificado no existe");
            }

            // Validar matrícula única (excluyendo el nino actual)
            if (ninoExistente.NumeroMatricula != nino.NumeroMatricula)
            {
                if (await ExisteMatriculaAsync(nino.NumeroMatricula))
                {
                    throw new ArgumentException("Ya existe un nino con ese número de matrícula");
                }
            }

            await _ninoRepository.ActualizarAsync(nino);
        }

        public async Task DarBajaNinoAsync(int ninoId, DateTime fechaBaja)
        {
            var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId);
            if (nino == null)
            {
                throw new ArgumentException("Nino no encontrado");
            }

            if (fechaBaja < nino.FechaIngreso)
            {
                throw new ArgumentException("La fecha de baja no puede ser anterior a la fecha de ingreso");
            }

            await _ninoRepository.DarBajaAsync(ninoId, fechaBaja);
        }

        public async Task ReactivarNinoAsync(int ninoId)
        {
            var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId);
            if (nino == null)
            {
                throw new ArgumentException("Nino no encontrado");
            }

            await _ninoRepository.ReactivarAsync(ninoId);
        }

        public async Task AsignarResponsablePagoAsync(int ninoId, int responsablePagoId)
        {
            var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId);
            if (nino == null)
            {
                throw new ArgumentException("Nino no encontrado");
            }

            var responsablePago = await _responsablePagoRepository.ObtenerPorIdAsync(responsablePagoId);
            if (responsablePago == null)
            {
                throw new ArgumentException("Responsable de pago no encontrado");
            }

            nino.ResponsablePagoId = responsablePagoId;
            await _ninoRepository.ActualizarAsync(nino);
        }

        public async Task<bool> ValidarDatosObligatoriosAsync(Nino nino)
        {
            if (nino == null) return false;

            return !string.IsNullOrWhiteSpace(nino.Nombre) &&
                   nino.FechaNacimiento != default(DateTime) &&
                   nino.FechaNacimiento <= DateTime.Now.AddYears(-1); // Al menos 1 ano
        }

        public async Task<bool> ExisteMatriculaAsync(string numeroMatricula)
        {
            return await _ninoRepository.ExisteMatriculaAsync(numeroMatricula);
        }

        public async Task<string> GenerarNumeroMatriculaAsync()
        {
            var ano = DateTime.Now.Year;
            var contador = await _ninoRepository.ContarActivosAsync() + 1;

            // Formato: YYYY-NNNN (ej: 2024-0001)
            return $"{ano}-{contador:D4}";
        }

        public async Task<int> ContarNinosActivosAsync()
        {
            return await _ninoRepository.ContarActivosAsync();
        }

        public async Task<List<string>> ObtenerAlergiasAsync(int ninoId)
        {
            return await _alergiaRepository.GetIngredientesAlergenosPorNinoAsync(ninoId);
        }

        public async Task EliminarNinoAsync(int id)
        {
            var nino = await _ninoRepository.ObtenerPorIdAsync(id);
            if (nino == null)
            {
                throw new ArgumentException("Nino no encontrado");
            }

            // Solo permitir eliminar ninos que no tengan registros asociados o estén inactivos
            if (nino.Activo)
            {
                throw new InvalidOperationException("No se puede eliminar un nino activo. Debe darlo de baja primero.");
            }

            await _ninoRepository.EliminarAsync(id);
        }
    }
}