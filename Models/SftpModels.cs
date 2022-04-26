using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Consumir_Aplicacion.Models
{
    public class SftpModels
    {



        public IEnumerable<HttpPostedFileBase> inputFile { get; set; }
        public string ruta { get; set; }
        public static string Mensaje() 
        {
            return "Si llego hasta el mensaje";
        }


    }
}