using Inventario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventario.Controllers;

public class InventarioController : Controller
{
    public IActionResult Existencias(string? buscar, string? estado, int? sucursal, int? categoria, int? marca)
    {
        var inventario = new Inventarios().MostrarInventario();

        if (!string.IsNullOrEmpty(buscar))
            inventario = inventario.Where(i => i.inv_producto!.prod_nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase)).ToList();
        if (sucursal != null)
            inventario = inventario.Where(i => i.inv_sucursal?.suc_id == sucursal).ToList();
        if (categoria != null)
            inventario = inventario.Where(i => i.inv_producto?.prod_categoria?.cat_id == categoria).ToList();
        if (marca != null)
            inventario = inventario.Where(i => i.inv_producto?.prod_marca?.mar_id == marca).ToList();
        if (!string.IsNullOrEmpty(estado))
            inventario = inventario.Where(i => i.Estado == estado).ToList();

        var resumen = new Inventarios().ObtenerResumen();
        ViewBag.Total = resumen.total;
        ViewBag.ConStock = resumen.conStock;
        ViewBag.StockBajo = resumen.stockBajo;
        ViewBag.SinExistencias = resumen.sinExistencias;
        ViewBag.Productos = new Producto().MostrarProductos();

        ViewBag.Buscar = buscar;
        ViewBag.Estados = new SelectList(new[] { "Disponible", "Stock bajo", "Sin existencias" }, estado);
        ViewBag.Sucursal = new SelectList(new Sucursal().MostrarSucursal(), "suc_id", "suc_nombre", sucursal);
        ViewBag.Categorias = new SelectList(new Categoria().MostrarCategoriasPublico(), "cat_id", "cat_nombre", categoria);
        ViewBag.Marcas = new SelectList(new Marca().MostrarMarcas(), "mar_id", "mar_nombre", marca);

        return View(inventario);
    }

    [HttpPost]
    public IActionResult AgregarInventario(Inventarios inventario)
    {
        if (inventario.InsertInventario())
            TempData["Exito"] = "Inventario agregado correctamente.";
        else
            TempData["Error"] = "No se pudo agregar el inventario.";

        return RedirectToAction("Existencias");
    }

    public IActionResult Ajuste()
    {
        var resumen = new Inventarios().ObtenerResumen();
        ViewBag.Total = resumen.total;
        ViewBag.ConStock = resumen.conStock;

        ViewBag.Sucursales = new SelectList(new Sucursal().MostrarSucursal(), "suc_id", "suc_nombre");
        ViewBag.Categorias = new SelectList(new Categoria().MostrarCategoriasPublico(), "cat_id", "cat_nombre");
        ViewBag.Productos = new SelectList(new Producto().MostrarProductos(), "prod_id", "prod_nombre");

        return View(new ConteoInventario().MostrarInventariado());
    }

    
}