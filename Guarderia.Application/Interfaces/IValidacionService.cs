using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Domain.Entities;

namespace Guarderia.Application.Interfaces
{
    public interface IValidacionService
    {
        // Validaciones de Niño
        Task<bool> ValidarDatosNiñoAsync(Nino nino);
        Task<bool> ValidarMatriculaUnicaAsync(string numeroMatricula, int? ninoIdExcluir = null);

        // Validaciones de Persona Autorizada
        Task<bool> ValidarDatosPersonaAutorizadaAsync(PersonaAutorizada persona);
        Task<bool> ValidarCedulaAsync(string cedula);
        Task<bool> ValidarTelefonoAsync(string telefono);

        // Validaciones de Responsable de Pago
        Task<bool> ValidarDatosResponsablePagoAsync(ResponsablePago responsable);
        Task<bool> ValidarCuentaCorrienteAsync(string cuentaCorriente);

        // Validaciones de Menú y Alergias
        Task<bool> ValidarMenuParaNiñoAsync(int menuId, int ninoId);
        Task<List<string>> ValidarAlergiasEnMenuAsync(int menuId, int ninoId);
        Task<bool> ValidarPlatoParaNiñoAsync(int platoId, int ninoId);

        // Validaciones de Fechas
        Task<bool> ValidarFechaNacimientoAsync(DateTime fechaNacimiento);
        Task<bool> ValidarFechaIngresoAsync(DateTime fechaIngreso, DateTime fechaNacimiento);
        Task<bool> ValidarFechaBajaAsync(DateTime fechaBaja, DateTime fechaIngreso);

        // Validaciones Generales
        Task<bool> ValidarEmailAsync(string email);
        Task<bool> ValidarTextoObligatorioAsync(string texto);
        Task<bool> ValidarMontoAsync(decimal monto);
    }
}
