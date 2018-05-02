using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ContractorOrganization;
using ERP.Wholesale.UI.ViewModels.EconomicAgent;
using ERP.Wholesale.UI.ViewModels.Provider;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ProviderController : WholesaleController
    {
        #region Поля

        private readonly IProviderPresenter providerPresenter;

        #endregion

        #region Конструктор

        public ProviderController(IProviderPresenter providerPresenter)           
        {
            this.providerPresenter = providerPresenter;
        }

        #endregion

        #region Список поставщиков

        /// <summary>
        /// Получить список поставщиков
        /// </summary>
        [HttpGet]
        public ActionResult List()
        {
            try
            {
                var model = providerPresenter.List(UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение грида поставщиков
        /// </summary>
        [HttpPost]
        public ActionResult ShowProviderGrid(GridState state)
        {
            try
            {
                var grid = providerPresenter.GetProviderGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ProviderGrid", grid);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / редактирование поставщика

        [HttpGet]
        public ActionResult Create(string backURL)
        {
            try
            {
                var model = providerPresenter.Create(backURL, UserSession.CurrentUserInfo);

                return View("Edit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id, string backURL)
        {
            try
            {
                ValidationUtils.NotNull(id, "Неверное значение входного параметра.");

                var model = providerPresenter.Edit(id.Value, backURL, UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(ProviderEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                var id = providerPresenter.Save(model, UserSession.CurrentUserInfo);

                return Content(id.ToString());

            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получить типы поставщиков
        /// </summary>
        /// <returns>Объект JSon</returns>
        [HttpGet]
        public ActionResult GetProviderTypes()
        {
            var x = providerPresenter.GetProviderTypes();

            return Json(x, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Детали поставщика

        #region Детали общие

        [HttpGet]
        public ActionResult Details(int? id, string backURL)
        {
            try
            {
                ValidationUtils.NotNull(id, "Неверное значение входного параметра.");

                var model = providerPresenter.Details(id.Value, backURL, UserSession.CurrentUserInfo);
                model.TaskGrid.GridPartialViewAction = "/Provider/ShowTaskGrid/";
                model.TaskGrid.HelpContentUrl = "/Help/GetHelp_Provider_Details_TaskGrid";

                return View("Details", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Грид "Закупки у поставщика"

        /// <summary>
        /// Получение грида "Закупки у поставщика"
        /// </summary>
        /// <param name="state">Состояние грида</param>
        [HttpPost]
        public ActionResult ShowReceiptWaybillGrid(GridState state)
        {
            try
            {
                var grid = providerPresenter.GetReceiptWaybillGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ProviderReceiptWaybillGrid", grid);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Грид "Организации поставщика"

        /// <summary>
        /// Получение грида "Организации поставщика"
        /// </summary>
        /// <param name="state">Состояние грида</param>
        [HttpPost]
        public ActionResult ShowProviderOrganizationGrid(GridState state)
        {
            try
            {
                var grid = providerPresenter.GetProviderOrganizationGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ProviderOrganizationGrid", grid);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Грид "Договоры"

        /// <summary>
        /// Получение грида "Договоры"
        /// </summary>
        /// <param name="state">Состояние грида</param>
        [HttpPost]
        public ActionResult ShowProviderContractGrid(GridState state)
        {
            try
            {
                var grid = providerPresenter.GetProviderContractGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ProviderContractGrid", grid);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Грид "Задачи"

        [HttpPost]
        public ActionResult ShowTaskGrid(GridState state)
        {
            try
            {
                var model = providerPresenter.GetTaskGrid(state, UserSession.CurrentUserInfo);
                model.GridPartialViewAction = "/Provider/ShowTaskGrid/";

                return PartialView("~/Views/Task/NewTaskGrid.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Создание, редактирование, удаление организаций поставщика

        #region Добавление организации поставщика

        #region Модальная форма для создания новой организации

        /// <summary>
        /// Возвращает модальную форму для создания новой организации
        /// </summary>
        [HttpGet]
        public ActionResult CreateContractorOrganization(int? contractorId)
        {
            try
            {
                ValidationUtils.NotNull(contractorId, "Неверное значение входного параметра.");

                var model = providerPresenter.CreateContractorOrganization(contractorId.Value);

                return PartialView("~/Views/EconomicAgent/EconomicAgentTypeSelector.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Создание организации поставщика
        /// </summary>
        [HttpPost]
        public ActionResult EditJuridicalPerson(JuridicalPersonEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model.LegalFormId, "Не задана правовая форма.");
                ValidationUtils.NotNull(model.ContractorId, "Неверно указан контрагент.");

                if (model.OrganizationId != 0) // если пытаются отредактировать организацию (метод предназначен только для создания, а не для редактирования)
                {
                    throw new Exception("Неверное значение входного параметра.");
                }

                var result = providerPresenter.SaveJuridicalPerson(model, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Создание организации поставщика
        /// </summary>
        [HttpPost]
        public ActionResult EditPhysicalPerson(PhysicalPersonEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model.LegalFormId, "Не задана правовая форма.");
                ValidationUtils.NotNull(model.ContractorId, "Неверно указан контрагент.");

                if (model.OrganizationId != 0) // если пытаются отредактировать организацию (метод предназначен только для создания, а не для редактирования)
                {
                    throw new Exception("Неверное значение входного параметра.");
                }

                var result = providerPresenter.SavePhysicalPerson(model, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Добавление организации контрагента

        /// <summary>
        /// Добавить организацию контрагента с данным Id к поставщику.
        /// </summary>
        /// <param name="providerId">код поставщика</param>
        /// <param name="organizationId">код организации</param>
        [HttpPost]
        public ActionResult AddContractorOrganization(int? providerId, int? organizationId)
        {
            try
            {
                ValidationUtils.NotNull(providerId, "Неверное значение входного параметра.");
                ValidationUtils.NotNull(organizationId, "Неверное значение входного параметра.");

                var result = providerPresenter.AddContractorOrganization(providerId.Value, organizationId.Value, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Удаление организации поставщика

        /// <summary>
        /// Удаление организации поставщика
        /// </summary>
        /// <param name="providerId">код поставщика</param>
        /// <param name="providerOrganizationId">код организации поставщика (ProviderOrganization)</param>
        [HttpPost]
        public ActionResult RemoveProviderOrganization(int? providerId, int? providerOrganizationId)
        {
            try
            {
                ValidationUtils.NotNull(providerId, "Неверное значение входного параметра.");
                ValidationUtils.NotNull(providerOrganizationId, "Неверное значение входного параметра.");

                var result = providerPresenter.RemoveProviderOrganization(providerId.Value, providerOrganizationId.Value, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Создание, редактирование, удаление договоров

        #region Добавление и редактирование договора

        /// <summary>
        /// Создание договора
        /// </summary>
        /// <param name="providerId">код поставщика, для которого создается договор</param>
        [HttpGet]
        public ActionResult CreateContract(int? providerId)
        {
            try
            {
                ValidationUtils.NotNull(providerId, "Неверное значение входного параметра.");

                var model = providerPresenter.CreateContract(providerId.Value, UserSession.CurrentUserInfo);

                return PartialView("ProviderContractEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование договора
        /// </summary>
        /// <param name="providerId">код поставщика, у которого редактируется договор</param>
        /// <param name="contractId">код редактируемого договора</param>
        [HttpGet]
        public ActionResult EditContract(int? providerId, short? contractId)
        {
            try
            {
                ValidationUtils.NotNull(providerId, "Неверное значение входного параметра.");
                ValidationUtils.NotNull(contractId, "Неверное значение входного параметра.");

                var model = providerPresenter.EditContract(providerId.Value, contractId.Value, UserSession.CurrentUserInfo);

                return PartialView("ProviderContractEdit", model);

            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Создание / редактирование договора
        /// </summary>
        [HttpPost]
        public ActionResult EditContract(ProviderContractEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model.ProviderId, "Неверное значение входного параметра.");
                ValidationUtils.NotNull(model.AccountOrganizationId, "Собственная организация не выбрана.");
                ValidationUtils.NotNull(model.ProviderOrganizationId, "Организация поставщика не выбрана.");
                ValidationUtils.NotNull(model.Date, "Введите дату в правильном формате или выберите из списка.");
                ValidationUtils.NotNull(model.Id, "Неверное значение входного параметра.");
                ValidationUtils.NotNull(model.Name, "Не указано название договора.");

                var result = providerPresenter.SaveContract(model, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление договора

        /// <summary>
        /// Удаление договора
        /// </summary>
        /// <param name="providerId">код поставщика</param>
        /// <param name="contractId">код договора</param>
        [HttpPost]
        public ActionResult DeleteContract(int? providerId, short? contractId)
        {
            try
            {
                ValidationUtils.NotNull(providerId, "Неверное значение входного параметра.");
                ValidationUtils.NotNull(contractId, "Неверное значение входного параметра.");

                var result = providerPresenter.DeleteContract(providerId.Value, contractId.Value, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Модальная форма выбора организации поставщика

        /// <summary>
        /// Возвращает модальную форму для выбора организации поставщика
        /// Имеет 2 режима:
        /// - (mode == "excludeprovider") выбрать организацию поставщика из списка существующих, но не входящих в организации конкретного поставщика
        /// - (mode == "includeprovider") выбрать организацию поставщика только среди организаций данного поставщика
        /// </summary>
        [HttpGet]
        public ActionResult SelectContractorOrganization(int? providerId, string mode)
        {
            try
            {
                ValidationUtils.NotNull(providerId, "Неверное значение входного параметра.");
                
                if (String.IsNullOrWhiteSpace(mode))
                {
                    throw new Exception("Неверное значение входного параметра.");
                }

                var model = providerPresenter.SelectContractorOrganization(providerId.Value, mode, UserSession.CurrentUserInfo);

                return PartialView("~/Views/ContractorOrganization/ContractorOrganizationSelector.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение грида доступных организаций контрагента
        /// </summary>
        /// <param name="state">состояние грида</param>
        [HttpPost]
        public ActionResult ShowProviderOrganizationSelectGrid(GridState state)
        {
            try
            {
                GridData data = providerPresenter.GetProviderOrganizationSelectGrid(state);

                return PartialView("~/Views/ContractorOrganization/ContractorOrganizationSelectGrid.ascx", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление поставщика

        /// <summary>
        /// Удаление поставщика
        /// </summary>
        /// <param name="providerId">Код поставщика</param>
        [HttpPost]
        public ActionResult Delete(int? providerId)
        {
            try
            {
                ValidationUtils.NotNull(providerId, "Неверное значение входного параметра.");

                providerPresenter.Delete(providerId.Value, UserSession.CurrentUserInfo);

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
