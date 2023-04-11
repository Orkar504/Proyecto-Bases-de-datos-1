using cuentasPorCobrar.Models;
using MySql.Data.MySqlClient;
using System.Text;

namespace CuentasPorCobrar.Models
{
    public class ProyectoContext
    {
        private static MySqlConnection conexion;
        public static MySqlConnection ObtenerConexion()
        {
            if (conexion == null)
            {
                // Parámetros de conexión
                string server = "localhost";
                string database = "sistema_cuentasXcobrar";
                string uid = "root";
                string password = "GRRM@8398/*";

                // Cadena de conexión
                string connectionString = $"Server={server};Database={database};Uid={uid};Pwd={password};";

                // Crear la conexión
                conexion = new MySqlConnection(connectionString);
            }

            return conexion;
        }
        

        // funciones para cliente
        public List<Cliente> GetClientes()
        {
            List<Cliente> clientes = new List<Cliente>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT clienteID, p_nombre, correo FROM clientes";

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    Cliente cliente = new Cliente();
                    // Obtener los valores de las columnas de la tabla
                    cliente.Id = reader.GetInt16(0);
                    cliente.Nombres = reader.GetString(1);
                    cliente.Email = reader.GetString(2);



                    clientes.Add(cliente);
                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Cerrar la conexión
            connection.Close();

            return clientes;
        }

        public Cliente GetCliente(int Id)
        {
            Cliente cliente = new Cliente();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT id_cliente, nombre_cliente, correo_electronico FROM clientes where id_cliente = "+Id;

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    // Obtener los valores de las columnas de la tabla
                    cliente.Id = reader.GetInt16(0);
                    cliente.Nombres = reader.GetString(1);
                    cliente.Email = reader.GetString(2);
                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Cerrar la conexión
            connection.Close();

            return cliente;
        }

        public Cliente agregarCliente(Cliente cliente)
        {

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "INSERT INTO clientes VALUES ("+cliente.Id+",2,'00000000000','"+cliente.Nombres+"','1986-01-01',1,'"+cliente.Email+"')";

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);
                               
                int afectados = command.ExecuteNonQuery();

            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Cerrar la conexión
            connection.Close();

            return cliente;
        }


        // funciones para Empleados
        public List<Empleado> GetEmpleados()
        {
            List<Empleado> Empleado = new List<Empleado>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT empleadoID, p_nombre, correo FROM empleados";

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    Empleado empleado = new Empleado();
                    // Obtener los valores de las columnas de la tabla
                  empleado.Id = reader.GetInt16(0);
                  empleado.Nombre = reader.GetString(1);
                  empleado.email = reader.GetString(2);

                    Empleado.Add(empleado);
                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Cerrar la conexión
            connection.Close();

            return Empleado;
        }


        // funciones para Solicitud
        public List<Solicitud> GetSolicitud()
        {
            List<Solicitud> Solicitud = new List<Solicitud>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT solicitud_ID, monto_prestamo FROM solicitud_prestamo";

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    Solicitud solicitud = new Solicitud();
                    // Obtener los valores de las columnas de la tabla
                    solicitud.Id = reader.GetInt16(0);
                    solicitud.monto = reader.GetDecimal(1);

                    Solicitud.Add(solicitud);
                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Cerrar la conexión
            connection.Close();

            return Solicitud;
        }

        // Función Detalles para Prestamos
        public Prestamo GetDetallePrestamo(int id)
        {
            Prestamo prestamo = new Prestamo();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT IDprestamo,IdComite,Capital,Tasa_interes, Tipo_interes,Fecha_inicio,Fecha_final,Monto_cuota,Cantidad_cuota,Periodo_pago,descripcion FROM prestamo WHERE IDprestamo="+id;

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    
                    // Obtener los valores de las columnas de la tabla
                    prestamo.Id = reader.GetInt16(0);
                    prestamo.IdComite = reader.GetInt16(1);
                    prestamo.Capital = reader.GetDecimal(2);
                    prestamo.Tasa_interes = reader.GetDecimal(3);
                    prestamo.Tipo_interes = reader.GetString(4);
                    prestamo.Fecha_inicio = reader.GetDateTime(5).ToString("yyyy-MM-dd");
                    prestamo.Fecha_final = reader.GetDateTime(6).ToString("yyyy-MM-dd");
                    prestamo.Monto_cuota = reader.GetDecimal(7);
                    prestamo.Cantidad_cuota = reader.GetInt16(8);
                    prestamo.Periodo_pago = reader.GetString(9);
                    prestamo.descripcion = reader.GetString(10);

                    
                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Cerrar la conexión
            connection.Close();

            return prestamo;
        }


