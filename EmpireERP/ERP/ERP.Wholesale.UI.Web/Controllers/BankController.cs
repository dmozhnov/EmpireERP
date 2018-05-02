using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Bank;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class BankController : WholesaleController
    {
        #region Поля

        private readonly IBankPresenter bankPresenter;

        #endregion

        #region Конструкторы

        public BankController(IBankPresenter bankPresenter)
        {
            this.bankPresenter = bankPresenter;
        }

        #endregion

        #region Методы

        #region Список
        
        public ActionResult List()
        {
            try
            {
                return View(bankPresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ShowRussianBankGrid(GridState state)
        {
            try
            {
                return PartialView("RussianBankGrid", bankPresenter.GetRussianBankGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ShowForeignBankGrid(GridState state)
        {
            try
            {
                return PartialView("ForeignBankGrid", bankPresenter.GetForeignBankGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Редактирование

        #region Российские банки
        
        public ActionResult AddRussianBank()
        {
            try
            {
                return PartialView("RussianBankEdit", bankPresenter.AddRussianBank(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult EditRussianBank(string id)
        {
            try
            {
                int bankId = ValidationUtils.TryGetInt(id);

                return PartialView("RussianBankEdit", bankPresenter.EditRussianBank(bankId, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        [HttpPost]
        public ActionResult EditRussianBank(RussianBankEditViewModel model)
        {
            try
            {
                bankPresenter.SaveRussianBank(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult DeleteRussianBank(string id)
        {
            try
            {
                int bankId = ValidationUtils.TryGetInt(id);

                bankPresenter.DeleteRussianBank(bankId, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Иностранные банки

        public ActionResult AddForeignBank()
        {
            try
            {
                return PartialView("ForeignBankEdit", bankPresenter.AddForeignBank(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult EditForeignBank(string id)
        {
            try
            {
                int bankId = ValidationUtils.TryGetInt(id);

                return PartialView("ForeignBankEdit", bankPresenter.EditForeignBank(bankId, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        [HttpPost]
        public ActionResult EditForeignBank(ForeignBankEditViewModel model)
        {
            try
            {
                bankPresenter.SaveForeignBank(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult DeleteForeignBank(string id)
        {
            try
            {
                int bankId = ValidationUtils.TryGetInt(id);

                bankPresenter.DeleteForeignBank(bankId, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
