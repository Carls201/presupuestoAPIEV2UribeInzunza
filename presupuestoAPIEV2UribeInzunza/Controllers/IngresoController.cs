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
    public class IngresoController : ControllerBase
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
                

                var ingreso = await db.Ingresos
                   .Join(db.FuenteIngresos,
                         ingreso => ingreso.IdFuente,
                         fuente => fuente.IdFuente,
                         (ingreso, fuente) => new
                         {
                             ingreso.IdIngreso,
                             ingreso.IdUsuario,
                             fuente = fuente.Nombre,
                             ingreso.Monto
                         })
                   .ToListAsync();

                if (ingreso.Any())
                {
                    r.Data = ingreso;
                    r.Success = true;
                    r.Message = "Los ingresos se cargaron exitosamente";
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
        public async Task<IActionResult> PostIngreso(Ingreso ingreso)
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
            if (ingreso.IdUsuario <= 0 || ingreso.IdFuente <= 0 || ingreso.Monto <= 0
                || ingreso.IdUsuario == null || ingreso.IdFuente == null || ingreso.Monto == null)
            {
                r.Message = "Completa los campos vacios";
                return BadRequest(r);
            }

            var existeUsuario = await db.Usuarios.FirstOrDefaultAsync(x => x.IdUsuario == ingreso.IdUsuario);
            var existeFuente = await db.FuenteIngresos.FirstOrDefaultAsync(x => x.IdFuente == ingreso.IdFuente);

            if (existeUsuario == null || existeFuente == null)
            {
                r.Message = "La fuente o el usuario ingresado no existe";
                return BadRequest(r);
            }

            var resIngreso = new
            {
                ingreso.IdIngreso,
                ingreso.IdUsuario,
                ingreso.IdFuente,
                ingreso.Monto
            };

            db.Ingresos.Add(ingreso);
            await db.SaveChangesAsync();
            r.Message = "Ingreso guardado";
            r.Success = true;
            r.Data = ingreso.IdIngreso;
            return CreatedAtAction("Get", r, resIngreso);
        }

        // DELETE
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteIngreso(int id)
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
            var ingreso = await db.Ingresos.FindAsync(id);

            if (ingreso == null)
            {
                r.Message = "El ingreso que desea eliminar no se encuentra";
                return BadRequest(r);
            }

            db.Ingresos.Remove(ingreso);
            await db.SaveChangesAsync();
            r.Success = true;
            r.Message = "Ingreso eliminado";
            return Ok(r);
        }

        // EDIT
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutIngreso(int id, Ingreso ingreso)
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
            var ingresox = await db.Ingresos.Select(x => new
            {
                x.IdIngreso,
                x.IdFuente,
                x.IdUsuario,
                x.Monto
            }).FirstOrDefaultAsync(x => x.IdIngreso== ingreso.IdIngreso
            );

            if (ingresox == null)
            {
                r.Message = "El ingreso que desea modificar no se encuentra";
                return BadRequest(r);
            }

            if (id != ingreso.IdIngreso)
            {
                r.Message = "El id ingresado no coincide con el id del ingreso que desea modificar";
                return BadRequest(r);
            }

            db.Ingresos.Update(ingreso);
            await db.SaveChangesAsync();
            r.Success = true;
            r.Message = "El ingreso se ha modificado con exito";
            return Ok(r);
        }

        // GET POR ID
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetIngreso(int id)
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
            var ingreso = await db.Ingresos.Select(x => new
            {
                x.IdIngreso,
                x.IdUsuario,
                x.IdFuente,
                x.Monto
            }).FirstOrDefaultAsync(x => x.IdIngreso == id);

            if (ingreso == null)
            {
                r.Message = $"No se encuentra el ingreso con id: {id}";
                return BadRequest(r);
            }

            r.Data = ingreso;
            r.Success = true;
            return Ok(r);
        }
    }
}
