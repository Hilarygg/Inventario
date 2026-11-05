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

    public IActionResult AgregarCategoria(Categoria categoria, IFormFile? imagen)
    {
        categoria.cat_imagen = _imagen.Guardar(imagen, "Categorias");

        if (categoria.InsertCategoria())
            TempData["Exito"] = "Categoría agregada correctamente.";
        else
            TempData["Error"] = "No se pudo agregar la categoría.";

        return RedirectToAction("Categorias");
    }
}


