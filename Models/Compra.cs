using System.Text;
using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Compra : ConsultaBase
{
    public int com_id { get; set; }
    public string com_folio { get; set; } = "";
    public DateTime com_fechaCreacion { get; set; }
    public DateTime com_fechaRecepcion { get; set; }
    public decimal com_subtotal { get; set; }
    public decimal com_impuesto { get; set; }
    public decimal com_total { get; set; }
    public string com_estado { get; set; } = "";
    public Proveedor? com_proveedor { get; set; }
    public Sucursal? com_sucursal { get; set; }
    public Usuario? com_usuario { get; set; }

    public string ObtenerFolio()
    {
        string query = @"SELECT TOP 1 com_folio
                         FROM Compra
                         ORDER BY com_id DESC";

        SqlDataReader reader = null!;
        string folio = "COM-000000";

        ConsultaDB(ref reader, query);

        if (reader != null)
        {
            if (reader.Read())
                folio = reader["com_folio"].ToString() ?? "COM-000000";

            reader.Close();
        }

        return folio;
    }

    // Solo para mostrar en la pantalla; el folio definitivo lo asigna GuardarCompra.
    public string SiguienteFolio()
    {
        int.TryParse(ObtenerFolio().Replace("COM-", ""), out int ultimo);
        return $"COM-{ultimo + 1:D6}";
    }

    public List<Compra> MostrarCompras()
    {
        string query = @"SELECT c.com_id, c.com_folio, c.com_fechaCreacion, c.com_fechaRecepcion,
                                c.com_subtotal, c.com_impuesto, c.com_total, c.com_estado,
                                p.pro_id, p.pro_nombre, s.suc_id, s.suc_nombre, u.usu_id, u.usu_username
                         FROM Compra c
                         INNER JOIN Proveedor p ON p.pro_id = c.com_proveedor
                         INNER JOIN Sucursal s ON s.suc_id = c.com_sucursal
                         INNER JOIN Usuario u ON u.usu_id = c.com_usuario
                         ORDER BY c.com_id DESC";

        SqlDataReader reader = null!;
        List<Compra> lista = new List<Compra>();

        ConsultaDB(ref reader, query);

        while (reader.Read())
        {
            lista.Add(new Compra()
            {
                com_id = reader.GetInt32(0),
                com_folio = reader.GetString(1),
                com_fechaCreacion = reader.GetDateTime(2),
                com_fechaRecepcion = reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3),
                com_subtotal = reader.GetDecimal(4),
                com_impuesto = reader.GetDecimal(5),
                com_total = reader.GetDecimal(6),
                com_estado = reader.GetString(7),
                com_proveedor = new Proveedor
                {
                    pro_id = reader.GetInt32(8),
                    pro_nombre = reader.GetString(9)
                },
                com_sucursal = new Sucursal
                {
                    suc_id = reader.GetInt32(10),
                    suc_nombre = reader.GetString(11)
                },
                com_usuario = new Usuario
                {
                    usu_id = reader.GetInt32(12),
                    usu_username = reader.GetString(13)
                }
            });
        }
        reader.Close();

        return lista;
    }

    // Devuelve el folio de la compra guardada, o null si algo falló.
    public string? GuardarCompra(int[] productoId, decimal[] cantidad, decimal[] costo)
    {
        if (productoId.Length == 0 || productoId.Length != cantidad.Length || productoId.Length != costo.Length)
            return null;
        if (cantidad.Any(c => c <= 0) || costo.Any(c => c <= 0))
            return null;

        decimal subtotal = 0;
        for (int i = 0; i < productoId.Length; i++)
            subtotal += cantidad[i] * costo[i];

        var sql = new StringBuilder(@"
            SET NOCOUNT ON;
            SET XACT_ABORT ON;
            BEGIN TRANSACTION;

            DECLARE @IdCompra INT;
            DECLARE @Folio VARCHAR(20);

            INSERT INTO Compra (com_folio, com_fechaCreacion, com_fechaRecepcion, com_subtotal, com_impuesto,
                                com_total, com_estado, com_proveedor, com_sucursal, com_usuario)
            VALUES ('PENDIENTE', @Fecha, @Fecha, @Subtotal, 0, @Subtotal, 'Recibida', @Proveedor, @Sucursal, @Usuario);

            SET @IdCompra = SCOPE_IDENTITY();
            SET @Folio = 'COM-' + RIGHT('000000' + CAST(@IdCompra AS VARCHAR(10)), 6);
            UPDATE Compra SET com_folio = @Folio WHERE com_id = @IdCompra;");

        var parametros = new Dictionary<string, object>()
        {
            { "@Fecha", com_fechaCreacion },
            { "@Subtotal", subtotal },
            { "@Proveedor", com_proveedor?.pro_id ?? 0 },
            { "@Sucursal", com_sucursal?.suc_id ?? 0 },
            { "@Usuario", com_usuario?.usu_id ?? 0 }
        };

        for (int i = 0; i < productoId.Length; i++)
        {
            sql.Append($@"
            INSERT INTO DetalleCompra (det_compra, det_producto, det_cantidad, det_costoUnitario, det_subtotal)
            VALUES (@IdCompra, @P{i}, @C{i}, @K{i}, @C{i} * @K{i});

            IF EXISTS (SELECT 1 FROM Inventario WHERE inv_producto = @P{i} AND inv_sucursal = @Sucursal)
                UPDATE Inventario
                SET inv_cantidad = inv_cantidad + @C{i}, inv_fechaActualizacion = GETDATE()
                WHERE inv_producto = @P{i} AND inv_sucursal = @Sucursal;
            ELSE
                INSERT INTO Inventario (inv_producto, inv_sucursal, inv_cantidad, inv_fechaActualizacion)
                VALUES (@P{i}, @Sucursal, @C{i}, GETDATE());");

            parametros[$"@P{i}"] = productoId[i];
            parametros[$"@C{i}"] = cantidad[i];
            parametros[$"@K{i}"] = costo[i];
        }

        sql.Append(@"
            COMMIT TRANSACTION;
            SELECT @Folio;");

        SqlDataReader reader = null!;
        try
        {
            ConsultaDB(ref reader, sql.ToString(), parametros);
            string? folio = reader.Read() ? reader.GetString(0) : null;
            reader.Close();

            return folio;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al guardar la compra: " + ex.Message);
            return null;
        }
    }
}