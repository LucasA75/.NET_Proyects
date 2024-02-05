using DocumentFormat.OpenXml.Spreadsheet;
using System.Security.Claims;

namespace ManejadorDePresupuestos.Services
{
    public interface IServicioUsuarios
    {
        int ObtenerUsuarioID();
    }

    public class ServicioUsuarios:IServicioUsuarios
    {
        private readonly HttpContext httpContext;

        public ServicioUsuarios(IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext;
        }

        public int ObtenerUsuarioID()
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                var claims = httpContext.User.Claims.ToList();
                var usuarioIDReal = claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();

                return int.Parse(usuarioIDReal.Value);


            }
            else
            {
                throw new ApplicationException("El usuario no esta autenticado");
            }

            return 1;
        }
    }
}
