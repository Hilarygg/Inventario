namespace Inventario.Models;

public class MenuItemVM
{
    public string Nombre { get; set; } = "";
    public string? Ruta { get; set; }
    public string? Icono { get; set; }
    public List<MenuItemVM> Hijos { get; set; } = new();
}