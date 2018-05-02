using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bizpulse.Admin.UI.Web.Controllers
{
    public class ErrorController : Controller
    {            
        public ActionResult Error()    
        {       
            return View();    
        }     
        
        public ActionResult Http404()
        {
            return View();
        }        
    }
}
