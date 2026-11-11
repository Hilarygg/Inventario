using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Sucursal : ConsultaBase
{
    public int suc_id { get; set; }
    public string suc_nombre { get; set; } = "";
    public string suc_descripcion { get; set; } = "";
    public Activo? suc_activo { get; set; }
}