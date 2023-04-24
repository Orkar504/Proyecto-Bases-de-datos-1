using Microsoft.Build.Framework;

namespace cuentasPorCobrar.Models
{
    public class Direccion
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string calle { get; set;}
        [Required]
        public int numero { get; set;}
        [Required]
        public string ciudad { get; set;}
        [Required]
        public string provincia { get; set;}
        [Required]
        public string pais { get; set;}
        [Required]
        public string codigo_postal { get; set;}
        [Required]
        public String TipoDireccion { get; set;}
        [Required]
        public int clienteId { get; set; }
    }
}
