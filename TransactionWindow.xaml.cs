using GestorDeInventario.Data;
using GestorDeInventario.Models;
using GestorDeInventario.Services;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GestorDeInventario
{
    public partial class TransactionWindow : Window
    {
        private readonly InventarioService _inventarioService;
        private readonly ApplicationDbContext _context;
        private readonly int _usuarioId;
        private readonly int _productoId;

        public TransactionWindow(int productoId, InventarioService inventarioService, int usuarioId)
        {
            InitializeComponent();
            _productoId = productoId;
            _inventarioService = inventarioService;
            _usuarioId = usuarioId;
            _context = new ApplicationDbContext();

            lblProductoId.Content = productoId.ToString();
            CargarTiposTransaccion();

            // Según la nueva regla, el costo unitario siempre está deshabilitado
            txtCostoUnitario.IsEnabled = false;
        }

        private void CargarTiposTransaccion()
        {
            cmbTipoTransaccion.Items.Add("Entrada");
            cmbTipoTransaccion.Items.Add("Salida");
            cmbTipoTransaccion.SelectedIndex = 0;
        }

        private void cmbTipoTransaccion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // La lógica para habilitar o deshabilitar txtCostoUnitario ya no es necesaria,
            // ya que siempre está deshabilitado. Solo aseguramos que su valor sea "0".
            txtCostoUnitario.Text = "0";
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Por favor, ingresa una cantidad válida (solo números mayores a 0).", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string tipo = cmbTipoTransaccion.SelectedValue.ToString();
            string comentarios = txtComentarios.Text;
            decimal costoUnitario = 0; // El costo unitario es siempre 0, según la nueva regla de negocio.

            try
            {
                if (tipo == "Entrada")
                {
                    _inventarioService.RegistrarEntrada(_productoId, cantidad, costoUnitario, _usuarioId, comentarios);
                }
                else if (tipo == "Salida")
                {
                    // ¡Corrección importante!
                    // Reincorporamos la validación de stock para evitar inventario negativo.
                    if (!_inventarioService.PuedeRealizarSalida(_productoId, cantidad))
                    {
                        MessageBox.Show("No hay suficiente stock para realizar esta salida.", "Error de Validación", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    _inventarioService.RegistrarSalida(_productoId, cantidad, _usuarioId, comentarios);
                }

                MessageBox.Show("La transacción se registró con éxito en el Kardex.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}