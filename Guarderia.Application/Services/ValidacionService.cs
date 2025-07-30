using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Guarderia.Application.Interfaces;
using Guarderia.Domain.Entities;
using Guarderia.Domain.Interfaces;

namespace Guarderia.Application.Services
{
    public class ValidacionService : IValidacionService
    {
        private readonly INinoRepository _ninoRepository;
        private readonly IPersonaAutorizadaRepository _personaAutorizadaRepository;
        private readonly IResponsablePagoRepository _responsablePagoRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IPlatoRepository _platoRepository;
        private readonly IAlergiaRepository _alergiaRepository;

        public ValidacionService(
            INinoRepository ninoRepository = null,
            IPersonaAutorizadaRepository personaAutorizadaRepository = null,
            IResponsablePagoRepository responsablePagoRepository = null,
            IMenuRepository menuRepository = null,
            IPlatoRepository platoRepository = null,
            IAlergiaRepository alergiaRepository = null)
        {
            _ninoRepository = ninoRepository;
            _personaAutorizadaRepository = personaAutorizadaRepository;
            _responsablePagoRepository = responsablePagoRepository;
            _menuRepository = menuRepository;
            _platoRepository = platoRepository;
            _alergiaRepository = alergiaRepository;
        }

        public async Task<bool> ValidarDatosNiñoAsync(Nino nino)
        {
            if (nino == null) return false;

            if (!await ValidarTextoObligatorioAsync(nino.Nombre))
            {
                return false;
            }

            if (!await ValidarFechaNacimientoAsync(nino.FechaNacimiento))
            {
                return false;
            }

            if (!await ValidarFechaIngresoAsync(nino.FechaIngreso, nino.FechaNacimiento))
            {
                return false;
            }

            if (nino.FechaBaja.HasValue)
            {
                if (!await ValidarFechaBajaAsync(nino.FechaBaja.Value, nino.FechaIngreso))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> ValidarMatriculaUnicaAsync(string numeroMatricula, int? ninoIdExcluir = null)
        {
            if (_ninoRepository == null) return true;

            if (string.IsNullOrWhiteSpace(numeroMatricula))
            {
                return false;
            }

            var ninoExistente = await _ninoRepository.ObtenerPorMatriculaAsync(numeroMatricula);

            if (ninoExistente == null)
            {
                return true;
            }

            // Si existe pero es el mismo niño que estamos editando, es válida
            if (
                ninoIdExcluir.HasValue &&
                ninoExistente.Id == ninoIdExcluir.Value
            )
            {
                return true;
            }

            // Ya existe en otro niño
            return false;
        }

        public async Task<bool> ValidarDatosPersonaAutorizadaAsync(PersonaAutorizada persona)
        {
            if (persona == null) return false;

            if (!await ValidarCedulaAsync(persona.Cedula))
            {
                return false;
            }

            if (!await ValidarTextoObligatorioAsync(persona.Nombre))
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(persona.Telefono))
            {
                if (!await ValidarTelefonoAsync(persona.Telefono))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> ValidarCedulaAsync(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
                return false;

            // Formato básico de cédula dominicana: ###-#######-#
            var cedulaPattern = @"^\d{3}-\d{7}-\d{1}$";
            var cedulaLimpia = cedula.Replace(" ", "");

            // Permitir formato sin guiones también
            if (!Regex.IsMatch(cedulaLimpia, cedulaPattern))
            {
                // Intentar con formato sin guiones: 11 dígitos
                var cedulaSinGuiones = cedulaLimpia.Replace("-", "");
                if (
                    cedulaSinGuiones.Length != 11 ||
                    !cedulaSinGuiones.All(char.IsDigit)
                )
                {
                    return false;
                }
            }

            await Task.CompletedTask;
            return true;
        }

        public async Task<bool> ValidarTelefonoAsync(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
            {
                return true; // Es opcional
            }

            // Remover espacios y caracteres especiales comunes
            var telefonoLimpio = telefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

            // Debe tener entre 7 y 15 dígitos
            if (telefonoLimpio.Length < 7 || telefonoLimpio.Length > 15)
            {
                return false;
            }

            // Solo números
            if (!telefonoLimpio.All(char.IsDigit))
            {
                return false;
            }

            await Task.CompletedTask;
            return true;
        }

        public async Task<bool> ValidarDatosResponsablePagoAsync(ResponsablePago responsable)
        {
            if (responsable == null) return false;

            if (!await ValidarCedulaAsync(responsable.Cedula))
            {
                return false;
            }

            if (!await ValidarTextoObligatorioAsync(responsable.Nombre))
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(responsable.Telefono))
            {
                if (!await ValidarTelefonoAsync(responsable.Telefono))
                {
                    return false;
                }
            }

            if (!string.IsNullOrWhiteSpace(responsable.CuentaCorriente))
            {
                if (!await ValidarCuentaCorrienteAsync(responsable.CuentaCorriente))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> ValidarCuentaCorrienteAsync(string cuentaCorriente)
        {
            if (string.IsNullOrWhiteSpace(cuentaCorriente))
            {
                // Es opcional
                return true;
            }

            var cuentaLimpia = cuentaCorriente.Replace("-", "").Replace(" ", "");

            // Debe tener entre 10 y 20 dígitos
            if (cuentaLimpia.Length < 10 || cuentaLimpia.Length > 20)
            {
                return false;
            }

            // Solo números
            if (!cuentaLimpia.All(char.IsDigit))
            {
                return false;
            }

            await Task.CompletedTask;
            return true;
        }

        public async Task<bool> ValidarMenuParaNiñoAsync(int menuId, int ninoId)
        {
            if (_menuRepository == null || _alergiaRepository == null)
            {
                return true; // Sin acceso a repositorios, asumimos válido
            }

            // Obtener alergias del niño
            var alergias = await _alergiaRepository.GetIngredientesAlergenosPorNinoAsync(ninoId);
            if (!alergias.Any())
            {
                return true;
            }

            // Verificar si el menú contiene ingredientes alérgenos
            var tieneAlergenos = await _menuRepository.TieneIngredienteAlergenoAsync(menuId, alergias);
            return !tieneAlergenos;
        }

        public async Task<List<string>> ValidarAlergiasEnMenuAsync(int menuId, int ninoId)
        {
            var alergiasEncontradas = new List<string>();

            if (_menuRepository == null || _alergiaRepository == null)
            {
                return alergiasEncontradas;
            }

            var alergiasDelNino = await _alergiaRepository.GetIngredientesAlergenosPorNinoAsync(ninoId);
            if (!alergiasDelNino.Any())
            {
                return alergiasEncontradas;
            }

            var menu = await _menuRepository.ObtenerPorIdAsync(menuId);
            if (menu == null)
            {
                return alergiasEncontradas;
            }

            // Verificar cada plato del menú
            foreach (var menuPlato in menu.Platos)
            {
                if (_platoRepository != null)
                {
                    var ingredientesDelPlato = await _platoRepository.ObtenerIngredientesDePlatoAsync(menuPlato.PlatoId);

                    foreach (var ingrediente in ingredientesDelPlato)
                    {
                        if (
                            alergiasDelNino.Contains(ingrediente, StringComparer.OrdinalIgnoreCase) &&
                            !alergiasEncontradas.Contains(ingrediente, StringComparer.OrdinalIgnoreCase)
                        )
                        {
                            alergiasEncontradas.Add(ingrediente);
                        }
                    }
                }
            }

            return alergiasEncontradas;
        }

        public async Task<bool> ValidarPlatoParaNiñoAsync(int platoId, int ninoId)
        {
            if (_platoRepository == null || _alergiaRepository == null)
                return true;

            var alergias = await _alergiaRepository.GetIngredientesAlergenosPorNinoAsync(ninoId);
            if (!alergias.Any())
                return true;

            var ingredientesDelPlato = await _platoRepository.ObtenerIngredientesDePlatoAsync(platoId);

            // Verificar si algún ingrediente es alérgeno
            foreach (var ingrediente in ingredientesDelPlato)
            {
                if (alergias.Contains(ingrediente, StringComparer.OrdinalIgnoreCase))
                    return false;
            }

            return true;
        }

        public async Task<bool> ValidarFechaNacimientoAsync(DateTime fechaNacimiento)
        {
            // Debe ser una fecha pasada
            if (fechaNacimiento.Date >= DateTime.Now.Date)
                return false;

            // No puede ser más de 120 años atrás (validación razonable)
            if (fechaNacimiento.Date < DateTime.Now.AddYears(-120).Date)
                return false;

            // Para guardería, edad máxima razonable sería 10 años
            if (fechaNacimiento.Date < DateTime.Now.AddYears(-10).Date)
                return false;

            await Task.CompletedTask;
            return true;
        }

        public async Task<bool> ValidarFechaIngresoAsync(DateTime fechaIngreso, DateTime fechaNacimiento)
        {
            // No puede ser anterior al nacimiento
            if (fechaIngreso.Date < fechaNacimiento.Date)
                return false;

            // No puede ser más de 1 año en el futuro
            if (fechaIngreso.Date > DateTime.Now.AddYears(1).Date)
                return false;

            // El niño debe tener al menos 6 meses al ingresar
            if (fechaIngreso.Date < fechaNacimiento.AddMonths(6).Date)
                return false;

            await Task.CompletedTask;
            return true;
        }

        public async Task<bool> ValidarFechaBajaAsync(DateTime fechaBaja, DateTime fechaIngreso)
        {
            // No puede ser anterior al ingreso
            if (fechaBaja.Date < fechaIngreso.Date)
                return false;

            // No puede ser más de 1 año en el futuro
            if (fechaBaja.Date > DateTime.Now.AddYears(1).Date)
                return false;

            await Task.CompletedTask;
            return true;
        }

        public async Task<bool> ValidarEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return true; // Email es opcional en este contexto

            var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            await Task.CompletedTask;
            return Regex.IsMatch(email, emailPattern);
        }

        public async Task<bool> ValidarTextoObligatorioAsync(string texto)
        {
            await Task.CompletedTask;
            return !string.IsNullOrWhiteSpace(texto) && texto.Trim().Length >= 2;
        }

        public async Task<bool> ValidarMontoAsync(decimal monto)
        {
            await Task.CompletedTask;
            return monto >= 0;
        }
    }
}