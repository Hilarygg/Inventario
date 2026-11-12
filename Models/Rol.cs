using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Rol : ConsultaBase
{
    public int rol_id { get; set; }
    public string rol_nombre { get; set; } = "";
    public string rol_descripcion { get; set; } = "";
    public Activo? rol_activo { get; set; }

    public List<Rol> MostrarRoles()
    {
        string query = @"SELECT rol_id, rol_nombre FROM Rol";

        SqlDataReader reader = null!;
        List<Rol> lista = new List<Rol>();
        ConsultaDB(ref reader, query);

        while(reader.Read())
        {
            Rol rol = new Rol()
            {
                rol_id = reader.GetInt32(0),
                rol_nombre = reader.GetString(1)
            };
            lista.Add(rol);
        } 
        return lista;
    }
}