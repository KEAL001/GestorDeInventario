using GestorDeInventario.Data;
using GestorDeInventario.Models;
using System.Windows;

namespace GestorDeInventario
{
    public partial class ProveedorForm : Window
    {
        private int _proveedorId = 0;
        private readonly ApplicationDbContext _context;

        public ProveedorForm()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
        }

        // Constructor para el modo de edición
        // Este es el constructor que faltaba y que el botón "Editar" necesita.
        public ProveedorForm(Proveedor proveedor) : this()
        {
            _proveedorId = proveedor.ID;
            txtNombreProveedor.Text = proveedor.NombreProveedor;
            txtContacto.Text = proveedor.Contacto;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreProveedor.Text))
            {
                MessageBox.Show("El nombre del proveedor es obligatorio.", "Error de Validación");
                return;
            }

            if (_proveedorId == 0)
            {
                // Lógica para agregar un nuevo proveedor
                var nuevoProveedor = new Proveedor
                {
                    NombreProveedor = txtNombreProveedor.Text.ToUpper(),
                    Contacto = txtContacto.Text.ToUpper()
                };
                _context.Proveedores.Add(nuevoProveedor);
            }
            else
            {
                // Lógica para editar un proveedor existente
                var proveedorExistente = _context.Proveedores.Find(_proveedorId);
                if (proveedorExistente != null)
                {
                    proveedorExistente.NombreProveedor = txtNombreProveedor.Text.ToUpper();
                    proveedorExistente.Contacto = txtContacto.Text.ToUpper();
                }
            }
            _context.SaveChanges();
            this.Close();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}