using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Categoria : ConsultaBase
{
    public int cat_id { get; set; }
    public string cat_nombre { get; set; } = "";
    public string? cat_imagen { get; set; }
    public Activo? cat_activo { get; set; }

    public List<Categoria> MostrarCategorias()
    {
        string query = "SELECT TOP 5 cat_id, cat_nombre, cat_imagen FROM Categoria";

        SqlDataReader reader = null!;
        List<Categoria> lista = new List<Categoria>();
        ConsultaDB(ref reader, query);

        while (reader.Read())
        {
            lista.Add(new Categoria
            {
                cat_id = reader.GetInt32(0),
                cat_nombre = reader.GetString(1),
                cat_imagen = reader.IsDBNull(2) ? null : reader.GetString(2)
            });
        }

        return lista;
    }


    public List<Categoria> MostrarCategoriasPublico()
    {
        string query = "SELECT cat_id, cat_nombre, cat_imagen FROM Categoria WHERE cat_activo = 1";

        SqlDataReader reader = null!;
        List<Categoria> lista = new List<Categoria>();
        ConsultaDB(ref reader, query);

        while (reader.Read())
        {
            lista.Add(new Categoria
            {
                cat_id = reader.GetInt32(0),
                cat_nombre = reader.GetString(1),
                cat_imagen = reader.IsDBNull(2) ? null : reader.GetString(2)
            });
        }

        return lista;
    }
}