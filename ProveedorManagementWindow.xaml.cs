using GestorDeInventario.Data;
using GestorDeInventario.Models;
using GestorDeInventario.Utils;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GestorDeInventario
{
    public partial class ProveedorManagementWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public ProveedorManagementWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CargarProveedores();
        }

        private void CargarProveedores()
        {
            dgProveedores.ItemsSource = _context.Proveedores.ToList();
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            var proveedorForm = new ProveedorForm();
            proveedorForm.ShowDialog();
            CargarProveedores(); // Recarga la lista después de agregar
                
            _context.SaveChanges();
            DataRefreshService.NotifyDataRefreshed();
            

        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgProveedores.SelectedItem is Proveedor proveedorSeleccionado)
            {
                var proveedorForm = new ProveedorForm(proveedorSeleccionado);
                proveedorForm.ShowDialog();
                CargarProveedores(); // Recarga la lista después de editar
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un proveedor para editar.", "Error");
            }
            _context.SaveChanges();
            DataRefreshService.NotifyDataRefreshed();
            
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgProveedores.SelectedItem is Proveedor proveedor)
            {
                MessageBoxResult result = MessageBox.Show($"¿Estás seguro de que quieres eliminar a {proveedor.NombreProveedor}?", "Confirmar Eliminación", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _context.Proveedores.Remove(proveedor);
                    _context.SaveChanges();
                    CargarProveedores(); // Recarga la lista después de eliminar
                }
            }
            _context.SaveChanges();
            DataRefreshService.NotifyDataRefreshed();
            
        }
    }
}