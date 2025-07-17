using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guarderia.Shared;

namespace Guarderia.Domain.Entities
{
    public class FamiliarOConocido
    {
        public string Nombre = String.Empty;

        // Tiene formato de la cedula JCE.
        public string Cedula = "000-0000000-0";
        public string Direccion = String.Empty;

        public Telefono[] Contactos = [];

        // Relacion entre el familiar y su
        // hijo.
        public Nino Nino;
    }
}