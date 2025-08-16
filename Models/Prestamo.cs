namespace BibliotecaWeb.Models
{
    public class Prestamo
    {
        public int Id { get; set; }

        public int LibroId { get; set; }
        public Libro? Libro { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public DateTime FechaPrestamo { get; set; } = DateTime.Now;

        public DateTime? FechaDevolucion { get; set; }

        public DateTime FechaLimite { get; set; }

        public string Estado { get; set; } = "Prestado"; // Prestado, Devuelto, Vencido
    }
}