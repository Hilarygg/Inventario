using Inventario.Models;
using Inventario.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Inventario.Models;

public class ComprasController : Controller
{
    public IActionResult Nueva()
    {
        ViewBag.Proveedores = new SelectList(new Proveedor().MostrarProveedores(), "pro_id", "pro_nombre");
        return View(new Producto().MostrarProductos());
    }

    [HttpPost]
    public IActionResult Guardar(int proveedor, DateTime fecha, string folio,
                                int[] productoId, decimal[] cantidad, decimal[] costo)
    {
        // guarda el ticket y suma el stock (pendiente)
        return RedirectToAction("Nueva");
    }
}