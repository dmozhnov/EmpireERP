using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Organization;
using ERP.Wholesale.UI.ViewModels.Producer;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ProducerController : WholesaleController
    {
        #region Поля

        private readonly IProducerPresenter producerPresenter;

        #endregion

        #region Конструкторы

        public ProducerController(IProducerPresenter producerPresenter)
        {
            this.producerPresenter = producerPresenter;
        }

        #endregion

        #region Методы

        #region Список

        public ActionResult List()
        {
            try
            {
                return View(producerPresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowProducersGrid(GridState state)
        {
            try
            {
                return PartialView("ProducersGrid", producerPresenter.GetProducersGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / редактирование

        public ActionResult Create(string backURL)
        {
            try
            {
                return View("Edit", producerPresenter.Create(backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Edit(string id, string backURL)
        {
            try
            {
                int producerId = ValidationUtils.TryGetInt(id);

                return View("Edit", producerPresenter.Edit(producerId, backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(ProducerEditViewModel model)
        {
            try
            {
                return Content(producerPresenter.Save(model, UserSession.CurrentUserInfo).ToString());
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление

        [HttpPost]
        public ActionResult Delete(string producerId)
        {
            try
            {
                producerPresenter.Delete(ValidationUtils.TryGetInt(producerId), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Детали

        public ActionResult Details(string id, string backURL)
        {
            try
            {
                int producerId = ValidationUtils.TryGetInt(id);
                var model = producerPresenter.Details(producerId, backURL, UserSession.CurrentUserInfo);
                model.TaskGrid.GridPartialViewAction = "/Producer/ShowTaskGrid/";
                model.TaskGrid.HelpContentUrl = "/Help/GetHelp_Producer_Details_TaskGrid";

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowProductionOrdersGrid(GridState state)
        {
            try
            {
                return PartialView("ProductionOrdersGrid", producerPresenter.GetProductionOrdersGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowPaymentsGrid(GridState state)
        {
            try
            {
                return PartialView("ProducerPaymentsGrid", producerPresenter.GetPaymentsGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowBankAccountsGrid(GridState state)
        {
            try
            {
                return PartialView("~/Views/Organization/RussianBankAccountGrid.ascx", producerPresenter.GetBankAccountsGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowForeignBankAccountsGrid(GridState state)
        {
            try
            {
                return PartialView("~/Views/Organization/ForeignBankAccountGrid.ascx", producerPresenter.GetForeignBankAccountsGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult GetMainChangeableIndicators(string producerId)
        {
            try
            {
                return Json(producerPresenter.GetMainChangeableIndicators(ValidationUtils.TryGetInt(producerId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowTaskGrid(GridState state)
        {
            try
            {
                var model = producerPresenter.GetTaskGrid(state, UserSession.CurrentUserInfo);
                model.GridPartialViewAction = "/Producer/ShowTaskGrid/";
                model.HelpContentUrl = "/Help/GetHelp_Producer_Details_TaskGrid";

                return PartialView("~/Views/Task/NewTaskGrid.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Расчетные счета

        public ActionResult AddRussianBankAccount(string producerId)
        {
            try
            {
                return PartialView("~/Views/Organization/RussianBankAccountEdit.ascx", producerPresenter.AddRussianBankAccount(ValidationUtils.TryGetInt(producerId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult AddForeignBankAccount(string producerId)
        {
            try
            {
                return PartialView("~/Views/Organization/ForeignBankAccountEdit.ascx", producerPresenter.AddForeignBankAccount(ValidationUtils.TryGetInt(producerId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult EditRussianBankAccount(string producerId, string bankAccountId)
        {
            try
            {
                return PartialView("~/Views/Organization/RussianBankAccountEdit.ascx",
                    producerPresenter.EditRussianBankAccount(ValidationUtils.TryGetInt(producerId), ValidationUtils.TryGetInt(bankAccountId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult EditForeignBankAccount(string producerId, string bankAccountId)
        {
            try
            {
                return PartialView("~/Views/Organization/ForeignBankAccountEdit.ascx",
                    producerPresenter.EditForeignBankAccount(ValidationUtils.TryGetInt(producerId), ValidationUtils.TryGetInt(bankAccountId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveRussianBankAccount(RussianBankAccountEditViewModel model)
        {
            try
            {
                producerPresenter.SaveRussianBankAccount(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveForeignBankAccount(ForeignBankAccountEditViewModel model)
        {
            try
            {
                producerPresenter.SaveForeignBankAccount(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult RemoveRussianBankAccount(string producerId, string bankAccountId)
        {
            try
            {
                producerPresenter.RemoveRussianBankAccount(ValidationUtils.TryGetInt(producerId), ValidationUtils.TryGetInt(bankAccountId), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult RemoveForeignBankAccount(string producerId, string bankAccountId)
        {
            try
            {
                producerPresenter.RemoveForeignBankAccount(ValidationUtils.TryGetInt(producerId), ValidationUtils.TryGetInt(bankAccountId), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Работа с Фабриками-изготовителями

        public ActionResult ShowManufacturerGrid(GridState state)
        {
            try
            {
                var model = producerPresenter.GetManufacturerGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ProducerManufacturerGrid", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult AddManufacturer(string producerId, string manufacturerId)
        {
            try
            {
                producerPresenter.AddManufacturer(ValidationUtils.TryGetInt(producerId), ValidationUtils.TryGetShort(manufacturerId), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult RemoveManufacturer(string producerId, string manufacturerId)
        {
            try
            {
                producerPresenter.RemoveManufacturer(ValidationUtils.TryGetInt(producerId), ValidationUtils.TryGetShort(manufacturerId), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Модальная форма выбора производителя

        /// <summary>
        /// Возвращает модальную форму для выбора производителя
        /// </summary>
        [HttpGet]
        public ActionResult SelectProducer()
        {
            try
            {
                var model = producerPresenter.SelectProducer();

                return PartialView("ProducerSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение грида доступных производителей
        /// </summary>
        /// <param name="state">Состояние грида</param>
        [HttpPost]
        public ActionResult ShowProducerSelectGrid(GridState state)
        {
            try
            {
                GridData gridData = producerPresenter.GetProducerSelectGrid(state);

                return PartialView("ProducerSelectGrid", gridData);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Грид оплат

        [HttpPost]
        public ActionResult DeleteProducerPayment(string productionOrderId, string paymentId)
        {
            try
            {
                var currentDateTime = DateTimeUtils.GetCurrentDateTime();
                var obj = producerPresenter.DeletePayment(ValidationUtils.TryGetNotEmptyGuid(productionOrderId), ValidationUtils.TryGetGuid(paymentId),
                    UserSession.CurrentUserInfo, currentDateTime);

                return Json(obj, JsonRequestBehavior.AllowGet);
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
