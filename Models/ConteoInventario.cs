using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class ConteoInventario : ConsultaBase
{
    public int coninv_id { get; set; }
    public DateTime coninv_fecha { get; set; }
    public string coninv_estado { get; set; } = "";
    public Sucursal? coninv_sucursal { get; set; }
    public Categoria? coninv_categoria {get; set;}
    public Usuario? coninv_usuario { get; set; }

    public List<ConteoInventario> MostrarInventariado()
    {
        string query = @"SELECT c.coninv_id, c.coninv_fecha, c.coninv_estado,
                        s.suc_id, s.suc_nombre,
                        ca.cat_id, ca.cat_nombre,
                        u.usu_id, u.usu_nombre
                 FROM ConteoInventario c
                 INNER JOIN Sucursal s ON s.suc_id = c.coninv_sucursal
                 LEFT JOIN Categoria ca ON ca.cat_id = c.coninv_categoria
                 INNER JOIN Usuario u ON u.usu_id = c.coninv_usuario
                 ORDER BY c.coninv_fecha DESC";

        SqlDataReader reader = null!;
        List<ConteoInventario> lista = new List<ConteoInventario>();

        ConsultaDB(ref reader, query);

        while(reader.Read())
        {
            ConteoInventario conteo = new ConteoInventario()
            {
                coninv_id = reader.GetInt32(0),
                coninv_fecha = reader.GetDateTime(1),
                coninv_estado = reader.GetString(2),
                coninv_sucursal = new Sucursal()
                {
                    suc_id = reader.GetInt32(3),
                    suc_nombre = reader.GetString(4)
                },
                coninv_categoria = new Categoria()
                {
                    cat_id = reader.GetInt32(5),
                    cat_nombre = reader.GetString(6)
                },
                coninv_usuario = new Usuario()
                {
                    usu_id = reader.GetInt32(7),
                    usu_nombre = reader.GetString(8)
                }
            };lista.Add(conteo);
        }
        return lista;
    }

    public bool InsertarConteoInventario()
    {
        string query = @"INSERT INTO ConteoInventario
                        (coninv_fecha, coninv_estado, coninv_sucursal, coninv_categoria, coninv_usuario)
                        VALUES
                        (@fecha, @estado, @sucursal, @categoria, @usuario)";

        Dictionary<string, dynamic?> parametros = new Dictionary<string, dynamic?>()
        {
            {"@fecha", coninv_fecha},
            {"@estado", coninv_estado},
            {"@sucursal", coninv_sucursal?.suc_id},
            {"@categoria", coninv_categoria?.cat_id},
            {"@usuario", coninv_usuario?.usu_id}
        };

        return ExecutedCommand(query, parametros);
    }
}