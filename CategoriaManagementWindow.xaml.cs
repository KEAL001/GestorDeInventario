using GestorDeInventario.Data;
using GestorDeInventario.Models;
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
            _context = new ApplicationDbContext();
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
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgCategorias.SelectedItem is Categoria categoria)
            {
                MessageBoxResult result = MessageBox.Show($"¿Estás seguro de que quieres eliminar la categoría '{categoria.NombreCategoria}'?", "Confirmar Eliminación", MessageBoxButton.YesNo);
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
