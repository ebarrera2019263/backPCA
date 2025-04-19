using System.Text.Json.Serialization;

namespace EmpleadosAPI.Models
{
    public class Usuario
    {
        public int UsuarioID { get; set; }
        public string NombreUsuario { get; set; }
        public string Contrase�aHash { get; set; }
        public string Contrase�aSalt { get; set; }
        public int EmpleadoID { get; set; }

        [JsonIgnore] // Evita la serializaci�n de la propiedad Empleado para prevenir ciclos.
        public Empleado Empleado { get; set; }

        public ICollection<UsuarioRol> Roles { get; set; }
    }
}
