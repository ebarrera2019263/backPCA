using System.Text.Json.Serialization;

namespace EmpleadosAPI.Models
{
    public class Empleado
    {
        public int EmpleadoID { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string CorreoElectronico { get; set; }
        public string Puesto { get; set; }
        public string Departamento { get; set; }
        public DateTime FechaContratacion { get; set; }
        public decimal Salario { get; set; }
        public string EstadoLaboral { get; set; }

        [JsonIgnore] // Evita la serialización de la propiedad Usuario para prevenir ciclos.
        public Usuario Usuario { get; set; }
    }
}
