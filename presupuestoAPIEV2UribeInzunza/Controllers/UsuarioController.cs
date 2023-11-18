
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using presupuestoAPIEV2UribeInzunza.Models;
using presupuestoAPIEV2UribeInzunza.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace presupuestoAPIEV2UribeInzunza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private PresupuestodbEv2Context db = new();
        private IConfiguration _configuration;
        public UsuarioController(IConfiguration config)
        {
            _configuration = config;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] Object login)
        {
            var data = JsonConvert.DeserializeObject<dynamic>(login.ToString());
            var email = "";
            var pass = "";
            if (data.email != null && data.pass != null)
            {
                email = data.email;
                pass = data.pass;
            }

            var user = db.Usuarios
                .Where(x => x.Email == email && x.Pass == pass)
                .FirstOrDefault();

            Resp r = new();
            if (user == null)
            {
                r.Message = "Clave o Email incorrecto";
                return NotFound(r);
            }

            var jwt = _configuration.GetSection("Jwt").Get<Jwt>();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim("IdUsuario", user.IdUsuario.ToString()),
                new Claim("Email", user.Email.ToString()),
                new Claim("IdRol", user.IdRol.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var logeo = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                    jwt.Issuer,
                    jwt.Audience,
                    claims,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: logeo
                );

            r.Message = "Se ha logeado con exito";
            r.Success = true;
            r.Data = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(r);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var token = Jwt.ValidarToken(identity);
            Resp r = new();
            if (!token.Success)
            {
                r.Message = token.Message;
                r.Data = token.Data;
                return Unauthorized(r);
            }

            try
            {
                var usuarios = await db.Usuarios
                    .Join(db.Rols,
                          usuario => usuario.IdRol,
                          rol => rol.IdRol,
                          (usuario, rol) => new
                          {
                              usuario.IdUsuario,
                              rol.Rol1,
                              usuario.Nombre,
                              usuario.Apellido,
                              usuario.Edad,
                              usuario.Direccion,
                              usuario.Email,
                              usuario.Pass
                          })
                    .ToListAsync();

               

                if (usuarios.Any())
                {
                    r.Data = usuarios;
                    r.Success = true;
                    r.Message = "Los datos se cargaron exitosamente";
                    return Ok(r);
                }

                r.Message = "No se encontraron datos";
                r.Success = true;
                return Ok(r);
            }
            catch (Exception ex)
            {
                r.Message = ex.Message;
                return BadRequest(r);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostUsuario(Usuario usuario)
        {

            Resp r = new();

            
            if (usuario.Nombre == "" || usuario.Apellido == "" || usuario.Edad == 0 || usuario.Direccion == "")
            {
                r.Message = "Primero tiene que completar los campos vacios";
                return BadRequest(r);
            }

            bool hasUsers = await db.Usuarios.AnyAsync();
            if (!hasUsers) usuario.IdRol = 1;
            else usuario.IdRol = 2;

            var resUser = new
            {
                usuario.IdUsuario,
                usuario.Nombre,
                usuario.Apellido,
                usuario.Edad,
                usuario.Direccion,
                usuario.Email,
                usuario.Pass,
                usuario.IdRol
            };

            try
            {
                db.Usuarios.Add(usuario);
                await db.SaveChangesAsync();
                r.Message = "Usuario guardado";
                r.Success = true;
                r.Data = usuario.IdUsuario;
                return CreatedAtAction("Get", r, resUser);

            }
            catch (Exception ex) { 
                r.Message = ex.ToString(); 
                return BadRequest(r);
            }
        }

        
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var token = Jwt.ValidarToken(identity);
            Resp r = new();
            if (!token.Success)
            {
                r.Message = token.Message;
                r.Data = token.Data;
                return Unauthorized(r);
            }
            var usuario = await db.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                r.Message = "El usuario que desea eliminar no se encuentra";
                return BadRequest(r);
            }

            db.Usuarios.Remove(usuario);
            await db.SaveChangesAsync();
            r.Success = true;
            r.Message = "Usuario eliminado";
            return Ok(r);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var token = Jwt.ValidarToken(identity);
            Resp r = new();
            if (!token.Success)
            {
                r.Message = token.Message;
                r.Data = token.Data;
                return Unauthorized(r);
            }
            var user = await db.Usuarios.Select(x => new
            {
                id = x.IdUsuario,
                rol = x.IdRol,
                nombre = x.Nombre,
                apellido = x.Apellido,
                edad = x.Edad,
                direccion = x.Direccion
            }).FirstOrDefaultAsync(x => x.id == id);

            if (user == null)
            {
                r.Message = "El usario que desea modificar no se encuentra";
                return BadRequest(r);
            }
            if (id != usuario.IdUsuario)
            {
                r.Message = "El id que ingreso no coincide con el id del usuario que desea modificar";
                return BadRequest(r);
            }

            db.Usuarios.Update(usuario);
            await db.SaveChangesAsync();
            r.Success = true;
            r.Message = "Usuario editado";
            return Ok(r);

        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUsuario(int id)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var token = Jwt.ValidarToken(identity);
            Resp r = new();
            if (!token.Success)
            {
                r.Message = token.Message;
                r.Data = token.Data;
                return Unauthorized(r);
            }

            var usuario = await db.Usuarios.Select(x => new
            {
                id = x.IdUsuario,
                rol = x.IdRol,
                nombre = x.Nombre,
                apellido = x.Apellido,
                edad = x.Edad,
                direccion = x.Direccion,
                x.Email,
                x.Pass
            }).FirstAsync(x => x.id == id);

            if (usuario == null)
            {
                r.Message = $"No se encuentra el usuario de id: {id}";
                return NotFound(r);
            }
            r.Success = true;
            r.Data = usuario;
            return Ok(r);
        }
    }
}
