
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using presupuestoAPIEV2UribeInzunza.Models;
using presupuestoAPIEV2UribeInzunza.Response;
using System.Security.Claims;

namespace presupuestoAPIEV2UribeInzunza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolController : ControllerBase
    {
        private readonly PresupuestodbEv2Context db = new();

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
                var rols = await db.Rols.Select(a => new
                {
                    id = a.IdRol,
                    rol = a.Rol1
                }).ToListAsync();

                if (rols.Any())
                {
                    r.Data = rols;
                    r.Success = true;
                    r.Message = "Los datos se han mostrado con exito";
                    return Ok(r);
                }

                r.Message = "No existen registros";
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
        [Authorize]
        public async Task<IActionResult> PostRol(Rol rol)
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

            if (rol.Rol1 == null || rol.Rol1 == "")
            {
                r.Message = "Los campos no pueden quedar vacio";
                return BadRequest(r);
            }

            db.Rols.Add(rol);
            await db.SaveChangesAsync();
            r.Message = "Se ha guardado con exito";
            r.Success = true;
            r.Data = rol.Rol1;
            return CreatedAtAction("Get", r, rol);

        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRol(int id)
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

            var rol = await db.Rols.FindAsync(id);

            if (rol == null)
            {
                r.Message = "No existe el 'rol";
                return BadRequest(r);
            }

            db.Rols.Remove(rol);
            await db.SaveChangesAsync();
            r.Success = true;
            r.Message = "El Rol se ha eliminado con exito";
            return Ok(r);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutRol(int id, Rol rol)
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

            var rolx = await db.Rols.Select(r => new
            {
                id = r.IdRol,
                rol = r.Rol1
            }).FirstOrDefaultAsync(x => x.id == id);

            if (rolx == null)
            {
                r.Message = "El rol que desea modificar no se encuentra";
                return BadRequest(r);
            }
            if (id != rol.IdRol)
            {
                r.Message = "El id que ingreso no coincide con el id del rol que desea modificar";
                return BadRequest(r);
            }

            db.Rols.Update(rol);
            await db.SaveChangesAsync();
            r.Success = true;
            r.Message = "Rol editado con exito";
            return Ok(r);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetRol(int id)
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

            var rol = await db.Rols.Select(r => new
            {
                id = r.IdRol,
                rol = r.Rol1
            }).FirstOrDefaultAsync(x => x.id == id);

            if (rol == null)
            {
                r.Message = $"No se encuentra el rol con id: {id}";
                return NotFound(r);
            }
            r.Success = true;
            r.Data = rol;
            return Ok(r);
        }
    }
}
