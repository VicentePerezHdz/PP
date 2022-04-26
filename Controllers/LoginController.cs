using Entidades.General;
using Entidades.Negocio.Estilos;
using Entidades.Negocio.Login;
using Negocio.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Consumir_Aplicacion.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            OEstilos estilo = new OEstilos();
            //estilo.button = "btn-success";
            estilo.master = "~/Views/Shared/_LayoutLogin.cshtml";
            //estilo.master = "~/Views/Shared/_Layout.cshtml";
            return View(estilo);
        }


        public JsonResult lsValidar(OUsuario pmtPeticion)
        {
            WsLogin _ws = new WsLogin();
            ORespuesta<OUsuario> _respuesta = _ws.ValidarUsuario(pmtPeticion);
            if (_respuesta.Exitoso)
            {
                HttpContext.Session["usuario"] = pmtPeticion;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CerrarSesion()
        {
            Session.Abandon();
            Session.RemoveAll();
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }

    }
}
