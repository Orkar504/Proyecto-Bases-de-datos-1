using System.ComponentModel.DataAnnotations;

namespace CuentasPorCobrar.Models
{
    public class Cliente
    {
        /*public int Id { get; set; }
         [Required]
         public string Nombre { get; set; }
         [Required]
         public string email { get; set; }*/

        public int Id { get; set; }
        [Required]
        public string Nombres { get; set; }
        [Required]
        //public string Apellidos { get; set; }
        //[Required]
        public string Email { get; set; }
        //[Required]
        /* string telefono { get; set; }
        [Required]
        public string direccion { get; set; }
        [Required]
        public DateTime fecha_nacimiento { get; set; }*/

     }
}
