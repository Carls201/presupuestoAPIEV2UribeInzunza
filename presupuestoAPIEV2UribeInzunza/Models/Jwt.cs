using presupuestoAPIEV2UribeInzunza.Response;
using System.Security.Claims;

namespace presupuestoAPIEV2UribeInzunza.Models
{
    public class Jwt
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }
        public int ExpireDay { get; set; }

        public static dynamic ValidarToken(ClaimsIdentity identity)
        {
            Resp r = new();
            try
            {
                //pregunta si no trae prompiedades
                if (identity.Claims.Count() == 0)
                {
                    r.Message = "Token no es válido.";
                    return r;
                }
                //verifica si existe el IdUser dentro de los claims que trae el token
                var id = identity.Claims.Where(u => u.Type == "IdUsuario").FirstOrDefault().Value;
                PresupuestodbEv2Context db = new();
                //verifica a que usuario pertenece ese token
                var user = db.Usuarios.Find(int.Parse(id));
                r.Success = true;
                r.Message = "Token válido";
                r.Data = user;
                return r;
            }
            catch (Exception ex)
            {
                r.Message = ex.ToString();
                return r;
            }
        }
    }
}
