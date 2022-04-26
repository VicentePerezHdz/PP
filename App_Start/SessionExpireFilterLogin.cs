using Entidades.Negocio.Login;
using System.Web;
using System.Web.Mvc;

namespace Consumir_Aplicacion.App_Start
{
    public class SessionExpireFilterLogin : ActionFilterAttribute
    {
        /// <summary>
        /// Funcion para verificar si la session esta activa cada vez que cambian de ventana se debe de agregar a cada metodo 
        /// del controlador la siguiente etiqueta [SessionExpireFilterLogin]
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            // check if session is supported
            if (ctx.Session != null)
            {
                //Se cambia por el objeto que se va a crear cuando pasan el login

                OUsuario usuarioActivo = (OUsuario)ctx.Session["usuario"];
                if (usuarioActivo == null)
                {
                    var requestContext = HttpContext.Current.Request.RequestContext;
                    RedirectResult rr = new RedirectResult(new UrlHelper(requestContext).Action("CerrarSesion", "Login"));
                    filterContext.Result = rr;
                }


            }
            base.OnActionExecuting(filterContext);
        }
    }
}