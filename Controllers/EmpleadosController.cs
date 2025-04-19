using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using EmpleadosAPI.Data;
using EmpleadosAPI.Models;
using EmpleadosAPI.Services;
using EmpleadosAPI.Dtos;

namespace EmpleadosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;

        public EmpleadosController(ApplicationDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            var usuario = _context.Usuarios
                .Include(u => u.Roles).ThenInclude(ur => ur.Rol)
                .FirstOrDefault(u => u.NombreUsuario == login.NombreUsuario);

            if (usuario == null) return Unauthorized("Usuario no encontrado");
            if (usuario.ContraseñaHash != login.Password) return Unauthorized("Credenciales inválidas");

            var roles = usuario.Roles.Select(r => r.Rol.NombreRol).ToList();
            var token = _authService.GenerateJwtToken(usuario, roles);

            return Ok(new { token });
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Editor,Auditor")]
        public IActionResult GetEmpleados()
        {
            var empleados = _context.Empleados
                .Include(e => e.Usuario)
                    .ThenInclude(u => u.Roles)
                        .ThenInclude(ur => ur.Rol)
                .Where(e => e.EstadoLaboral != "Inactivo")
                .ToList()
                .Select(e => new EmpleadoConRolDto
                {
                    EmpleadoID = e.EmpleadoID,
                    Nombre = e.Nombre,
                    FechaNacimiento = e.FechaNacimiento,
                    Direccion = e.Direccion,
                    Telefono = e.Telefono,
                    CorreoElectronico = e.CorreoElectronico,
                    Puesto = e.Puesto,
                    Departamento = e.Departamento,
                    FechaContratacion = e.FechaContratacion,
                    Salario = e.Salario,
                    EstadoLaboral = e.EstadoLaboral,
                    Rol = e.Usuario?.Roles.FirstOrDefault()?.Rol?.NombreRol
                })
                .ToList();

            return Ok(empleados);
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetEmpleado(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var usuario = _context.Usuarios
                .Include(u => u.Roles)
                .ThenInclude(ur => ur.Rol)
                .FirstOrDefault(u => u.UsuarioID == userId);

            if (usuario == null) return Unauthorized();

            var esAdmin = usuario.Roles?.Any(r => r.Rol?.NombreRol == "Administrador") ?? false;

            if (!esAdmin && usuario.EmpleadoID != id)
                return Forbid("Solo puedes ver tu información.");

            var empleado = _context.Empleados
                .Include(e => e.Usuario)
                    .ThenInclude(u => u.Roles)
                        .ThenInclude(r => r.Rol)
                .FirstOrDefault(e => e.EmpleadoID == id);

            if (empleado == null) return NotFound("Empleado no encontrado");

            return Ok(empleado);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public IActionResult CrearEmpleado([FromBody] EmpleadoRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.Usuario?.Rol))
                return BadRequest("Debes especificar el rol para el nuevo usuario.");

            var rol = _context.Roles.FirstOrDefault(r => r.NombreRol == dto.Usuario.Rol);
            if (rol == null) return BadRequest($"El rol '{dto.Usuario.Rol}' no existe.");

            var usuario = new Usuario
            {
                NombreUsuario = dto.Usuario.NombreUsuario,
                ContraseñaHash = dto.Usuario.ContraseñaHash,
                ContraseñaSalt = "",
                Roles = new List<UsuarioRol>
                {
                    new UsuarioRol { Rol = rol }
                }
            };

            var empleado = new Empleado
            {
                Nombre = dto.Nombre,
                FechaNacimiento = dto.FechaNacimiento,
                Direccion = dto.Direccion,
                Telefono = dto.Telefono,
                CorreoElectronico = dto.CorreoElectronico,
                Puesto = dto.Puesto,
                Departamento = dto.Departamento,
                FechaContratacion = dto.FechaContratacion,
                Salario = dto.Salario,
                EstadoLaboral = dto.EstadoLaboral,
                Usuario = usuario
            };

            _context.Empleados.Add(empleado);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetEmpleado), new { id = empleado.EmpleadoID }, empleado);
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult ActualizarEmpleado(int id, [FromBody] EmpleadoUpdateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim);
            var usuario = _context.Usuarios
                .Include(u => u.Roles).ThenInclude(ur => ur.Rol)
                .FirstOrDefault(u => u.UsuarioID == userId);

            if (usuario == null) return Unauthorized();

            var esAdminOEditor = usuario.Roles?.Any(r =>
                r.Rol != null && (r.Rol.NombreRol == "Administrador" || r.Rol.NombreRol == "Editor")) ?? false;

            if (!esAdminOEditor && usuario.EmpleadoID != id)
                return Forbid("No tienes permiso para modificar este empleado.");

            var empleado = _context.Empleados.FirstOrDefault(e => e.EmpleadoID == id);
            if (empleado == null) return NotFound();

            empleado.Nombre = dto.Nombre;
            empleado.FechaNacimiento = dto.FechaNacimiento;
            empleado.Direccion = dto.Direccion;
            empleado.Telefono = dto.Telefono;
            empleado.CorreoElectronico = dto.CorreoElectronico;
            empleado.Puesto = dto.Puesto;
            empleado.Departamento = dto.Departamento;
            empleado.FechaContratacion = dto.FechaContratacion;
            empleado.Salario = dto.Salario;
            empleado.EstadoLaboral = dto.EstadoLaboral;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public IActionResult EliminarEmpleado(int id)
        {
            var empleado = _context.Empleados.Find(id);
            if (empleado == null) return NotFound();

            empleado.EstadoLaboral = "Inactivo";
            _context.SaveChanges();

            return NoContent();
        }

        [HttpGet("perfil")]
        [Authorize]
        public IActionResult ObtenerPerfil()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var usuario = _context.Usuarios.Include(u => u.Empleado).FirstOrDefault(u => u.UsuarioID == userId);

            if (usuario?.Empleado == null)
                return NotFound("No se encontró el perfil.");

            return Ok(usuario.Empleado);
        }

        [HttpPut("perfil")]
        [Authorize]
        public IActionResult ActualizarPerfil([FromBody] EmpleadoUpdateDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var usuario = _context.Usuarios.Include(u => u.Empleado).FirstOrDefault(u => u.UsuarioID == userId);

            if (usuario?.Empleado == null)
                return NotFound("Empleado no encontrado");

            var empleado = usuario.Empleado;

            empleado.Nombre = dto.Nombre;
            empleado.FechaNacimiento = dto.FechaNacimiento;
            empleado.Direccion = dto.Direccion;
            empleado.Telefono = dto.Telefono;
            empleado.CorreoElectronico = dto.CorreoElectronico;
            empleado.Puesto = dto.Puesto;
            empleado.Departamento = dto.Departamento;
            empleado.FechaContratacion = dto.FechaContratacion;
            empleado.Salario = dto.Salario;
            empleado.EstadoLaboral = dto.EstadoLaboral;

            _context.SaveChanges();
            return NoContent();
        }
    }
}
