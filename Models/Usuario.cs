using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Usuario : ConsultaBase
{
    public int usu_id { get; set; }
    public string usu_nombre { get; set; } = "";
    public string usu_paterno { get; set; } = "";
    public string usu_materno { get; set; } = "";
    public string usu_username { get; set; } = "";
    public string usu_password { get; set; } = "";
    public string usu_correo { get; set; } = "";
    public DateTime usu_fecha { get; set; }
    public Activo? usu_activo { get; set; }
    public Rol? usu_rol { get; set; }
    public Sucursal? usu_sucursal { get; set; }

    public List<Usuario> MostrarUsuarios()
    {
        string query = @"SELECT u.usu_id, u.usu_nombre, u.usu_paterno, u.usu_materno, u.usu_username, u.usu_password, u.usu_correo, u.usu_fechaCreacion, 
                        u.usu_activo, a.act_nombre, u.usu_rol, r.rol_nombre, u.usu_sucursal, s.suc_nombre
                        FROM Usuario u
                        INNER JOIN Activo a ON u.usu_activo = a.act_id
                        INNER JOIN Rol r ON u.usu_rol = r.rol_id
                        INNER JOIN Sucursal s ON u.usu_sucursal = s.suc_id";

        SqlDataReader reader = null!;
        List<Usuario> lista = new List<Usuario>();
        ConsultaDB(ref reader, query);

        while(reader.Read())
        {
            Usuario usuario = new Usuario()
            {
                usu_id = reader.GetInt32(0),
                usu_nombre = reader.GetString(1),
                usu_paterno = reader.GetString(2),
                usu_materno = reader.GetString(3),
                usu_username = reader.GetString(4),
                usu_password = reader.GetString(5),
                usu_correo = reader.GetString(6),
                usu_fecha = reader.GetDateTime(7),
                usu_activo = new Activo()
                {
                    act_id = reader.GetInt32(8),
                    act_nombre = reader.GetString(9)
                },
                usu_rol = new Rol()
                {
                    rol_id = reader.GetInt32(10),
                    rol_nombre = reader.GetString(11)
                },
                usu_sucursal = new Sucursal()
                {
                    suc_id = reader.GetInt32(12),
                    suc_nombre = reader.GetString(13)
                }
            };
            lista.Add(usuario);
        }
        return lista;
    }

    public bool InsertarUsuario()
    {
        string query = @"INSERT INTO Usuario
                (usu_nombre, usu_paterno, usu_materno, usu_username, usu_password,
                 usu_correo, usu_fechaCreacion, usu_activo, usu_rol, usu_sucursal)
                VALUES
                (@usu_nombre, @usu_paterno, @usu_materno, @usu_username, @usu_password,
                 @usu_correo, GETDATE(), @usu_activo, @usu_rol, @usu_sucursal)";

        Dictionary<string, object?> Parametros = new Dictionary<string, object?>()
        {
            {"@usu_nombre", usu_nombre},
            {"@usu_paterno", usu_paterno},
            {"@usu_materno", usu_materno},
            {"@usu_username", usu_username},
            {"@usu_password", usu_password},
            {"@usu_correo", usu_correo},
            {"@usu_activo", usu_activo?.act_id},
            {"@usu_rol", usu_rol?.rol_id},
            {"@usu_sucursal", usu_sucursal?.suc_id}
        };
        return ExecutedCommand(query, Parametros);
    }

    public Usuario ObtenerUsuario(int usu_id)
    {
        string query = @"SELECT u.usu_nombre, u.usu_paterno, u.usu_materno, u.usu_username, u.usu_password, u.usu_correo,
                u.usu_activo, a.act_nombre, u.usu_rol, r.rol_nombre, u.usu_sucursal, s.suc_nombre
                FROM Usuario u
                INNER JOIN Activo a ON u.usu_activo = a.act_id
                INNER JOIN Rol r ON u.usu_rol = r.rol_id
                INNER JOIN Sucursal s ON u.usu_sucursal = s.suc_id
                WHERE u.usu_id = @Id";

        Dictionary<string, dynamic> parametros = new Dictionary<string, dynamic>()
        {
            { "@Id", usu_id}  
        };
        
        SqlDataReader reader = null!;
        Usuario usuario = new Usuario();
        ConsultaDB(ref reader, query, parametros);

        if (reader.Read())
        {
            usuario.usu_id = usu_id;
            usuario.usu_nombre = reader.GetString(0);
            usuario.usu_paterno = reader.GetString(1);
            usuario.usu_materno = reader.GetString(2);
            usuario.usu_username = reader.GetString(3);
            usuario.usu_password = reader.GetString(4);
            usuario.usu_correo = reader.GetString(5);
            usuario.usu_activo = new Activo()
            {
                act_id = reader.GetInt32(6),
                act_nombre = reader.GetString(7)
            };
            usuario.usu_rol = new Rol()
            {
                rol_id = reader.GetInt32(8),
                rol_nombre = reader.GetString(9)
            };
            usuario.usu_sucursal = new Sucursal()
            {
                suc_id = reader.GetInt32(10),
                suc_nombre = reader.GetString(11)
            };
            reader.Close();
        }
        return usuario;
    }

    public bool ActualizarUsuario()
    {
        string query = @"UPDATE Usuario SET usu_nombre = @Nombre, usu_materno = @Materno, usu_paterno = @Paterno, usu_username = @Username,
                        usu_password = @Password, usu_correo = @Correo, usu_activo = @Activo, usu_rol = @Rol, usu_sucursal = @Sucursal
                        WHERE usu_id = @Id";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>()
        {
            { "@Nombre", usu_nombre },
            { "@Materno", usu_materno },
            { "@Paterno", usu_paterno },
            { "@Username", usu_username },
            { "@Password", usu_password },
            { "@Correo", usu_correo },
            { "@Activo", usu_activo?.act_id ?? 1 },
            { "@Rol", usu_rol?.rol_id },
            { "@Sucursal", usu_sucursal?.suc_id },
            { "@Id", usu_id }
        };

        return ExecutedCommand(query, parametros);
    }

    public bool EliminarUsuario(int usu_id)
    {
        string query = @"DELETE FROM Usuario WHERE usu_id = @Id ";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>()
        {
            {"@Id",usu_id}
        };

        return ExecutedCommand(query, parametros);
    }

    public bool ValidarUsuarioInactivo(int id)
    {
        string query = @"SELECT COUNT(*)
                        FROM Usuario u
                        WHERE u.usu_id = @Id
                        AND u.usu_activo = (SELECT act_id FROM Activo WHERE act_nombre = 'Inactivo')";

        Dictionary<string, object> parametros = new Dictionary<string, object>()
        {
            { "@Id", id }
        };

        SqlDataReader reader = null!;
        ConsultaDB(ref reader, query, parametros);

        bool validar = reader.Read() && reader.GetInt32(0) > 0;
        reader.Close();

        return validar;
    }
}