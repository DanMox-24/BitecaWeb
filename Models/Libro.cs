using System.ComponentModel.DataAnnotations;

namespace BibliotecaWeb.Models
{
    public class Libro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El autor es obligatorio")]
        public string Autor { get; set; } = string.Empty;

        public string? ISBN { get; set; }

        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }

        [Required]
        public string TipoLibro { get; set; } = string.Empty; // Virtual, Fisico

        public string? SubTipo { get; set; } // Para virtuales: E-libro, Bibliotecas Publicas, Online

        public DateTime? FechaPublicacion { get; set; }

        public string? Descripcion { get; set; }

        public string? ImagenUrl { get; set; }

        public bool Disponible { get; set; } = true;

        public int CantidadTotal { get; set; } = 1;

        public int CantidadDisponible { get; set; } = 1;

        public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
    }
}