
namespace Guarderia.Domain.Entities
{
    public class Ingrediente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool EsAlergeno { get; set; }

        // Relaciones
        public List<PlatoIngrediente> Platos { get; set; }
        public List<Alergia> Alergias { get; set; }
    }
}
