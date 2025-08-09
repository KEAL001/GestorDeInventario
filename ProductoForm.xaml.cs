using GestorDeInventario.Data;
using GestorDeInventario.Models;
using GestorDeInventario.Utils;
using System;
using System.Linq;
using System.Windows;
using GestorDeInventario.Services;

namespace GestorDeInventario
{
    public partial class ProductoForm : Window
    {
        private int _productoId = 0;
        private readonly ApplicationDbContext _context;

        public ProductoForm()
        {
            InitializeComponent();
            _context = DbContextManager.Instance; // Usar contexto singleton  
            CargarDatos();
        }

        public ProductoForm(Producto producto) : this()
        {
            _productoId = producto.ID;
            txtNombre.Text = producto.Nombre;
            txtDescripcion.Text = producto.Descripcion;
            txtStockMinimo.Text = producto.StockMinimo.ToString();
            cmbCategoria.SelectedValue = producto.CategoriaID;
            cmbProveedor.SelectedValue = producto.ProveedorID;
        }

        // AGREGA este nuevo constructor para recibir contexto  
        public ProductoForm(Producto producto, ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context; // Usar el contexto pasado  
            CargarDatos();

            _productoId = producto.ID;
            txtNombre.Text = producto.Nombre;
            txtDescripcion.Text = producto.Descripcion;
            txtStockMinimo.Text = producto.StockMinimo.ToString();
            cmbCategoria.SelectedValue = producto.CategoriaID;
            cmbProveedor.SelectedValue = producto.ProveedorID;
        }

        private void CargarDatos()
        {
            cmbCategoria.ItemsSource = _context.Categorias.ToList();
            cmbProveedor.ItemsSource = _context.Proveedores.ToList();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || cmbCategoria.SelectedValue == null || cmbProveedor.SelectedValue == null)
            {
                MessageBox.Show("Por favor, completa todos los campos.", "Error de Validación");
                return;
            }

            if (!int.TryParse(txtStockMinimo.Text, out int stockMinimo) || stockMinimo < 0)
            {
                MessageBox.Show("Por favor, ingresa un valor de stock mínimo válido.", "Error de Validación");
                return;
            }

            if (_productoId == 0)
            {
                // Lógica para agregar un nuevo producto
                var categoria = _context.Categorias.Find(cmbCategoria.SelectedValue);

                var nuevoProducto = new Producto
                {
                    // Genera un SKU basado en el nombre del producto, la categoría y un ID único
                    SKU = GenerarSKU(txtNombre.Text, categoria.NombreCategoria),
                    Nombre = txtNombre.Text.ToUpper(),
                    Descripcion = txtDescripcion.Text.ToUpper(),
                    CategoriaID = (int)cmbCategoria.SelectedValue,
                    ProveedorID = (int)cmbProveedor.SelectedValue,
                    StockMinimo = stockMinimo
                };
                _context.Productos.Add(nuevoProducto);
            }
            else
            {
                // Lógica para editar un producto existente
                var productoExistente = _context.Productos.Find(_productoId);
                if (productoExistente != null)
                {
                    var categoria = _context.Categorias.Find(cmbCategoria.SelectedValue);
                    productoExistente.SKU = GenerarSKU(txtNombre.Text, categoria.NombreCategoria);
                    productoExistente.Nombre = txtNombre.Text.ToUpper();
                    productoExistente.Descripcion = txtDescripcion.Text.ToUpper();
                    productoExistente.CategoriaID = (int)cmbCategoria.SelectedValue;
                    productoExistente.ProveedorID = (int)cmbProveedor.SelectedValue;
                    productoExistente.StockMinimo = stockMinimo;

                }
            }
            DataRefreshService.NotifyDataRefreshed();
            _context.SaveChanges();
            this.Close();
        }


        // Método para generar un SKU único
        private string GenerarSKU(string nombreProducto, string categoria)
        {
            //LA LOGICA DE GENERACION DEL SKU SE BASA EN EL NOMBRE DEL PRODUCTO, CATEGORIA Y DESCRIPCION
            string productoParte = new string(nombreProducto.Take(4).ToArray()).ToUpper();
            string categoriaParte = new string(categoria.Take(4).ToArray()).ToUpper();
            string Descripcionparte = new string(txtDescripcion.Text.Take(8).ToArray()).ToUpper();

            // Genera una cadena aleatoria de 4 caracteres alfanuméricos
           /*const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var randomParte = new string(Enumerable.Repeat(chars, 4)
              .Select(s => s[random.Next(s.Length)]).ToArray());*/

            return $"{productoParte}-{categoriaParte}-{Descripcionparte}";
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}