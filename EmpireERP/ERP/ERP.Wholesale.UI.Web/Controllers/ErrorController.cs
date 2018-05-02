using System.Net;
using System.Web.Mvc;
using System.Web.UI;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ErrorController : WholesaleController
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
