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

    public IActionResult Usuarios(string? buscar, int? rol, int? sucursal, int? estado = 1)
    {
        var usuarios = new Usuario().MostrarUsuarios();

        if (!string.IsNullOrEmpty(buscar))
            usuarios = usuarios.Where(u => u.usu_nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase)).ToList();
        if (estado != null)
            usuarios = usuarios.Where(u => u.usu_activo?.act_id == estado).ToList();
        if (rol != null)
            usuarios = usuarios.Where(u => u.usu_rol?.rol_id == rol).ToList();
        if (sucursal != null)
            usuarios = usuarios.Where(u => u.usu_sucursal?.suc_id == rol).ToList();

        ViewBag.Buscar = buscar;
        ViewBag.Estados = new SelectList(new Activo().MostrarActivos(), "act_id", "act_nombre", estado);
        ViewBag.Rol = new SelectList(new Rol().MostrarRoles(), "rol_id", "rol_nombre", rol);
        ViewBag.Sucursal = new SelectList(new Sucursal().MostrarSucursal(), "suc_id", "suc_nombre", sucursal);
        return View(usuarios);
    }

    [HttpPost]
    public IActionResult AgregarUsuario(Usuario usuario)
    {
        if (usuario.InsertarUsuario())
            TempData["Exito"] = "Usuario agregado correctamente.";
        else
            TempData["Error"] = "No se pudo agregar el usuario.";

        return RedirectToAction("Usuarios");
    }

    public IActionResult ModificarUsuario(int id)
    {
        var usuario = new Usuario().ObtenerUsuario(id);
        ViewBag.Estados = new SelectList(new Activo().MostrarActivos(), "act_id", "act_nombre", usuario.usu_activo?.act_id);
        ViewBag.Rol = new SelectList(new Rol().MostrarRoles(), "rol_id", "rol_nombre", usuario.usu_rol?.rol_id);
        ViewBag.Sucursal = new SelectList(new Sucursal().MostrarSucursal(), "suc_id", "suc_nombre", usuario.usu_sucursal?.suc_id);
        return PartialView("ModificarUsuario", usuario);
    }

    [HttpPost]
    public IActionResult ModificarUsuario(Usuario usuario)
    {
        if (usuario.ActualizarUsuario())
            TempData["Exito"] = "Usuario modificado correctamente.";
        else
            TempData["Error"] = "No se pudo modificar el Usuario.";

        return RedirectToAction("Usuarios");
    }

    [HttpPost]
    public IActionResult EliminarUsuario(int id)
    {
        var usuario = new Usuario { usu_id = id };

        if (usuario.ValidarUsuarioInactivo(id))
            if (usuario.EliminarUsuario(id))
                TempData["Exito"] = "Usuario eliminada correctamente.";
            else
                TempData["Error"] = "No se pudo eliminar el usuario.";
        else
            TempData["Error"] = "Usuario Activo, no se puede eliminar. Cambie el estado a Inactivo para poder eliminarla.";

        return RedirectToAction("Usuarios");
    }

}