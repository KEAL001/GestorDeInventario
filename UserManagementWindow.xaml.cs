using GestorDeInventario.Data;
using GestorDeInventario.Models;
using GestorDeInventario.Services;
using GestorDeInventario.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Globalization;
using System.Windows.Data;


namespace GestorDeInventario
{
    public partial class UserManagementWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public UserManagementWindow()
        {
            InitializeComponent();
            _context = DbContextManager.Instance;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CargarUsuarios();
        }
        private void dgUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool haySeleccion = dgUsuarios.SelectedItem != null;
            btnEditarUsuario.IsEnabled = haySeleccion;
            btnEliminarUsuario.IsEnabled = haySeleccion;
        }
        private void btnToggleActivo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Usuario usuario)
            {
                string accion = usuario.Activo ? "desactivar" : "activar";
                MessageBoxResult result = MessageBox.Show(
                    $"¿Estás seguro de que quieres {accion} al usuario '{usuario.NombreUsuario}'?",
                    "Confirmar Cambio de Estado",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    usuario.Activo = !usuario.Activo;
                    _context.SaveChanges();
                    CargarUsuarios(); // Recargar para actualizar la vista  
                }
            }
        }

        private void CargarUsuarios()
        {
            dgUsuarios.ItemsSource = _context.Usuarios.Include(u => u.Rol).ToList();
        }

        private void btnAgregarUsuario_Click(object sender, RoutedEventArgs e)
        {
            // Crea una nueva instancia del formulario de usuario
            var userForm = new UserForm();
            userForm.ShowDialog();
            CargarUsuarios(); // Recarga la lista después de la acción
            _context.SaveChanges();
            DataRefreshService.NotifyDataRefreshed();
        }

        private void btnEditarUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsuarios.SelectedItem is Usuario usuarioSeleccionado)
            {
                var userForm = new UserForm(usuarioSeleccionado);
                userForm.ShowDialog();
                CargarUsuarios(); // Recarga la lista después de la acción
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un usuario para editar.", "Error");
            }
            _context.SaveChanges();
            DataRefreshService.NotifyDataRefreshed();
        }

        private void btnEliminarUsuario_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsuarios.SelectedItem is Usuario usuario)
            {
                // Verificar que no sea el último admin  
                var adminCount = _context.Usuarios.Include(u => u.Rol)
                    .Count(u => u.Rol.NombreRol == "Admin" && u.Activo);

                if (usuario.Rol.NombreRol == "Admin" && adminCount <= 1)
                {
                    MessageBox.Show("No se puede eliminar el último usuario administrador del sistema.",
                                  "Eliminación no permitida", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBoxResult result = MessageBox.Show(
                    $"¿Estás seguro de que quieres eliminar a {usuario.NombreUsuario}?",
                    "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _context.Usuarios.Remove(usuario);
                    _context.SaveChanges();
                    CargarUsuarios();
                }
            }
        }
    }
    public class BoolToToggleTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive)
            {
                return isActive ? "DESACTIVAR" : "ACTIVAR";
            }
            return "TOGGLE";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
