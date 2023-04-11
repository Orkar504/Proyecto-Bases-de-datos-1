using System.ComponentModel.DataAnnotations;

namespace CuentasPorCobrar.Models
{
    public class Empleado
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string email { get; set; }
    }
}
