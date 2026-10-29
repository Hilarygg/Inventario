using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Inventario.Models;

namespace Inventario.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        Categoria categoria = new Categoria();

        ViewBag.Productos = new Producto().MostrarProductosDestacados();
        return View(categoria.MostrarCategorias());
    }
       public IActionResult Panel()
    {
        return View();
    }

    public IActionResult Categorias()
    {
        Categoria categoria = new Categoria();
        return View(categoria.MostrarCategoriasPublico());
    }

    public IActionResult Nosotros()
    {
        return View();
    }   

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
