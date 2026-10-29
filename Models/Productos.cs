using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Producto : ConsultaBase
{
    public int prod_id { get; set; }
    public string prod_nombre { get; set; } = "";
    public string prod_descripcion { get; set; } = "";
    public string prod_codigoBarras { get; set; } = "";
    public decimal prod_precio { get; set; }
    public decimal prod_stock { get; set; }
    public string prod_imagen { get; set; } = "";
    public decimal prod_costo { get; set; }
    public DateTime prod_fecha { get; set; }
    public Activo? prod_activo { get; set; }
    public Categoria? prod_categoria { get; set; }
    public Marca? prod_marca { get; set; }
    public Proveedor? prod_proveedor { get; set; }
    public Unidad? prod_unidad { get; set; }

    public List<Producto> MostrarProductos()
    {
        string query = @"SELECT p.prod_id, p.prod_imagen, p.prod_nombre, p.prod_precioventa,
                        m.mar_id, m.mar_nombre, p.prod_categoria
                 FROM Producto p
                 INNER JOIN Marca m ON p.prod_marca = m.mar_id";

        SqlDataReader reader = null!;
        List<Producto> lista = new List<Producto>();
        ConsultaBase consultaBase = new ConsultaBase();
        ConsultaDB(ref reader, query);

        while (reader.Read())
        {
            Producto producto = new Producto()
            {
                prod_id = reader.GetInt32(0),
                prod_imagen = reader.GetString(1),
                prod_nombre = reader.GetString(2),
                prod_precio = reader.GetDecimal(3),
                prod_marca = new Marca()
                {
                    mar_id = reader.GetInt32(4),
                    mar_nombre = reader.GetString(5)
                },
                prod_categoria = new Categoria { cat_id = reader.GetInt32(6) }
            };
            lista.Add(producto);
        }

        return lista;
    }

        public List<Producto> MostrarProductosDestacados()
    {
        string query = @"SELECT TOP 6 p.prod_id, p.prod_imagen, p.prod_nombre, p.prod_precioventa,
                                m.mar_id, m.mar_nombre, p.prod_categoria
                            FROM Producto p
                            INNER JOIN Marca m ON p.prod_marca = m.mar_id";

        SqlDataReader reader = null!;
        List<Producto> lista = new List<Producto>();
        ConsultaBase consultaBase = new ConsultaBase();
        ConsultaDB(ref reader, query);

        while (reader.Read())
        {
            Producto producto = new Producto()
            {
                prod_id = reader.GetInt32(0),
                prod_imagen = reader.GetString(1),
                prod_nombre = reader.GetString(2),
                prod_precio = reader.GetDecimal(3),
                prod_marca = new Marca()
                {
                    mar_id = reader.GetInt32(4),
                    mar_nombre = reader.GetString(5)
                },
                prod_categoria = new Categoria { cat_id = reader.GetInt32(6) }
            };
            lista.Add(producto);
        }

        return lista;
    }


    public bool InsertProducto()
    {
        string query = @"INSERT INTO Producto (prod_nombre, prod_descripcion, prod_codigoBarras, prod_precio, prod_stock, prod_imagen, prod_costo, prod_fecha, prod_activo, prod_categoria, prod_marca, prod_proveedor, prod_unidad)
                         VALUES (@Nombre, @Descripcion, @CodigoBarras, @Precio, @Stock, @Imagen, @Costo, @Fecha, @Activo, @CategoriaId, @MarcaId, @ProveedorId, @UnidadId)";

        Dictionary<string, object?> parameters = new Dictionary<string, object?>()
        {
            { "@Nombre", prod_nombre },
            { "@Descripcion", prod_descripcion },
            { "@CodigoBarras", prod_codigoBarras },
            { "@Precio", prod_precio },
            { "@Stock", prod_stock },
            { "@Imagen", prod_imagen },
            { "@Costo", prod_costo },
            { "@Fecha", prod_fecha },
            { "@Activo", prod_activo?.act_id },
            { "@CategoriaId", prod_categoria?.cat_id },
            { "@MarcaId", prod_marca?.mar_id },
            { "@ProveedorId", prod_proveedor?.pro_id },
            { "@UnidadId", prod_unidad?.uni_id }
        };

        return ExecutedCommand(query, parameters);
        
    }
}