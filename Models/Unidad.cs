using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Unidad : ConsultaBase
{
    public int uni_id { get; set; }
    public string uni_nombre { get; set; } = "";
    public string uni_abreviatura { get; set; } = "";
    public bool uni_descripcion { get; set; }

    public List<Unidad> MostrarUnidades()
    {
        string query = @"SELECT uni_id, uni_nombre FROM UnidadMedida";

        SqlDataReader reader = null!;
        List<Unidad> lista = new List<Unidad>();
        ConsultaDB(ref reader, query);

        while(reader.Read())
        {
            Unidad unidad = new Unidad()
            {
                uni_id = reader.GetInt32(0),
                uni_nombre = reader.GetString(1)
            };
            lista.Add(unidad);
        }
        return lista;
    }
}