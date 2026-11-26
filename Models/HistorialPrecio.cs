using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class HistorialPrecio : ConsultaBase
{
    public int his_id { get; set; }
    public decimal his_precioAnterior { get; set; }
    public decimal his_precioNuevo { get; set; }
    public decimal his_costoAnterior { get; set; }
    public decimal his_costoNuevo { get; set; }
    public DateTime his_fechaCambio { get; set; }
    public Producto? his_producto { get; set; }
    public Usuario? his_usuario { get; set; }

    public bool InsertHistorialPrecio()
    {
        string query = @"
            INSERT INTO HistorialPrecio
            (
                his_precioAnterior,
                his_precioNuevo,
                his_costoAnterior,
                his_costoNuevo,
                his_fechaCambio,
                his_producto,
                his_usuario
            )
            VALUES
            (
                @PrecioAnterior,
                @PrecioNuevo,
                @CostoAnterior,
                @CostoNuevo,
                @FechaCambio,
                @Producto,
                @Usuario
            )";

        Dictionary<string, object?> parametros = new()
        {
            {"@PrecioAnterior", his_precioAnterior},
            {"@PrecioNuevo", his_precioNuevo},
            {"@CostoAnterior", his_costoAnterior},
            {"@CostoNuevo", his_costoNuevo},
            {"@FechaCambio", his_fechaCambio},
            {"@Producto", his_producto?.prod_id},
            {"@Usuario", his_usuario?.usu_id}
        };

        return ExecutedCommand(query, parametros);
    }

    public List<HistorialPrecio> MostrarHistorial(int productoId, int? anio)
    {
        string query = @"SELECT h.his_id, h.his_precioAnterior, h.his_precioNuevo, h.his_costoAnterior, h.his_costoNuevo,  h.his_fechaCambio,
                                h.his_usuario, u.usu_username
                        FROM HistorialPrecio h
                        INNER JOIN Usuario u ON h.his_usuario = u.usu_id
                        WHERE h.his_producto = @Producto
                        AND (@Anio IS NULL OR YEAR(h.his_fechaCambio) = @Anio)
                        ORDER BY h.his_fechaCambio";

        Dictionary<string, object> parametros = new Dictionary<string, object>()
        {
            { "@Producto", productoId },
            { "@Anio", (object?)anio ?? DBNull.Value }
        };

        SqlDataReader reader = null!;
        List<HistorialPrecio> lista = new List<HistorialPrecio>();
        ConsultaDB(ref reader, query, parametros);

        while (reader.Read())
        {
            lista.Add(new HistorialPrecio()
            {
                his_id = reader.GetInt32(0),
                his_precioAnterior = reader.GetDecimal(1),
                his_precioNuevo = reader.GetDecimal(2),
                his_costoAnterior = reader.GetDecimal(3),
                his_costoNuevo = reader.GetDecimal(4),
                his_fechaCambio = reader.GetDateTime(5),
                his_usuario = new Usuario()
                {
                    usu_id = reader.GetInt32(6),
                    usu_username = reader.GetString(7)
                }
            });
        }
        reader.Close();

        return lista;
    }
}