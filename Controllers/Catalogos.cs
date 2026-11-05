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

}


