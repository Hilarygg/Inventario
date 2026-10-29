using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Unidad : ConsultaBase
{
    public int uni_id { get; set; }
    public string uni_nombre { get; set; } = "";
    public string uni_abreviatura { get; set; } = "";
    public bool uni_descripcion { get; set; }
}