using Microsoft.Data.SqlClient;
namespace Inventario.Models;

public class Marca : ConsultaBase
{
    public int mar_id { get; set; }
    public string mar_nombre { get; set; } = "";
    public Activo? mar_activo { get; set; }

    public List<Marca> MostrarMarcas()
    {
        string query = @"SELECT m.mar_id, m.mar_nombre, m.mar_activo, a.act_nombre FROM Marca m
                        INNER JOIN Activo a ON m.mar_activo = a.act_id";

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
                    act_id = reader.GetInt32(2),
                    act_nombre = reader.GetString(3)
                }
            };
            lista.Add(marca);
        }
        return lista;
    }

    public bool InsertMarca()
    {
        string query = @"INSERT INTO Marca (mar_nombre, mar_activo) 
                        VALUES (@mar_nombre, @mar_activo)";

        Dictionary<string, object?> parameters = new Dictionary<string, object?>
        {
            { "@mar_nombre", mar_nombre },
            { "@mar_activo", mar_activo?.act_id ?? 1 }
        };

        return ExecutedCommand(query, parameters);
    }

    public Marca ObtenerMarca(int mar_id)
    {
        string query = @"SELECT m.mar_id, m.mar_nombre, m.mar_activo, a.act_nombre FROM Marca m
                        INNER JOIN Activo a ON m.mar_activo = a.act_id
                        WHERE m.mar_id = @mar_id";

        SqlDataReader reader = null!;
        Marca marca = new Marca();

        Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>
        {
            { "@mar_id", mar_id }
        };

        ConsultaDB(ref reader, query, parameters);

        if (reader.Read())
        {
            marca.mar_id = reader.GetInt32(0);
            marca.mar_nombre = reader.GetString(1);
            marca.mar_activo = new Activo()
            {
                act_id = reader.GetInt32(2),
                act_nombre = reader.GetString(3)
            };
        }
        return marca;
    }
    public bool ActualizarMarca()
    {
        string query = @"UPDATE Marca SET mar_nombre = @mar_nombre, mar_activo = @mar_activo
                        WHERE mar_id = @mar_id";

        Dictionary<string, object?> parameters = new Dictionary<string, object?>
        {
            { "@mar_nombre", mar_nombre },
            { "@mar_activo", mar_activo?.act_id ?? 1 },
            { "@mar_id", mar_id }
        };

        return ExecutedCommand(query, parameters);
    }

    public bool EliminarMarca(int mar_id)
    {
        string query = @"DELETE FROM Marca WHERE mar_id = @mar_id";

        Dictionary<string, object?> parameters = new Dictionary<string, object?>
        {
            { "@mar_id", mar_id }
        };

        return ExecutedCommand(query, parameters);
    }

    public bool ValidarMarcaInactiva(int id)
    {
        string query = @"SELECT COUNT(*)
                        FROM Marca m
                        WHERE m.mar_id = @Id
                        AND m.mar_activo = (SELECT act_id FROM Activo WHERE act_nombre = 'Inactivo')";

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

    public bool ValidarMarcaProductos(int id)
    {
        string query = @"SELECT COUNT(*)
                        FROM Producto p
                        WHERE p.prod_marca = @Id";

        Dictionary<string, object> parametros = new Dictionary<string, object>()
        {
            { "@Id", id }
        };

        SqlDataReader reader = null!;
        ConsultaDB(ref reader, query, parametros);

        bool validar = reader.Read() && reader.GetInt32(0) == 0;
        reader.Close();

        return validar;
    }

}