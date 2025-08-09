using GestorDeInventario.Data;
using GestorDeInventario.Models;
using GestorDeInventario.Services;
using GestorDeInventario.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
                MessageBoxResult result = MessageBox.Show($"¿Estás seguro de que quieres eliminar a {usuario.NombreUsuario}?", "Confirmar Eliminación", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _context.Usuarios.Remove(usuario);
                    _context.SaveChanges();
                    CargarUsuarios();
                }
            }
            _context.SaveChanges();
            DataRefreshService.NotifyDataRefreshed();
        }
    }
}
