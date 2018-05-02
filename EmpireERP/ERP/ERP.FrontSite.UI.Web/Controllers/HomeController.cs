using System.Web.Mvc;
using System.Web.UI;
using System;
using ERP.Utils;

namespace ERP.FrontSite.UI.Web.Controllers
{
    [OutputCache(Duration=43200, Location = OutputCacheLocation.Server)] 
    public class HomeController : Controller
    {
        /// <summary>
        /// Что такое Bizpulse
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Что может
        /// </summary>
        /// <returns></returns>
        public ActionResult Features(string tab)
        {
            if (!string.IsNullOrEmpty(tab))
            {
                ValidationUtils.TryGetInt(tab, "Неверное значение входного параметра.");
            }
            
            ViewBag.TabId = tab ?? "1";
            
            return View();
        }

        /// <summary>
        /// Сколько стоит
        /// </summary>
        /// <returns></returns>
        public ActionResult Prices()
        {
            return View();
        }

        /// <summary>
        /// Безопасность
        /// </summary>
        /// <returns></returns>
        public ActionResult Security()
        {
            return View();
        }

        /// <summary>
        /// О компании
        /// </summary>
        /// <returns></returns>
        public ActionResult Company()
        {
            return View();
        }
    }
}
