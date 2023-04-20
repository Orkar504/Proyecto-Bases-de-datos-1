using cuentasPorCobrar.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CuentasPorCobrar.Models
{
    public class ProyectoContext
    {
        private static MySqlConnection conexion;
        private static Usuario usuario;

        public ProyectoContext()
        {
            usuario = new Usuario();
            usuario.user = "root";
            usuario.pass = "GRRM@8398/*";
        }

        public ProyectoContext(String user, String pass)
        {
                usuario = new Usuario();
                usuario.user = user;
                usuario.pass = pass;
                conexion = null;
                ObtenerConexion();
        }

        public Operacion validaConexion()
        {
            Operacion operacion = new Operacion();
            if(conexion != null)
            {
                try
                {
                    if(conexion.State!=System.Data.ConnectionState.Open)
                    {
                        conexion.Open();
                        string sql = "SHOW GRANTS FOR CURRENT_USER()";

                        // Crear un objeto MySqlCommand
                        MySqlCommand command = new MySqlCommand(sql, conexion);

                        // Ejecutar la consulta y obtener un objeto MySqlDataReader
                        MySqlDataReader reader = command.ExecuteReader();

                        // Recorrer los resultados de la consulta
                        while (reader.Read())
                        {
                            String permisos = reader.GetString(0);
                            if (permisos.Contains("cuentasxcobrar") || usuario.user=="root")
                            {
                                if (!permisos.ToUpper().Contains("SELECT"))
                                {
                                    operacion.Mensaje = "Usuario no tiene permisos para realizar operaciones";
                                    operacion.esValida = false;
                                }
                                else
                                {
                                    operacion.Mensaje = "";
                                    operacion.esValida = true;
                                    break;
                                }
                                
                            }
                            else
                                continue;
                            
                            // ...
                        }
                        conexion.Close();
                    }
                }
                catch(MySqlException ex)
                {
                    if(ex.Number == 1044)
                    {
                        operacion.esValida = false;
                        operacion.Mensaje = "Usuario no tiene permisos al sistema";
                    }
                }
            }
            return operacion;
        }



        public static MySqlConnection ObtenerConexion()
        {
            try
            {
                if (conexion == null)
                {
                    // Parámetros de conexión
                    string server = "localhost";
                    string database = "sistema_cuentasXcobrar";
                    string uid = usuario.user;
                    string password = usuario.pass;

                    // Cadena de conexión
                    string connectionString = $"Server={server};Database={database};Uid={uid};Pwd={password};";

                    // Crear la conexión
                    conexion = new MySqlConnection(connectionString);
                }
            }
            catch(MySqlException ex)
            {

            }
            

            return conexion;
        }

//**************************CLIENTES****************************************//
        // funciones LISTAR para cliente
        public Operacion GetClientes()
        {
            Operacion retorno = new Operacion();
            List<Cliente> clientes = new List<Cliente>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT clienteID, tipo_identificacionId, direccionId, estado_civilId, generoId, num_identificacion, p_nombre, p_apellido, fecha_nacimiento, telefono, correo, ocupacion, ingreso_mensual FROM clientes WHERE estado_RegistroClientesId = 1 ORDER BY clienteID DESC";

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
                    cliente.tipo_identificacion = reader.GetInt16(1);
                    cliente.Direccion = reader.GetInt16(2);
                    cliente.estado_civil = reader.GetInt16(3);
                    cliente.genero = reader.GetInt16(4);
                    cliente.num_identificacion = reader.GetString(5);
                    cliente.Nombres = reader.GetString(6);
                    cliente.Apellidos = reader.GetString(7);
                    cliente.fecha_nacimiento = reader.GetDateTime(8).ToString("yyyy-MM-dd");
                    cliente.telefono = reader.GetString(9);
                    cliente.Email = reader.GetString(10);
                    cliente.ocupacion = reader.GetString(11);
                    cliente.ingresos = reader.GetDecimal(12);



                    clientes.Add(cliente);
                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();
                

            }
            catch (MySqlException ex)
            {
                retorno.esValida = false;
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar consultas";
                }
            }

            // Cerrar la conexión
            connection.Close();
            retorno.resultado = clientes;
            return retorno;
        }

        public Operacion GetClientesList()
        {
            Operacion retorno = new Operacion();
            List<ClienteList> clientes = new List<ClienteList>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT distinct c.clienteID, ti.nom_tipoIdentificacion, '' dir, ec.nombre, g.nom_genero, num_identificacion, p_nombre, p_apellido, fecha_nacimiento, telefono, correo, ocupacion, ingreso_mensual " +
                    "FROM clientes c " +
                    "left join direccion d " +
                    "on c.clienteID = d.clienteId " +
                    "inner join tipo_identificacion ti " +
                    "on c.tipo_identificacionId = ti.tipo_identificacionId " +
                    "inner join genero g " +
                    "on c.generoId = g.generoId " +
                    "inner join estado_civil ec " +
                    "on c.estado_civilId = ec.estado_civilId " +
                    "WHERE estado_RegistroClientesId = 1 ORDER BY clienteID DESC";

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    ClienteList cliente = new ClienteList();
                    // Obtener los valores de las columnas de la tabla
                    cliente.Id = reader.GetInt16(0);
                    cliente.tipo_identificacion = reader.GetString(1);
                    cliente.Direccion = reader.GetString(2);
                    cliente.estado_civil = reader.GetString(3);
                    cliente.genero = reader.GetString(4);
                    cliente.num_identificacion = reader.GetString(5);
                    cliente.Nombres = reader.GetString(6);
                    cliente.Apellidos = reader.GetString(7);
                    cliente.fecha_nacimiento = reader.GetDateTime(8).ToString("yyyy-MM-dd");
                    cliente.telefono = reader.GetString(9);
                    cliente.Email = reader.GetString(10);
                    cliente.ocupacion = reader.GetString(11);
                    cliente.ingresos = reader.GetDecimal(12);



                    clientes.Add(cliente);
                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();
                retorno.esValida = true;

            }
            catch (MySqlException ex)
            {
                retorno.esValida = false;
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar consultas";
                }
            }

            // Cerrar la conexión
            connection.Close();
            retorno.resultado = clientes;
            return retorno;
        }

        //Función Detalle Clientes
        public Operacion GetDetalleClientes(int id)
        {
            Operacion retorno = new Operacion();
            Cliente cliente = new Cliente();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT clienteID, tipo_identificacionId, direccionId, estado_civilId, generoId, num_identificacion, p_nombre, p_apellido, fecha_nacimiento, telefono, correo, ocupacion, ingreso_mensual FROM clientes WHERE clienteID=" + id;

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {

                    // Obtener los valores de las columnas de la tabla
                    cliente.Id = reader.GetInt16(0);
                    cliente.tipo_identificacion = reader.GetInt16(1);
                    cliente.Direccion = reader.GetInt16(2);
                    cliente.estado_civil = reader.GetInt16(3);
                    cliente.genero = reader.GetInt16(4);
                    cliente.num_identificacion = reader.GetString(5);
                    cliente.Nombres = reader.GetString(6);
                    cliente.Apellidos = reader.GetString(7);
                    cliente.fecha_nacimiento = reader.GetDateTime(8).ToString("yyyy-MM-dd");
                    cliente.telefono = reader.GetString(9);
                    cliente.Email = reader.GetString(10);
                    cliente.ocupacion = reader.GetString(11);
                    cliente.ingresos = reader.GetDecimal(12);


                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                retorno.esValida = false;
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar consultas";
                }
            }

            // Cerrar la conexión
            connection.Close();
            retorno.resultado = cliente;
            return retorno;
        }

        public Operacion GetDetalleClientesList(int id)
        {
            Operacion retorno = new Operacion();
            ClienteList cliente = new ClienteList();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT distinct c.clienteID, ti.nom_tipoIdentificacion, ifnull(d.calle, '') dir, ec.nombre, g.nom_genero, num_identificacion, p_nombre, p_apellido, fecha_nacimiento, telefono, correo, ocupacion, ingreso_mensual " +
                    "FROM clientes c " +
                    "left join direccion d " +
                    "on c.clienteID = d.clienteId " +
                    "inner join tipo_identificacion ti " +
                    "on c.tipo_identificacionId = ti.tipo_identificacionId " +
                    "inner join genero g " +
                    "on c.generoId = g.generoId " +
                    "inner join estado_civil ec " +
                    "on c.estado_civilId = ec.estado_civilId " +
                    "WHERE c.clienteID = " + id;

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {

                    // Obtener los valores de las columnas de la tabla
                    cliente.Id = reader.GetInt16(0);
                    cliente.tipo_identificacion = reader.GetString(1);
                    cliente.Direccion = reader.GetString(2);
                    cliente.estado_civil = reader.GetString(3);
                    cliente.genero = reader.GetString(4);
                    cliente.num_identificacion = reader.GetString(5);
                    cliente.Nombres = reader.GetString(6);
                    cliente.Apellidos = reader.GetString(7);
                    cliente.fecha_nacimiento = reader.GetDateTime(8).ToString("yyyy-MM-dd");
                    cliente.telefono = reader.GetString(9);
                    cliente.Email = reader.GetString(10);
                    cliente.ocupacion = reader.GetString(11);
                    cliente.ingresos = reader.GetDecimal(12);


                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                retorno.esValida = false;
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar consultas";
                }
            }

            // Cerrar la conexión
            connection.Close();
            retorno.resultado = cliente;
            return retorno;
        }

        //Función para Actualizar clientes
        public Operacion UpdateClientes(Cliente cliente)
        {
            Operacion retorno = new cuentasPorCobrar.Models.Operacion();

            MySqlConnection connection = ObtenerConexion();
            StringBuilder query = new StringBuilder();
            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL

                query.Append("UPDATE clientes SET  ");
                query.Append(string.Format("clienteID = {0} ,                              ", cliente.Id));
                query.Append(string.Format("tipo_identificacionId = {0} ,                  ", cliente.tipo_identificacion));
                query.Append(string.Format("direccionId = {0} ,                            ", cliente.Direccion));
                query.Append(string.Format("estado_civilId = {0} ,                         ", cliente.estado_civil));
                query.Append(string.Format("generoId = {0} ,                               ", cliente.genero));
                query.Append(string.Format("num_identificacion = {0} ,                     ", cliente.num_identificacion));
                query.Append(string.Format("p_nombre = '{0}' ,                             ", cliente.Nombres));
                query.Append(string.Format("p_apellido = '{0}' ,                           ", cliente.Apellidos));
                query.Append(string.Format("fecha_nacimiento = '{0}' ,                     ", cliente.fecha_nacimiento));
                query.Append(string.Format("telefono = {0} ,                               ", cliente.telefono));
                query.Append(string.Format("correo = '{0}' ,                               ", cliente.Email));
                query.Append(string.Format("ocupacion = '{0}' ,                            ", cliente.ocupacion));
                query.Append(string.Format("ingreso_mensual = {0}                          ", cliente.ingresos));
                query.Append(string.Format("WHERE clienteID = {0}                        ", cliente.Id));
                // Crear un objeto MySqlCommands
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                retorno.esValida = false;
                if (  ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar esta operacion";
                }
                
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }

        //Función para Crear Clientes
        public Operacion CreateClientes(Cliente cliente)
        {
            Operacion retorno = new cuentasPorCobrar.Models.Operacion();

            MySqlConnection connection = ObtenerConexion();
            StringBuilder query = new StringBuilder();
            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL

                query.Append("INSERT INTO clientes                        ");
                query.Append("(clienteID,                                 ");
                query.Append("tipo_identificacionId,                      ");
                query.Append("direccionId,                                ");
                query.Append("estado_civilId,                             ");
                query.Append("generoId,                                   ");
                query.Append("num_identificacion,                         ");
                query.Append("p_nombre,                                   ");
                query.Append("p_apellido,                                 ");
                query.Append("fecha_nacimiento,                           ");
                query.Append("telefono,                                   ");
                query.Append("correo,                                     ");
                query.Append("ocupacion,                                  ");
                query.Append("ingreso_mensual, estado_RegistroClientesId) ");
                query.Append("VALUES                                      ");
                query.Append(string.Format("({0} ,    ","null"));
                query.Append(string.Format("{0} ,     ", cliente.tipo_identificacion));
                query.Append(string.Format("{0} ,     ", cliente.Direccion));
                query.Append(string.Format("{0} ,     ", cliente.estado_civil));
                query.Append(string.Format("{0} ,     ", cliente.genero));
                query.Append(string.Format("'{0}' ,   ", cliente.num_identificacion));
                query.Append(string.Format("'{0}' ,   ", cliente.Nombres));
                query.Append(string.Format("'{0}' ,   ", cliente.Apellidos));
                query.Append(string.Format("'{0}' ,   ", cliente.fecha_nacimiento));
                query.Append(string.Format("'{0}' ,   ", cliente.telefono));
                query.Append(string.Format("'{0}' ,   ", cliente.Email));
                query.Append(string.Format("'{0}' ,   ", cliente.ocupacion));
                query.Append(string.Format("{0},{1} )     ", cliente.ingresos,cliente.estado_RegistroClientesId));
                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                retorno.esValida = false;
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar esta operacion";
                }
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }

        //Función para Eliminar Clientes
        public Operacion DeleteClientes(int id)
        {
            Operacion retorno = new cuentasPorCobrar.Models.Operacion();

            MySqlConnection connection = ObtenerConexion();
            StringBuilder query = new StringBuilder();
            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL

                //query.Append("DELETE FROM CLIENTES WHERE  clienteID= "+id);
                query.Append("update  CLIENTES set estado_RegistroClientesId = 2 WHERE  clienteID = " + id);
                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                retorno.esValida = false;
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar esta operacion";
                }
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }

        // Función para obtener tipos de identificacion
        public List<TipoIdentificacion> GetTiposIdentificacion()
        {
            List<TipoIdentificacion> tipos = new List<TipoIdentificacion>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT * FROM tipo_identificacion";

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    TipoIdentificacion tipoIdentificacion = new TipoIdentificacion();
                    // Obtener los valores de las columnas de la tabla
                    tipoIdentificacion.Id = reader.GetInt16(0);
                    tipoIdentificacion.Nombre = reader.GetString(1);

                    tipos.Add(tipoIdentificacion);
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

            return tipos;
        }

        // Función para obtener genero
        public List<Genero> GetGenero()
        {
            List<Genero> generos = new List<Genero>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT * FROM genero";

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    Genero genero = new Genero();
                    // Obtener los valores de las columnas de la tabla
                    genero.Id = reader.GetInt16(0);
                    genero.Nombre = reader.GetString(1);

                    generos.Add(genero);
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

            return generos;
        }

        // Función para obtener Estado Civil
        public List<EstadoCivil> GetEstadoCivil()
        {
            List<EstadoCivil> estados = new List<EstadoCivil>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT * FROM estado_civil;";

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    EstadoCivil estado = new EstadoCivil();
                    // Obtener los valores de las columnas de la tabla
                    estado.Id = reader.GetInt16(0);
                    estado.Nombre = reader.GetString(1);

                    estados.Add(estado);
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

            return estados;
        }



        public List<Direccion> GetDireccionesPorCliente(int clienteID)
        {
            List<Direccion> direcciones = new List<Direccion>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT direccionId, calle, numero, ciudad, provincia, pais, codigo_postal, td.nom_tipDireccion FROM direccion d " +
                    "INNER JOIN tipo_direccion td " +
                    "ON td.tipo_direccionId = d.tipoDireccionId " +
                    "WHERE d.clienteID = " + clienteID;

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    Direccion direccion = new Direccion();
                    // Obtener los valores de las columnas de la tabla
                    direccion.Id = reader.GetInt16(0);
                    direccion.calle = reader.GetString(1);
                    direccion.numero = reader.GetInt32(2);
                    direccion.ciudad = reader.GetString(3);
                    direccion.provincia = reader.GetString(4);
                    direccion.pais = reader.GetString(5);
                    direccion.codigo_postal = reader.GetString(6);
                    direccion.TipoDireccion = reader.GetString(7);



                    direcciones.Add(direccion);
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

            return direcciones;
        }



        //*************************************************************************//

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

