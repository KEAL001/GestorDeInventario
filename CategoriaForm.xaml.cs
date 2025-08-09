using GestorDeInventario.Data;
using GestorDeInventario.Models;
using GestorDeInventario.Services;
using GestorDeInventario.Utils;
using System.Windows;

namespace GestorDeInventario
{
    public partial class CategoriaForm : Window
    {
        private int _categoriaId = 0;
        private readonly ApplicationDbContext _context;

        public CategoriaForm()
        {
            InitializeComponent();
            _context = DbContextManager.Instance;
        }

        // Constructor para edición
        public CategoriaForm(Categoria categoria) : this()
        {
            _categoriaId = categoria.ID;
            txtNombreCategoria.Text = categoria.NombreCategoria;
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreCategoria.Text))
            {
                MessageBox.Show("El nombre de la categoría es obligatorio.", "Error de Validación");
                return;
            }

            if (_categoriaId == 0)
            {
                // Lógica para agregar
                var nuevaCategoria = new Categoria { NombreCategoria = txtNombreCategoria.Text.ToUpper() };
                _context.Categorias.Add(nuevaCategoria);
            }
            else
            {
                // Lógica para editar
                var categoriaExistente = _context.Categorias.Find(_categoriaId);
                if (categoriaExistente != null)
                {
                    categoriaExistente.NombreCategoria = txtNombreCategoria.Text.ToUpper();
                }
            }
            _context.SaveChanges();
            DataRefreshService.NotifyDataRefreshed();
            this.Close();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
