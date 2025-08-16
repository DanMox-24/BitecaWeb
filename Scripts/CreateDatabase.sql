-- Script de creación de base de datos para la Biblioteca Web
-- Ejecutar este script en SQL Server Management Studio

CREATE DATABASE BibliotecaWeb;
GO

USE BibliotecaWeb;
GO

-- Tabla de Usuarios
CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    FechaRegistro DATETIME DEFAULT GETDATE(),
    Activo BIT DEFAULT 1
);

-- Tabla de Categorías
CREATE TABLE Categorias (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL
);

-- Tabla de Libros
CREATE TABLE Libros (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Titulo NVARCHAR(200) NOT NULL,
    Autor NVARCHAR(100) NOT NULL,
    ISBN NVARCHAR(20),
    CategoriaId INT FOREIGN KEY REFERENCES Categorias(Id),
    TipoLibro NVARCHAR(20) CHECK (TipoLibro IN ('Virtual', 'Fisico')) NOT NULL,
    SubTipo NVARCHAR(50), -- Para virtuales: E-libro, Bibliotecas Publicas, Online
    FechaPublicacion DATE,
    Descripcion NVARCHAR(MAX),
    ImagenUrl NVARCHAR(500),
    Disponible BIT DEFAULT 1,
    CantidadTotal INT DEFAULT 1,
    CantidadDisponible INT DEFAULT 1
);

-- Tabla de Préstamos
CREATE TABLE Prestamos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    LibroId INT FOREIGN KEY REFERENCES Libros(Id),
    UsuarioId INT FOREIGN KEY REFERENCES Usuarios(Id),
    FechaPrestamo DATETIME DEFAULT GETDATE(),
    FechaDevolucion DATETIME,
    FechaLimite DATETIME,
    Estado NVARCHAR(20) CHECK (Estado IN ('Prestado', 'Devuelto', 'Vencido')) DEFAULT 'Prestado'
);

-- Insertar datos de prueba

-- Categorías
INSERT INTO Categorias (Nombre) VALUES 
('Literatura'),
('Técnicos'),
('General');

-- Usuarios de prueba
INSERT INTO Usuarios (Nombre, Email, Password) VALUES 
('Admin Usuario', 'admin@biblioteca.com', 'admin123'),
('Juan Pérez', 'juan@email.com', 'juan123'),
('María García', 'maria@email.com', 'maria123'),
('Carlos López', 'carlos@email.com', 'carlos123');

-- Libros Virtuales
INSERT INTO Libros (Titulo, Autor, ISBN, CategoriaId, TipoLibro, SubTipo, FechaPublicacion, Descripcion, ImagenUrl) VALUES 
-- E-libros
('Cien años de soledad (Digital)', 'Gabriel García Márquez', '978-0307474728', 1, 'Virtual', 'E-libro', '1967-06-05', 'Obra maestra del realismo mágico', '/images/cien-anos.jpg'),
('El Quijote (Digital)', 'Miguel de Cervantes', '978-8424926083', 1, 'Virtual', 'E-libro', '1605-01-16', 'La obra cumbre de la literatura española', '/images/quijote.jpg'),
('Programación en C# (Digital)', 'Microsoft Press', '978-1509307760', 2, 'Virtual', 'E-libro', '2020-03-15', 'Guía completa de programación en C#', '/images/csharp.jpg'),

-- Bibliotecas Públicas Online
('Don Quijote Online', 'Miguel de Cervantes', '978-8424926084', 1, 'Virtual', 'Bibliotecas Publicas', '1615-01-16', 'Segunda parte del Quijote disponible online', '/images/quijote2.jpg'),
('Bases de Datos Online', 'Elmasri & Navathe', '978-0133970777', 2, 'Virtual', 'Bibliotecas Publicas', '2015-04-03', 'Fundamentos de sistemas de bases de datos', '/images/bd.jpg'),

