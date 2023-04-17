using Microsoft.Build.Framework;

namespace cuentasPorCobrar.Models
{
    public class Cargo
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string nombre_cargo { get; set; }
    }
}
