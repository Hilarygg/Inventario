using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Compra : ConsultaBase
{
    public int com_id { get; set; }
    public string com_folio { get; set; } = "";
    public DateTime com_fechaCreacion { get; set;}
    public DateTime com_fechaRecepcion { get; set; }
    public decimal com_subtotal { get; set;}
    public decimal com_impuesto { get; set; }
    public decimal com_total { get; set; }
    public string com_estado {get; set; } = "";
    public Proveedor? com_proveedor { get; set; }
    public Sucursal? com_sucursal { get; set; }
    public Usuario? com_usuario { get; set; }

    public List<Compra> MostrarCompras()
    {
        string query = @"SELECT 
                        c.com_id, c.com_folio, c.com_fechaCreacion, c.com_fechaRecepcion, c.com_subtotal, c.com_impuesto, c.com_total, c.com_estado, p.pro_nombre, s.suc_nombre, u.usu_username
                        FROM Compra c 
                        INNER JOIN Proveedor p ON p.pro_id = c.com_proveedor
                        INNER JOIN Sucursal s ON s.suc_id = c.com_sucursal
                        INNER JOIN Usuario u ON u.usu_id = c.com_usuario";

        SqlDataReader reader = null!;
        List<Compra> lista = new List<Compra>();

        ConsultaDB(ref reader, query);

        while(reader.Read())
        {
            Compra compra = new Compra()
            {
                com_id = reader.GetInt32(0),
                com_folio = reader.GetString(1),
                com_fechaCreacion = reader.GetDateTime(2),
                com_fechaRecepcion = reader.GetDateTime(3),
                com_subtotal = reader.GetDecimal(4),
                com_impuesto = reader.GetDecimal(5),
                com_total = reader.GetDecimal(6),
                com_estado = reader.GetString(7),
                com_proveedor = new Proveedor()
                {
                    pro_id = reader.GetInt32(8),
                    pro_nombre = reader.GetString(9)
                },
                com_sucursal = new Sucursal()
                {
                    suc_id = reader.GetInt32(10),
                    suc_nombre = reader.GetString(11)
                },
                com_usuario = new Usuario()
                {
                    usu_id = reader.GetInt32(12),
                    usu_username = reader.GetString(13)
                }
            };
            lista.Add(compra);
        }
        return lista;
    }

    public bool NuevaCompra()
    {
        string query = @"INSERT INTO Compra
                        (com_folio, com_fechaCreacion, com_fechaRecepcion, com_subtotal, com_impuesto, com_total, com_estado, com_proveedor, com_sucursal, com_usuario)
                        VALUES
                        (@folio, @fechaCreacion, @fechaRecepcion, @subtotal, @impuesto, @total, @estado, @proveedor, @sucursal, @usuario)";

        Dictionary<string, dynamic?> parametros = new Dictionary<string, dynamic?>()
        {
          {"@folio", com_folio},
          {"@fechaCreacion", com_fechaCreacion},
          {"@fechaRecepcion", com_fechaRecepcion},
          {"@subtotal", com_subtotal},
          {"@impuesto", com_impuesto},
          {"@total", com_total},
          {"@estado", com_estado},
          {"@proveedor", com_proveedor?.pro_id},
          {"@sucursal", com_sucursal?.suc_id},
          {"@usuario", com_usuario?.usu_id} 
        };

        return ExecutedCommand(query, parametros);
    }
}