//===============Solicitudes====================================//
        // funciones para lISTAR Solicitud
        public List<Solicitud> GetSolicitud()
        {
            List<Solicitud> Solicitud = new List<Solicitud>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT solicitud_ID, fecha_solicitud, monto_prestamo, clienteID, estadoId FROM solicitud_prestamo WHERE estadoId = 2 ORDER BY solicitud_ID DESC";

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
                    solicitud.fecha_solicitud = reader.GetDateTime(1).ToString("yyyy-MM-dd");
                    solicitud.monto = reader.GetDecimal(2);
                    solicitud.clienteId = reader.GetInt16(3);
                    solicitud.estado_solicitud = reader.GetInt16(4);

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

        //Función Detalle para Solicitud
        public Solicitud GetDetalleSolicitud(int id)
        {
            Solicitud solicitud = new Solicitud();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT solicitud_ID, fecha_solicitud, monto_prestamo, clienteID, estadoId FROM solicitud_prestamo WHERE solicitud_ID=" + id;

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {

                    // Obtener los valores de las columnas de la tabla
                    solicitud.Id = reader.GetInt16(0);
                    solicitud.fecha_solicitud = reader.GetDateTime(1).ToString("yyyy-MM-dd");
                    solicitud.monto = reader.GetDecimal(2);
                    solicitud.clienteId = reader.GetInt16(3);
                    solicitud.estado_solicitud = reader.GetInt16(4);


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

            return solicitud;
        }

        //Función para ACTUALIZAR solicitudes
        public Operacion UpdateSolicitud(Solicitud solicitud)
        {
            Operacion retorno = new  cuentasPorCobrar.Models.Operacion();

            MySqlConnection connection = ObtenerConexion();
            StringBuilder query = new StringBuilder();
            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL

                query.Append("UPDATE solicitud_prestamo SET ");
                query.Append(string.Format("solicitud_ID = {0} , ", solicitud.Id));
                query.Append(string.Format("fecha_solicitud = '{0}' , ", solicitud.fecha_solicitud));
                query.Append(string.Format("monto_prestamo = {0} , ", solicitud.monto));
                query.Append(string.Format("clienteID = {0}  ", solicitud.clienteId));
                query.Append(string.Format("WHERE solicitud_ID = {0} ", solicitud.Id));
                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                retorno.esValida = false;
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar esta operacion";
                }
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }

        //Función para ELIMINAR solicitud
        public Operacion DeleteSolicitud(int id)
        {
            Operacion retorno = new cuentasPorCobrar.Models.Operacion();

            MySqlConnection connection = ObtenerConexion();
            StringBuilder query = new StringBuilder();
            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL

                //query.Append("DELETE FROM prestamo WHERE  IDprestamo= "+id);
                query.Append("update  solicitud_prestamo set estadoId = 1 WHERE  solicitud_ID = " + id);
                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar esta operacion";
                }
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }

        //Función para CREAR solicitud
        public Operacion CreateSolicitud(Solicitud solicitud)
        {
            Operacion retorno = new cuentasPorCobrar.Models.Operacion();

            MySqlConnection connection = ObtenerConexion();
            StringBuilder query = new StringBuilder();
            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL

                query.Append("INSERT INTO solicitud_prestamo ");
                query.Append("(solicitud_ID, ");
                query.Append("fecha_solicitud, ");
                query.Append("monto_prestamo,        ");
                query.Append("clienteID, estadoId  ) ");
                query.Append("VALUES          ");
                query.Append(string.Format("({0} ,    ", "null"));
                query.Append(string.Format("'{0}',       ", solicitud.fecha_solicitud));
                query.Append(string.Format("{0} ,        ", solicitud.monto));
                query.Append(string.Format("{0},{1} )   ", solicitud.clienteId, solicitud.estado_solicitud));
                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar esta operacion";
                }
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }

