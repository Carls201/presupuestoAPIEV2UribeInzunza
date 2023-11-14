
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using presupuestoAPIEV2UribeInzunza.Models;
using presupuestoAPIEV2UribeInzunza.Response;
using System.Security.Claims;

namespace presupuestoAPIEV2UribeInzunza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GastoController : ControllerBase
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
                //var gasto = await db.Gastos.Select(x => new
                //{
                //    x.IdGasto,
                //    x.IdUsuario,
                //    x.IdCategoria,
                //    x.Monto
                //}).ToListAsync();

                var gasto = await db.Gastos
                   .Join(db.CategoriaGastos,
                         gasto => gasto.IdCategoria,
                         categoria => categoria.IdCategoria,
                         (gasto, categoria) => new
                         {
                             gasto.IdGasto,
                             gasto.IdUsuario,
                             categoria = categoria.Nombre,
                             gasto.Monto
                         })
                   .ToListAsync();

                if (gasto.Any())
                {
                    r.Data = gasto;
                    r.Success = true;
                    r.Message = "Los datos se cargaron exitosaente";
                    return Ok(r);
                }

                r.Message = "No se encuentran datos";
                r.Success = true;
                return Ok(r);

            }catch (Exception ex)
            {
                r.Message = ex.Message;
                return BadRequest(r);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostGasto(Gasto gasto)
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

            if (gasto.IdUsuario <= 0 || gasto.IdCategoria <= 0 || gasto.Monto <= 0
                || gasto.IdUsuario == null || gasto.IdCategoria == null || gasto.Monto == null)
            {
                r.Message = "Primero tiene que completar los campos vacios";
                return BadRequest(r);
            }
            
            var existeUsuario = await db.Usuarios.FirstOrDefaultAsync(x => x.IdUsuario == gasto.IdUsuario);
            var existeCategoria = await db.CategoriaGastos.FirstOrDefaultAsync(x => x.IdCategoria == gasto.IdCategoria);

            if (existeUsuario == null || existeCategoria == null)
            {
                r.Message = "La categoria o el usario ingresado no existe";
                return BadRequest(r);
            }

            var resGasto = new
            {
                gasto.IdGasto,
                gasto.IdUsuario,
                gasto.IdCategoria,
                gasto.Monto
            };

            db.Gastos.Add(gasto);
            await db.SaveChangesAsync();
            r.Message = "Gasto guardado";
            r.Success = true;
            r.Data = gasto.IdGasto;
            return CreatedAtAction("Get", r, resGasto);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteGasto(int id)
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

            var gasto = await db.Gastos.FindAsync(id);

            if(gasto == null)
            {
                r.Message = "El gasto que desea eliminar no se encuentra";
                return BadRequest(r);
            }

            db.Gastos.Remove(gasto);
            await db.SaveChangesAsync();
            r.Success = true;
            r.Message = "Gasto eliminado";
            return Ok(r);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutGasto(int id, Gasto gasto)
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

            var gastox = await db.Gastos.Select(x => new
            {
                x.IdGasto,
                x.IdUsuario,
                x.IdCategoria,
                x.Monto
            }).FirstOrDefaultAsync(x => x.IdGasto == gasto.IdGasto);

            if(gastox == null)
            {
                r.Message = "El gasto que desea eliminar no se encuentra";
                return BadRequest(r);
            }

            if(id != gasto.IdGasto)
            {
                r.Message = "El id ingresado no coincide con el id del gasto que desea eliminar";
                return BadRequest(r);
            }

            db.Gastos.Update(gasto);
            await db.SaveChangesAsync();
            r.Success = true;
            r.Message = "El gasto se ha modificado con exito";
            return Ok(r);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetGasto(int id)
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

            var gasto = await db.Gastos.Select(x => new
            {
                x.IdGasto,
                x.IdUsuario,
                x.IdCategoria,
                x.Monto
            }).FirstOrDefaultAsync(x => x.IdGasto == id);

            if(gasto == null)
            {
                r.Message = $"No se encuentra el gasto con id: {id}";
                return BadRequest(r);
            }

            r.Data = gasto;
            r.Success = true;
            return Ok(r);
        }
    }
}
