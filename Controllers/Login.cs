using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Inventario.Models;

namespace Inventario.Controllers;

public class LoginController : Controller
{
    public IActionResult Index()
    {
        return View(new Login());
    }

   [HttpPost]
    public async Task<IActionResult> Index(Login modelo)
    {
        if (!modelo.Validar())
        {
            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View(modelo);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, modelo.usu_id.ToString()),
            new Claim(ClaimTypes.Name, modelo.usu_nombre),
            new Claim("RolId", modelo.usu_rol.ToString())
        };

        var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad));

        return RedirectToAction("Panel", "Home");
    }
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}