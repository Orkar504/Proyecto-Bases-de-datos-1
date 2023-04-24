using Microsoft.Build.Framework;

namespace cuentasPorCobrar.Models
{
    public class Comite
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int empleadoId { get; set; }
        [Required]
        public int SolicitudId { get; set; }
        [Required]
        public String estado_solicitud { get; set; }
    }
}
