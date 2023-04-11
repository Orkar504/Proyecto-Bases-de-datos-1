using Microsoft.Build.Framework;

namespace cuentasPorCobrar.Models
{
    public class Solicitud
    {
        public int Id { get; set; }
        [Required]
        //public DateTime fecha_solicitud { get; set; }
        //[Required]
        public decimal monto { get; set; }
        /*[Required]
        public int clienteId { get; set; }*/
    }
}
