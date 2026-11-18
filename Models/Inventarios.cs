using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Inventarios : ConsultaBase
{
    public int inv_id { get; set; }
    public decimal inv_cantidad { get; set; }
    public DateTime inv_fechaActualizacion { get; set; }
    public Producto? inv_producto { get; set; }
    public Sucursal? inv_sucursal { get; set; }

    public string Estado => inv_cantidad <= 0 ? "Sin existencias"
                          : inv_cantidad < 10 ? "Stock bajo"
                          : "Disponible";

    public List<Inventarios> MostrarInventario()
    {
        string query = @"SELECT i.inv_id, i.inv_cantidad, i.inv_fechaActualizacion,
                                p.prod_id, p.prod_nombre,
                                c.cat_id, c.cat_nombre,
                                s.suc_id, s.suc_nombre,
                                m.mar_id, m.mar_nombre,
                                a.act_id, a.act_nombre,
                                p.prod_imagen
                         FROM Inventario i
                         INNER JOIN Producto p ON p.prod_id = i.inv_producto
                         INNER JOIN Categoria c ON c.cat_id = p.prod_categoria
                         INNER JOIN Marca m ON m.mar_id = p.prod_marca
                         INNER JOIN Activo a ON a.act_id = p.prod_activo
                         INNER JOIN Sucursal s ON s.suc_id = i.inv_sucursal";

        SqlDataReader reader = null!;
        List<Inventarios> lista = new List<Inventarios>();

        ConsultaDB(ref reader, query);

        while (reader.Read())
        {
            lista.Add(new Inventarios()
            {
                inv_id = reader.GetInt32(0),
                inv_cantidad = reader.GetDecimal(1),
                inv_fechaActualizacion = reader.GetDateTime(2),
                inv_producto = new Producto()
                {
                    prod_id = reader.GetInt32(3),
                    prod_nombre = reader.GetString(4),
                    prod_categoria = new Categoria { cat_id = reader.GetInt32(5), cat_nombre = reader.GetString(6) },
                    prod_marca = new Marca { mar_id = reader.GetInt32(9), mar_nombre = reader.GetString(10) },
                    prod_activo = new Activo { act_id = reader.GetInt32(11), act_nombre = reader.GetString(12) },
                    prod_imagen = reader.IsDBNull(13) ? "" : reader.GetString(13)
                },
                inv_sucursal = new Sucursal
                {
                    suc_id = reader.GetInt32(7),
                    suc_nombre = reader.GetString(8)
                }
            });
        }
        reader.Close();

        return lista;
    }

    public (int total, int conStock, int stockBajo, int sinExistencias) ObtenerResumen(int umbral = 10)
    {
        string query = @"SELECT COUNT(*),
                                ISNULL(SUM(CASE WHEN t.cant >= @Umbral THEN 1 ELSE 0 END), 0),
                                ISNULL(SUM(CASE WHEN t.cant > 0 AND t.cant < @Umbral THEN 1 ELSE 0 END), 0),
                                ISNULL(SUM(CASE WHEN t.cant <= 0 THEN 1 ELSE 0 END), 0)
                         FROM (SELECT p.prod_id, ISNULL(SUM(i.inv_cantidad), 0) AS cant
                               FROM Producto p
                               LEFT JOIN Inventario i ON i.inv_producto = p.prod_id
                               WHERE p.prod_activo = (SELECT act_id FROM Activo WHERE act_nombre = 'Activo')
                               GROUP BY p.prod_id) t";

        Dictionary<string, object> parametros = new Dictionary<string, object>()
        {
            { "@Umbral", umbral }
        };

        SqlDataReader reader = null!;
        ConsultaDB(ref reader, query, parametros);

        var resumen = (total: 0, conStock: 0, stockBajo: 0, sinExistencias: 0);
        if (reader.Read())
            resumen = (reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3));
        reader.Close();

        return resumen;
    }

    public bool InsertInventario()
    {
        string query = @"IF NOT EXISTS (SELECT 1 FROM Inventario WHERE inv_producto = @Producto AND inv_sucursal = @Sucursal)
                            INSERT INTO Inventario (inv_producto, inv_sucursal, inv_cantidad, inv_fechaActualizacion)
                            VALUES (@Producto, @Sucursal, @Cantidad, GETDATE())";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>()
        {
            { "@Producto", inv_producto?.prod_id },
            { "@Sucursal", inv_sucursal?.suc_id },
            { "@Cantidad", inv_cantidad }
        };

        return ExecutedCommand(query, parametros);
    }
}