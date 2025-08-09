using GestorDeInventario.Data;
using GestorDeInventario.Models;
using System.Linq;
using System.Windows;
using BCrypt.Net;

namespace GestorDeInventario
{
    public partial class UserForm : Window
    {
        private int _usuarioId = 0;
        private readonly ApplicationDbContext _context;

        // Constructor para agregar un nuevo usuario (vacío)
        public UserForm()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            CargarRolesEnComboBox();
        }

        // Constructor para editar un usuario existente (recibe el objeto Usuario)
        public UserForm(Usuario usuario) : this()
        {
            _usuarioId = usuario.ID;
            txtNombreUsuario.Text = usuario.NombreUsuario;
            cmbRol.SelectedValue = usuario.RolID;
            // La contraseña no se carga por seguridad
        }

        private void CargarRolesEnComboBox()
        {
            cmbRol.ItemsSource = _context.Roles.ToList();
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text) || cmbRol.SelectedValue == null)
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error de Validación");
                return;
            }

            if (_usuarioId == 0 && string.IsNullOrWhiteSpace(txtContraseña.Password))
            {
                MessageBox.Show("La contraseña es obligatoria para un nuevo usuario.", "Error de Validación");
                return;
            }

            if (txtContraseña.Password != txtConfirmarContraseña.Password)
            {
                MessageBox.Show("Las contraseñas no coinciden.", "Error de Validación");
                return;
            }

            if (_usuarioId == 0)
            {
                // Lógica para agregar nuevo usuario
                var nuevoUsuario = new Usuario
                {
                    NombreUsuario = txtNombreUsuario.Text,
                    ContraseñaHash = BCrypt.Net.BCrypt.HashPassword(txtContraseña.Password),
                    RolID = (int)cmbRol.SelectedValue,
                    Activo = true
                };
                _context.Usuarios.Add(nuevoUsuario);
            }
            else
            {
                // Lógica para editar usuario
                var usuarioExistente = _context.Usuarios.Find(_usuarioId);
                if (usuarioExistente != null)
                {
                    usuarioExistente.NombreUsuario = txtNombreUsuario.Text;
                    usuarioExistente.RolID = (int)cmbRol.SelectedValue;

                    if (!string.IsNullOrWhiteSpace(txtContraseña.Password))
                    {
                        usuarioExistente.ContraseñaHash = BCrypt.Net.BCrypt.HashPassword(txtContraseña.Password);
                    }
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
