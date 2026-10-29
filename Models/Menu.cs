using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class Menu : ConsultaBase
{
    public int men_id { get; set; }
    public string men_nombre { get; set; } = "";
    public string? men_ruta { get; set; }
    public string? men_icono { get; set; }
    public int men_orden { get; set; }
    public int? men_padre { get; set; }
    public string? men_imagen { get; set; }

}