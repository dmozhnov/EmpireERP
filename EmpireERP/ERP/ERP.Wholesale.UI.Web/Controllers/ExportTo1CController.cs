using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Export._1C;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ExportTo1CController : WholesaleController
    {
        #region Поля

        private readonly IExportTo1CPresenter exportTo1CPresenter;

        #endregion

        #region Конструкторы

        public ExportTo1CController(IExportTo1CPresenter exportTo1CPresenter)
        {
            this.exportTo1CPresenter = exportTo1CPresenter;
        }

        #endregion

        #region Методы

        public ActionResult ExportTo1CSettings()
        {
            try
            {
                return View("~/Views/Export/1C/ExportTo1CSettings.aspx", exportTo1CPresenter.ExportTo1CSettings(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ExportOperationsTo1C(ExportTo1CSettingsViewModel settings)
        {
            try
            {
                var fileName = "";
                
                switch (settings.OperationTypeId)
                {
                    // Реализация + передача на комиссию
                    case "1": fileName = "Реализация.xlsx"; break;

                    // Внутрискладское перемещение в рамках организации
                    case "2": fileName = "Перемещение.xlsx"; break;

                    // Возвраты от клиентов + возвраты комиссионеров
                    case "3": fileName = "ВозвратОтПокуп.xlsx"; break;

                    // Поступление на комиссию
                    case "4": fileName = "Поступление.xlsx"; break;

                    default:
                        throw new Exception("Неизвестный тип операции для экспорта данных в 1С.");
                }

                return ExcelFile(exportTo1CPresenter.ExportOperationsTo1C(settings, UserSession.CurrentUserInfo), fileName);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult GetCommissionaireOrganizationsList()
        {
            try
            {
                return PartialView("~/Views/Export/1C/ExportTo1CSettingsSelector.cshtml", exportTo1CPresenter.GetCommissionaireOrganizationsList());
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult GetReturnsFromCommissionaireOrganizationsList()
        {
            try
            {
                return PartialView("~/Views/Export/1C/ExportTo1CSettingsSelector.cshtml", exportTo1CPresenter.GetReturnsFromCommissionaireOrganizationsList());
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult GetReturnsAcceptedByCommissionaireOrganizationsList()
        {
            try
            {
                return PartialView("~/Views/Export/1C/ExportTo1CSettingsSelector.cshtml", exportTo1CPresenter.GetReturnsAcceptedByCommissionaireOrganizationsList());
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult GetConsignorOrganizationsList()
        {
            try
            {
                return PartialView("~/Views/Export/1C/ExportTo1CSettingsSelector.cshtml", exportTo1CPresenter.GetConsignorOrganizationsList());
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        
        #endregion

    }
}
