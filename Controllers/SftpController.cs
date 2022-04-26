using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Consumir_Aplicacion.Models;
using Consumir_Aplicacion.WsBlackBoard;
using System.Text;
using System.Web.Helpers;
using System.Xml;
using System.Data;
using System.Net;
using Consumir_Aplicacion.App_Start;

namespace Consumir_Aplicacion.Controllers
{
    public class SftpController : Controller
    {
        SftpModels objeto = new SftpModels();

        SwnBlackBoardClient cliente = new SwnBlackBoardClient();
        #region Declaracion de variables
        private static string url = "https://universidadinsurgentes-sandbox.mrooms.net/blocks/conduit/webservices/rest";
        private static string token = "token=55fa8fa9-cf0c-40d2-886f-b3a5c4123aa0";
        private static string pobjAdjuntarSlash = "/";
        private static string pobjAdjuntar = "&";
        private static string pobjMetodoex = "method=";
        private static string pobjExtension = ".php?";
        private List<string> Urls;
        private string pobjstrFiltro;
        string metodo = string.Empty;
        private DataTable pobjDtResultadoCalificacion;
        private DataTable pobjDtResultado;
        public enum endPoints
        {
            course = 1,
            enroll = 2,
            groups = 3,
            role_assign = 4,
            user = 5
        }
        public enum Metodos
        {
            get_course,
            get_course_grades,
            get_groups,
            get_user,
            get_user_course_recent_activity,
            get_user_course_activities_due,
            get_user_course_events,
            get_user_grades

        }

        #endregion





        // GET: /Sftp/
        [SessionExpireFilterLogin]
        public ActionResult Index()
        {


            return View();
        }


