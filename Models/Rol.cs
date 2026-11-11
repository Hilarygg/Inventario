using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Rol : ConsultaBase
{
    public int rol_id { get; set; }
    public string rol_nombre { get; set; } = "";
    public string rol_descripcion { get; set; } = "";
    public Activo? rol_activo { get; set; }
}