using Microsoft.Build.Framework;

namespace cuentasPorCobrar.Models
{
    public class EstadoCivil
    {
        public int Id { get; set; }
        [Required]
        public String nombre { get; set; }
    }
}
