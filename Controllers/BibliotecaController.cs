using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BibliotecaWeb.Data;
using BibliotecaWeb.Models;

namespace BibliotecaWeb.Controllers
{
    public class BibliotecaController : Controller
    {
        private readonly BibliotecaDbContext _context;

        public BibliotecaController(BibliotecaDbContext context)
        {
            _context = context;
        }

        // Form2: Biblioteca Virtual
        public async Task<IActionResult> Virtual()
        {
            var librosVirtuales = await _context.Libros
                .Include(l => l.Categoria)
                .Where(l => l.TipoLibro == "Virtual")
                .ToListAsync();

            var librosAgrupados = librosVirtuales.GroupBy(l => l.SubTipo).ToList();
            return View(librosAgrupados);
        }

        // Form3: Biblioteca Física  
        public async Task<IActionResult> Fisica()
        {
            var librosFisicos = await _context.Libros
                .Include(l => l.Categoria)
                .Where(l => l.TipoLibro == "Fisico")
                .ToListAsync();

            var librosPorCategoria = librosFisicos.GroupBy(l => l.Categoria?.Nombre ?? "Sin categoría").ToList();
            return View(librosPorCategoria);
        }

        // Form4: Préstamos
        public async Task<IActionResult> Prestamos()
        {
            if (HttpContext.Session.GetString("UsuarioId") == null)
            {
                TempData["Error"] = "Debe iniciar sesión para ver los préstamos";
                return RedirectToAction("Login", "Usuario");
            }

            var prestamos = await _context.Prestamos
                .Include(p => p.Libro)
                .Include(p => p.Usuario)
                .OrderByDescending(p => p.FechaPrestamo)
                .ToListAsync();

            return View(prestamos);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarEstadoPrestamo(int prestamoId, string nuevoEstado)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(p => p.Id == prestamoId);

            if (prestamo != null)
            {
                string estadoAnterior = prestamo.Estado;
                prestamo.Estado = nuevoEstado;

                if (nuevoEstado == "Devuelto" && estadoAnterior != "Devuelto")
                {
                    prestamo.FechaDevolucion = DateTime.Now;
                    if (prestamo.Libro != null)
                    {
                        prestamo.Libro.CantidadDisponible++;
                    }
                }
                else if (estadoAnterior == "Devuelto" && nuevoEstado != "Devuelto")
                {
                    prestamo.FechaDevolucion = null;
                    if (prestamo.Libro != null)
                    {
                        prestamo.Libro.CantidadDisponible--;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Estado del préstamo actualizado correctamente";
            }

            return RedirectToAction("Prestamos");
        }

        [HttpPost]
        public async Task<IActionResult> SolicitarPrestamo(int libroId)
        {
            var usuarioIdStr = HttpContext.Session.GetString("UsuarioId");
            if (usuarioIdStr == null)
            {
                TempData["Error"] = "Debe iniciar sesión para solicitar un préstamo";
                return RedirectToAction("Login", "Usuario");
            }

            int usuarioId = int.Parse(usuarioIdStr);
            var libro = await _context.Libros.FindAsync(libroId);

            if (libro == null)
            {
                TempData["Error"] = "Libro no encontrado";
                return RedirectToAction("Fisica");
            }

            if (libro.CantidadDisponible <= 0)
            {
                TempData["Error"] = "El libro no está disponible en este momento";
                return RedirectToAction("Fisica");
            }

            // Verificar si el usuario ya tiene el libro prestado
            var prestamoExistente = await _context.Prestamos
                .FirstOrDefaultAsync(p => p.LibroId == libroId && p.UsuarioId == usuarioId && p.Estado == "Prestado");

            if (prestamoExistente != null)
            {
                TempData["Error"] = "Ya tiene este libro en préstamo";
                return RedirectToAction("Fisica");
            }

            var nuevoPrestamo = new Prestamo
            {
                LibroId = libroId,
                UsuarioId = usuarioId,
                FechaPrestamo = DateTime.Now,
                FechaLimite = DateTime.Now.AddDays(15),
                Estado = "Prestado"
            };

            libro.CantidadDisponible--;

            _context.Prestamos.Add(nuevoPrestamo);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Préstamo solicitado correctamente";
            return RedirectToAction("Prestamos");
        }

        public async Task<IActionResult> DetalleLibro(int id)
        {
            var libro = await _context.Libros
                .Include(l => l.Categoria)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }
    }
}