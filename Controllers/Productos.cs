using Inventario.Models;
using Inventario.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventario.Controllers;

public class ProductoController : Controller
{

    private readonly ImagenService _imagen;

    public ProductoController(ImagenService imagen)
    {
        _imagen = imagen;
    }

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
        ViewBag.Categorias = new SelectList(new Categoria().MostrarCategoriasPublico(), "cat_id", "cat_nombre", categoria);
        ViewBag.Marcas = new SelectList(new Marca().MostrarMarcas(), "mar_id", "mar_nombre", marca);
        ViewBag.Estados = new SelectList(new Activo().MostrarActivos(), "act_id", "act_nombre", estado);
        ViewBag.Proveedores = new SelectList(new Proveedor().MostrarProveedores(), "pro_id", "pro_nombre", proveedor);
        ViewBag.Unidades = new SelectList(new Unidad().MostrarUnidades(), "uni_id", "uni_nombre", unidad);

        return View(productos);
    }

    [HttpPost]
    public IActionResult Agregar(Producto producto, IFormFile? imagen)
    {
        producto.prod_imagen = _imagen.Guardar(imagen, "Productos");

        if (producto.InsertProducto())
            TempData["Exito"] = "Producto agregado correctamente.";
        else
            TempData["Error"] = "No se pudo agregar el producto.";

        return RedirectToAction("Index");
    }

    public IActionResult Modificar(int id)
    {
        var producto = new Producto().ObtenerProducto(id);
        if (producto == null)
        {
            return NotFound();
        }

        ViewBag.Categorias = new SelectList(new Categoria().MostrarCategoriasPublico(), "cat_id", "cat_nombre", producto.prod_categoria?.cat_id);
        ViewBag.Marcas = new SelectList(new Marca().MostrarMarcas(), "mar_id", "mar_nombre", producto.prod_marca?.mar_id);
        ViewBag.Estados = new SelectList(new Activo().MostrarActivos(), "act_id", "act_nombre", producto.prod_activo?.act_id);
        ViewBag.Proveedores = new SelectList(new Proveedor().MostrarProveedores(), "pro_id", "pro_nombre", producto.prod_proveedor?.pro_id);
        ViewBag.Unidades = new SelectList(new Unidad().MostrarUnidades(), "uni_id", "uni_nombre", producto.prod_unidad?.uni_id);
        

        return View(producto);
    }

    
    [HttpPost]
    public IActionResult Modificar(Producto producto, IFormFile? imagen)
    {
        producto.prod_imagen = _imagen.Guardar(imagen, "Productos", producto.prod_imagen);

        if (producto.ActualizarProducto())
            TempData["Exito"] = "Producto actualizado correctamente.";
        else
            TempData["Error"] = "No se pudo actualizar el producto.";

        return RedirectToAction("Index");
    }
}