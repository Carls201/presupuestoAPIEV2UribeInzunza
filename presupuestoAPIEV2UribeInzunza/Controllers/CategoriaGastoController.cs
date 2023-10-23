
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
    public class CategoriaGastoController : ControllerBase
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
                var catGastos = await db.CategoriaGastos.Select(x => new
                {
                    x.IdCategoria,
                    x.Nombre
                }).ToListAsync();

                if (catGastos.Any())
                {
                    r.Data = catGastos;
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostCategoriaGasto(CategoriaGasto categoriaGasto)
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
            if(categoriaGasto.Nombre == "" || categoriaGasto.Nombre == null)
            {
                r.Message = "Los campos no pueden quedar vacio";
                return BadRequest(r);
            }

            db.Add(categoriaGasto);
            await db.SaveChangesAsync();
            r.Message = "Se ha guardado con exito";
            r.Success = true;
            r.Data = categoriaGasto.IdCategoria;
            return CreatedAtAction("Get", r, categoriaGasto);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategoriaGasto(int id)
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

            var catGasto = await db.CategoriaGastos.FindAsync(id);

            if(catGasto == null)
            {
                r.Message = "La categoria no existe";
                return BadRequest(r);
            }

            db.CategoriaGastos.Remove(catGasto);
            await db.SaveChangesAsync();
            r.Success = true;
            r.Message = "Se ha eliminado con exito";
            return Ok(r);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCategoriaGasto(int id, CategoriaGasto catGasto)
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

            var cat = await db.CategoriaGastos.Select(x => new
            {
                x.IdCategoria,
                x.Nombre
            }).FirstOrDefaultAsync(x => x.IdCategoria == id);

            if(cat == null)
            {
                r.Message = "Categoria no encntrada";
                return BadRequest(r);
            }
            if(id != catGasto.IdCategoria)
            {
                r.Message = "El id ingresado no coincide con el id de la categoria que desea modificar";
                return BadRequest(r);
            }

            db.CategoriaGastos.Update(catGasto);
            await db.SaveChangesAsync();
            r.Success = true;
            r.Message = "Categoria modificada";
            return Ok(r);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetCategoriaGasto(int id)
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

            var catGasto = await db.CategoriaGastos.Select(x => new
            {
                x.IdCategoria,
                x.Nombre
            }).FirstOrDefaultAsync(x => x.IdCategoria == id);

            if(catGasto == null)
            {
                r.Message = "Categoria con encontrada";
                return NotFound(r);
            }

            r.Success = true;
            r.Data = catGasto;
            return Ok(r);
        }

    }
}