//==============================================================//

/******************************PRESTAMOS******************************/
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
        public Operacion UpdatePrestamos(Prestamo prestamo)
        {
            Operacion retorno = new cuentasPorCobrar.Models.Operacion();

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
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar esta operacion";
                }
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }

        // funciones para Eliminar Prestamos
        public Operacion DeletePrestamos(int id)
        {
            Operacion retorno = new cuentasPorCobrar.Models.Operacion();

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
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar esta operacion";
                }
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }

        // funciones para Crear Prestamos
        public Operacion CreatePrestamos(Prestamo prestamo)
        {
            Operacion retorno = new cuentasPorCobrar.Models.Operacion();

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
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar esta operacion";
                }
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }
/*********************************************************************/

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
        public Operacion UpdatePagos(Pagos pagos)
        {
            Operacion retorno = new cuentasPorCobrar.Models.Operacion();

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
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar esta operacion";
                }
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }

        //Funciones para Eliminar Pagos
        public Operacion DeletePagos(int id)
        {
            Operacion retorno = new cuentasPorCobrar.Models.Operacion();

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
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar esta operacion";
                }
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }

        //Función para crear Pagos
        public Operacion CreatePagos(Pagos pagos)
        {
            Operacion retorno = new cuentasPorCobrar.Models.Operacion();

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
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar esta operacion";
                }
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }

//********************************************************************//

        //Función para listar Cargos
        public List<Cargo> GetCargos()
        {
            List<Cargo> cargos = new List<Cargo>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT  cargoId, nombre_cargo FROM cargo";

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    Cargo cargo = new Cargo();
                    // Obtener los valores de las columnas de la tabla
                    cargo.Id = reader.GetInt16(0);
                    cargo.nombre_cargo = reader.GetString(1);


                    cargos.Add(cargo);
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

            return cargos;
        }
    }
}