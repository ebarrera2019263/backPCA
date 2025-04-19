namespace EmpleadosAPI.Models
{
    public class UsuarioRol
    {
        public int UsuarioID { get; set; }
        public Usuario Usuario { get; set; }

        public int RolID { get; set; }
        public Rol Rol { get; set; }
    }
}
