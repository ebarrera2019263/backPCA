using System.Text.Json.Serialization;

namespace EmpleadosAPI.Models
{
    public class Usuario
    {
        public int UsuarioID { get; set; }
        public string NombreUsuario { get; set; }
        public string ContraseñaHash { get; set; }
        public string ContraseñaSalt { get; set; }
        public int EmpleadoID { get; set; }

        [JsonIgnore] // Evita la serialización de la propiedad Empleado para prevenir ciclos.
        public Empleado Empleado { get; set; }

        public ICollection<UsuarioRol> Roles { get; set; }
    }
}
