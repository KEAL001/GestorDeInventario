using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorDeInventario.Models
{
    public class Producto
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(255)]
        public string Nombre { get; set; }

        [StringLength(255)]
        public string Descripcion { get; set; }

        [Required]
        [StringLength(50)]
        public string SKU { get; set; }

        public decimal PrecioVenta { get; set; }
        public decimal CostoCompra { get; set; }
        public int StockMinimo { get; set; }

        // Definimos las claves foráneas con las anotaciones
        public int CategoriaID { get; set; }
        [ForeignKey("CategoriaID")]
        public Categoria Categoria { get; set; }

        public int ProveedorID { get; set; }
        [ForeignKey("ProveedorID")]
        public Proveedor Proveedor { get; set; }
    }
}