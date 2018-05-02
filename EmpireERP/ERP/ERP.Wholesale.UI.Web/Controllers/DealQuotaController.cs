using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.DealQuota;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class DealQuotaController : WholesaleController
    {
        #region Поля

        private readonly IDealQuotaPresenter dealQuotaPresenter;

        #endregion

        #region Конструкторы

        public DealQuotaController(IDealQuotaPresenter dealQuotaPresenter)
        {
            this.dealQuotaPresenter = dealQuotaPresenter;
        }

        #endregion

        #region Список

        public ActionResult List()
        {
            try
            {
                var model = dealQuotaPresenter.List(UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ShowActiveDealQuotaGrid(GridState state)
        {
            try
            {
                var grid = dealQuotaPresenter.GetActiveDealQuotaGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ActiveDealQuotaGrid", grid);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ShowInactiveDealQuotaGrid(GridState state)
        {
            try
            {
                var grid = dealQuotaPresenter.GetInactiveDealQuotaGrid(state, UserSession.CurrentUserInfo);

                return PartialView("InactiveDealQuotaGrid", grid);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion
        
        #region Добавление / редактирование квоты

        /// <summary>
        /// Добавление новой квоты
        /// </summary>
        /// <param name="dealId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                return PartialView("DealQuotaEdit", dealQuotaPresenter.Create(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование квоты
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="quotaId">Код квоты</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(string id)
        {
            try
            {
                return View("DealQuotaEdit", dealQuotaPresenter.Edit(ValidationUtils.TryGetInt(id), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование квоты
        /// </summary>
        /// <param name="model"></param>
        [HttpPost]
        public ActionResult Save(DealQuotaEditViewModel model)
        {
            try
            {
                var result = dealQuotaPresenter.Save(model, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление квоты

        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                dealQuotaPresenter.Delete(ValidationUtils.TryGetInt(id), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Выбор квоты

        /// <summary>
        /// 1) При mode == "Deal" возвращает грид квот, которые можно указать при создании реализации, то есть те квоты, которые добавлены в сделку это реализации
        /// 2) При mode == "Sale" возвращает грид квот, которые можно добавить в сделку, то есть:
        ///     - действующие
        ///     - не добавленные в эту сделку
        /// </summary>
        /// <param name="dealId"></param>
        /// <returns></returns>
        public ActionResult SelectDealQuota(string dealId, string mode)
        {
            try
            {
                return PartialView("DealQuotaSelector", dealQuotaPresenter.SelectDealQuota(ValidationUtils.TryGetInt(dealId), UserSession.CurrentUserInfo, mode));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ShowDealQuotaSelectGrid(GridState state)
        {
            try
            {
                var grid = dealQuotaPresenter.GetDealQuotaSelectGrid(state, UserSession.CurrentUserInfo);

                return PartialView("DealQuotaSelectGrid", grid);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        
        #endregion
    }
}
