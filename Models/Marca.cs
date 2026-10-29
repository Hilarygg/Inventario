using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Marca : ConsultaBase
{
    public int mar_id { get; set; }
    public string mar_nombre { get; set; } = "";
    public Activo? mar_activo { get; set; }

    public List<Marca> MostrarMarcas()
    {
        string query = "SELECT mar_id, mar_nombre, mar_activo FROM Marca";

        SqlDataReader reader = null!;
        List<Marca> lista = new List<Marca>();
        ConsultaBase consultaBase = new ConsultaBase();

        ConsultaDB(ref reader, query);

        while (reader.Read())
        {
            Marca marca = new Marca()
            {
                mar_id = reader.GetInt32(0),
                mar_nombre = reader.GetString(1),
                mar_activo = new Activo()
                {
                    act_id = reader.GetInt32(2)
                }
            };
            lista.Add(marca);
        }
        return lista;
    }
}