        // funciones para Listar Prestamos
        public List<Prestamo> GetPrestamos()
        {
            List<Prestamo> Prestamo = new List<Prestamo>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT IDprestamo,IdComite,Capital,Tasa_interes, Tipo_interes,Fecha_inicio,Fecha_final,Monto_cuota,Cantidad_cuota,Periodo_pago,descripcion FROM prestamo where estado_prestamoId = 2 ORDER BY IDprestamo DESC";

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    Prestamo prestamo = new Prestamo();
                    // Obtener los valores de las columnas de la tabla
                    prestamo.Id = reader.GetInt16(0);
                    prestamo.IdComite = reader.GetInt16(1);
                    prestamo.Capital = reader.GetDecimal(2);
                    prestamo.Tasa_interes = reader.GetDecimal(3);
                    prestamo.Tipo_interes = reader.GetString(4);
                    prestamo.Fecha_inicio = reader.GetDateTime(5).ToString("yyyy-MM-dd");
                    prestamo.Fecha_final = reader.GetDateTime(6).ToString("yyyy-MM-dd");
                    prestamo.Monto_cuota = reader.GetDecimal(7);
                    prestamo.Cantidad_cuota = reader.GetInt16(8);
                    prestamo.Periodo_pago = reader.GetString(9);
                    prestamo.descripcion = reader.GetString(10);

                    Prestamo.Add(prestamo);
                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Cerrar la conexión
            connection.Close();

            return Prestamo;
        }
        // funciones para Listar Prestamos por solicitud
        public List<Prestamo> GetPrestamosPorSolicitud(int idSolicitud)
        {
            List<Prestamo> Prestamo = new List<Prestamo>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT IDprestamo,IdComite,Capital,Tasa_interes, Tipo_interes,Fecha_inicio,Fecha_final,Monto_cuota,Cantidad_cuota,Periodo_pago,descripcion FROM prestamo where estado_prestamoId = 2 and solicitud_ID = " + idSolicitud+" ORDER BY IDprestamo DESC";

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    Prestamo prestamo = new Prestamo();
                    // Obtener los valores de las columnas de la tabla
                    prestamo.Id = reader.GetInt16(0);
                    prestamo.IdComite = reader.GetInt16(1);
                    prestamo.Capital = reader.GetDecimal(2);
                    prestamo.Tasa_interes = reader.GetDecimal(3);
                    prestamo.Tipo_interes = reader.GetString(4);
                    prestamo.Fecha_inicio = reader.GetDateTime(5).ToString("yyyy-MM-dd");
                    prestamo.Fecha_final = reader.GetDateTime(6).ToString("yyyy-MM-dd");
                    prestamo.Monto_cuota = reader.GetDecimal(7);
                    prestamo.Cantidad_cuota = reader.GetInt16(8);
                    prestamo.Periodo_pago = reader.GetString(9);
                    prestamo.descripcion = reader.GetString(10);

                    Prestamo.Add(prestamo);
                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Cerrar la conexión
            connection.Close();

            return Prestamo;
        }

