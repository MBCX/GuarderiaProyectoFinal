namespace Guarderia.Domain.Entities
{
    public class ConsumoDiario
    {
        public int Id { get; set; }                     
        public int NinoId { get; set; }                 
        public DateTime Fecha { get; set; }            
        public string Descripcion { get; set; } = "";   
        public decimal Monto { get; set; }              
      
    }
}
