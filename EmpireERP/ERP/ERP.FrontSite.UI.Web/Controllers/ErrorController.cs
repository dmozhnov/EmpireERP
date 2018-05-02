using System.Net;
using System.Web.Mvc;

namespace ERP.FrontSite.UI.Web.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Error()
        {
            return View();
        }
        
        public ActionResult HttpError404()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;

            return View();
        }
    }
}
