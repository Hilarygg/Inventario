using Microsoft.AspNetCore.Mvc;
using Inventario.Models;
using Inventario.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventario.Controllers;

public class CatalogosController : Controller
{
    private readonly ImagenService _imagen;

    public CatalogosController(ImagenService imagen)
    {
        _imagen = imagen;
    }

    public IActionResult CategoriasPublico(string? buscar)
    {
        var categorias = new Categoria().MostrarCategoriasPublico();

        if (!string.IsNullOrEmpty(buscar))
            categorias = categorias.Where(c => c.cat_nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase)).ToList();

        ViewBag.Buscar = buscar;
        ViewBag.Categorias = categorias;

        return View();
    }

    public IActionResult Categorias(string? buscar, int? estado = 1)
    {
        var categorias = new Categoria().MostrarCategoriasAdmin();

        if (!string.IsNullOrEmpty(buscar))
            categorias = categorias.Where(c => c.cat_nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase)).ToList();
        if (estado != null)
            categorias = categorias.Where(c => c.cat_activo?.act_id == estado).ToList();

        ViewBag.Buscar = buscar;
        ViewBag.Estados = new SelectList(new Activo().MostrarActivos(), "act_id", "act_nombre", estado);
        return View(categorias);
    }

    [HttpPost]
    public IActionResult AgregarCategoria(Categoria categoria, IFormFile? imagen)
    {
        categoria.cat_imagen = _imagen.Guardar(imagen, "Categorias");

        if (categoria.InsertCategoria())
            TempData["Exito"] = "Categoría agregada correctamente.";
        else
            TempData["Error"] = "No se pudo agregar la categoría.";

        return RedirectToAction("Categorias");
    }

    public IActionResult ModificarCategoria(int id)
    {
        var categoria = new Categoria().ObtenerCategoria(id);
        ViewBag.Estados = new SelectList(new Activo().MostrarActivos(), "act_id", "act_nombre", categoria.cat_activo?.act_id);
        return PartialView("ModificarCategoria", categoria);
    }

    [HttpPost]
    public IActionResult ModificarCategoria(Categoria categoria, IFormFile? imagen)
    {
        categoria.cat_imagen = _imagen.Guardar(imagen, "Categorias", categoria.cat_imagen ?? "");

        if (categoria.ActualizarCategoria())
            TempData["Exito"] = "Categoría modificada correctamente.";
        else
            TempData["Error"] = "No se pudo modificar la categoría.";

        return RedirectToAction("Categorias");
    }

    public IActionResult Marcas(string? buscar, int? estado = 1)
    {
        var marcas = new Marca().MostrarMarcas();

        if (!string.IsNullOrEmpty(buscar))
            marcas = marcas.Where(m => m.mar_nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase)).ToList();
        if (estado != null)
            marcas = marcas.Where(m => m.mar_activo?.act_id == estado).ToList();

        ViewBag.Buscar = buscar;
        ViewBag.Estados = new SelectList(new Activo().MostrarActivos(), "act_id", "act_nombre", estado);
        return View(marcas);
    }

    [HttpPost]
    public IActionResult AgregarMarca(Marca marca)
    {
        if (marca.InsertMarca())
            TempData["Exito"] = "Marca agregada correctamente.";
        else
            TempData["Error"] = "No se pudo agregar la marca.";

        return RedirectToAction("Marcas");
    }

    public IActionResult ModificarMarca(int id)
    {
        var marca = new Marca().ObtenerMarca(id);
        ViewBag.Estados = new SelectList(new Activo().MostrarActivos(), "act_id", "act_nombre", marca.mar_activo?.act_id);
        return PartialView("ModificarMarca", marca);
    }

    [HttpPost]
    public IActionResult ModificarMarca(Marca marca)
    {
        if (marca.ActualizarMarca())
            TempData["Exito"] = "Marca modificada correctamente.";
        else
            TempData["Error"] = "No se pudo modificar la marca.";

        return RedirectToAction("Marcas");
    }
    
    [HttpPost]
    public IActionResult EliminarMarca(int id)
    {
        var marca = new Marca { mar_id = id };

        if (marca.ValidarMarcaInactiva(id))
            if(marca.ValidarMarcaProductos(id))
                if (marca.EliminarMarca(id))
                TempData["Exito"] = "Marca eliminada correctamente.";
                else
                    TempData["Error"] = "No se pudo eliminar la marca.";
            else
                TempData["Error"] = "No se puede eliminar la marca porque tiene productos asociados.";
        else
            TempData["Error"] = "Marca Activa, no se puede eliminar. Cambie el estado a Inactivo para poder eliminarla.";

        return RedirectToAction("Marcas");
    }

    public IActionResult Proveedores(string? buscar, int? estado = 1)
    {
        var proveedores = new Proveedor().MostrarProveedoresAdmin();

        if (!string.IsNullOrEmpty(buscar))
            proveedores = proveedores.Where(p => p.pro_nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase)).ToList();
        if (estado != null)
            proveedores = proveedores.Where(p => p.pro_activo?.act_id == estado).ToList();

        ViewBag.Buscar = buscar;
        ViewBag.Estados = new SelectList(new Activo().MostrarActivos(), "act_id", "act_nombre", estado);
        return View(proveedores);
    }

    [HttpPost]
    public IActionResult AgregarProveedor(Proveedor proveedor)
    {
        if (proveedor.InsertProveedor())
            TempData["Exito"] = "Proveedor agregado correctamente.";
        else
            TempData["Error"] = "No se pudo agregar el proveedor.";

        return RedirectToAction("Proveedores");
    }

    public IActionResult ModificarProveedor(int id)
    {
        var proveedor = new Proveedor().ObtenerProveedor(id);
        ViewBag.Estados = new SelectList(new Activo().MostrarActivos(), "act_id", "act_nombre", proveedor.pro_activo?.act_id);
        return PartialView("ModificarProveedor", proveedor);
    }

    [HttpPost]
    public IActionResult ModificarProveedor(Proveedor proveedor)
    {
        if (proveedor.ActualizarProveedor())
            TempData["Exito"] = "Proveedor modificado correctamente.";
        else
            TempData["Error"] = "No se pudo modificar el proveedor.";

        return RedirectToAction("Proveedores");
    }

    [HttpPost]
    public IActionResult EliminarProveedor(int id)
    {
        var proveedor = new Proveedor { pro_id = id };

        if (proveedor.ValidarProveedorInactivo(id))
            if(proveedor.ValidarProveedorProductos(id))
                if (proveedor.EliminarProveedor(id))
                    TempData["Exito"] = "Proveedor eliminado correctamente.";
                else
                    TempData["Error"] = "No se pudo eliminar el proveedor.";
            else
                TempData["Error"] = "No se puede eliminar el proveedor porque tiene productos asociados.";
        else
            TempData["Error"] = "Proveedor Activo, no se puede eliminar. Cambie el estado a Inactivo para poder eliminarlo.";

        return RedirectToAction("Proveedores");
    }
}
