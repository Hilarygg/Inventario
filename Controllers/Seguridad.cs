using Microsoft.AspNetCore.Mvc;
using Inventario.Models;
using Inventario.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using AspNetCoreGeneratedDocument;

namespace Inventario.Controllers;

public class SeguridadController : Controller
{
    private readonly ImagenService _imagen;

    public SeguridadController(ImagenService imagen)
    {
        _imagen = imagen;
    }

    public IActionResult Usuarios(string? buscar, int? estado = 1)
    {
        var usuarios = new Usuario().MostrarUsuarios();

        if (!string.IsNullOrEmpty(buscar))
            usuarios = usuarios.Where(u => u.usu_nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase)).ToList();
        if (estado != null)
            usuarios = usuarios.Where(u => u.usu_activo?.act_id == estado).ToList();

        ViewBag.Buscar = buscar;
        ViewBag.Estados = new SelectList(new Activo().MostrarActivos(), "act_id", "act_nombre", estado);
        return View(usuarios);
    }

    public IActionResult AgregarUsuario(Usuario usuario)
    {
        if (usuario.InsertarUsuario())
            TempData["Exito"] = "Usuario agregado correctamente.";
        else
            TempData["Error"] = "No se pudo agregar el usuario.";

        return RedirectToAction("Usuarios");
    }
}