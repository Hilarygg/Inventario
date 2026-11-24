using Inventario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Inventario.Controllers;

public class ComprasController : Controller
{
    public IActionResult Nueva()
    {
        ViewBag.Folio = new Compra().SiguienteFolio();
        ViewBag.Proveedores = new SelectList(new Proveedor().MostrarProveedores(), "pro_id", "pro_nombre");
        ViewBag.Sucursal = new SelectList(new Sucursal().MostrarSucursal(), "suc_id", "suc_nombre");
        return View(new Producto().MostrarProductos());
    }

    public IActionResult Historial()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Guardar(int proveedor, int sucursal, DateTime fecha,
                                 int[] productoId, decimal[] cantidad, decimal[] costo)
    {
        int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var usuarioId);

        var compra = new Compra
        {
            com_fechaCreacion = fecha,
            com_proveedor = new Proveedor { pro_id = proveedor },
            com_sucursal = new Sucursal { suc_id = sucursal },
            com_usuario = new Usuario { usu_id = usuarioId }
        };

        string? folio = compra.GuardarCompra(productoId, cantidad, costo);

        if (folio != null)
            TempData["Exito"] = $"Compra {folio} registrada correctamente.";
        else
            TempData["Error"] = "No se pudo registrar la compra.";

        return RedirectToAction("Nueva");
    }
}