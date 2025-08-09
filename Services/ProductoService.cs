using GestorDeInventario.Data;
using GestorDeInventario.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorDeInventario.Services
{
    public class ProductoService
    {
        private readonly ApplicationDbContext _context;

        public ProductoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Producto> ObtenerTodosLosProductos()
        {
            // Retorna una lista de todos los productos de la base de datos
            return _context.Productos.ToList();
        }
    }
}