        // funcion para listar solicitudes para los pagos
        public List<SolicitudPagos> getSolicitudesPagos()
        {
            List<SolicitudPagos> solicitudes = new List<SolicitudPagos>();
            StringBuilder query = new StringBuilder();
            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();
                query.Append("select solicitud_ID, ");
                query.Append("concat(c.p_nombre, ' ', c.s_nombre, ' ',c.p_apellido, ' ', c.s_apellido) Nombre   ");
                query.Append("from solicitud_prestamo s ");
                query.Append("inner join clientes c ");
                query.Append("on s.clienteID = c.clienteID ");
                query.Append("where estadoId = 1 ");
                // Crear la consulta SQL
                
                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    SolicitudPagos solicitud = new SolicitudPagos();
                    // Obtener los valores de las columnas de la tabla
                    solicitud.Id = reader.GetInt16(0);
                    solicitud.nombreCliente = reader.GetString(1);
                    solicitudes.Add(solicitud);
                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Cerrar la conexión
            connection.Close();

            return solicitudes;
        }
        // funciones para actualizar Prestamos
        public bool UpdatePrestamos(Prestamo prestamo)
        {
            bool  retorno = false;

            MySqlConnection connection = ObtenerConexion();
            StringBuilder query = new StringBuilder();
            try
            {
                // Abrir la conexión
                connection.Open();
                    
                // Crear la consulta SQL
               
                query.Append("UPDATE prestamo SET ");
                query.Append(string.Format("IDprestamo = {0} ,              ", prestamo.Id));
                query.Append(string.Format("IdComite = {0} ,                  ", prestamo.IdComite));
                query.Append(string.Format("Capital = {0} ,                    ",prestamo.Capital));
                query.Append(string.Format("Tasa_interes = {0} ,          ",prestamo.Tasa_interes));
                query.Append(string.Format("Tipo_interes = '{0}' ,          ",prestamo.Tipo_interes));
                query.Append(string.Format("Fecha_inicio = '{0}' ,          ", prestamo.Fecha_inicio));
                query.Append(string.Format("Fecha_final = '{0}' ,            ", prestamo.Fecha_final));
                query.Append(string.Format("Monto_cuota = {0} ,            ", prestamo.Monto_cuota));
                query.Append(string.Format("Cantidad_cuota = {0} ,      ", prestamo.Cantidad_cuota));
                query.Append(string.Format("Periodo_pago = '{0}',          ", prestamo.Periodo_pago));
                query.Append(string.Format("descripcion = '{0}'              ", prestamo.descripcion));
                query.Append(string.Format("WHERE IDprestamo = {0}                ", prestamo.Id));
                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
               int afectados =  command.ExecuteNonQuery();
                retorno = afectados > 0;


            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }

        // funciones para Eliminar Prestamos
        public bool DeletePrestamos(int id)
        {
            bool retorno = false;

            MySqlConnection connection = ObtenerConexion();
            StringBuilder query = new StringBuilder();
            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL

                //query.Append("DELETE FROM prestamo WHERE  IDprestamo= "+id);
                query.Append("update  prestamo set estado_prestamoId = 1 WHERE  IDprestamo= " + id);
                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno = afectados > 0;


            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }

        // funciones para Crear Prestamos
        public bool CreatePrestamos(Prestamo prestamo)
        {
            bool retorno = false;

            MySqlConnection connection = ObtenerConexion();
            StringBuilder query = new StringBuilder();
            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL

                query.Append("INSERT INTO prestamo ");
                query.Append("(IDprestamo, ");
                query.Append("IdComite, ");
                query.Append("Capital,        ");
                query.Append("Tasa_interes,   ");
                query.Append("Tipo_interes,   ");
                query.Append("Fecha_inicio,   ");
                query.Append("Fecha_final,    ");
                query.Append("Monto_cuota,    ");
                query.Append("Cantidad_cuota, ");
                query.Append("Periodo_pago,   ");
                query.Append("descripcion,estado_prestamoId,solicitud_ID)    ");
                query.Append("VALUES          ");
                query.Append(string.Format("({0} ,    ", "null"));
                query.Append(string.Format("{0} ,       ", prestamo.IdComite));
                query.Append(string.Format("{0} ,        ", prestamo.Capital));
                query.Append(string.Format("{0} ,   ", prestamo.Tasa_interes));
                query.Append(string.Format("'{0}' ,   ", prestamo.Tipo_interes));
                query.Append(string.Format("'{0}' ,   ", prestamo.Fecha_inicio));
                query.Append(string.Format("'{0}' ,    ", prestamo.Fecha_final));
                query.Append(string.Format("{0} ,    ", prestamo.Monto_cuota));
                query.Append(string.Format("{0} , ", prestamo.Cantidad_cuota));
                query.Append(string.Format("'{0}' ,   ", prestamo.Periodo_pago));
                query.Append(string.Format("'{0}',{1},{2} )   ", prestamo.descripcion,prestamo.estado,prestamo.solicitudId));
                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno = afectados > 0;


            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }
        

//**************************PAGOS**************************************//
        // funciones para Listar Pagos
        public List<Pagos> GetPagos()
        {
            List<Pagos> Pagos = new List<Pagos>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT IdPagos, IDprestamo, fecha_pago, monto FROM pagos WHERE estado_pagoId = 2  ORDER BY IdPagos DESC";

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    Pagos pagos = new Pagos();
                    // Obtener los valores de las columnas de la tabla
                    pagos.Id = reader.GetInt16(0);
                    pagos.IDprestamo = reader.GetInt16(1);
                    pagos.fecha_pago = reader.GetDateTime(2).ToString("yyyy-MM-dd");
                    pagos.monto = reader.GetDecimal(3);



                    Pagos.Add(pagos);
                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Cerrar la conexión
            connection.Close();

            return Pagos;
        }

        //Función Detalles para Pagos
        public Pagos GetDetallePagos(int id)
        {
            Pagos pagos = new Pagos();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT IdPagos, IDprestamo, fecha_pago, monto FROM pagos WHERE IdPagos=" + id;

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {

                    // Obtener los valores de las columnas de la tabla
                    pagos.Id = reader.GetInt16(0);
                    pagos.IDprestamo = reader.GetInt16(1);
                    pagos.fecha_pago = reader.GetDateTime(2).ToString("yyyy-MM-dd");
                    pagos.monto = reader.GetDecimal(3);


                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Cerrar la conexión
            connection.Close();

            return pagos;
        }


        //Funciones para Actualizar Pagos
        public bool UpdatePagos(Pagos pagos)
        {
            bool retorno = false;

            MySqlConnection connection = ObtenerConexion();
            StringBuilder query = new StringBuilder();
            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL

                query.Append("UPDATE pagos SET ");
                query.Append(string.Format("IdPagos = {0} ,	      ", pagos.Id));
                query.Append(string.Format("IDprestamo = {0} ,    ", pagos.IDprestamo));
                query.Append(string.Format("fecha_pago = '{0}' ,  ", pagos.fecha_pago));
                query.Append(string.Format("monto = {0}           ", pagos.monto));
                query.Append(string.Format("WHERE IdPagos = {0};  ", pagos.Id));
                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno = afectados > 0;


            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }

        //Funciones para Eliminar Pagos
        public bool DeletePagos(int id)
        {
            bool retorno = false;

            MySqlConnection connection = ObtenerConexion();
            StringBuilder query = new StringBuilder();
            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL

                //query.Append("DELETE FROM prestamo WHERE  IDprestamo= "+id);
                query.Append("update  pagos set estado_pagoId = 1 WHERE  IdPagos= " + id);
                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno = afectados > 0;


            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }

        //Función para crear Pagos
        public bool CreatePagos(Pagos pagos)
        {
            bool retorno = false;

            MySqlConnection connection = ObtenerConexion();
            StringBuilder query = new StringBuilder();
            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL

                query.Append("INSERT INTO pagos	");
                query.Append("(IdPagos, ");
                query.Append("IDprestamo, ");
                query.Append("fecha_pago, ");
                query.Append("monto, estado_pagoId) ");
                query.Append("VALUES ");
                query.Append(string.Format("({0} , ", "null"));
                query.Append(string.Format("{0} , ", pagos.IDprestamo));
                query.Append(string.Format("'{0}' , ", pagos.fecha_pago));
                query.Append(string.Format("{0} , 2) ", pagos.monto));
                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno = afectados > 0;


            }
            catch (MySqlException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }

        //********************************************************************//
    }
}
