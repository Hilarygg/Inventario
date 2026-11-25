using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class DetalleCompra : ConsultaBase
{
    public int det_id { get; set; }
    public decimal det_cantidad { get; set; }
    public decimal det_costoUnitario { get; set; }
    public decimal det_subtotal { get; set; }
    public Compra? det_compra { get; set; }
    public Producto? det_producto { get; set; }

    public List<DetalleCompra> MostrarDetalleCompra(int com_id)
    {
        string query = @"SELECT d.det_id, d.det_cantidad, d.det_costoUnitario, d.det_subtotal,
                                c.com_id, p.prod_id, p.prod_nombre, p.prod_imagen
                         FROM DetalleCompra d
                         INNER JOIN Compra c ON c.com_id = d.det_compra
                         INNER JOIN Producto p ON p.prod_id = d.det_producto
                         WHERE c.com_id = @Compra
                         ORDER BY d.det_id";

        Dictionary<string, dynamic> parametros = new Dictionary<string, dynamic>()
        {
            { "@Compra", com_id }
        };

        SqlDataReader reader = null!;
        List<DetalleCompra> lista = new List<DetalleCompra>();
        ConsultaDB(ref reader, query, parametros);

        while (reader.Read())
        {
            lista.Add(new DetalleCompra()
            {
                det_id = reader.GetInt32(0),
                det_cantidad = reader.GetDecimal(1),
                det_costoUnitario = reader.GetDecimal(2),
                det_subtotal = reader.GetDecimal(3),
                det_compra = new Compra { com_id = reader.GetInt32(4) },
                det_producto = new Producto()
                {
                    prod_id = reader.GetInt32(5),
                    prod_nombre = reader.GetString(6),
                    prod_imagen = reader.IsDBNull(7) ? "" : reader.GetString(7)
                }
            });
        }
        reader.Close();

        return lista;
    }
}