using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Proveedor : ConsultaBase
{
    public int pro_id { get; set; }
    public string pro_nombre { get; set; } = "";
    public string pro_telefono  { get; set; } = "";
    public string pro_correo { get; set; } = "";
    public Activo? pro_activo { get; set; }
}