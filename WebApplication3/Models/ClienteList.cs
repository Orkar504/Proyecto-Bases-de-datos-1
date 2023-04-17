using System.ComponentModel.DataAnnotations;

namespace CuentasPorCobrar.Models
{
    public class ClienteList
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public String tipo_identificacion { get; set; }
        [Required]
        public String Direccion { get; set; }
        [Required]
        public String estado_civil { get; set; }
        [Required]
        public String genero { get; set; }
        [Required]
        public String num_identificacion { get; set; }
        [Required]

        public string Nombres { get; set; }
        [Required]
        public string Apellidos { get; set; }
        [Required]
        public String fecha_nacimiento { get; set; }
        [Required]
       // [MaxLength(8, ErrorMessage = "Este campo solo acepta 8 caracteres")]
        public String telefono { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string ocupacion { get; set; }
        [Required]
        public decimal ingresos { get; set;}
        [Required]
        public int estado_RegistroClientesId { get; set; }

     }
}
