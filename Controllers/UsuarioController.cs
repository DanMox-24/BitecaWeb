using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaWeb.Data;
using BibliotecaWeb.Models;

namespace BibliotecaWeb.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly BibliotecaDbContext _context;

        public UsuarioController(BibliotecaDbContext context)
        {
            _context = context;
        }

        // Form5: Usuarios (Login y Registro)
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password && u.Activo);

                if (usuario != null)
                {
                    HttpContext.Session.SetString("UsuarioId", usuario.Id.ToString());
                    HttpContext.Session.SetString("UsuarioNombre", usuario.Nombre);
                    HttpContext.Session.SetString("UsuarioEmail", usuario.Email);

                    TempData["Success"] = $"¡Bienvenido {usuario.Nombre}!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Email o contraseña incorrectos");
                }
            }

            return View(model);
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el email ya existe
                var usuarioExistente = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("Email", "Este email ya está registrado");
                    return View(model);
                }

                var nuevoUsuario = new Usuario
                {
                    Nombre = model.Nombre,
                    Email = model.Email,
                    Password = model.Password, // En producción, usar hash
                    FechaRegistro = DateTime.Now,
                    Activo = true
                };

                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Registro exitoso. Ya puede iniciar sesión.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Sesión cerrada correctamente";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Usuarios()
        {
            if (HttpContext.Session.GetString("UsuarioEmail") != "admin@biblioteca.com")
            {
                TempData["Error"] = "No tiene permisos para acceder a esta sección";
                return RedirectToAction("Index", "Home");
            }

            var usuarios = _context.Usuarios.OrderByDescending(u => u.FechaRegistro).ToList();
            return View(usuarios);
        }
    }
}