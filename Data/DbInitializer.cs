using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestorDeInventario.Models;
using BCrypt.Net;

namespace GestorDeInventario.Data
{
    public static class DbInitializer
    {
        public static void SeedData(ApplicationDbContext context)
        {
            // Crea roles si no existen
            if (!context.Roles.Any())
            {
                context.Roles.Add(new Rol { NombreRol = "Admin", Descripcion = "Administrador del sistema" });
                context.Roles.Add(new Rol { NombreRol = "Empleado", Descripcion = "Empleado con permisos limitados" });
                context.SaveChanges();
            }

            // Crea un usuario administrador por defecto si no existe
            if (!context.Usuarios.Any())
            {
                var adminRol = context.Roles.FirstOrDefault(r => r.NombreRol == "Admin");
                if (adminRol != null)
                {
                    context.Usuarios.Add(new Usuario
                    {
                        NombreUsuario = "admin",
                        ContraseñaHash = BCrypt.Net.BCrypt.HashPassword("12345"), // Contraseña por defecto: 12345
                        RolID = adminRol.ID,
                        Activo = true
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}