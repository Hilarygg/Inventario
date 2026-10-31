using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Producto : ConsultaBase
{
    public int prod_id { get; set; }
    public string prod_nombre { get; set; } = "";
    public string prod_codigoBarras { get; set; } = "";
    public decimal prod_precio { get; set; }
    public decimal prod_stock { get; set; }
    public string prod_imagen { get; set; } = "";
    public decimal prod_costo { get; set; }
    public string prod_codigo { get; set; } = "";
    public int prod_contenido {get; set;}
    public DateTime prod_fecha { get; set; }
    public Activo? prod_activo { get; set; }
    public Categoria? prod_categoria { get; set; }
    public Marca? prod_marca { get; set; }
    public Proveedor? prod_proveedor { get; set; }
    public Unidad? prod_unidad { get; set; }

    public List<Producto> MostrarProductos()
    {
        string query = @"SELECT p.prod_id, p.prod_imagen, p.prod_nombre, p.prod_contenido, u.uni_id, u.uni_abreviatura, p.prod_precioventa,
                m.mar_id, m.mar_nombre, c.cat_id, c.cat_nombre,
                p.prod_codigoBarras, p.prod_stockMinimo, a.act_id, a.act_nombre
         FROM Producto p
         INNER JOIN Marca m ON p.prod_marca = m.mar_id
         INNER JOIN Categoria c ON p.prod_categoria = c.cat_id
         INNER JOIN Activo a ON p.prod_activo = a.act_id
         INNER JOIN UnidadMedida u ON p.prod_unidad = u.uni_id";

        SqlDataReader reader = null!;
        List<Producto> lista = new List<Producto>();
        ConsultaBase consultaBase = new ConsultaBase();
        ConsultaDB(ref reader, query);

        while (reader.Read())
        {
            Producto producto = new Producto()
            {
                prod_id = reader.GetInt32(0),
                prod_imagen = reader.IsDBNull(1) ? "" : reader.GetString(1),
                prod_nombre = reader.GetString(2),
                prod_contenido = reader.GetInt32(3),
                prod_unidad = new Unidad()
                {
                    uni_id = reader.GetInt32(4),
                    uni_abreviatura = reader.GetString(5)
                },
                prod_precio = reader.GetDecimal(6),
                prod_marca = new Marca()
                {
                    mar_id = reader.GetInt32(7),
                    mar_nombre = reader.GetString(8)
                },
                prod_categoria = new Categoria()
                { 
                    cat_id = reader.GetInt32(9), 
                    cat_nombre = reader.GetString(10),
                },
                prod_codigo = reader.GetString(11),
                prod_stock = reader.GetDecimal(12),
                prod_activo = new Activo()
                {
                    act_id = reader.GetInt32(13),
                    act_nombre = reader.GetString(14)
                }
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
        string query = @"INSERT INTO Producto (prod_nombre, prod_codigoBarras, prod_precioventa, prod_stockMinimo, prod_imagen, prod_costoactual, prod_fechaCreacion, prod_contenido, prod_activo, prod_categoria, prod_marca, prod_proveedor, prod_unidad)
                        VALUES (@Nombre, @CodigoBarras, @Precio, @Stock, @Imagen, @Costo, @Fecha, @Contenido, @Activo, @CategoriaId, @MarcaId, @ProveedorId, @UnidadId)";

        Dictionary<string, object?> parameters = new Dictionary<string, object?>()
        {
            { "@Nombre", prod_nombre },
            { "@CodigoBarras", prod_codigoBarras ?? "" },
            { "@Precio", prod_precio },
            { "@Stock", prod_stock },
            { "@Imagen", prod_imagen ?? "" },
            { "@Costo", prod_costo },
            { "@Fecha", DateTime.Now },
            { "@Contenido", prod_contenido },
            { "@Activo", prod_activo?.act_id },
            { "@CategoriaId", prod_categoria?.cat_id },
            { "@MarcaId", prod_marca?.mar_id },
            { "@ProveedorId", prod_proveedor?.pro_id },
            { "@UnidadId", prod_unidad?.uni_id }
        };

        return ExecutedCommand(query, parameters);
    }
}