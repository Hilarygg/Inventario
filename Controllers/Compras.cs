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

    public IActionResult Historial(string? buscar, int? sucursal, int? proveedor, int? usuario)
    {
        var compras = new Compra().MostrarCompras();

        if (!string.IsNullOrEmpty(buscar))
            compras = compras.Where(c => c.com_folio.Contains(buscar, StringComparison.OrdinalIgnoreCase)).ToList();
        if (sucursal != null)
            compras = compras.Where(c => c.com_sucursal?.suc_id == sucursal).ToList();
        if (proveedor != null)
            compras = compras.Where(c => c.com_proveedor?.pro_id == proveedor).ToList();
        if (usuario != null)
            compras = compras.Where(c => c.com_usuario?.usu_id == usuario).ToList();

        ViewBag.Buscar = buscar;
        ViewBag.Sucursal = new SelectList(new Sucursal().MostrarSucursal(), "suc_id", "suc_nombre", sucursal);
        ViewBag.Proveedor = new SelectList(new Proveedor().MostrarProveedores(), "pro_id", "pro_nombre", proveedor);
        ViewBag.Usuario = new SelectList(new Usuario().MostrarUsuarios(), "usu_id", "usu_username", usuario);

        return View(compras);
    }

    public IActionResult Detalle(int id)
    {
        var compra = new Compra().MostrarCompras().FirstOrDefault(c => c.com_id == id);
        if (compra == null)
            return NotFound();

        ViewBag.Detalle = new DetalleCompra().MostrarDetalleCompra(id);
        return View(compra);
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