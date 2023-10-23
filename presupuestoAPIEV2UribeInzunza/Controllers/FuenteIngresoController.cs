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
    public class FuenteIngresoController : ControllerBase
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
                var fuenteIngreso = await db.FuenteIngresos.Select(x => new
                {
                    x.IdFuente,
                    x.Nombre
                }).ToListAsync();

                if (fuenteIngreso.Any())
                {
                    r.Data = fuenteIngreso;
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
        public async Task<IActionResult> PostFuenteIngreso(FuenteIngreso fuenteIngreso)
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
            if (fuenteIngreso.Nombre == "" || fuenteIngreso.Nombre == null)
            {
                r.Message = "Los campos no pueden quedar vacio";
                return BadRequest(r);
            }

            db.Add(fuenteIngreso);
            await db.SaveChangesAsync();
            r.Message = "Se ha guardado con exito";
            r.Success = true;
            r.Data = fuenteIngreso.IdFuente;
            return CreatedAtAction("Get", r, fuenteIngreso);
        }

        // DELETE
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFuenteIngreso(int  id)
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
            var fIngreso = await db.FuenteIngresos.FindAsync(id);

            if (fIngreso == null)
            {
                r.Message = "La fuente de ingreso no existe";
                return BadRequest(r);
            }

            db.FuenteIngresos.Remove(fIngreso);
            await db.SaveChangesAsync();
            r.Success = true;
            r.Message = "Se ha eliminado con exito";
            return Ok(r);
        }

        // EDIT
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutFuenteIngreso(int id, FuenteIngreso fIngreso)
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
            var fIng = await db.FuenteIngresos.Select(x => new
            {
                x.IdFuente,
                x.Nombre
            }).FirstOrDefaultAsync(x => x.IdFuente == id);

            if (fIng == null)
            {
                r.Message = "Fuente de ingreso no encontrada";
                return BadRequest(r);
            }
            if (id != fIngreso.IdFuente)
            {
                r.Message = "El id ingresado no coincide con el id de la fuente de ingreso que desea modificar";
                return BadRequest(r);
            }

            db.FuenteIngresos.Update(fIngreso);
            await db.SaveChangesAsync();
            r.Success = true;
            r.Message = "Fuente de Ingreso modificada";
            return Ok(r);
        }

        // GET POR ID
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetFuenteIngreso(int id)
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
            var fIngreso = await db.FuenteIngresos.Select(x => new
            {
                x.IdFuente,
                x.Nombre
            }).FirstOrDefaultAsync(x => x.IdFuente== id);

            if (fIngreso == null)
            {
                r.Message = "Fuente de ingreso no encontrada";
                return NotFound(r);
            }

            r.Success = true;
            r.Data = fIngreso;
            return Ok(r);
        }
    }
}
