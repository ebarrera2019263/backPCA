namespace EmpleadosAPI.Dtos
{
    public class EmpleadoConRolDto
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
        public string Rol { get; set; } 
    }
}
