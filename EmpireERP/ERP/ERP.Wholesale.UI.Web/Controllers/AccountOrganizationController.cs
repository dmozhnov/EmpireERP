using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.AccountOrganization;
using ERP.Wholesale.UI.ViewModels.EconomicAgent;
using ERP.Wholesale.UI.ViewModels.Organization;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class AccountOrganizationController : WholesaleController
    {
        #region Поля

        private readonly IAccountOrganizationPresenter accountOrganizationPresenter;

        #endregion

        #region Конструктор

        public AccountOrganizationController(IAccountOrganizationPresenter accountOrganizationPresenter)
        {
            this.accountOrganizationPresenter = accountOrganizationPresenter;
        }

        #endregion

        #region Список

        public ActionResult List()
        {
            try
            {
                var model = accountOrganizationPresenter.List(UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion      

        #region Вывод основных деталей

        [HttpGet]
        public ActionResult Details(int id, string backURL)
        {
            try
            {
                var model = accountOrganizationPresenter.Details(id, backURL, UserSession.CurrentUserInfo);

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        
        /// <summary>
        /// Вывод главных деталей организации
        /// </summary>
        /// <param name="accountOrganizationId">Идентификатор организации</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ShowMainDetails(int? accountOrganizationId)
        {
            try
            {
                ValidationUtils.NotNull(accountOrganizationId, "Собственная организация указана неверно.");
                
                var model = accountOrganizationPresenter.MainDetails(accountOrganizationId.Value);

                return PartialView("AccountOrganizationMainDetails", model);               
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Создание гридов

        #region Грид организаций

        
        [HttpPost]
        public ActionResult ShowAccountOrganizationGrid(GridState state)
        {
            try
            {
                GridData data = accountOrganizationPresenter.GetAccountOrganizationGrid(state, UserSession.CurrentUserInfo);

                return PartialView("AccountOrganizationGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        
        #endregion

        #region Грид расчетных счетов

        public ActionResult ShowRussianBankAccounts(GridState state)
        {
            try
            {
                GridData data = accountOrganizationPresenter.GetRussianBankAccountGrid(state, UserSession.CurrentUserInfo);

                return PartialView("~/Views/Organization/RussianBankAccountGrid.ascx", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ShowForeignBankAccounts(GridState state)
        {
            try
            {
                GridData data = accountOrganizationPresenter.GetForeignBankAccountGrid(state, UserSession.CurrentUserInfo);

                return PartialView("~/Views/Organization/ForeignBankAccountGrid.ascx", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Список мест хранения
        
        [HttpPost]
        public ActionResult ShowStorageGrid(GridState state)
        {            
            try
            {
                GridData data = accountOrganizationPresenter.GetStorageGrid(state, UserSession.CurrentUserInfo);

                return PartialView("StorageGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Создание собственной организации

        /// <summary>
        /// Создание собственной организации
        /// </summary>
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                var model = accountOrganizationPresenter.Create(UserSession.CurrentUserInfo);

                return PartialView("~/Views/EconomicAgent/EconomicAgentTypeSelector.ascx", model);  
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Редактирование организации

        /// <summary>
        /// Редактирование собственной организации
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int? accountOrganizationId = null)
        {
            try
            {
                ValidationUtils.NotNull(accountOrganizationId, "Неверное значение входного параметра.");

                var model = accountOrganizationPresenter.Edit(accountOrganizationId.Value, UserSession.CurrentUserInfo);
                
                if (model is JuridicalPersonEditViewModel)
                {                    
                    return PartialView("~/Views/EconomicAgent/JuridicalPersonEdit.ascx", model as JuridicalPersonEditViewModel);
                }                
                
                if (model is PhysicalPersonEditViewModel)
                {                    
                    return PartialView("~/Views/EconomicAgent/PhysicalPersonEdit.ascx", model as PhysicalPersonEditViewModel);
                }                

                throw new Exception("Неверное значение входного параметра.");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
                        
        [HttpPost]
        public ActionResult EditJuridicalPerson(JuridicalPersonEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model.LegalFormId, "Неверное значение входного параметра.");

                accountOrganizationPresenter.SaveJuridicalPerson(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult EditPhysicalPerson(PhysicalPersonEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model.LegalFormId, "Неверное значение входного параметра.");

                accountOrganizationPresenter.SavePhysicalPerson(model, UserSession.CurrentUserInfo);

                return Content("");               
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление организации

        /// <summary>
        /// Удаление организации
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int accountOrganizationId)
        {
            try
            {
                accountOrganizationPresenter.Delete(accountOrganizationId, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Связанные места хранения

        #region Добавление места хранения

        [HttpGet]
        public ActionResult StoragesList(int? orgId)
        {
            try
            {
                ValidationUtils.NotNull(orgId, "Неверное значение входного параметра.");

                var model = accountOrganizationPresenter.GetStorageListForAddition(orgId.Value, UserSession.CurrentUserInfo);                
                
                return PartialView("StorageSelectList", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        
        [HttpPost]
        public ActionResult StoragesList(LinkedStorageListViewModel model)
        {
            try
            {                
                if (model.OrganizationId == 0)
                {
                    throw new Exception("Неверное значение входного параметра.");
                }

                ValidationUtils.NotNull(model.StorageId, "Неверное значение входного параметра.");

                accountOrganizationPresenter.AddStorage(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление места хранения

        public ActionResult DeleteStorage(int? accountOrganizationId, short? storageId)
        {
            try
            {
                ValidationUtils.NotNull(accountOrganizationId, "Неверное значение входного параметра.");
                ValidationUtils.NotNull(storageId, "Неверное значение входного параметра.");

                accountOrganizationPresenter.RemoveStorage(accountOrganizationId.Value, storageId.Value, UserSession.CurrentUserInfo);                
               
                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Расчетные счета

        #region Создание расчетного счета

        /// <summary>
        /// Создание нового счета
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddRussianBankAccount(int accountOrganizationId)
        {
            try
            {
                var model = accountOrganizationPresenter.CreateRussianBankAccount(accountOrganizationId, UserSession.CurrentUserInfo);                

                return PartialView("~/Views/Organization/RussianBankAccountEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Создание нового счета
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddForeignBankAccount(int accountOrganizationId)
        {
            try
            {
                var model = accountOrganizationPresenter.CreateForeignBankAccount(accountOrganizationId, UserSession.CurrentUserInfo);

                return PartialView("~/Views/Organization/ForeignBankAccountEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Редактирование расчетного счета

        /// <summary>
        /// Редактирование счета
        /// </summary>
        [HttpGet]
        public ActionResult EditRussianBankAccount(int? accountOrganizationId, int? bankAccountId)
        {
            try
            {
                ValidationUtils.NotNull(accountOrganizationId, "Неверное значение входного параметра.");
                ValidationUtils.NotNull(bankAccountId, "Неверное значение входного параметра.");

                var model = accountOrganizationPresenter.EditRussianBankAccount(accountOrganizationId.Value, bankAccountId.Value, UserSession.CurrentUserInfo);
                
                return PartialView("~/Views/Organization/RussianBankAccountEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование счета
        /// </summary>
        [HttpGet]
        public ActionResult EditForeignBankAccount(int? accountOrganizationId, int? bankAccountId)
        {
            try
            {
                ValidationUtils.NotNull(accountOrganizationId, "Неверное значение входного параметра.");
                ValidationUtils.NotNull(bankAccountId, "Неверное значение входного параметра.");

                var model = accountOrganizationPresenter.EditForeignBankAccount(accountOrganizationId.Value, bankAccountId.Value, UserSession.CurrentUserInfo);

                return PartialView("~/Views/Organization/ForeignBankAccountEdit.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование расчетного счета
        /// </summary>        
        [HttpPost]
        public ActionResult EditRussianBankAccount(RussianBankAccountEditViewModel model)
        {
            try
            {
                accountOrganizationPresenter.SaveRussianBankAccount(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование расчетного счета
        /// </summary>        
        [HttpPost]
        public ActionResult EditForeignBankAccount(ForeignBankAccountEditViewModel model)
        {
            try
            {
                accountOrganizationPresenter.SaveForeignBankAccount(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Удаление расчетного счета

        /// <summary>
        /// Удаление расчетного счета
        /// </summary>
        /// <param name="bankAccountId">Идентификатор расчетного счета</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveRussianBankAccount(int accountOrganizationId, int bankAccountId)
        {
            try
            {
                accountOrganizationPresenter.DeleteRussianBankAccount(accountOrganizationId, bankAccountId, UserSession.CurrentUserInfo);
                
                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>  
        /// Удаление расчетного счета в иностранном банке
        /// </summary>
        /// <param name="bankAccountId">Идентификатор расчетного счета</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveForeignBankAccount(int accountOrganizationId, int bankAccountId)
        {
            try
            {
                accountOrganizationPresenter.DeleteForeignBankAccount(accountOrganizationId, bankAccountId, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }       

        #endregion

        #endregion

        #region Модальная форма выбора собственной организации

        /// <summary>
        /// Возвращает модальную форму для выбора собственной организации
        /// </summary>
        [HttpGet]
        public ActionResult SelectAccountOrganization()
        {
            try
            {
                var model = accountOrganizationPresenter.SelectAccountOrganization();

                return PartialView("AccountOrganizationSelector", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Возвращает модальную форму для выбора собственной организации, связанной с конкретным местом хранения
        /// </summary>
        [HttpGet]
        public ActionResult SelectAccountOrganizationForStorage(string storageId)
        {
            try
            {
                var model = accountOrganizationPresenter.SelectAccountOrganizationForStorage(ValidationUtils.TryGetShort(storageId));

                return PartialView("AccountOrganizationSelector", model);
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
        public ActionResult ShowAccountOrganizationSelectGrid(GridState state)
        {
            try
            {
                AccountOrganizationSelectGridViewModel data = accountOrganizationPresenter.GetAccountOrganizationSelectGrid(state);

                return PartialView("AccountOrganizationSelectGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

    }
}
