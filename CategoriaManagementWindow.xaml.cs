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
    public partial class CategoriaManagementWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public CategoriaManagementWindow()
        {
            InitializeComponent();
            _context = DbContextManager.Instance;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CargarCategorias();
        }

        private void CargarCategorias()
        {
            dgCategorias.ItemsSource = _context.Categorias.ToList();
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            var categoriaForm = new CategoriaForm();
            categoriaForm.ShowDialog();
            CargarCategorias();
            _context.SaveChanges();
            DataRefreshService.NotifyDataRefreshed();
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgCategorias.SelectedItem is Categoria categoriaSeleccionada)
            {
                var categoriaForm = new CategoriaForm(categoriaSeleccionada);
                categoriaForm.ShowDialog();
                CargarCategorias();
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una categoría para editar.", "Error");
            }
            _context.SaveChanges();
            DataRefreshService.NotifyDataRefreshed();
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgCategorias.SelectedItem is Categoria categoria)
            {
                // Verificar si la categoría tiene productos con transacciones de kardex  
                var tieneTransacciones = _context.TransaccionesInventario
                    .Include(t => t.Producto)
                    .Any(t => t.Producto.CategoriaID == categoria.ID);

                if (tieneTransacciones)
                {
                    MessageBox.Show($"No se puede eliminar la categoría '{categoria.NombreCategoria}' porque tiene productos con transacciones de kardex registradas.",
                                  "Eliminación no permitida", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBoxResult result = MessageBox.Show($"¿Estás seguro de que quieres eliminar la categoría '{categoria.NombreCategoria}'?",
                                                        "Confirmar Eliminación", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _context.Categorias.Remove(categoria);
                    _context.SaveChanges();
                    CargarCategorias();
                }
            }
        }

    }
}
