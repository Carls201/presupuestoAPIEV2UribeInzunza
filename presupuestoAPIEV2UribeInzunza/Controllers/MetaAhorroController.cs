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
    public class MetaAhorroController : ControllerBase
    {
        private readonly PresupuestodbEv2Context db = new();

        // GET
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
                var metaAhorro = await db.MetaAhorros.Select(x => new
                {
                    x.IdMeta,
                    x.Nombre
                }).ToListAsync();

                if (metaAhorro.Any())
                {
                    r.Data = metaAhorro;
                    r.Success = true;
                    r.Message = "Los datos se han mostrado con exito";
                    return Ok(r);
                }
                r.Message = "No se han encotrado datos";
                r.Success = true;
                return Ok(r);
            }
            catch (Exception ex)
            {
                r.Message = ex.Message;
                return BadRequest(r);
            }
        }

        // POST
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostMetaAhorro(MetaAhorro metaAhorro)
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
            if (metaAhorro.Nombre == "" || metaAhorro.Nombre == null)
            {
                r.Message = "Los campos no pueden quedar vacio";
                return BadRequest(r);
            }

            db.Add(metaAhorro);
            await db.SaveChangesAsync();
            r.Message = "Se ha guardado con exito";
            r.Success = true;
            r.Data = metaAhorro.IdMeta;
            return CreatedAtAction("Get", r, metaAhorro);
        }

        // DELETE
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteMetaAhorro(int id)
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
            var mAhorro = await db.MetaAhorros.FindAsync(id);

            if (mAhorro == null)
            {
                r.Message = "La meta no existe";
                return BadRequest(r);
            }

            db.MetaAhorros.Remove(mAhorro);
            await db.SaveChangesAsync();
            r.Success = true;
            r.Message = "Se ha eliminado con exito";
            return Ok(r);
        }

        // EDIT
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutMetaAhorro(int id, MetaAhorro mAhorro)
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
            var meta = await db.MetaAhorros.Select(x => new
            {
                x.IdMeta,
                x.Nombre
            }).FirstOrDefaultAsync(x => x.IdMeta== id);

            if (meta == null)
            {
                r.Message = "Meta no encontrada";
                return BadRequest(r);
            }
            if (id != mAhorro.IdMeta)
            {
                r.Message = "El id ingresado no coincide con el id de la meta que desea modificar";
                return BadRequest(r);
            }

            db.MetaAhorros.Update(mAhorro);
            await db.SaveChangesAsync();
            r.Success = true;
            r.Message = "Meta modificada";
            return Ok(r);
        }

        // GET POR ID
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetMetaAhorro(int id)
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
            var metaAhorro = await db.MetaAhorros.Select(x => new
            {
                x.IdMeta,
                x.Nombre
            }).FirstOrDefaultAsync(x => x.IdMeta == id);

            if (metaAhorro == null)
            {
                r.Message = "Meta con encontrada";
                return NotFound(r);
            }

            r.Success = true;
            r.Data = metaAhorro;
            return Ok(r);
        }
    }
}
