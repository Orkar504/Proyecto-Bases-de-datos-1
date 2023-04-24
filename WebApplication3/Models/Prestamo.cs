using System.ComponentModel.DataAnnotations;

/*
 Poner mensajes en caso de error al ingresar los datos
 */

namespace cuentasPorCobrar.Models
{
    public class Prestamo
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo Id no es correcto")]
        public int IdComite { get; set; }
        [Required]
        public decimal Capital { get; set; }
        [Required]
        public decimal Tasa_interes { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage ="Este campo no puede tener mas de 50 caracteres")]
        public String Tipo_interes { get; set; }
        [Required]
        public string Fecha_inicio { get; set; }
        [Required]
        public string Fecha_final { get; set; }
        [Required]
        public decimal Monto_cuota { get; set; }
        [Required]
        public int Cantidad_cuota { get; set; }
        [Required]
        [MaxLength(50,ErrorMessage ="Este campo no debe tener mas de 50 caracteres.")]
        public String Periodo_pago { get; set; }
        [Required]
        [MaxLength(255, ErrorMessage = "Este campo no debe tener mas de 255 caracteres.")]
        public String descripcion { get; set; }
        [Required]
        public int estado { get; set; }
        [Required]
        public int solicitudId { get; set; }
    }
}
