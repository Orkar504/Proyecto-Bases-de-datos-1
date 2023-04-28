using cuentasPorCobrar.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Drawing;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CuentasPorCobrar.Models
{
    public class ProyectoContext
    {
        private static MySqlConnection conexion;
        private static Usuario usuario;
        private static String sitio = ""; //Para hacer pruebas locales dejar vacia
        //private static String sitio = "cuentasPorCobrar"; //Antes de publicar 

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
        //Validar conexiones
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
                                operacion.esValida = permisos.ToUpper().Contains("SELECT");
                                                    
                                if (!operacion.esValida)
                                {
                                    operacion.Mensaje = "Usuario no tiene permisos para realizar operaciones";
                                }
                                else
                                {
                                    operacion.Mensaje = "";
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
                        operacion.Mensaje = "Usuario no tiene permisos para realizar operaciones en el sistema";
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


        public bool EscribirLogs(String linea, String operacion = "")
        {
            try
            {
                String mensajeFinal = String.Format("{1}:{2}:{0}", 
                    linea, 
                    DateTime.Now.ToString("HH:mm:ss"),
                    operacion
                    );
                String ruta = "C:\\Logs\\cuentasPorCobrar\\";
                string fileName = ruta+DateTime.Now.ToString("yyyy-MM-dd")+".txt";

                // Crea el archivo y escribe el texto en él
                using (StreamWriter writer = new StreamWriter(fileName, true))
                {
                    writer.WriteLine(mensajeFinal);

                }
            }
            catch
            {

            }


            return false;
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
                    //cliente.Direccion = reader.GetInt16(2);
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
                EscribirLogs(ex.Message, "Listar Clientes");
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

        //Listar Clientes
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
                string sql = "SELECT  distinct c.clienteID, ti.nom_tipoIdentificacion, '' dir, ec.nombre, g.nom_genero, num_identificacion, p_nombre, p_apellido, fecha_nacimiento, telefono, correo, ocupacion, ingreso_mensual " +
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
                EscribirLogs(ex.Message, "Listar Clientes");
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
                    //cliente.Direccion = reader.GetInt16(2);
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
                EscribirLogs(ex.Message, "Detalle Clientes");
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
                EscribirLogs(ex.Message, "Detalle Clientes LIst");
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
                query.Append(string.Format("estado_civilId = {0} ,                         ", cliente.estado_civil));
                query.Append(string.Format("generoId = {0} ,                               ", cliente.genero));
                query.Append(string.Format("num_identificacion = '{0}' ,                     ", cliente.num_identificacion));
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
                EscribirLogs(ex.Message);
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
                //query.Append("direccionId,                                ");
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
                //query.Append(string.Format("{0} ,     ", cliente.Direccion));
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
                EscribirLogs(ex.Message, "Actualizar Clientes");
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
                EscribirLogs(ex.Message, "Eliminar Clientes");
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
                EscribirLogs(ex.Message, "Obtener tipos de identificación");
            }

            // Cerrar la conexión
            connection.Close();

            return tipos;
        }

        // Función para obtener tipos de direccion
        public List<TipoDireccion> GetTiposDireccion()
        {
            List<TipoDireccion> tipos = new List<TipoDireccion>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT * FROM tipo_direccion";

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    TipoDireccion tipoDireccion = new TipoDireccion();
                    // Obtener los valores de las columnas de la tabla
                    tipoDireccion.Id = reader.GetInt16(0);
                    tipoDireccion.Nombre = reader.GetString(1);

                    tipos.Add(tipoDireccion);
                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                EscribirLogs(ex.Message, "Obtener tipos de direccion");
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
                EscribirLogs(ex.Message, "Obtener Genero");
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
                EscribirLogs(ex.Message, "Obtener Estado civil");
            }

            // Cerrar la conexión
            connection.Close();

            return estados;
        }

        // Función para obtener Cargos
        public List<Cargo> GetCargo()
        {
            List<Cargo> cargos = new List<Cargo>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT * FROM cargo";

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
                EscribirLogs(ex.Message, "Obtener cargos");
            }

            // Cerrar la conexión
            connection.Close();

            return cargos;
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
                EscribirLogs(ex.Message, "Obtener dirección por cliente");
            }

            // Cerrar la conexión
            connection.Close();

            return direcciones;
        }


        public Direccion GetDireccion(int id)
        {
            Direccion direccion = new Direccion();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT direccionId, calle, numero, ciudad, provincia, pais, codigo_postal, td.nom_tipDireccion, d.clienteId FROM direccion d " +
                    "INNER JOIN tipo_direccion td " +
                    "ON td.tipo_direccionId = d.tipoDireccionId " +
                    "WHERE d.direccionId = " + id + " ";

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    
                    // Obtener los valores de las columnas de la tabla
                    direccion.Id = reader.GetInt16(0);
                    direccion.calle = reader.GetString(1);
                    direccion.numero = reader.GetInt32(2);
                    direccion.ciudad = reader.GetString(3);
                    direccion.provincia = reader.GetString(4);
                    direccion.pais = reader.GetString(5);
                    direccion.codigo_postal = reader.GetString(6);
                    direccion.TipoDireccion = reader.GetString(7);
                    direccion.clienteId = reader.GetInt32(8);

                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                EscribirLogs(ex.Message, "Obtener dirección");
            }

            // Cerrar la conexión
            connection.Close();

            return direccion;
        }

        public Direccion GetDireccionEdit(int id)
        {
            Direccion direccion = new Direccion();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT direccionId, calle, numero, ciudad, provincia, pais, codigo_postal, d.tipoDireccionId, d.clienteId FROM direccion d " +
                    "WHERE d.direccionId = " + id + " ";

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {

                    // Obtener los valores de las columnas de la tabla
                    direccion.Id = reader.GetInt16(0);
                    direccion.calle = reader.GetString(1);
                    direccion.numero = reader.GetInt32(2);
                    direccion.ciudad = reader.GetString(3);
                    direccion.provincia = reader.GetString(4);
                    direccion.pais = reader.GetString(5);
                    direccion.codigo_postal = reader.GetString(6);
                    direccion.TipoDireccion = reader.GetString(7);
                    direccion.clienteId = reader.GetInt32(8);

                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                EscribirLogs(ex.Message, "Editar dirección");
            }

            // Cerrar la conexión
            connection.Close();

            return direccion;
        }


        //*************************************************************************//


        //Listar Empleados
        public Operacion GetEmpleadoList()
        {
            Operacion retorno = new Operacion();
            List<EmpleadoList> empleados = new List<EmpleadoList>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT distinct e.empleadoID, ti.nom_tipoIdentificacion, ec.nombre, " +
                    "g.nom_genero,c.nombre_cargo, num_identificacion, p_nombre, p_apellido, fecha_nacimiento, " +
                    "telefono, correo " +
                    "FROM empleados e " +
                    "inner join tipo_identificacion ti " +
                    "on e.tipo_identificacionId = ti.tipo_identificacionId " +
                    "inner join genero g " +
                    "on e.generoId = g.generoId " +
                    "inner join estado_civil ec " +
                    "on e.estado_civilId = ec.estado_civilId " +
                    "inner join cargo c " +
                    "on c.cargoId = e.cargoId " +
                    "WHERE estado_empleadoId = 1 ORDER BY empleadoID DESC";

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    EmpleadoList empleado = new EmpleadoList();
                    // Obtener los valores de las columnas de la tabla
                    empleado.Id = reader.GetInt16(0);
                    empleado.tipo_identificacion = reader.GetString(1);
                    empleado.estado_civil = reader.GetString(2);
                    empleado.genero = reader.GetString(3);
                    empleado.cargo = reader.GetString(4);
                    empleado.num_identificacion = reader.GetString(5);
                    empleado.Nombres = reader.GetString(6);
                    empleado.Apellidos = reader.GetString(7);
                    empleado.fecha_nacimiento = reader.GetDateTime(8).ToString("yyyy-MM-dd");
                    empleado.telefono = reader.GetString(9);
                    empleado.Email = reader.GetString(10);



                    empleados.Add(empleado);
                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();
                retorno.esValida = true;

            }
            catch (MySqlException ex)
            {
                EscribirLogs(ex.Message, "Listar empleados List");
                retorno.esValida = false;
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar consultas";
                }
            }

            // Cerrar la conexión
            connection.Close();
            retorno.resultado = empleados;
            return retorno;
        }

        //Detalles Empleados
        public Operacion GetDetalleEmpleadoList(int id)
        {
            Operacion retorno = new Operacion();
            EmpleadoList empleado = new EmpleadoList();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT distinct e.empleadoID, ti.nom_tipoIdentificacion, ec.nombre, " +
                    "g.nom_genero,c.nombre_cargo, num_identificacion, p_nombre, p_apellido, fecha_nacimiento, " +
                    "telefono, correo " +
                    "FROM empleados e " +
                    "inner join tipo_identificacion ti " +
                    "on e.tipo_identificacionId = ti.tipo_identificacionId " +
                    "inner join genero g " +
                    "on e.generoId = g.generoId " +
                    "inner join estado_civil ec " +
                    "on e.estado_civilId = ec.estado_civilId " +
                    "inner join cargo c " +
                    "on c.cargoId = e.cargoId " +
                    "WHERE empleadoID =" + id;

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {

                    // Obtener los valores de las columnas de la tabla
                    empleado.Id = reader.GetInt16(0);
                    empleado.tipo_identificacion = reader.GetString(1);
                    empleado.estado_civil = reader.GetString(2);
                    empleado.genero = reader.GetString(3);
                    empleado.cargo = reader.GetString(4);
                    empleado.num_identificacion = reader.GetString(5);
                    empleado.Nombres = reader.GetString(6);
                    empleado.Apellidos = reader.GetString(7);
                    empleado.fecha_nacimiento = reader.GetDateTime(8).ToString("yyyy-MM-dd");
                    empleado.telefono = reader.GetString(9);
                    empleado.Email = reader.GetString(10);


                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                EscribirLogs(ex.Message, "GetDetalleEmpleadoList");
                retorno.esValida = false;
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar consultas";
                }
            }

            // Cerrar la conexión
            connection.Close();
            retorno.resultado = empleado;
            return retorno;
        }

        public Operacion GetDetalleEmpleado(int id)
        {
            Operacion retorno = new Operacion();
            Empleado empleado = new Empleado();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT empleadoID, tipo_identificacionId, estado_civilId,generoId,cargoId,num_identificacion,p_nombre,p_apellido,fecha_nacimiento,telefono,correo FROM empleados WHERE empleadoID =" + id;

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {

                    // Obtener los valores de las columnas de la tabla
                    empleado.Id = reader.GetInt16(0);
                    empleado.tipo_identificacionId = reader.GetInt16(1);
                    empleado.estado_civilId = reader.GetInt16(2);
                    empleado.generoId = reader.GetInt16(3);
                    empleado.cargoId = reader.GetInt16(4);
                    empleado.num_identificacion = reader.GetString(5);
                    empleado.Nombres = reader.GetString(6);
                    empleado.Apellidos = reader.GetString(7);
                    empleado.fecha_nacimiento = reader.GetDateTime(8).ToString("yyyy-MM-dd");
                    empleado.telefono = reader.GetString(9);
                    empleado.Email = reader.GetString(10);


                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                EscribirLogs(ex.Message, "Detalles Empleado");
                retorno.esValida = false;
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar consultas";
                }
            }

            // Cerrar la conexión
            connection.Close();
            retorno.resultado = empleado;
            return retorno;
        }

        //Actualizar Empleados
        public Operacion UpdateEmpleado(Empleado empleado)
        {
            Operacion retorno = new cuentasPorCobrar.Models.Operacion();

            MySqlConnection connection = ObtenerConexion();
            StringBuilder query = new StringBuilder();
            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL

                query.Append("UPDATE empleados SET  ");
                query.Append(string.Format("empleadoID = {0} ,                              ", empleado.Id));
                query.Append(string.Format("tipo_identificacionId = {0} ,                  ", empleado.tipo_identificacionId));
                query.Append(string.Format("estado_civilId = {0} ,                         ", empleado.estado_civilId));
                query.Append(string.Format("generoId = {0} ,                               ", empleado.generoId));
                query.Append(string.Format("cargoId = {0} ,                               ", empleado.cargoId));
                query.Append(string.Format("num_identificacion = {0} ,                     ", empleado.num_identificacion));
                query.Append(string.Format("p_nombre = '{0}' ,                             ", empleado.Nombres));
                query.Append(string.Format("p_apellido = '{0}' ,                           ", empleado.Apellidos));
                query.Append(string.Format("fecha_nacimiento = '{0}' ,                     ", empleado.fecha_nacimiento));
                query.Append(string.Format("telefono = {0} ,                               ", empleado.telefono));
                query.Append(string.Format("correo = '{0}'                               ", empleado.Email));
                query.Append(string.Format("WHERE empleadoID = {0}                        ", empleado.Id));
                // Crear un objeto MySqlCommands
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                EscribirLogs(ex.Message, "Actualizar empleado");
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

        //Función para Eliminar Empleados
        public Operacion DeleteEmpleados(int id)
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
                query.Append("update  empleados set estado_empleadoId = 2 WHERE  empleadoID = " + id);
                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                EscribirLogs(ex.Message, "Eliminar empleado");
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

        //Función para Crear Empleados
        public Operacion CreateEmpleados(Empleado empleado)
        {
            Operacion retorno = new cuentasPorCobrar.Models.Operacion();

            MySqlConnection connection = ObtenerConexion();
            StringBuilder query = new StringBuilder();
            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL

                query.Append("INSERT INTO empleados                  ");
                query.Append("(empleadoID,                           ");
                query.Append("tipo_identificacionId,                 ");
                query.Append("estado_civilId,                        ");
                query.Append("generoId,                              ");
                query.Append("cargoId,                               ");
                query.Append("num_identificacion,                    ");
                query.Append("p_nombre,                              ");
                query.Append("p_apellido,                            ");
                query.Append("fecha_nacimiento,                      ");
                query.Append("telefono,                              ");
                query.Append("correo, estado_empleadoId)             ");
                query.Append("VALUES                                 ");
                query.Append(string.Format("({0}, ",    "null"));
                query.Append(string.Format("{0},   	 ", empleado.tipo_identificacionId));
                query.Append(string.Format("{0},     ", empleado.estado_civilId));
                query.Append(string.Format("{0},     ", empleado.generoId));
                query.Append(string.Format("{0},     ", empleado.cargoId));
                query.Append(string.Format("{0},     ", empleado.num_identificacion));
                query.Append(string.Format("'{0}',   ", empleado.Nombres));
                query.Append(string.Format("'{0}',   ", empleado.Apellidos));
                query.Append(string.Format("'{0}',   ", empleado.fecha_nacimiento));
                query.Append(string.Format("'{0}',   ", empleado.telefono));
                query.Append(string.Format("'{0}',{1})   ", empleado.Email, empleado.estado_RegistroEmpleado));
                
                // Crear un objeto MySqlCommand
                                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                EscribirLogs(ex.Message, "Crear empleados");
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

        //Función para Crear Direcciones
        public Operacion CreateDirecciones(Direccion direccion)
        {
            Operacion retorno = new cuentasPorCobrar.Models.Operacion();

            MySqlConnection connection = ObtenerConexion();
            StringBuilder query = new StringBuilder();
            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL

                query.Append("INSERT INTO direccion                  ");
                query.Append("VALUES                                 ");
                query.Append(string.Format("(null, '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', {6}, {7})", 
                    direccion. calle,
                    direccion. numero ,
                    direccion. ciudad ,
                    direccion. provincia ,
                    direccion. pais,
                    direccion. codigo_postal ,
                    direccion.clienteId ,
                    direccion. TipoDireccion));

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                EscribirLogs(ex.Message, "Crear direcciones");
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


        //Función para actualizar Direcciones
        public Operacion UpdateDirecciones(Direccion direccion)
        {
            Operacion retorno = new cuentasPorCobrar.Models.Operacion();

            MySqlConnection connection = ObtenerConexion();
            StringBuilder query = new StringBuilder();
            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL

                query.Append("Update  direccion                  ");
                query.AppendFormat("SET calle =  '{0}' , ",   direccion.calle);
                query.AppendFormat(" numero =  '{0}' , ",   direccion.numero);
                query.AppendFormat(" ciudad =  '{0}' , ",   direccion.ciudad);
                query.AppendFormat(" provincia =  '{0}' , ",   direccion.provincia);
                query.AppendFormat(" pais =  '{0}' , ",   direccion.pais);
                query.AppendFormat(" codigo_postal =  '{0}' , ",   direccion.codigo_postal);
                query.AppendFormat(" tipoDireccionId =  {0}  ",   direccion.TipoDireccion);
                query.AppendFormat(" where  direccionId =  {0}  ", direccion.Id);

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                EscribirLogs(ex.Message);
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


        //===============Solicitudes====================================//
        // funciones para lISTAR Solicitud
        public List<Solicitud> GetSolicitud(int estadoId)
        {
            List<Solicitud> Solicitud = new List<Solicitud>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT solicitud_ID, fecha_solicitud, monto_prestamo, clienteID, estadoId FROM solicitud_prestamo WHERE estadoId = "+ estadoId + " ORDER BY solicitud_ID DESC";

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
                EscribirLogs(ex.Message, "Listar solicitud");
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
                EscribirLogs(ex.Message, "Detalles solicitud");
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
                EscribirLogs(ex.Message, "Actualizar solicitud");
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
        public Operacion UpdateEstadoSolicitud(int id, int estado)
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
                query.AppendFormat("update  solicitud_prestamo set estadoId = {0} WHERE  solicitud_ID ={1} " , estado, id);
                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                EscribirLogs(ex.Message, "Eliminar solicitud");
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
                EscribirLogs(ex.Message, "Crear solicitud");
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
                EscribirLogs(ex.Message, "Detalle Prestamo");
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
                EscribirLogs(ex.Message, "Listar prestamos");
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
                EscribirLogs(ex.Message, "Listar prestamos por solicitud");
            }

            // Cerrar la conexión
            connection.Close();

            return Prestamo;
        }

        // funcion para listar solicitudes para los pagos
        public List<SolicitudPagos> getSolicitudesPagosParaPrestamos()
        {
            List<SolicitudPagos> solicitudes = new List<SolicitudPagos>();
            StringBuilder query = new StringBuilder();
            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();
                query.Append("select solicitud_ID, ");
                query.Append("concat(c.p_nombre, ' ',c.p_apellido) Nombre,monto_prestamo   ");
                query.Append("from solicitud_prestamo s ");
                query.Append("inner join clientes c ");
                query.Append("on s.clienteID = c.clienteID ");
                query.Append("where estadoId = 3 ");
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
                    solicitud.nombreCliente = String.Format("{0}, Monto: {1}", reader.GetString(1), reader.GetString(2));
                    solicitudes.Add(solicitud);
                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                EscribirLogs(ex.Message,"Lisitar solicitudes de pagos para los prestamos");
            }

            // Cerrar la conexión
            connection.Close();

            return solicitudes;
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
                query.Append("select distinct s.solicitud_ID, ");
                query.Append("concat(c.p_nombre, ' ',c.p_apellido) Nombre,monto_prestamo   ");
                query.Append("from solicitud_prestamo s ");
                query.Append("inner join clientes c ");
                query.Append("on s.clienteID = c.clienteID ");
                query.Append("inner join prestamo p ");
                query.Append("on p.solicitud_ID = s.solicitud_ID ");
                query.Append("where estadoId = 4 and p.estado_prestamoId = 2 ");
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
                EscribirLogs(ex.Message, "Listar solicitudes para los pagos");
            }

            // Cerrar la conexión
            connection.Close();

            return solicitudes;
        }
        public double getMontoSolicitud(int idSolicitud)
        {
            double retorno = 0.0;

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT monto_prestamo FROM solicitud_prestamo where solicitud_ID="+ idSolicitud;

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    retorno = reader.GetDouble(0);
                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                EscribirLogs(ex.Message, "getMontoSolicitud");
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
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
                EscribirLogs(ex.Message, "Actualizar prestamos");
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
                EscribirLogs(ex.Message);
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
                EscribirLogs(ex.Message, "Crear prestamos");
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
                EscribirLogs(ex.Message, "Listar pagos");
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
                EscribirLogs(ex.Message, "Detalles pagos");
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
                EscribirLogs(ex.Message, "Actualizar pagos");
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
                EscribirLogs(ex.Message, "Eliminar pagos");
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
                EscribirLogs(ex.Message);
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
                EscribirLogs(ex.Message, "Listar cargos");
            }

            // Cerrar la conexión
            connection.Close();

            return cargos;
        }


        //**************************************Comites**********************************//

        //Función para Listar Comites
        public List<Comite> GetComite()
        {
            List<Comite> comites = new List<Comite>();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT IdComite,solicitud_ID,empleadoID,estado FROM comite_prestamo where estado='en revisión' ORDER BY IdComite DESC";

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    Comite comite = new Comite();
                    // Obtener los valores de las columnas de la tabla
                    comite.Id = reader.GetInt16(0);
                    comite.empleadoId = reader.GetInt16(1);
                    comite.SolicitudId = reader.GetInt16(2);
                    comite.estado_solicitud = reader.GetString(3);

                    comites.Add(comite);
                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                EscribirLogs(ex.Message, "Listar comites");
            }

            // Cerrar la conexión
            connection.Close();

            return comites;
        }

        //Función Detalles para Comite
        public Comite GetDetalleComite(int id)
        {
            Comite comites = new Comite();

            MySqlConnection connection = ObtenerConexion();

            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL
                string sql = "SELECT IdComite,solicitud_ID,empleadoID,estado FROM comite_prestamo where IdComite" + id;

                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();

                // Recorrer los resultados de la consulta
                while (reader.Read())
                {

                    // Obtener los valores de las columnas de la tabla
                    comites.Id = reader.GetInt16(0);
                    comites.empleadoId = reader.GetInt16(1);
                    comites.SolicitudId = reader.GetInt16(2);
                    comites.estado_solicitud = reader.GetString(3);



                    // ...
                }

                // Cerrar el MySqlDataReader
                reader.Close();

            }
            catch (MySqlException ex)
            {
                EscribirLogs(ex.Message, "Detalles comite");
            }

            // Cerrar la conexión
            connection.Close();

            return comites;
        }

        //Funciones para Actualizar Comite
        public Operacion UpdateComite(Comite comite)
        {
            Operacion retorno = new cuentasPorCobrar.Models.Operacion();

            MySqlConnection connection = ObtenerConexion();
            StringBuilder query = new StringBuilder();
            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL

                query.Append("UPDATE comite_prestamo SET ");
                query.Append(string.Format("IdComite = {0} ,	      ", comite.Id));
                query.Append(string.Format("solicitud_ID = {0} ,    ", comite.SolicitudId));
                query.Append(string.Format("empleadoID = {0} ,  ", comite.empleadoId));
                query.Append(string.Format("estado = '{0}'           ", comite.estado_solicitud));
                query.Append(string.Format("WHERE IdComite = {0};  ", comite.Id));
                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                EscribirLogs(ex.Message, "Actualizar comite");
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar esta operacion";
                }
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }

        // funciones para Crear Comites
        public Operacion CreateComite(Comite comite)
        {
            Operacion retorno = new cuentasPorCobrar.Models.Operacion();

            MySqlConnection connection = ObtenerConexion();
            StringBuilder query = new StringBuilder();
            try
            {
                // Abrir la conexión
                connection.Open();

                // Crear la consulta SQL

                query.Append("INSERT INTO comite_prestamo ");
                query.Append("(IdComite, ");
                query.Append("solicitud_ID, ");
                query.Append("empleadoID,        ");
                query.Append("estado )  ");
                query.Append("VALUES          ");
                query.Append(string.Format("({0} ,    ", "null"));
                query.Append(string.Format("{0} ,       ", comite.SolicitudId));
                query.Append(string.Format("{0} ,        ", comite.empleadoId));
                query.Append(string.Format("'{0}' ,   ", comite.estado_solicitud));
                // Crear un objeto MySqlCommand
                MySqlCommand command = new MySqlCommand(query.ToString(), connection);

                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                int afectados = command.ExecuteNonQuery();
                retorno.esValida = afectados > 0;


            }
            catch (MySqlException ex)
            {
                EscribirLogs(ex.Message, "Crear comite");
                if (ex.Number == 1142)
                {
                    retorno.Mensaje = "Usuario no tiene permisos para realizar esta operacion";
                }
            }

            // Cerrar la conexión
            connection.Close();

            return retorno;
        }
        /*Comite*/
        public Operacion perteneceAComite(String user, int comite)
        {
            Operacion operacion = new Operacion();
            operacion.resultado = false;
            try
            {
                String sql = String.Format( "SELECT usuario FROM comiteuser where cimiteId = {0} and usuario='{1}'",comite, user);
                MySqlConnection connection = ObtenerConexion();
                connection.Open();
                // para la consulta
                MySqlCommand command = new MySqlCommand(sql, connection);
                // Ejecutar la consulta y obtener un objeto MySqlDataReader
                MySqlDataReader reader = command.ExecuteReader();
                // Recorrer los resultados de la consulta
                while (reader.Read())
                {
                    // Obtener los valores de las columnas de la tabla
                    operacion.resultado = true;
                    break;
                    // ...
                }
                connection.Close();
                // Cerrar el MySqlDataReader
                reader.Close();
            }
            catch(MySqlException ex)
            {
                if (ex.Number == 1142)
                {
                    operacion.Mensaje = "Usuario no tiene permisos para realizar esta operacion";
                }
            }
            // Cerrar la conexión
            
            return operacion;
        }

        public String getSitio()
        {
            return sitio;
        }
    }
}