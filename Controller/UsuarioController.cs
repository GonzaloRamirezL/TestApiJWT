namespace ApiJWT.Controller
{
    using ApiJWT.DB;
    using ApiJWT.ViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using ApiJWT.DTO;

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new { u.Id, u.Nombre, u.Correo })
                .ToListAsync();

            return Ok(usuarios);
        }

        // GET: api/usuarios/correo/gonzalo@mail.com
        [HttpGet("correo/{correo}")]
        public async Task<IActionResult> GetUsuarioPorCorreo(string correo)
        {
            var usuario = await _context.Usuarios
                .Where(u => u.Correo == correo)
                .Select(u => new { u.Id, u.Nombre, u.Correo })
                .FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound("Usuario no encontrado");

            return Ok(usuario);
        }

        // PUT: api/usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarUsuario(int id, [FromBody] UsuarioVM updated)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound("Usuario no encontrado");

            usuario.Nombre = updated.Nombre;
            usuario.Correo = updated.Correo;
            usuario.Password = updated.Password;

            await _context.SaveChangesAsync();
            return Ok("Usuario actualizado");
        }

        // DELETE: api/usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound("Usuario no encontrado");

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok("Usuario eliminado");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Registrar([FromBody] AddUserDTO nuevoUsuario)
        {
            // Validar si el correo ya existe
            var existe = await _context.Usuarios.AnyAsync(u => u.Correo == nuevoUsuario.Correo);
            if (existe)
                return BadRequest("El correo ya está registrado");

            var usuario = new UsuarioVM
            {
                Nombre = nuevoUsuario.Nombre,
                Correo = nuevoUsuario.Correo,
                Password = nuevoUsuario.Password
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok("Usuario registrado exitosamente");
        }
    }

}
