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
    public class CostoFijoMensualService : ICostoFijoMensualService
    {
        private readonly ICostoFijoMensualRepository _costoFijoRepository;

        public CostoFijoMensualService(ICostoFijoMensualRepository costoFijoRepository)
        {
            _costoFijoRepository = costoFijoRepository;
        }

        public async Task<CostoFijoMensual> ObtenerActivoAsync()
        {
            return await _costoFijoRepository.ObtenerActivoAsync();
        }

        public async Task<decimal> ObtenerMontoVigenteAsync(DateTime fecha)
        {
            return await _costoFijoRepository.ObtenerMontoVigenteAsync(fecha);
        }

        public async Task<List<CostoFijoMensual>> ObtenerHistorialAsync()
        {
            return await _costoFijoRepository.ObtenerHistorialAsync();
        }

        public async Task<int> CrearCostoFijoAsync(decimal monto, DateTime fechaVigencia, string descripcion)
        {
            // Validaciones
            if (!await ValidarMontoAsync(monto))
            {
                throw new ArgumentException("El monto debe ser mayor a 0");
            }

            if (string.IsNullOrWhiteSpace(descripcion))
            {
                throw new ArgumentException("La descripción es obligatoria");
            }

            if (fechaVigencia.Date < DateTime.Now.Date)
            {
                throw new ArgumentException("La fecha de vigencia no puede ser anterior al día actual");
            }

            // Desactivar el costo fijo actual si existe uno activo
            var costoActual = await ObtenerActivoAsync();
            if (costoActual != null)
            {
                await DesactivarCostoAsync(costoActual.Id);
            }

            var nuevoCosto = new CostoFijoMensual
            {
                Monto = monto,
                FechaVigenciaDesde = fechaVigencia.Date,
                FechaVigenciaHasta = null,
                Descripcion = descripcion,
                Activo = true
            };

            await _costoFijoRepository.AgregarAsync(nuevoCosto);
            return nuevoCosto.Id;
        }

        public async Task ActualizarCostoFijoAsync(CostoFijoMensual costoFijo)
        {
            if (costoFijo == null)
            {
                throw new ArgumentException("El costo fijo no puede ser nulo");
            }

            var costoExistente = await _costoFijoRepository.ObtenerPorIdAsync(costoFijo.Id);
            if (costoExistente == null)
            {
                throw new ArgumentException("El costo fijo especificado no existe");
            }

            if (!await ValidarMontoAsync(costoFijo.Monto))
            {
                throw new ArgumentException("El monto debe ser mayor a 0");
            }

            if (string.IsNullOrWhiteSpace(costoFijo.Descripcion))
            {
                throw new ArgumentException("La descripción es obligatoria");
            }

            await _costoFijoRepository.ActualizarAsync(costoFijo);
        }

        public async Task ActivarNuevoCostoAsync(int id, DateTime fechaVigencia)
        {
            var costoFijo = await _costoFijoRepository.ObtenerPorIdAsync(id);
            if (costoFijo == null)
            {
                throw new ArgumentException("El costo fijo especificado no existe");
            }

            if (fechaVigencia.Date < DateTime.Now.Date)
            {
                throw new ArgumentException("La fecha de vigencia no puede ser anterior al día actual");
            }

            // Desactivar el costo actual si existe
            var costoActual = await ObtenerActivoAsync();
            if (costoActual != null && costoActual.Id != id)
            {
                costoActual.FechaVigenciaHasta = fechaVigencia.Date.AddDays(-1);
                costoActual.Activo = false;
                await _costoFijoRepository.ActualizarAsync(costoActual);
            }

            // Activar el nuevo costo
            costoFijo.FechaVigenciaDesde = fechaVigencia.Date;
            costoFijo.FechaVigenciaHasta = null;
            costoFijo.Activo = true;

            await _costoFijoRepository.ActualizarAsync(costoFijo);
        }

        public async Task DesactivarCostoAsync(int id)
        {
            var costoFijo = await _costoFijoRepository.ObtenerPorIdAsync(id);
            if (costoFijo == null)
            {
                throw new ArgumentException("El costo fijo especificado no existe");
            }

            if (!costoFijo.Activo)
            {
                throw new InvalidOperationException("El costo fijo ya está desactivado");
            }

            costoFijo.FechaVigenciaHasta = DateTime.Now.Date;
            costoFijo.Activo = false;

            await _costoFijoRepository.ActualizarAsync(costoFijo);
        }

        public async Task<bool> ValidarMontoAsync(decimal monto)
        {
            await Task.CompletedTask;
            return monto > 0;
        }

        public async Task<bool> TieneCostoVigenteAsync(DateTime fecha)
        {
            var monto = await ObtenerMontoVigenteAsync(fecha);
            return monto > 0;
        }
    }
}