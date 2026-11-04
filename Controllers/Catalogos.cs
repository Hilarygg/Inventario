using Microsoft.AspNetCore.Mvc;
using Inventario.Models;
using Inventario.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventario.Controllers;

public class CategoriaController : Controller
{
    private readonly ImagenService _imagen;

    public CategoriaController(ImagenService imagen)
    {
        _imagen = imagen;
    }

    public IActionResult Index(string? buscar, int? estado = 1)
    {
        var categorias = new Categoria().MostrarCategorias();

        if (!string.IsNullOrEmpty(buscar))
            categorias = categorias.Where(c => c.cat_nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase)).ToList();
        if (estado != null)
            categorias = categorias.Where(c => c.cat_activo?.act_id == estado).ToList();

        ViewBag.Buscar = buscar;
        ViewBag.Estados = new SelectList(new Activo().MostrarActivos(), "act_id", "act_nombre", estado);

        return View(categorias);
    }
}