-- Online
('Enciclopedia Universal', 'Varios Autores', '978-8467032451', 3, 'Virtual', 'Online', '2022-01-01', 'Acceso completo a enciclopedia digital', '/images/enciclopedia.jpg'),
('Revista Científica Digital', 'Editorial Científica', '978-1234567890', 2, 'Virtual', 'Online', '2023-06-15', 'Revista de avances tecnológicos', '/images/revista.jpg');

-- Libros Físicos
INSERT INTO Libros (Titulo, Autor, ISBN, CategoriaId, TipoLibro, SubTipo, FechaPublicacion, Descripcion, ImagenUrl, CantidadTotal, CantidadDisponible) VALUES 
-- Literatura
('Cien años de soledad', 'Gabriel García Márquez', '978-0307474729', 1, 'Fisico', 'Literatura', '1967-06-05', 'Obra maestra del realismo mágico', '/images/cien-anos-fisico.jpg', 3, 2),
('1984', 'George Orwell', '978-0451524935', 1, 'Fisico', 'Literatura', '1949-06-08', 'Distopía clásica sobre el totalitarismo', '/images/1984.jpg', 2, 1),
('El amor en los tiempos del cólera', 'Gabriel García Márquez', '978-0307387387', 1, 'Fisico', 'Literatura', '1985-03-06', 'Historia de amor que trasciende el tiempo', '/images/amor-colera.jpg', 2, 2),

-- Técnicos
('Algoritmos y Estructuras de Datos', 'Robert Sedgewick', '978-0321573513', 2, 'Fisico', 'Tecnicos', '2011-03-24', 'Fundamentos de algoritmos y programación', '/images/algoritmos.jpg', 4, 3),
('Sistemas Operativos', 'Abraham Silberschatz', '978-1118063330', 2, 'Fisico', 'Tecnicos', '2012-12-17', 'Conceptos fundamentales de sistemas operativos', '/images/so.jpg', 3, 2),
('Redes de Computadores', 'Andrew Tanenbaum', '978-0132126953', 2, 'Fisico', 'Tecnicos', '2010-10-28', 'Guía completa sobre redes de computadores', '/images/redes.jpg', 2, 1),

-- General
('Historia Universal', 'H.G. Wells', '978-8420674452', 3, 'Fisico', 'General', '1920-11-01', 'Compendio de la historia mundial', '/images/historia.jpg', 3, 3),
('Filosofía para Principiantes', 'Jostein Gaarder', '978-8478888368', 3, 'Fisico', 'General', '1991-08-01', 'Introducción accesible a la filosofía', '/images/filosofia.jpg', 2, 2),
('Ciencias Naturales', 'Carl Sagan', '978-0345331359', 3, 'Fisico', 'General', '1980-09-28', 'Exploración del cosmos y la ciencia', '/images/cosmos.jpg', 2, 1);

-- Préstamos de ejemplo
INSERT INTO Prestamos (LibroId, UsuarioId, FechaPrestamo, FechaLimite, Estado) VALUES 
(8, 2, DATEADD(day, -5, GETDATE()), DATEADD(day, 10, GETDATE()), 'Prestado'),
(9, 3, DATEADD(day, -3, GETDATE()), DATEADD(day, 12, GETDATE()), 'Prestado'),
(11, 4, DATEADD(day, -8, GETDATE()), DATEADD(day, 7, GETDATE()), 'Prestado'),
(13, 2, DATEADD(day, -15, GETDATE()), DATEADD(day, -1, GETDATE()), 'Vencido');

-- Actualizar disponibilidad de libros prestados
UPDATE Libros SET CantidadDisponible = CantidadDisponible - 1 WHERE Id IN (8, 9, 11, 13);

-- Crear índices para mejorar el rendimiento
CREATE INDEX IX_Libros_TipoLibro ON Libros(TipoLibro);
CREATE INDEX IX_Libros_CategoriaId ON Libros(CategoriaId);
CREATE INDEX IX_Prestamos_Estado ON Prestamos(Estado);
CREATE INDEX IX_Prestamos_UsuarioId ON Prestamos(UsuarioId);

PRINT 'Base de datos creada exitosamente con datos de prueba';