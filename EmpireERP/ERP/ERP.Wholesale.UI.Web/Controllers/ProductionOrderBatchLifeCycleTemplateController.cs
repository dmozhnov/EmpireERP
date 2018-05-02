using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ProductionOrderBatchLifeCycleTemplate;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ProductionOrderBatchLifeCycleTemplateController : WholesaleController
    {
        #region Поля

        private readonly IProductionOrderBatchLifeCycleTemplatePresenter productionOrderBatchLifeCycleTemplatePresenter;

        #endregion

        #region Конструкторы

        public ProductionOrderBatchLifeCycleTemplateController(IProductionOrderBatchLifeCycleTemplatePresenter productionOrderBatchLifeCycleTemplatePresenter)
        {
            this.productionOrderBatchLifeCycleTemplatePresenter = productionOrderBatchLifeCycleTemplatePresenter;
        }

        #endregion

        #region Методы

        #region Список шаблонов жизненного цикла заказа

        public ActionResult List()
        {
            try
            {
                return View(productionOrderBatchLifeCycleTemplatePresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowProductionOrderBatchLifeCycleTemplateGrid(GridState state)
        {
            try
            {
                return PartialView("ProductionOrderBatchLifeCycleTemplateGrid",
                    productionOrderBatchLifeCycleTemplatePresenter.GetProductionOrderBatchLifeCycleTemplateGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Создание / Редактирование / Удаление

        public ActionResult Create()
        {
            try
            {
                return View("Edit", productionOrderBatchLifeCycleTemplatePresenter.Create(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Edit(string id)
        {
            try
            {
                return View(productionOrderBatchLifeCycleTemplatePresenter.Edit(ValidationUtils.TryGetShort(id), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(ProductionOrderBatchLifeCycleTemplateEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Json(productionOrderBatchLifeCycleTemplatePresenter.Save(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                productionOrderBatchLifeCycleTemplatePresenter.Delete(ValidationUtils.TryGetShort(id), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Детали

        #region Детали общие

        public ActionResult Details(string id, string backUrl)
        {
            try
            {
                return View(productionOrderBatchLifeCycleTemplatePresenter.Details(ValidationUtils.TryGetShort(id), backUrl, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Гриды

        [HttpPost]
        public ActionResult ShowProductionOrderBatchLifeCycleTemplateStageGrid(GridState state)
        {
            try
            {
                return PartialView("ProductionOrderBatchLifeCycleTemplateStageGrid", productionOrderBatchLifeCycleTemplatePresenter.GetProductionOrderBatchLifeCycleTemplateStageGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Работа с этапами

        public ActionResult AddStage(string productionOrderBatchLifeCycleTemplateId, string id, string position)
        {
            try
            {
                return View("ProductionOrderBatchLifeCycleTemplateStageEdit",
                    productionOrderBatchLifeCycleTemplatePresenter.AddStage(ValidationUtils.TryGetShort(productionOrderBatchLifeCycleTemplateId),
                    ValidationUtils.TryGetInt(id), ValidationUtils.TryGetShort(position), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult EditStage(string productionOrderBatchLifeCycleTemplateId, string id)
        {
            try
            {
                return View("ProductionOrderBatchLifeCycleTemplateStageEdit",
                    productionOrderBatchLifeCycleTemplatePresenter.EditStage(ValidationUtils.TryGetShort(productionOrderBatchLifeCycleTemplateId),
                    ValidationUtils.TryGetInt(id), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult DeleteStage(string productionOrderBatchLifeCycleTemplateId, string id)
        {
            try
            {
                productionOrderBatchLifeCycleTemplatePresenter.DeleteStage(ValidationUtils.TryGetShort(productionOrderBatchLifeCycleTemplateId),
                    ValidationUtils.TryGetInt(id), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult MoveStageUp(string productionOrderBatchLifeCycleTemplateId, string id)
        {
            try
            {
                productionOrderBatchLifeCycleTemplatePresenter.MoveStageUp(ValidationUtils.TryGetShort(productionOrderBatchLifeCycleTemplateId),
                    ValidationUtils.TryGetInt(id), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult MoveStageDown(string productionOrderBatchLifeCycleTemplateId, string id)
        {
            try
            {
                productionOrderBatchLifeCycleTemplatePresenter.MoveStageDown(ValidationUtils.TryGetShort(productionOrderBatchLifeCycleTemplateId),
                    ValidationUtils.TryGetInt(id), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveStage(ProductionOrderBatchLifeCycleTemplateStageEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Content(productionOrderBatchLifeCycleTemplatePresenter.SaveStage(model, UserSession.CurrentUserInfo).ToString());
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Модальная форма выбора шаблона

        /// <summary>
        /// Возвращает модальную форму для выбора шаблона
        /// </summary>
        [HttpGet]
        public ActionResult SelectProductionOrderBatchLifeCycleTemplate()
        {
            try
            {
                var model = productionOrderBatchLifeCycleTemplatePresenter.SelectProductionOrderBatchLifeCycleTemplate(UserSession.CurrentUserInfo);

                return PartialView("ProductionOrderBatchLifeCycleTemplateSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение грида доступных шаблонов
        /// </summary>
        /// <param name="state">Состояние грида</param>
        [HttpPost]
        public ActionResult ShowProductionOrderBatchLifeCycleTemplateSelectGrid(GridState state)
        {
            try
            {
                GridData gridData = productionOrderBatchLifeCycleTemplatePresenter.GetProductionOrderBatchLifeCycleTemplateSelectGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ProductionOrderBatchLifeCycleTemplateSelectGrid", gridData);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion
    }
}
