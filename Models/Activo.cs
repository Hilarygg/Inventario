using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Activo : ConsultaBase
{
    public int act_id { get; set; }
    public string act_nombre { get; set; } = "";

    public List<Activo> MostrarActivos()
    {
        string query = "SELECT act_id, act_nombre FROM Activo";

        SqlDataReader reader = null!;
        List<Activo> lista = new List<Activo>();
        ConsultaDB(ref reader, query);
        
        while(reader.Read())
        {
            Activo activo = new Activo()
            {
                act_id = reader.GetInt32(0),
                act_nombre = reader.GetString(1) 
            };
            lista.Add(activo);
        }

        return lista;
    }
}

