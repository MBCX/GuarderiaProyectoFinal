﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Domain.Entities
{
    public class PersonaAutorizada
    {
        public int Id { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Relacion { get; set; } // Relación con el niño

        // Relación con niños que puede recoger
        public List<NinoPersonaAutorizada> NinosAutorizados { get; set; }
    }
}
