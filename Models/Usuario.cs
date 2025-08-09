using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace GestorDeInventario.Models
{
    public class Usuario
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreUsuario { get; set; }

        [Required]
        [StringLength(255)]
        public string ContraseñaHash { get; set; }

        public int RolID { get; set; }
        public bool Activo { get; set; } = true;
        public Rol Rol { get; set; }
    }
}
