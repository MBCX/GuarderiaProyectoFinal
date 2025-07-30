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
    public class AsistenciaService : IAsistenciaService
    {
        private readonly IAsistenciaRepository _asistenciaRepository;
        private readonly INinoRepository _ninoRepository;
        private readonly ICostoFijoMensualRepository _costoFijoRepository;

        public AsistenciaService(
            IAsistenciaRepository asistenciaRepository,
            INinoRepository ninoRepository,
            ICostoFijoMensualRepository costoFijoRepository = null)
        {
            _asistenciaRepository = asistenciaRepository;
            _ninoRepository = ninoRepository;
            _costoFijoRepository = costoFijoRepository;
        }

        public async Task<List<Asistencia>> ObtenerAsistenciasPorNiñoAsync(int ninoId)
        {
            return await _asistenciaRepository.ObtenerPorNinoIdAsync(ninoId);
        }

        public async Task<List<Asistencia>> ObtenerAsistenciasPorFechaAsync(DateTime fecha)
        {
            return await _asistenciaRepository.ObtenerPorFechaAsync(fecha);
        }

        public async Task<List<Asistencia>> ObtenerAsistenciasPorMesAsync(int mes, int año)
        {
            return await _asistenciaRepository.ObtenerPorMesYAñoAsync(mes, año);
        }

        public async Task<Asistencia> ObtenerAsistenciaPorNiñoYFechaAsync(int ninoId, DateTime fecha)
        {
            var asistencias = await _asistenciaRepository.ObtenerPorNinoIdAsync(ninoId);
            return asistencias.FirstOrDefault(a => a.Fecha.Date == fecha.Date);
        }

        public async Task RegistrarAsistenciaAsync(int ninoId, DateTime fecha, bool asistio)
        {
            var nino = await _ninoRepository.ObtenerPorIdAsync(ninoId);
            if (nino == null)
            {
                throw new ArgumentException("El niño especificado no existe");
            }

            if (!nino.Activo)
            {
                throw new InvalidOperationException("No se puede registrar asistencia para un niño inactivo");
            }

            if (fecha.Date > DateTime.Now.Date)
            {
                throw new ArgumentException("No se puede registrar asistencia para fechas futuras");
            }

            if (fecha.Date < nino.FechaIngreso.Date)
            {
                throw new ArgumentException("No se puede registrar asistencia anterior a la fecha de ingreso");
            }

            if (await YaRegistradaAsync(ninoId, fecha))
            {
                throw new InvalidOperationException($"Ya existe un registro de asistencia para el niño en la fecha {fecha:dd/MM/yyyy}");
            }

            var asistencia = new Asistencia
            {
                NinoId = ninoId,
                Fecha = fecha.Date,
                Asistio = asistio,
                Nino = nino
            };

            await _asistenciaRepository.RegistrarAsync(asistencia);
        }

        public async Task ActualizarAsistenciaAsync(Asistencia asistencia)
        {
            if (asistencia == null)
            {
                throw new ArgumentException("La asistencia no puede ser nula");
            }

            var asistenciaExistente = await _asistenciaRepository.ObtenerPorIdAsync(asistencia.Id);
            if (asistenciaExistente == null)
            {
                throw new ArgumentException("La asistencia especificada no existe");
            }

            var nino = await _ninoRepository.ObtenerPorIdAsync(asistencia.NinoId);
            if (nino == null)
            {
                throw new ArgumentException("El niño especificado no existe");
            }

            await _asistenciaRepository.ActualizarAsync(asistencia);
        }

        public async Task<int> CalcularDiasAsistenciaDelMesAsync(int ninoId, int mes, int año)
        {
            return await _asistenciaRepository.ContarAsistenciasMensualesAsync(ninoId, mes, año);
        }

        public async Task<decimal> CalcularPorcentajeAsistenciaAsync(int ninoId, int mes, int año)
        {
            return await _asistenciaRepository.CalcularPorcentajeAsistenciaAsync(ninoId, mes, año);
        }

        public async Task<decimal> CalcularCostoAsistenciaMensualAsync(int ninoId, int mes, int año)
        {
            if (_costoFijoRepository == null)
            {
                return 0;
            }

            var diasAsistencia = await CalcularDiasAsistenciaDelMesAsync(ninoId, mes, año);

            // Obtener el costo fijo vigente para el mes/año especificado
            var fechaReferencia = new DateTime(año, mes, 1);
            var costoFijo = await _costoFijoRepository.ObtenerMontoVigenteAsync(fechaReferencia);

            // Calcular días hábiles del mes
            // (aproximación: 22 días hábiles por mes)
            var diasHabilesDelMes = CalcularDiasHabilesDelMes(mes, año);

            if (diasHabilesDelMes == 0)
            {
                return 0;
            }

            // Costo proporcional basado en días de asistencia
            var costoPorDia = costoFijo / diasHabilesDelMes;
            return costoPorDia * diasAsistencia;
        }

        public async Task<bool> YaRegistradaAsync(int ninoId, DateTime fecha)
        {
            return await _asistenciaRepository.YaRegistradaAsync(ninoId, fecha);
        }

        public async Task<List<Asistencia>> GenerarReporteAsistenciaAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            var asistencias = new List<Asistencia>();

            var fechaActual = fechaInicio.Date;
            while (fechaActual <= fechaFin.Date)
            {
                var asistenciasDia = await ObtenerAsistenciasPorFechaAsync(fechaActual);
                asistencias.AddRange(asistenciasDia);
                fechaActual = fechaActual.AddDays(1);
            }

            return asistencias.OrderBy(a => a.Fecha).ThenBy(a => a.Nino?.Nombre).ToList();
        }

        public async Task<List<Asistencia>> GenerarReporteAsistenciaMensualAsync(int mes, int año)
        {
            return await ObtenerAsistenciasPorMesAsync(mes, año);
        }

        public async Task EliminarAsistenciaAsync(int id)
        {
            var asistencia = await _asistenciaRepository.ObtenerPorIdAsync(id);
            if (asistencia == null)
            {
                throw new ArgumentException("La asistencia especificada no existe");
            }

            await _asistenciaRepository.EliminarAsync(id);
        }

        public async Task<int> ContarAsistentesDelDiaAsync(DateTime fecha)
        {
            var asistencias = await ObtenerAsistenciasPorFechaAsync(fecha);
            return asistencias.Count(a => a.Asistio);
        }

        private int CalcularDiasHabilesDelMes(int mes, int año)
        {
            DateTime primerDia = new DateTime(año, mes, 1);
            DateTime ultimoDia = primerDia.AddMonths(1).AddDays(-1);

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