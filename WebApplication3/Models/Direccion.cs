namespace cuentasPorCobrar.Models
{
    public class Direccion
    {
        public int Id { get; set; }
        public string calle { get; set;}
        public int numero { get; set;}
        public string ciudad { get; set;}
        public string provincia { get; set;}
        public string pais { get; set;}
        public string codigo_postal { get; set;}
        public String TipoDireccion { get; set;}
    }
}
