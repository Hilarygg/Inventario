using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Login : ConsultaBase
{
    public int usu_id { get; set; }
    public string usu_username { get; set; } = "";
    public string usu_nombre { get; set; } = "";
    public string usu_password { get; set; } = "";

    public bool Validar()
    {
        string query = @"SELECT usu_id, usu_nombre, usu_password FROM Usuario
                        WHERE usu_username = @usu_username AND usu_activo = 1";

        Dictionary<string, object> parametros = new Dictionary<string, object>
        {
            { "@usu_username", usu_username }
        };

        SqlDataReader reader = null!;
        bool valido = false;

        try
        {
            ConsultaDB(ref reader, query, parametros);

            if (reader.Read() && BCrypt.Net.BCrypt.Verify(usu_password, reader.GetString(2)))
            {
                usu_id = reader.GetInt32(0);
                usu_nombre = reader.GetString(1);
                valido = true;
            }
        }
        finally
        {
            reader?.Close();
        }

        usu_password = "";   // no dejar la contraseña escrita en el objeto
        return valido;
    }
}