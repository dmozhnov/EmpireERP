using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP.Wholesale.UI.Web.Infrastructure;
using System.Web.UI;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Settings;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class SettingController : WholesaleController
    {
        #region Поля

        private readonly ISettingPresenter settingPresenter;

        #endregion

        #region Конструктор

        public SettingController(ISettingPresenter settingPresenter)
        {
            this.settingPresenter = settingPresenter;
        }

        #endregion

        #region Методы
        
        public ActionResult List(string backUrl)
        {
            try
            {
                var model = settingPresenter.List(backUrl, UserSession.CurrentUserInfo);

                return View("List", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            } 
        }

        [HttpPost]
        public ActionResult Save(SettingViewModel model)
        {
            try
            {
                settingPresenter.Save(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion
    }
}