        [HttpPost]
        public ActionResult Index(SftpModels sftp)
        {
            string nombreArchivo;
            try
            {
                foreach (var archivos in sftp.inputFile)
                {

                    if (archivos != null && archivos.ContentLength > 0 && archivos.ContentType.Equals("application/vnd.ms-excel"))
                    {
                        nombreArchivo = archivos.FileName;
                        byte[] data;
                        using (var csvLector = new StreamReader(archivos.InputStream))
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                csvLector.BaseStream.CopyTo(ms);
                                data = ms.ToArray();
                            }
                            csvLector.Close();


                            ViewBag.ArchivosCargados += string.Format("<b></b> <br />" + nombreArchivo);
                        }

                    }
                    else if (archivos == null)
                    {
                        ViewBag.Mensaje = "Favor de verificar No, ha seleccionado al menos un archivo con extension .csv";
                    }
                    else if (!archivos.ContentType.Equals("application/vnd.ms-excel"))
                    {
                        ViewBag.Mensaje = "Favor de verificar que el  archivo seleccionado sea con extension .csv";
                    }

                }
                return View();
            }
            catch (Exception e)
            {

                return View();
            }
        }//

        public ActionResult Consultar()
        {
            return View();
        }
        public ActionResult ObtenerCalificaciones()
        {
            ArmarCadenas();
            RealizarBusqueda(1);
            return View("Calificaciones", pobjDtResultado);
        }
        [HttpPost]
        public ActionResult Consultar(string Selected, string SelectedBuscarUsuario, string txtFiltro)
        {
            if (!string.IsNullOrEmpty(Selected) && !string.IsNullOrEmpty(SelectedBuscarUsuario) && !string.IsNullOrEmpty(txtFiltro))
            {
                int algo = Convert.ToInt32(Selected);
                int algo2 = Convert.ToInt32(SelectedBuscarUsuario);
                //RealizarBusqueda(Selected);
            }
            if (!string.IsNullOrEmpty(txtFiltro))
            {
                ViewBag.Nombre = txtFiltro;
            }

            return View();
        }
      
        //[HttpPost]
        //public ActionResult Consultar(string Selected, string SelectedBuscarCurso)
        //{
        //    if (!string.IsNullOrEmpty(Selected) && !string.IsNullOrEmpty(SelectedBuscarCurso))
        //    {
        //        int algo = Convert.ToInt32(Selected);
        //        int algo2 = Convert.ToInt32(SelectedBuscarCurso);
        //        //RealizarBusqueda(Selected);
        //    }


        //    return View();
        //}

        public ActionResult RealizarBusquedaFiltrada(SftpModels sftp)
        {
            return View();
        }
        public void RealizarBusqueda(int id)
        {
            string n = string.Empty;
            switch (id)
            {
                case 1: n = "Buscar  Usuario";
                    switch (id)
                    {
                        case 1: n = "NO HAY";
                            ViewBag.Mensaje = "Obtener los datos de perfil de un usuario";
                            //GetDisporador(id);
                            break;
                        case 2: n = "NO HAY 2";
                            ViewBag.Mensaje = "Información acerca de todas las actividades";
                            break;
                        case 3: n = "NO HAY 3";
                            ViewBag.Mensaje = "Actividades que  debe entregar en un curso";
                            break;
                        case 4: n = "NO HAY 4";
                            ViewBag.Mensaje = "Eventos del Usuario dentro de un curso";
                            break;
                        case 5: n = "NO HAY 5";
                            ViewBag.Mensaje = "Calificaciones del Usuario";
                            break;
                        default: n = "Nohay";
                            break;
                    }
                    break;
                case 2: n = "Buscar  Curso";
                    switch (id)
                    {
                        case 1: n = "NO HAY Curso ";
                            GetDisporador(id);
                            break;
                        case 2: n = "NO HAY 2 Curso";
                            break;

                        default: n = "Nohay";
                            break;
                    }
                    break;


                    ViewBag.Mensaje = "Calificaciones";
                    metodo = RealizarConsultaRest(id);
                    ArmarMetodo(metodo);
                    break;
                case 3:
                    n = "Grupos";
                    ViewBag.mensaje = "En Construccion";
                    //RealizarConsultaRest(id);
                    break;

                default: n = "Nohay";
                    break;
            }
        }

        public List<string> ArmarCadenas()
        {
            StringBuilder pobjSbArmarCadenas = new StringBuilder();
            Urls = new List<string>();
            string pobjMetodo = string.Empty;

            pobjSbArmarCadenas.Append(url);
            pobjSbArmarCadenas.Append(pobjAdjuntarSlash);
            pobjMetodo = endPoints.course.ToString();
            pobjSbArmarCadenas.Append(pobjMetodo);
            pobjSbArmarCadenas.Append(pobjExtension);
            pobjSbArmarCadenas.Append(token);
            pobjSbArmarCadenas.Append(pobjAdjuntar + pobjMetodoex);
            pobjSbArmarCadenas.Append(Metodos.get_course);

            Urls.Add(pobjSbArmarCadenas.ToString());

            pobjSbArmarCadenas = new StringBuilder();
            pobjMetodo = string.Empty;
            pobjSbArmarCadenas.Append(url);
            pobjSbArmarCadenas.Append(pobjAdjuntarSlash);
            pobjMetodo = endPoints.course.ToString();
            pobjSbArmarCadenas.Append(pobjMetodo);
            pobjSbArmarCadenas.Append(pobjExtension);
            pobjSbArmarCadenas.Append(token);
            pobjSbArmarCadenas.Append(pobjAdjuntar + pobjMetodoex);
            pobjSbArmarCadenas.Append(Metodos.get_course_grades);

            Urls.Add(pobjSbArmarCadenas.ToString());




            pobjSbArmarCadenas = new StringBuilder();
            pobjMetodo = string.Empty;
            pobjSbArmarCadenas.Append(url);
            pobjSbArmarCadenas.Append(pobjAdjuntarSlash);
            pobjMetodo = endPoints.enroll.ToString();
            pobjSbArmarCadenas.Append(pobjMetodo);
            pobjSbArmarCadenas.Append(pobjExtension);
            pobjSbArmarCadenas.Append(token);
            pobjSbArmarCadenas.Append(pobjAdjuntar + pobjMetodoex);
            Urls.Add(pobjSbArmarCadenas.ToString());

            pobjSbArmarCadenas = new StringBuilder();
            pobjMetodo = string.Empty;
            pobjSbArmarCadenas.Append(url);
            pobjSbArmarCadenas.Append(pobjAdjuntarSlash);
            pobjMetodo = endPoints.groups.ToString();
            pobjSbArmarCadenas.Append(pobjMetodo);
            pobjSbArmarCadenas.Append(pobjExtension);
            pobjSbArmarCadenas.Append(token);
            pobjSbArmarCadenas.Append(pobjAdjuntar + pobjMetodoex);
            pobjSbArmarCadenas.Append(pobjAdjuntar + Metodos.get_groups);
            Urls.Add(pobjSbArmarCadenas.ToString());





            pobjSbArmarCadenas = new StringBuilder();
            pobjMetodo = string.Empty;
            pobjSbArmarCadenas.Append(url);
            pobjSbArmarCadenas.Append(pobjAdjuntarSlash);
            pobjMetodo = endPoints.enroll.ToString();
            pobjSbArmarCadenas.Append(pobjMetodo);
            pobjSbArmarCadenas.Append(pobjExtension);
            pobjSbArmarCadenas.Append(token);
            pobjSbArmarCadenas.Append(pobjAdjuntar + pobjMetodoex);
            Urls.Add(pobjSbArmarCadenas.ToString());

            pobjSbArmarCadenas = new StringBuilder();
            pobjMetodo = string.Empty;
            pobjSbArmarCadenas.Append(url);
            pobjSbArmarCadenas.Append(pobjAdjuntarSlash);
            pobjMetodo = endPoints.user.ToString();
            pobjSbArmarCadenas.Append(pobjMetodo);
            pobjSbArmarCadenas.Append(pobjExtension);
            pobjSbArmarCadenas.Append(token);
            pobjSbArmarCadenas.Append(pobjAdjuntar + pobjMetodoex);
            pobjSbArmarCadenas.Append(Metodos.get_user);
            Urls.Add(pobjSbArmarCadenas.ToString());

            pobjSbArmarCadenas = new StringBuilder();
            pobjMetodo = string.Empty;
            pobjSbArmarCadenas.Append(url);
            pobjSbArmarCadenas.Append(pobjAdjuntarSlash);
            pobjMetodo = endPoints.groups.ToString();
            pobjSbArmarCadenas.Append(pobjMetodo);
            pobjSbArmarCadenas.Append(pobjExtension);
            pobjSbArmarCadenas.Append(token);
            pobjSbArmarCadenas.Append(pobjAdjuntar + pobjMetodoex);
            pobjSbArmarCadenas.Append(Metodos.get_user_course_recent_activity);
            Urls.Add(pobjSbArmarCadenas.ToString());


            pobjSbArmarCadenas = new StringBuilder();
            pobjMetodo = string.Empty;
            pobjSbArmarCadenas.Append(url);
            pobjSbArmarCadenas.Append(pobjAdjuntarSlash);
            pobjMetodo = endPoints.groups.ToString();
            pobjSbArmarCadenas.Append(pobjMetodo);
            pobjSbArmarCadenas.Append(pobjExtension);
            pobjSbArmarCadenas.Append(token);
            pobjSbArmarCadenas.Append(pobjAdjuntar + pobjMetodoex);
            pobjSbArmarCadenas.Append(Metodos.get_user_course_activities_due);
            Urls.Add(pobjSbArmarCadenas.ToString());



            pobjSbArmarCadenas = new StringBuilder();
            pobjMetodo = string.Empty;
            pobjSbArmarCadenas.Append(url);
            pobjSbArmarCadenas.Append(pobjAdjuntarSlash);
            pobjMetodo = endPoints.groups.ToString();
            pobjSbArmarCadenas.Append(pobjMetodo);
            pobjSbArmarCadenas.Append(pobjExtension);
            pobjSbArmarCadenas.Append(token);
            pobjSbArmarCadenas.Append(pobjAdjuntar + pobjMetodoex);
            pobjSbArmarCadenas.Append(Metodos.get_user_course_events);
            Urls.Add(pobjSbArmarCadenas.ToString());



            pobjSbArmarCadenas = new StringBuilder();
            pobjMetodo = string.Empty;
            pobjSbArmarCadenas.Append(url);
            pobjSbArmarCadenas.Append(pobjAdjuntarSlash);
            pobjMetodo = endPoints.groups.ToString();
            pobjSbArmarCadenas.Append(pobjMetodo);
            pobjSbArmarCadenas.Append(pobjExtension);
            pobjSbArmarCadenas.Append(token);
            pobjSbArmarCadenas.Append(pobjAdjuntar + pobjMetodoex);
            pobjSbArmarCadenas.Append(Metodos.get_user_grades);
            Urls.Add(pobjSbArmarCadenas.ToString());
            return Urls;
        }

        private string RealizarConsultaRest(int id)
        {
            string s = string.Empty;


            if (Urls != null && Urls.Count > 0)
            {
                foreach (string url in Urls)
                {


                    if (id == 1)
                    {
                        if (url.Contains("get_course_grades"))
                        {
                            s = url;
                            return s;
                        }
                    }

                    else if (id == 3)
                    {
                        if (url.Contains("get_groups"))
                        {
                            s = url;
                            return s;
                        }
                    }
                    else if (id == 5)
                    {

                        if (url.Contains("get_user"))
                        {
                            s = url;

                            return s;
                        }
                    }

                }
            }
            return s;
        }



        public void GET_COURSE(int id)
        {
        }
        public void GET_COURSE_GRADES(int id)
        {
        }
        public void GET_USER(int id)
        {
            string metodo = string.Empty;
            if (id == 5)
            {
            }
        }
        public void GetDisporador(int id)
        {

            if (id == 5)
            {
                GET_USER_COURSE_RECENT_ACTIVITY(id);
                GET_USER_COURSE_ACTIVITIES_DUE(id);
                GET_USER_COURSE_EVENTS(id);
                GET_USER_GRADES(id);
            }

        }
        public void GET_USER_COURSE_RECENT_ACTIVITY(int id)
        {

            if (id == 5)
            {
                metodo = RealizarConsultaRest(id);
                metodo += pobjAdjuntar + "value=" + pobjstrFiltro;
            }
        }
        public void GET_USER_COURSE_ACTIVITIES_DUE(int id)
        {
            string metodo = string.Empty;
            if (id == 5)
            {
                metodo = RealizarConsultaRest(id);
                metodo += pobjAdjuntar + "value=" + pobjstrFiltro;
            }
        }
        public void GET_USER_COURSE_EVENTS(int id)
        {
            string metodo = string.Empty;
            if (id == 5)
            {
                metodo = RealizarConsultaRest(id);
                metodo += pobjAdjuntar + "value=" + pobjstrFiltro;
            }
        }
        public void GET_USER_GRADES(int id)
        {
            string metodo = string.Empty;
            if (id == 5)
            {
                metodo = RealizarConsultaRest(id);
                metodo += pobjAdjuntar + "value=" + pobjstrFiltro;
            }
        }


        private void ArmarMetodo(string Url)
        {
            try
            {
                WebRequest respuesta = WebRequest.Create(Url);


                respuesta.Timeout = 10 * 1000;
                respuesta.Method = "POST";

                respuesta.ContentType = "application/x-www-form-urlencoded";

                Stream dataStream = respuesta.GetRequestStream();


                WebResponse responder = respuesta.GetResponse();
                dataStream = responder.GetResponseStream();
                string respuestaServer = string.Empty;

                DataTable dt = new DataTable();

                XmlDocument pobjXmlDocumento = new XmlDocument();
                DataSet ds = new DataSet();
                XmlElement pobjElemento;
                XmlNodeList pobjLstNodo;
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    respuestaServer = reader.ReadToEnd();
                    pobjXmlDocumento.LoadXml(respuestaServer);

                    pobjElemento = pobjXmlDocumento.DocumentElement;
                    pobjLstNodo = pobjElemento.ChildNodes;
                    foreach (XmlNode nodo in pobjLstNodo)
                    {
                        if (nodo.ChildNodes != null)
                        {
                            XmlTextReader r = new XmlTextReader(new StringReader(nodo.OuterXml));
                            ds.ReadXml(r);
                        }
                    }
                    pobjDtResultadoCalificacion = ds.Tables["grade"];

                    for (int i = 0; i <= pobjDtResultadoCalificacion.Rows.Count - 1; i++)
                    {
                        for (int j = 0; j <= pobjDtResultadoCalificacion.Columns.Count - 1; j++)
                        {
                            if (pobjDtResultadoCalificacion.Columns[j].ColumnName.Equals("id"))
                            { //id 	userid 	finalgrade 	c 	gradepercent 	timemodified 	deleted 	grades_Id
                                pobjDtResultadoCalificacion.Columns.Remove("id");

                            }
                            if (pobjDtResultadoCalificacion.Columns[j].ColumnName.Equals("userid"))
                            {
                                pobjDtResultadoCalificacion.Columns[j].ColumnName = "Numero de Usuario";
                            }
                            if (pobjDtResultadoCalificacion.Columns[j].ColumnName.Equals("userid"))
                            {
                                pobjDtResultadoCalificacion.Columns[j].ColumnName = "Numero de Usuario";
                            }
                            if (pobjDtResultadoCalificacion.Columns[j].ColumnName.Equals("finalgrade"))
                            {
                                pobjDtResultadoCalificacion.Columns[j].ColumnName = "Calificacion Final";
                            }
                            if (pobjDtResultadoCalificacion.Columns[j].ColumnName.Equals("finalgrade"))
                            {
                                pobjDtResultadoCalificacion.Columns[j].ColumnName = "Calificacion en Letra";
                            }
                            if (pobjDtResultadoCalificacion.Columns[j].ColumnName.Equals("gradepercent"))
                            {
                                pobjDtResultadoCalificacion.Columns[j].ColumnName = "Porcentaje";
                            }
                            if (pobjDtResultadoCalificacion.Columns[j].ColumnName.Equals("timemodified"))
                            {
                                pobjDtResultadoCalificacion.Columns.Remove("timemodified");

                            }
                            if (pobjDtResultadoCalificacion.Columns[j].ColumnName.Equals("deleted"))
                            {
                                pobjDtResultadoCalificacion.Columns.Remove("deleted");

                            }


                        }
                    }

                    pobjDtResultado = pobjDtResultadoCalificacion;
                }
                dataStream.Close();
                responder.Close();
            }
            catch (Exception e)
            {

            }
        }




    }
}
