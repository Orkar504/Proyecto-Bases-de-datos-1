using Microsoft.Build.Framework;

namespace cuentasPorCobrar.Models
{
    public class Usuario
    {
        [Required]
        public String user { get; set; }
        [Required]
        public String pass { get; set; }   
    }
}
