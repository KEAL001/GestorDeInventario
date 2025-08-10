// En LoginWindow.xaml.cs

using GestorDeInventario.Data;
using GestorDeInventario.Models;
using GestorDeInventario.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GestorDeInventario
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            string nombreUsuario = txtNombreUsuario.Text;
            string contrasena = txtContrasena.Password;

            if (string.IsNullOrEmpty(nombreUsuario) || string.IsNullOrEmpty(contrasena))
            {
                MessageBox.Show("Por favor, ingrese un nombre de usuario y una contraseña.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new ApplicationDbContext())
            {
                var usuario = context.Usuarios
                    .Include(u => u.Rol)
                    .FirstOrDefault(u => u.NombreUsuario == nombreUsuario);

                // MODIFICAR ESTA LÍNEA: Agregar verificación de usuario activo  
                if (usuario != null && usuario.Activo && BCrypt.Net.BCrypt.Verify(contrasena, usuario.ContraseñaHash))
                {
                    SessionManager.IniciarSesion(usuario);
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    // Mensaje más específico para usuarios inactivos  
                    if (usuario != null && !usuario.Activo)
                    {
                        MessageBox.Show("Su cuenta ha sido desactivada. Contacte al administrador.", "Cuenta Inactiva", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show("Nombre de usuario o contraseña incorrectos.", "Error de Autenticación", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        

        // Nuevo método para manejar el evento KeyDown
        private void txtContrasena_KeyDown(object sender, KeyEventArgs e)
        {
            // Si la tecla presionada es Enter
            if (e.Key == Key.Enter)
            {
                // Llamamos al método de inicio de sesión
                btnIniciarSesion_Click(sender, e);
            }
        }
    }
}