using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Proveedor : ConsultaBase
{
    public int pro_id { get; set; }
    public string pro_nombre { get; set; } = "";
    public string pro_telefono  { get; set; } = "";
    public string pro_correo { get; set; } = "";
    public Activo? pro_activo { get; set; }


    public List<Proveedor> MostrarProveedores()
    {
        string query = @"SELECT pro_id, pro_nombre FROM Proveedor";

        SqlDataReader reader = null!;
        List<Proveedor> lista = new List<Proveedor>();

        ConsultaDB(ref reader, query);

        while(reader.Read())
        {
            Proveedor proveedor = new Proveedor()
            {
                pro_id = reader.GetInt32(0),
                pro_nombre = reader.GetString(1)
            };
            lista.Add(proveedor);
        }
        return lista;
    }
}