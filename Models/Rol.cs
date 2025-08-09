using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GestorDeInventario.Models
{
    public class Rol
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreRol { get; set; }

        [StringLength(255)]
        public string Descripcion { get; set; }
    }
}
