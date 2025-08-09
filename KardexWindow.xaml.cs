using GestorDeInventario.Data;
using GestorDeInventario.Models;
using GestorDeInventario.Services;
using GestorDeInventario.Utils;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GestorDeInventario
{
    public partial class KardexWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public KardexWindow()
        {
            InitializeComponent();
            _context = DbContextManager.Instance;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // La ventana se carga con campos vacíos, a la espera de una búsqueda.
        }

        private void btnBuscarProducto_Click(object sender, RoutedEventArgs e)
        {
            string busqueda = txtBuscarProducto.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(busqueda))
            {
                MessageBox.Show("Por favor, ingresa un criterio de búsqueda.", "Advertencia");
                return;
            }

            // Realiza la búsqueda de productos por nombre o descripción.
            var resultados = _context.Productos
                                     .Where(p => p.Nombre.ToLower().Contains(busqueda) ||
                                                 p.Descripcion.ToLower().Contains(busqueda))
                                     .ToList();

            if (resultados.Any())
            {
                lvResultadosBusqueda.ItemsSource = resultados;
            }
            else
            {
                lvResultadosBusqueda.ItemsSource = null;
                MessageBox.Show("No se encontraron productos que coincidan con la búsqueda.", "Información");
            }

            // Limpiar los detalles y el kardex de la selección anterior.
            lblSKU.Text = string.Empty;
            lblDescripcion.Text = string.Empty;
            dgKardex.ItemsSource = null;
        }

        private void lvResultadosBusqueda_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvResultadosBusqueda.SelectedItem is Producto productoSeleccionado)
            {
                // Muestra los detalles del producto seleccionado
                lblSKU.Text = productoSeleccionado.SKU;
                lblDescripcion.Text = productoSeleccionado.Descripcion;

                // Carga el kardex del producto seleccionado
                CargarKardex(productoSeleccionado.ID);
            }
        }

        private void CargarKardex(int productoId)
        {
            var transacciones = _context.TransaccionesInventario
                .Include(t => t.Usuario)
                .Where(t => t.ProductoID == productoId)
                .OrderBy(t => t.FechaTransaccion)
                .ToList();

            dgKardex.ItemsSource = transacciones;
        }

        private void btnRegistrarTransaccion_Click(object sender, RoutedEventArgs e)
        {
            if (lvResultadosBusqueda.SelectedItem is Producto productoSeleccionado)
            {
                if (SessionManager.UsuarioActual == null)
                {
                    MessageBox.Show("No hay un usuario logeado. Inicie sesión para continuar.");
                    return;
                }

                using (var context = new ApplicationDbContext())
                {
                    var inventarioService = new InventarioService(context);
                    var transactionWindow = new TransactionWindow(productoSeleccionado.ID, inventarioService, SessionManager.UsuarioActual.ID);
                    transactionWindow.ShowDialog();

                    // Recarga el kardex después de registrar una transacción
                    CargarKardex(productoSeleccionado.ID);
                    
                }
                DataRefreshService.NotifyDataRefreshed();
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un producto de la lista de resultados.");
            }
            DataRefreshService.NotifyDataRefreshed();
        }
        private void txtBuscarProducto_KeyDown(object sender, KeyEventArgs e)
        {
            // Si la tecla presionada es Enter
            if (e.Key == Key.Enter)
            {
                // Llamamos al método de inicio de sesión
                btnBuscarProducto_Click(sender, e);
            }
        }
    }
}