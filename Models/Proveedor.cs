using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorDeInventario.Models
{
    public class Proveedor
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(255)]
        public string NombreProveedor { get; set; }

        [StringLength(255)]
        public string Contacto { get; set; }
    }
    
}
