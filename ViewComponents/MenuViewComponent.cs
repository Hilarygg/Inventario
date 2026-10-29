using System.Data;
using Inventario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Inventario.ViewComponents;

public class MenuViewComponent : ViewComponent
{
    private readonly string _connectionString;

    public MenuViewComponent(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Falta la cadena de conexión DefaultConnection.");
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        bool autenticado = UserClaimsPrincipal.Identity?.IsAuthenticated == true;

        var menus = new List<Menu>();

        // Con sesión: menús del rol. Sin sesión: solo los marcados como públicos.
        string sql = autenticado
            ? @"SELECT m.men_id, m.men_nombre, m.men_ruta, m.men_icono, m.men_orden, m.men_padre
                FROM Menu m
                INNER JOIN RolMenu rm ON rm.romen_menu = m.men_id
                WHERE rm.romen_rol = @rolId
                  AND rm.romen_puedeVer = 1
                  AND m.men_activo = 1
                ORDER BY m.men_orden"
            : @"SELECT men_id, men_nombre, men_ruta, men_icono, men_orden, men_padre
                FROM Menu
                WHERE men_publico = 1
                  AND men_activo = 1
                ORDER BY men_orden";

        await using var conn = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(sql, conn);

        if (autenticado)
        {
            int.TryParse(UserClaimsPrincipal.FindFirst("RolId")?.Value, out var rolId);
            cmd.Parameters.Add("@rolId", SqlDbType.Int).Value = rolId;
        }

        await conn.OpenAsync();
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            menus.Add(new Menu
            {
                men_id     = reader.GetInt32(0),
                men_nombre = reader.GetString(1),
                men_ruta   = reader.IsDBNull(2) ? null : reader.GetString(2),
                men_icono  = reader.IsDBNull(3) ? null : reader.GetString(3),
                men_orden  = reader.GetInt32(4),
                men_padre  = reader.IsDBNull(5) ? null : reader.GetInt32(5)
            });
        }

        var porPadre = menus.ToLookup(m => m.men_padre);

        List<MenuItemVM> Construir(int? padreId) =>
            porPadre[padreId]
                .Select(m => new MenuItemVM
                {
                    Nombre = m.men_nombre,
                    Ruta   = m.men_ruta,
                    Icono  = m.men_icono,
                    Hijos  = Construir(m.men_id)
                })
                .Where(x => x.Ruta != null || x.Hijos.Any())   // oculta grupos sin submenús visibles
                .ToList();

        var arbol = Construir(null);

        return View("~/Views/Menu/Default.cshtml", arbol);
    }
}