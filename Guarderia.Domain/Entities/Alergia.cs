

namespace Guarderia.Domain.Entities
{
    public class Alergia
    {
        public int Id { get; set; } //Identificador único de la alergia
        public Ingrediente? Ingrediente { get; set; } //Relación con el ingrediente al que el niño es alérgico 
    }

}

