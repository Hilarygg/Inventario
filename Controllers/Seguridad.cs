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
            usuarios = usuarios.Where(u => u.usu_sucursal?.suc_id == sucursal).ToList();

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
        usuario.usu_password = "";
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

    public IActionResult Permisos(int? rolId)
    {
        var roles = new Rol().MostrarRoles();
        var actual = roles.FirstOrDefault(r => r.rol_id == rolId) ?? roles.FirstOrDefault();

        ViewBag.RolActual = actual;
        if (actual != null)
            ViewBag.Permisos = new Rol().ObtenerPermisos(actual.rol_id);
        
        ViewBag.Estados = new SelectList(new Activo().MostrarActivos(), "act_id", "act_nombre");
        return View(roles);
    }

    [HttpPost]
    public IActionResult GuardarPermisos(int rolId, int[]? menus)
    {
        menus ??= Array.Empty<int>();

        if (new Rol().GuardarPermisos(rolId, menus))
            TempData["Exito"] = "Permisos actualizados correctamente.";
        else
            TempData["Error"] = "No se pudieron guardar los permisos.";

        return RedirectToAction("Permisos", new { rolId });
    }

    public IActionResult AgregarRol(Rol rol)
    {
        if (rol.InsertRol())
            TempData["Exito"] = "Rol agregado correctamente";
        else
            TempData["Error"] = "No se pudo agregar el rol";

        return RedirectToAction("Permisos");
    }

    public IActionResult EliminarRol(int id)
    {
        var rol = new Rol { rol_id = id };

        if (rol.ValidarRolInactivo(id))
            if (rol.EliminarRol(id))
                TempData["Exito"] = "Rol eliminado correctamente.";
            else
                TempData["Error"] = "No se pudo eliminar el rol.";
        else
            TempData["Error"] = "Rol Activo, no se puede eliminar. Cambie el estado a Inactivo para poder eliminarlo.";

        return RedirectToAction("Permisos");
    }

    public IActionResult ModificarRol(int id)
    {
        var rol = new Rol().ObtenerRol(id);
        ViewBag.Estados = new SelectList(new Activo().MostrarActivos(), "act_id", "act_nombre", rol.rol_activo?.act_id);
        return PartialView("ModificarRol", rol);
    }

    [HttpPost]
    public IActionResult ModificarRol(Rol rol)
    {
        if (rol.ActualizarRol())
            TempData["Exito"] = "Rol modificado correctamente.";
        else
            TempData["Error"] = "No se pudo modificar el Rol.";

        return RedirectToAction("Permisos");
    }

}