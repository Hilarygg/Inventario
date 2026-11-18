using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Sucursal : ConsultaBase
{
    public int suc_id { get; set; }
    public string suc_nombre { get; set; } = "";
    public string suc_direccion { get; set; } = "";
    public Activo? suc_activo { get; set; }

    public List<Sucursal> MostrarSucursal()
    {
        string query = @"SELECT s.suc_id, s.suc_nombre, s.suc_direccion, a.act_id, a.act_nombre 
                        FROM Sucursal s 
                        INNER JOIN Activo a ON a.act_id = s.suc_activo";

        SqlDataReader reader = null!;
        List<Sucursal> lista = new List<Sucursal>();
        ConsultaDB(ref reader, query);

        while(reader.Read())
        {
            Sucursal sucursal = new Sucursal()
            {
                suc_id = reader.GetInt32(0),
                suc_nombre = reader.GetString(1),
                suc_direccion = reader.GetString(2),
                suc_activo = new Activo()
                {
                    act_id = reader.GetInt32(3),
                    act_nombre = reader.GetString(4)
                }
            };
            lista.Add(sucursal);
        }
        return lista;
    }
}