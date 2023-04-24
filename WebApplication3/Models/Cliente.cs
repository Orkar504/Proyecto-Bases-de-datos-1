using System.ComponentModel.DataAnnotations;

namespace CuentasPorCobrar.Models
{
    public class Cliente
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int tipo_identificacion { get; set; }
        [Required]
        public int Direccion { get; set; }
        [Required]
        public int estado_civil { get; set; }
        [Required]
        public int genero { get; set; }
        [Required]
        [MaxLength(15, ErrorMessage ="Este campo solo acepta 15 caracteres")]
        public String num_identificacion { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage ="Este campo solo acepta 50 caracteres")]
        public string Nombres { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Este campo solo acepta 50 caracteres")]
        public string Apellidos { get; set; }
        [Required]
        public String fecha_nacimiento { get; set; }
        [Required]
       [MaxLength(13, ErrorMessage = "Este campo solo acepta 11 caracteres")]
        public String telefono { get; set; }
        [Required]
        [MaxLength(30, ErrorMessage = "Este campo solo acepta 30 caracteres")]
        public string Email { get; set; }
        [Required]
        [MaxLength(150, ErrorMessage = "Este campo solo acepta 30 caracteres")]
        public string ocupacion { get; set; }
        [Required]
        public decimal ingresos { get; set;}
        [Required]
        public int estado_RegistroClientesId { get; set; }

     }
}
