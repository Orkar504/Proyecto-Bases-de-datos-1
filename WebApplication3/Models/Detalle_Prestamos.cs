using Microsoft.Build.Framework;
using Microsoft.VisualBasic;

namespace cuentasPorCobrar.Models
{
    public class Detalle_Prestamos
    {
        public int Id { get; set; }
        [Required]
        /*public DateAndTime Fecha_pago { get; set; }
        [Required]
        public DateAndTime Fecha_vencimiento { get; set; }
        [Required]*/
        public decimal Monto_capital { get; set; }
        [Required]
        public decimal Monto_intereses { get; set; }
        [Required]
        public decimal Cuota_de_pago { get; set; }
        [Required]
        public int IDprestamo { get; set; }
    }
}
