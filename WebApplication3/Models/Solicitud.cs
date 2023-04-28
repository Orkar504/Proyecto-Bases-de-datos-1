using Microsoft.Build.Framework;

namespace cuentasPorCobrar.Models
{
    public class Solicitud
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public String fecha_solicitud { get; set; }
        [Required]
        public decimal monto { get; set; }
        [Required]
        public int clienteId { get; set; }
        [Required]
        public int estado_solicitud { get; set; }
    }
}
