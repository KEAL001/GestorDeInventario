using GestorDeInventario.Data;
using GestorDeInventario.Models;
using GestorDeInventario.Services;
using GestorDeInventario.Utils;
using GestorDeInventario.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GestorDeInventario
{
    public partial class MainWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private ObservableCollection<ProductoViewModel> _productosObservable;

        public MainWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _context.Database.EnsureCreated();
            _productosObservable = new ObservableCollection<ProductoViewModel>();
            dgProductos.ItemsSource = _productosObservable;

            // Suscribimos la ventana al servicio de notificación
            DataRefreshService.DataRefreshed += (sender, e) => CargarProductos();

        }
        protected override void OnClosed(EventArgs e)
        {
            DataRefreshService.DataRefreshed -= (sender, e) => CargarProductos();
            _context.Dispose();
            base.OnClosed(e);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CargarProductos();
            if (SessionManager.UsuarioActual != null && SessionManager.UsuarioActual.Rol.NombreRol == "Admin")
            {
                btnGestionUsuarios.Visibility = Visibility.Visible;
            }
        }

        private void CargarProductos()
        {
            _productosObservable.Clear();

            var inventarioService = new InventarioService(_context);
            var productos = _context.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Proveedor)
                .ToList();

            foreach (var p in productos)
            {
                _productosObservable.Add(new ProductoViewModel
                {
                    ID = p.ID,
                    Nombre = p.Nombre,
                    SKU = p.SKU,
                    PrecioVenta = p.PrecioVenta,
                    CostoCompra = p.CostoCompra,
                    StockMinimo = p.StockMinimo,
                    Categoria = p.Categoria?.NombreCategoria,
                    Proveedor = p.Proveedor?.NombreProveedor,
                    StockActual = inventarioService.ObtenerStockActual(p.ID)
                });
            }
        }

        private void btnAgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            var productoForm = new ProductoForm();
            if (productoForm.ShowDialog() == true)
            {
                // Al agregar un nuevo producto, recargamos la lista para asegurarnos de que la vista esté actualizada.
                // Esto también refresca el stock actual y otras propiedades.
                CargarProductos();
            }
            _context.SaveChanges();
            DataRefreshService.NotifyDataRefreshed();
            
        }

        // En MainWindow.xaml.cs

        private void btnEditarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (dgProductos.SelectedItem is ProductoViewModel productoSeleccionado)
            {
                var productoReal = _context.Productos.Find(productoSeleccionado.ID);
                if (productoReal != null)
                {
                    var productoForm = new ProductoForm(productoReal);
                    if (productoForm.ShowDialog() == true)
                    {
                        productoSeleccionado.Nombre = productoReal.Nombre;
                        productoSeleccionado.SKU = productoReal.SKU;
                        productoSeleccionado.PrecioVenta = productoReal.PrecioVenta;
                        productoSeleccionado.CostoCompra = productoReal.CostoCompra;
                        productoSeleccionado.StockMinimo = productoReal.StockMinimo;

                        // Aquí usamos CategoriaID y ProveedorID para que coincidan con tu clase Producto
                        productoSeleccionado.Categoria = _context.Categorias.Find(productoReal.CategoriaID)?.NombreCategoria;
                        productoSeleccionado.Proveedor = _context.Proveedores.Find(productoReal.ProveedorID)?.NombreProveedor;
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un producto para editar.", "Error");
            }
            _context.SaveChanges();
            DataRefreshService.NotifyDataRefreshed();
            
        }

        private void btnEliminarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (dgProductos.SelectedItem is ProductoViewModel productoSeleccionado)
            {
                MessageBoxResult result = MessageBox.Show($"¿Estás seguro de que quieres eliminar el producto '{productoSeleccionado.Nombre}'?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var productoAEliminar = _context.Productos.Find(productoSeleccionado.ID);
                        if (productoAEliminar != null)
                        {
                            _context.Productos.Remove(productoAEliminar);
                            _context.SaveChanges();

                            // Eliminamos el objeto de la ObservableCollection, lo que actualiza el DataGrid
                            _productosObservable.Remove(productoSeleccionado);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ocurrió un error al eliminar el producto: {ex.Message}", "Error");
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un producto para eliminar.", "Error");
            }
            
        }

        private void dgProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgProductos.SelectedItem != null)
            {
                btnEditarProducto.IsEnabled = true;
                btnEliminarProducto.IsEnabled = true;
            }
            else
            {
                btnEditarProducto.IsEnabled = false;
                btnEliminarProducto.IsEnabled = false;
            }
        }

        // --- MÉTODOS DE NAVEGACIÓN ---
        private void btnGestionProveedores_Click(object sender, RoutedEventArgs e)
        {
            var proveedoresWindow = new ProveedorManagementWindow();
            proveedoresWindow.Show();
        }

        private void btnGestionCategorias_Click(object sender, RoutedEventArgs e)
        {
            var categoriasWindow = new CategoriaManagementWindow();
            categoriasWindow.Show();
        }

        private void btnKardex_Click(object sender, RoutedEventArgs e)
        {
            var kardexWindow = new KardexWindow();
            kardexWindow.Show();
        }

        private void btnReportes_Click(object sender, RoutedEventArgs e)
        {
            var reportesWindow = new ReportesWindow();
            reportesWindow.Show();
        }

        private void btnGestionUsuarios_Click(object sender, RoutedEventArgs e)
        {
            var userManagementWindow = new UserManagementWindow();
            userManagementWindow.Show();
        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.CerrarSesion();
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}