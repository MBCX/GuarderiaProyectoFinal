using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guarderia.Shared
{
    public class Telefono
    {
        // Formato: (+)0-000-000-0000
        public int Numero = 0_000_000_0000;
        public string NombreContacto = String.Empty;
        
        public Telefono(int Numero, string NombreContacto)
        {
            this.Numero = Numero;
            this.NombreContacto = NombreContacto;
        }
    }
}
