namespace cuentasPorCobrar.Models
{
    public class Operacion
    {
        public Operacion()
        {
            esValida = false;
            Mensaje = "";
        }
        public bool esValida { get; set; }    
        public string Mensaje { get; set; }
        public object resultado { get; set; }  
    }
}
