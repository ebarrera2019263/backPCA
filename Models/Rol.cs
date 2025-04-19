using System.Collections.Generic;

namespace EmpleadosAPI.Models
{
    public class Rol
    {
        public int RolID { get; set; }
        public string NombreRol { get; set; }
        public string Descripcion { get; set; }

        public ICollection<UsuarioRol> UsuarioRoles { get; set; }
    }
}
