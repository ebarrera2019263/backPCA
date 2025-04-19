namespace EmpleadosAPI.Dtos
{
    public class EmpleadoUpdateDto
    {
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
    }
}
