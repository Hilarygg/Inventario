using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class HistorialPrecio : ConsultaBase
{
    public int his_id { get; set; }
    public decimal his_precioAnterior { get; set; }
    public decimal his_precioNuevo { get; set; }
    public DateTime his_fechaCambio { get; set; }
    public Producto? his_producto { get; set; }
    public Usuario? his_usuario { get; set; }

    public bool InsertHistorialPrecio()
    {
        string query = @"INSERT INTO HistorialPrecio 
                        (his_precioAnterior, his_precioNuevo, his_fechaCambio, his_producto, his_usuario)
                        VALUES
                        (@PrecioAnterior, @PrecioNuevo, @FechaCambio, @Producto, @Usuario)";

        Dictionary<string, object?> parametros = new Dictionary<string, object?>()
        {
            {"@PrecioAnterior", his_precioAnterior},
            {"@PrecioNuevo", his_precioNuevo},
            {"@FechaCambio", his_fechaCambio},
            {"@Producto", his_producto},
            {"@Usuario", his_usuario}
        };

        return ExecutedCommand(query, parametros);
    }

    public List<HistorialPrecio> MostrarHistorial(int his_producto, DateTime his_fechaCambio)
    {
        string query = @"SELECT h.his_id, h.his_precioAnterior, h.his_precioNuevo, h.his_fechaCambio
                        h.his_usuario, u.usu_username
                        FROM HistorialPrecio h
                        INNER JOIN Usuario u ON h.his_usuario = u.usu_id
                        WHERE h.his_producto = @his_producto AND h.his_fechaCambio = @his_fecha";

        Dictionary<string, dynamic> parametros = new Dictionary<string, dynamic>()
        {
            {"@his_producto", his_producto},
            {"@his_fecha", his_fechaCambio}
        };

        SqlDataReader reader = null!;
        List<HistorialPrecio> lista = new List<HistorialPrecio>();
        ConsultaDB(ref reader, query, parametros);

        while(reader.Read())
        {
            HistorialPrecio historial = new HistorialPrecio()
            {
                his_id = reader.GetInt32(0),
                his_precioAnterior = reader.GetDecimal(1),
                his_precioNuevo = reader.GetDecimal(2),
                his_fechaCambio = reader.GetDateTime(3),
                his_usuario = new Usuario()
                {
                    usu_id = reader.GetInt32(4),
                    usu_username = reader.GetString(5)
                }
            };
            lista.Add(historial);
        }
        return lista;
    }
}