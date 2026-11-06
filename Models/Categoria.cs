using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Categoria : ConsultaBase
{
    public int cat_id { get; set; }
    public string cat_nombre { get; set; } = "";
    public string? cat_imagen { get; set; }
    public string? cat_descripcion { get; set; }
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
        string query = "SELECT cat_id, cat_nombre, cat_imagen, cat_descripcion FROM Categoria WHERE cat_activo = 1";

        SqlDataReader reader = null!;
        List<Categoria> lista = new List<Categoria>();
        ConsultaDB(ref reader, query);

        while (reader.Read())
        {
            lista.Add(new Categoria
            {
                cat_id = reader.GetInt32(0),
                cat_nombre = reader.GetString(1),
                cat_imagen = reader.IsDBNull(2) ? null : reader.GetString(2),
                cat_descripcion = reader.IsDBNull(3) ? null : reader.GetString(3)   
            });
        }

        return lista;
    }

    public List<Categoria> MostrarCategoriasAdmin()
    {
        string query = @"SELECT c.cat_id, c.cat_nombre, c.cat_imagen, c.cat_descripcion, a.act_id, a.act_nombre FROM Categoria c 
                         JOIN Activo a ON c.cat_activo = a.act_id";

        SqlDataReader reader = null!;
        List<Categoria> lista = new List<Categoria>();
        ConsultaDB(ref reader, query);

        while (reader.Read())
        {
            lista.Add(new Categoria
            {
                cat_id = reader.GetInt32(0),
                cat_nombre = reader.GetString(1),
                cat_imagen = reader.IsDBNull(2) ? null : reader.GetString(2),
                cat_descripcion = reader.IsDBNull(3) ? null : reader.GetString(3),
                cat_activo = new Activo
                {
                    act_id = reader.GetInt32(4),
                    act_nombre = reader.GetString(5)
                }
            });
        }

        return lista;
    }

    public bool InsertCategoria()
    {
        string query = @"Insert into Categoria (cat_nombre, cat_imagen, cat_descripcion, cat_activo)
                         values (@cat_nombre, @cat_imagen, @cat_descripcion, @cat_activo)";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>()
        {
            { "@cat_nombre", cat_nombre },
            { "@cat_imagen", cat_imagen ?? (object)DBNull.Value },
            { "@cat_descripcion", cat_descripcion ?? (object)DBNull.Value },
            { "@cat_activo", cat_activo?.act_id ?? (object)DBNull.Value }
        };

        return ExecutedCommand(query, parametros);
    }

    public Categoria ObtenerCategoria(int id)
    {
        string query = @"SELECT c.cat_id, c.cat_nombre, c.cat_imagen, c.cat_descripcion, a.act_id, a.act_nombre
                        FROM Categoria c
                        INNER JOIN Activo a ON c.cat_activo = a.act_id
                        WHERE c.cat_id = @Id";

        Dictionary<string, object> parametros = new Dictionary<string, object>() { { "@Id", id } };

        SqlDataReader reader = null!;
        Categoria categoria = new Categoria();
        ConsultaDB(ref reader, query, parametros);

        if (reader.Read())
        {
            categoria.cat_id = reader.GetInt32(0);
            categoria.cat_nombre = reader.GetString(1);
            categoria.cat_imagen = reader.IsDBNull(2) ? null : reader.GetString(2);
            categoria.cat_descripcion = reader.IsDBNull(3) ? null : reader.GetString(3);
            categoria.cat_activo = new Activo { act_id = reader.GetInt32(4), act_nombre = reader.GetString(5) };
        }
        reader.Close();

        return categoria;
    }

    public bool ActualizarCategoria()
    {
        string query = @"UPDATE Categoria SET cat_nombre = @cat_nombre, cat_imagen = @cat_imagen, 
                         cat_descripcion = @cat_descripcion, cat_activo = @cat_activo WHERE cat_id = @cat_id";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>()
        {
            { "@cat_id", cat_id },
            { "@cat_nombre", cat_nombre },
            { "@cat_imagen", cat_imagen ?? (object)DBNull.Value },
            { "@cat_descripcion", cat_descripcion ?? (object)DBNull.Value },
            { "@cat_activo", cat_activo?.act_id ?? (object)DBNull.Value }
        };

        return ExecutedCommand(query, parametros);
    }
}