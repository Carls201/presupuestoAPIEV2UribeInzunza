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
    public class AhorroController : ControllerBase
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
                //var ahorro = await db.Ahorros.Select(x => new
                //{
                //    x.IdAhorro,
                //    x.IdUsuario,
                //    x.IdMeta,
                //    x.Monto
                //}).ToListAsync();

                var ahorro = await db.Ahorros
                   .Join(db.MetaAhorros,
                         ahorro => ahorro.IdMeta,
                         meta => meta.IdMeta,
                         (ahorro, meta) => new
                         {
                             ahorro.IdAhorro,
                             ahorro.IdUsuario,
                             meta = meta.Nombre,
                             ahorro.Monto
                         })
                   .ToListAsync();

                if (ahorro.Any())
                {
                    r.Data = ahorro;
                    r.Success = true;
                    r.Message = "Los ahorros se cargaron exitosamente";
                    return Ok(r);
                }

                r.Message = "No se encuentran datos";
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
        public async Task<IActionResult> PostAhorro(Ahorro ahorro)
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
            if (ahorro.IdUsuario <= 0 || ahorro.IdMeta <= 0 || ahorro.Monto <= 0
                || ahorro.IdUsuario == null || ahorro.IdMeta== null || ahorro.Monto == null)
            {
                r.Message = "Completa los campos vacios";
                return BadRequest(r);
            }

            var existeUsuario = await db.Usuarios.FirstOrDefaultAsync(x => x.IdUsuario == ahorro.IdUsuario);
            var existeMeta = await db.MetaAhorros.FirstOrDefaultAsync(x => x.IdMeta == ahorro.IdMeta);

            if (existeUsuario == null || existeMeta == null)
            {
                r.Message = "La meta o el usario ingresado no existe";
                return BadRequest(r);
            }

            var resAhorro = new
            {
                ahorro.IdAhorro,
                ahorro.IdUsuario,
                ahorro.IdMeta,
                ahorro.Monto
            };

            db.Ahorros.Add(ahorro);
            await db.SaveChangesAsync();
            r.Message = "Gasto guardado";
            r.Success = true;
            r.Data = ahorro.IdAhorro;
            return CreatedAtAction("Get", r, resAhorro);
        }

        // DELETE
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAhorro(int id)
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
            var ahorro = await db.Ahorros.FindAsync(id);

            if (ahorro == null)
            {
                r.Message = "El ahorro que desea eliminar no se encuentra";
                return BadRequest(r);
            }

            db.Ahorros.Remove(ahorro);
            await db.SaveChangesAsync();
            r.Success = true;
            r.Message = "Ahorro eliminado";
            return Ok(r);
        }

        // EDIT
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutAhorro(int id, Ahorro ahorro)
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
            var ahorrox = await db.Ahorros.Select(x => new
            {
                x.IdAhorro,
                x.IdUsuario,
                x.IdMeta,
                x.Monto
            }).FirstOrDefaultAsync(x => x.IdAhorro == ahorro.IdAhorro
            );

            if (ahorrox == null)
            {
                r.Message = "El ahorro que desea eliminar no se encuentra";
                return BadRequest(r);
            }

            if (id != ahorro.IdAhorro)
            {
                r.Message = "El id ingresado no coincide con el id del ahorro que desea eliminar";
                return BadRequest(r);
            }

            db.Ahorros.Update(ahorro);
            await db.SaveChangesAsync();
            r.Success = true;
            r.Message = "El ahorro se ha modificado con exito";
            return Ok(r);
        }

        // GET POR ID
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetAhorro(int id)
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
            var ahorro = await db.Ahorros.Select(x => new
            {
                x.IdAhorro,
                x.IdUsuario,
                x.IdMeta,
                x.Monto
            }).FirstOrDefaultAsync(x => x.IdAhorro== id);

            if (ahorro == null)
            {
                r.Message = $"No se encuentra el ahorro con id: {id}";
                return BadRequest(r);
            }

            r.Data = ahorro;
            r.Success = true;
            return Ok(r);
        }
    }
}
