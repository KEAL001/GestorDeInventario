using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorDeInventario.Models
{
    public class TransaccionInventario
    {
        [Key]
        public int ID { get; set; }

        public int ProductoID { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoTransaccion { get; set; }

        public int Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }

        public DateTime FechaTransaccion { get; set; } = DateTime.Now;

        public int UsuarioID { get; set; }

        [StringLength(255)]
        public string Comentarios { get; set; }

        public Producto Producto { get; set; }
        public Usuario Usuario { get; set; }
    }
}
