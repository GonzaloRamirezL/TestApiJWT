using ApiJWT.DB;
using ApiJWT.DTO;
using ApiJWT.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _context;

    public AuthController(IConfiguration config, AppDbContext context)
    {
        _config = config;
        _context = context;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDTO credenciales)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u =>
            u.Correo == credenciales.Correo && u.Password == credenciales.Password);

        if (usuario == null)
            return Unauthorized("Credenciales inválidas");

        var token = GenerarToken(usuario);
        return Ok(new { token });
    }


    private string GenerarToken(UsuarioVM usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Correo.ToString()),
            new Claim(ClaimTypes.Name, usuario.Password)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(int.Parse(_config["Jwt:ExpireMinutes"])),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
