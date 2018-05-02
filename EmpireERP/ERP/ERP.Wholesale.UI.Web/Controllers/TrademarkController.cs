using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class TrademarkController : WholesaleController
    {
        #region Поля

        private readonly ITrademarkPresenter trademarkPresenter;

        #endregion

        #region Конструктор

        public TrademarkController(ITrademarkPresenter trademarkPresenter)
        {
            this.trademarkPresenter = trademarkPresenter;
        }
        #endregion

        #region Просмотр списка

        public ActionResult List()
        {
            try
            {
                var model = trademarkPresenter.List(UserSession.CurrentUserInfo);

                return View("List", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowTrademarkGrid(GridState state)
        {
            try
            {
                var model = trademarkPresenter.GetTrademarkGrid(state, UserSession.CurrentUserInfo);

                return PartialView("TrademarkGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult GetTrademarks()
        {
            return Json(trademarkPresenter.GetTrademarks(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SelectTrademark()
        {
            try
            {
                var model = trademarkPresenter.SelectTrademark(UserSession.CurrentUserInfo);

                return View("TrademarkSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }


        public ActionResult ShowSelectTrademarkGrid(GridState state)
        {
            try
            {
                var model = trademarkPresenter.GetTrademarkSelectGrid(state);

                return View("TrademarkSelectGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                var model = trademarkPresenter.Create(UserSession.CurrentUserInfo);

                return View("TrademarkEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult Edit(short? id)
        {
            try
            {
                ValidationUtils.NotNullOrDefault(id, "Неверное значение входного параметра.");

                var model = trademarkPresenter.Edit(id.Value, UserSession.CurrentUserInfo);

                return View("TrademarkEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Edit(BaseDictionaryEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model);

                var result = trademarkPresenter.Save(model, UserSession.CurrentUserInfo);

                return Content(result.ToString());

            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(BaseDictionaryEditViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return PartialView("TrademarkEdit", model);
                }

                var obj = trademarkPresenter.Save(model, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Delete(short? id)
        {
            try
            {
                ValidationUtils.NotNull(id, "Неверное значение входного параметра.");

                trademarkPresenter.Delete(id.Value, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult CheckNameUniqueness(string name, string id)
        {
            try
            {
                trademarkPresenter.CheckNameUniqueness(name, ValidationUtils.TryGetShort(id));

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
