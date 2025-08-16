using System.Collections.Generic;
using System.Reflection.Emit;
using BibliotecaWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaWeb.Data
{
    public class BibliotecaDbContext(DbContextOptions<BibliotecaDbContext> options) : DbContext(options)
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FechaRegistro).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Activo).HasDefaultValue(true);
            });

            // Configuración de Categoria
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
            });

            // Configuración de Libro
            modelBuilder.Entity<Libro>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Autor).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ISBN).HasMaxLength(20);
                entity.Property(e => e.TipoLibro).IsRequired().HasMaxLength(20);
                entity.Property(e => e.SubTipo).HasMaxLength(50);
                entity.Property(e => e.ImagenUrl).HasMaxLength(500);
                entity.Property(e => e.Disponible).HasDefaultValue(true);
                entity.Property(e => e.CantidadTotal).HasDefaultValue(1);
                entity.Property(e => e.CantidadDisponible).HasDefaultValue(1);

                // Relación con Categoria
                entity.HasOne(e => e.Categoria)
                      .WithMany(c => c.Libros)
                      .HasForeignKey(e => e.CategoriaId);

                // Restricción en TipoLibro (nueva sintaxis)
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Libro_TipoLibro",
                    "TipoLibro IN ('Virtual', 'Fisico')"));
            });

            // Configuración de Prestamo
            modelBuilder.Entity<Prestamo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FechaPrestamo).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(20).HasDefaultValue("Prestado");

                // Relaciones
                entity.HasOne(e => e.Libro)
                      .WithMany(l => l.Prestamos)
                      .HasForeignKey(e => e.LibroId);

                entity.HasOne(e => e.Usuario)
                      .WithMany(u => u.Prestamos)
                      .HasForeignKey(e => e.UsuarioId);

                // Restricción en Estado (nueva sintaxis)
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Prestamo_Estado",
                    "Estado IN ('Prestado', 'Devuelto', 'Vencido')"));
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}