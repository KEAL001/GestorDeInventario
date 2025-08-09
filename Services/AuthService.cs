using BCrypt.Net;
using GestorDeInventario.Data;
using GestorDeInventario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;




namespace GestorDeInventario.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool ValidarCredenciales(string nombreUsuario, string contraseña)
        {
            // Busca el usuario por su nombre de usuario en la base de datos
            var usuario = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);

            // Si el usuario no existe, la autenticación falla
            if (usuario == null)
            {
                return false;
            }

            // Verifica si el hash de la contraseña coincide
            return BCrypt.Net.BCrypt.Verify(contraseña, usuario.ContraseñaHash);
        }
        public Usuario ObtenerUsuario(string nombreUsuario, string contraseña)
        {
            // Usamos .Include() para cargar la propiedad de navegación 'Rol'
            var usuario = _context.Usuarios
                                  .Include(u => u.Rol) // <-- Esto es lo que faltaba
                                  .FirstOrDefault(u => u.NombreUsuario == nombreUsuario);

            if (usuario != null && BCrypt.Net.BCrypt.Verify(contraseña, usuario.ContraseñaHash))
            {
                return usuario;
            }
            return null;
        }
    }
}