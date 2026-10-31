using Inventario.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventario.Controllers;

public class ProductoController : Controller
{
    public IActionResult Index(string? buscar, int? categoria, int? marca, int? proveedor, int? unidad, int? estado = 1)
    {
        var productos = new Producto().MostrarProductos();

        if (!string.IsNullOrEmpty(buscar))
            productos = productos.Where(p => p.prod_nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase)).ToList();
        if (categoria != null)
            productos = productos.Where(p => p.prod_categoria?.cat_id == categoria).ToList();
        if (marca != null)
            productos = productos.Where(p => p.prod_marca?.mar_id == marca).ToList();
        if (estado != null)
            productos = productos.Where(p => p.prod_activo?.act_id == estado).ToList();
        if (proveedor != null)
            productos = productos.Where(p => p.prod_proveedor?.pro_id == proveedor).ToList();
        if (unidad != null)
            productos = productos.Where(p => p.prod_unidad?.uni_id == unidad).ToList();

        ViewBag.Buscar = buscar;
        ViewBag.Categorias = new SelectList(new Categoria().MostrarCategorias(), "cat_id", "cat_nombre", categoria);
        ViewBag.Marcas = new SelectList(new Marca().MostrarMarcas(), "mar_id", "mar_nombre", marca);
        ViewBag.Estados = new SelectList(new Activo().MostrarActivos(), "act_id", "act_nombre", estado);
        ViewBag.Proveedores = new SelectList(new Proveedor().MostrarProveedores(), "pro_id", "pro_nombre", proveedor);
        ViewBag.Unidades = new SelectList(new Unidad().MostrarUnidades(), "uni_id", "uni_nombre", unidad);

        return View(productos);
    }

    [HttpPost]
    public IActionResult Agregar(Producto producto)
    {
        Console.WriteLine("Llegó a Agregar: " + producto.prod_nombre);
        
        producto.InsertProducto();
        return RedirectToAction("Index");
    }
}