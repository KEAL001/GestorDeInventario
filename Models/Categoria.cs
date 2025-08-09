using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GestorDeInventario.Models
{
    public class Categoria
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreCategoria { get; set; }

    }
}
