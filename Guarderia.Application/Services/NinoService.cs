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
        private readonly INiñoRepository _niñoRepository;

        public NinoService(INiñoRepository niñoRepository)
        {
            _niñoRepository = niñoRepository;
        }

        public async Task<Nino> ObtenerNiñoPorIdAsync(int id)
        {
            return await _niñoRepository.ObtenerPorIdAsync(id);
        }

        public async Task<List<Nino>> ObtenerTodosLosNiñosAsync()
        {
            return await _niñoRepository.ObtenerTodosAsync();
        }

        public async Task<List<Nino>> ObtenerNiñosActivosAsync()
        {
            var todosLosNiños = await _niñoRepository.ObtenerTodosAsync();
            return todosLosNiños.Where(n => n.FechaBaja == null).ToList();
        }

        public async Task<int> RegistrarNiñoAsync(Nino nino)
        {
            if (!await ValidarDatosObligatoriosAsync(nino))
            {
                throw new ArgumentException("Faltan campos obligatorios para el registro del niño");
            }

            nino.FechaIngreso = DateTime.Now;
            nino.FechaBaja = null; // Niño activo

            await _niñoRepository.AgregarAsync(nino);
            return nino.Id;
        }

        public async Task ActualizarNiñoAsync(Nino nino)
        {
            if (!await ValidarDatosObligatoriosAsync(nino))
            {
                throw new ArgumentException("Faltan campos obligatorios para actualizar el niño");
            }

            await _niñoRepository.ActualizarAsync(nino);
        }

        public async Task DarBajaNiñoAsync(int niñoId, DateTime fechaBaja)
        {
            var nino = await _niñoRepository.ObtenerPorIdAsync(niñoId);
            if (nino == null)
            {
                throw new ArgumentException("Niño no encontrado");
            }

            nino.FechaBaja = fechaBaja;
            await _niñoRepository.ActualizarAsync(nino);
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
}
