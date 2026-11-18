using Inventario.Models;
using Inventario.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

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

    [HttpPost]
    public IActionResult Modificar(Producto producto, IFormFile? imagen)
    {
        var anterior = new Producto().ObtenerProducto(producto.prod_id);
        producto.prod_imagen = _imagen.Guardar(imagen, "Productos", producto.prod_imagen);

        if (producto.ActualizarProducto())
        {
            if (anterior.prod_id != 0 && anterior.prod_precio != producto.prod_precio)
            {
                int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var usuarioId);

                new HistorialPrecio()
                {
                    his_precioAnterior = anterior.prod_precio,
                    his_precioNuevo = producto.prod_precio,
                    his_fechaCambio = DateTime.Now,
                    his_producto = new Producto { prod_id = producto.prod_id },
                    his_usuario = new Usuario { usu_id = usuarioId }
                }.InsertHistorialPrecio();
            }
            TempData["Exito"] = "Producto actualizado correctamente.";
        }
        else
            TempData["Error"] = "No se pudo actualizar el producto.";

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
    public IActionResult EliminarProducto(int id)
    {
        var producto = new Producto();

        if (!producto.ValidarProductoInactivo(id))
            TempData["Error"] = "Producto Activo, no se puede eliminar. Cambie el estado a Inactivo para poder eliminarlo.";
        else if (producto.EliminarProducto(id))
            TempData["Exito"] = "Producto eliminado correctamente.";
        else
            TempData["Error"] = "No se pudo eliminar el producto. Puede tener ventas, compras o inventario asociados.";

        return RedirectToAction("Index");
    }

    public IActionResult Precios(int? categoria, string buscar, string orden)
    {
        List<Producto> productos;

        productos = new Producto().BuscarPrecios(buscar, categoria, orden);

        ViewBag.Categorias = new Categoria().MostrarCategoriasPublico();
        ViewBag.CategoriaActual = categoria;
        ViewBag.Buscar = buscar;
        ViewBag.Orden = orden;

        return View(productos);
    }

    public IActionResult BuscarProductos(string busqueda, int? categoria, string orden)
    {
        var productos = new Producto().BuscarPrecios(busqueda, categoria, orden);

        return PartialView("_ListaProductos", productos);
    }

    public IActionResult HistorialPrecio(int? productoId, int? anio)
    {
        var productos = new Producto().MostrarProductos();
        anio ??= DateTime.Now.Year;

        if (productoId != null)
        {
            ViewBag.Historial = new HistorialPrecio().MostrarHistorial(productoId.Value, anio);
            ViewBag.ProductoNombre = productos.FirstOrDefault(p => p.prod_id == productoId)?.prod_nombre;
        }

        ViewBag.ProductoId = productoId;
        ViewBag.Anios = new SelectList(Enumerable.Range(DateTime.Now.Year - 5, 6).Reverse(), anio);

        return View(productos);
    }
}