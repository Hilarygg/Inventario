using Microsoft.Data.SqlClient;

namespace Inventario.Models;

public class MenuPermisoVM : ConsultaBase
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? Icono { get; set; }
    public bool Marcado { get; set; }
    public List<MenuPermisoVM> Hijos { get; set; } = new();
}