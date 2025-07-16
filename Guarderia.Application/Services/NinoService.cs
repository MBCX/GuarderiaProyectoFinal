using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;

namespace Guarderia.Application.Services
{
    internal class NinoService
    {
        private readonly INinoRepository _NinoRepository;

        public NinoService(INinoRepository NinoRepository)
        {
            _NinoRepository = NinoRepository;
        }

        public async Task<Nino> ObtenerNinoPorIdAsync(int id)
        {
            return await _NinoRepository.ObtenerPorIdAsync(id);
        }

        public async Task<List<Nino>> ObtenerTodosLosNinosAsync()
        {
            return await _NinoRepository.ObtenerTodosAsync();
        }

        public async Task<List<Nino>> ObtenerNinosActivosAsync()
        {
            var todosLosNinos = await _NinoRepository.ObtenerTodosAsync();
            return todosLosNinos.Where(n => n.FechaBaja == null).ToList();
        }

        public async Task<int> RegistrarNinoAsync(Nino nino)
        {
            if (!await ValidarDatosObligatoriosAsync(nino))
            {
                throw new ArgumentException("Faltan campos obligatorios para el registro del Nino");
            }

            nino.FechaIngreso = DateTime.Now;
            nino.FechaBaja = null; // Nino activo

            await _NinoRepository.AgregarAsync(nino);
            return nino.Id;
        }

        public async Task ActualizarNinoAsync(Nino nino)
        {
            if (!await ValidarDatosObligatoriosAsync(nino))
            {
                throw new ArgumentException("Faltan campos obligatorios para actualizar el Nino");
            }

            await _NinoRepository.ActualizarAsync(nino);
        }

        public async Task DarBajaNinoAsync(int NinoId, DateTime fechaBaja)
        {
            var nino = await _NinoRepository.ObtenerPorIdAsync(NinoId);
            if (nino == null)
            {
                throw new ArgumentException("Nino no encontrado");
            }

            nino.FechaBaja = fechaBaja;
            await _NinoRepository.ActualizarAsync(nino);
        }

        public async Task<bool> ValidarDatosObligatoriosAsync(Nino nino)
        {
            return await Task.FromResult(
                !string.IsNullOrWhiteSpace(nino.Nombre) &&
                nino.FechaNacimiento != default(DateTime)
            );
        }
    }
}
