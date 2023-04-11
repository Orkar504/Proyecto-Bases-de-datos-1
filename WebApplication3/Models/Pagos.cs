using Microsoft.Build.Framework;

namespace cuentasPorCobrar.Models
{
    public class Pagos
    {
        public int Id { get; set; }
        [Required]
        public int IDprestamo { get; set; }
        [Required]
        public String fecha_pago { get; set; }
        [Required]
        public decimal monto { get; set; }
        [Required]
        public int estado_pago { get; set; }
    }
}
