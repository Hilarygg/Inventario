using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Sucursal : ConsultaBase
{
    public int suc_id { get; set; }
    public string suc_nombre { get; set; } = "";
    public string suc_descripcion { get; set; } = "";
    public Activo? suc_activo { get; set; }

    public List<Sucursal> MostrarSucursal()
    {
        string query = @"SELECT suc_id, suc_nombre FROM Sucursal";

        SqlDataReader reader = null!;
        List<Sucursal> lista = new List<Sucursal>();
        ConsultaDB(ref reader, query);

        while(reader.Read())
        {
            Sucursal sucursal = new Sucursal()
            {
                suc_id = reader.GetInt32(0),
                suc_nombre = reader.GetString(1)
            };
            lista.Add(sucursal);
        }
        return lista;
    }
}