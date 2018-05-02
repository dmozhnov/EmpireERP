using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Report.Report0001;
using ERP.Wholesale.UI.ViewModels.Report.Report0002;
using ERP.Wholesale.UI.ViewModels.Report.Report0003;
using ERP.Wholesale.UI.ViewModels.Report.Report0004;
using ERP.Wholesale.UI.ViewModels.Report.Report0005;
using ERP.Wholesale.UI.ViewModels.Report.Report0006;
using ERP.Wholesale.UI.ViewModels.Report.Report0007;
using ERP.Wholesale.UI.ViewModels.Report.Report0008;
using ERP.Wholesale.UI.ViewModels.Report.Report0009;
using ERP.Wholesale.UI.ViewModels.Report.Report0010;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ReportController : WholesaleController
    {
        #region Поля

        private readonly IReportPresenter reportPresenter;
        private readonly IReport0001Presenter report0001Presenter;
        private readonly IReport0002Presenter report0002Presenter;
        private readonly IReport0003Presenter report0003Presenter;
        private readonly IReport0004Presenter report0004Presenter;
        private readonly IReport0005Presenter report0005Presenter;
        private readonly IReport0006Presenter report0006Presenter;
        private readonly IReport0007Presenter report0007Presenter;
        private readonly IReport0008Presenter report0008Presenter;
        private readonly IReport0009Presenter report0009Presenter;
        private readonly IReport0010Presenter report0010Presenter;

        #endregion

        #region Конструкторы

        public ReportController(IReportPresenter reportPresenter, IReport0001Presenter report0001Presenter, IReport0002Presenter report0002Presenter, IReport0003Presenter report0003Presenter,
            IReport0004Presenter report0004Presenter, IReport0005Presenter report0005Presenter, IReport0006Presenter report0006Presenter, IReport0007Presenter report0007Presenter,
            IReport0008Presenter report0008Presenter, IReport0009Presenter report0009Presenter, IReport0010Presenter report0010Presenter)
        {
            this.reportPresenter = reportPresenter;
            this.report0001Presenter = report0001Presenter;
            this.report0002Presenter = report0002Presenter;
            this.report0003Presenter = report0003Presenter;
            this.report0004Presenter = report0004Presenter;
            this.report0005Presenter = report0005Presenter;
            this.report0006Presenter = report0006Presenter;
            this.report0007Presenter = report0007Presenter;
            this.report0008Presenter = report0008Presenter;
            this.report0009Presenter = report0009Presenter;
            this.report0010Presenter = report0010Presenter;
        }

        #endregion

        #region Методы

        #region Список отчетов

        public ActionResult ArticlesAndPricesList()
        {
            try
            {
                return View(reportPresenter.ArticlesAndPricesList(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowArticlesAndPricesList(GridState state)
        {
            try
            {
                return PartialView("ReportsGrid", reportPresenter.GetArticlesAndPricesReportGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ArticleSaleList()
        {
            try
            {
                return View("ArticlesAndPricesList", reportPresenter.ArticleSaleList(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowArticleSaleList(GridState state)
        {
            try
            {
                return PartialView("ReportsGrid", reportPresenter.GetArticleSaleReportGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Товары и цены

        #region Report0001

        public ActionResult Report0001Settings(string backURL)
        {
            try
            {
                return View("~/Views/Report/Report0001/Report0001Settings.aspx", report0001Presenter.Report0001Settings(backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0001(Report0001SettingsViewModel settings)
        {
            try
            {
                return View("~/Views/Report/Report0001/Report0001.cshtml", report0001Presenter.Report0001(settings, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0001ExportToExcel(Report0001SettingsViewModel settings)
        {
            try
            {
                return ExcelFile(report0001Presenter.Report0001ExportToExcel(settings, UserSession.CurrentUserInfo), "Report0001.xlsx");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Report0004

        public ActionResult Report0004Settings(string backURL)
        {
            try
            {
                return View("~/Views/Report/Report0004/Report0004Settings.aspx", report0004Presenter.Report0004Settings(backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0004(Report0004SettingsViewModel settings)
        {
            try
            {
                return View("~/Views/Report/Report0004/Report0004.cshtml", report0004Presenter.Report0004(settings, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0004ExportToExcel(Report0004SettingsViewModel settings)
        {
            try
            {
                return ExcelFile(report0004Presenter.Report0004ExportToExcel(settings, UserSession.CurrentUserInfo), "Report0004.xlsx");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Report0005

        public ActionResult Report0005Settings(string backURL)
        {
            try
            {
                return View("~/Views/Report/Report0005/Report0005Settings.aspx", report0005Presenter.Report0005Settings(backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0005(Report0005SettingsViewModel settings)
        {
            try
            {
                return View("~/Views/Report/Report0005/Report0005.cshtml", report0005Presenter.Report0005(settings, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Report0008

        public ActionResult Report0008Settings(string backURL)
        {
            try
            {
                return View("~/Views/Report/Report0008/Report0008Settings.aspx", report0008Presenter.Report0008Settings(backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Подгрузка статусов накладных для вывода в отчет
        /// </summary>
        public ActionResult Report0008_GetWaybillOptionList(string waybillTypeId, string dateTypeId)
        {
            try
            {
                var obj = report0008Presenter.GetWaybillOptionList(waybillTypeId, dateTypeId, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Подгрузка дат для сортировки для вывода в отчет
        /// </summary>
        public ActionResult Report0008_GetWaybillSortDateTypeList(string waybillTypeId, string waybillOptionId)
        {
            try
            {
                var obj = report0008Presenter.GetWaybillSortDateTypeList(waybillTypeId, waybillOptionId, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Подгрузка типов группировки
        /// </summary>
        public ActionResult Report0008_GetWaybillGroupingTypeList(string waybillTypeId)
        {
            try
            {
                var obj = report0008Presenter.GetWaybillGroupingTypeList(waybillTypeId, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Подгрузка типов дат
        /// </summary>
        public ActionResult Report0008_GetWaybillDateTypeList(string waybillTypeId)
        {
            try
            {
                var obj = report0008Presenter.GetWaybillDateTypeList(waybillTypeId, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }


        /// <summary>
        /// Подгрузка формы для выбора клиентов
        /// </summary>
        public ActionResult Report0008_GetClientSelector()
        {
            try
            {
                return PartialView("~/Views/Report/Report0008/Report0008_ClientSelector.cshtml", report0008Presenter.GetClientSelector(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Подгрузка формы для выбора поставщиков
        /// </summary>
        public ActionResult Report0008_GetProviderSelector()
        {
            try
            {
                return PartialView("~/Views/Report/Report0008/Report0008_ProviderSelector.cshtml", report0008Presenter.GetProviderSelector(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Построение отчета
        /// </summary>
        public ActionResult Report0008(Report0008SettingsViewModel settings)
        {
            try
            {
                return PartialView("~/Views/Report/Report0008/Report0008.cshtml", report0008Presenter.Report0008(settings, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0008ExportToExcel(Report0008SettingsViewModel settings)
        {
            try
            {
                return ExcelFile(report0008Presenter.Report0008ExportToExcel(settings, UserSession.CurrentUserInfo), "Report0008.xlsx");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Report0009

        public ActionResult Report0009Settings(string backURL)
        {
            try
            {
                return View("~/Views/Report/Report0009/Report0009Settings.aspx", report0009Presenter.Report0009Settings(backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0009(Report0009SettingsViewModel settings)
        {
            try
            {
                return View("~/Views/Report/Report0009/Report0009.cshtml", report0009Presenter.Report0009(settings, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0009ExportToExcel(Report0009SettingsViewModel settings)
        {
            try
            {
                return ExcelFile(report0009Presenter.Report0009ExportToExcel(settings, UserSession.CurrentUserInfo), "Report0009.xlsx");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Продажи

        #region Report0002

        public ActionResult Report0002Settings(string backURL)
        {
            try
            {
                return View("~/Views/Report/Report0002/Report0002Settings.aspx", report0002Presenter.Report0002Settings(backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0002(Report0002SettingsViewModel settings)
        {
            try
            {
                return View("~/Views/Report/Report0002/Report0002.cshtml", report0002Presenter.Report0002(settings, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0002ExportToExcel(Report0002SettingsViewModel settings)
        {
            try
            {
                return ExcelFile(report0002Presenter.Report0002ExportToExcel(settings, UserSession.CurrentUserInfo), "Report0002.xlsx");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0002StorageSelector(string inAccountingPrice)
        {
            try
            {
                var model = report0002Presenter.GetStorageSelector(inAccountingPrice, UserSession.CurrentUserInfo);

                return View("~/Views/Report/Report0002/Report0002StorageSelector.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        #endregion

        #region Report0003

        public ActionResult Report0003Settings(string backURL)
        {
            try
            {
                return View("~/Views/Report/Report0003/Report0003Settings.aspx", report0003Presenter.Report0003Settings(backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0003(Report0003SettingsViewModel settings)
        {
            try
            {
                return View("~/Views/Report/Report0003/Report0003.cshtml", report0003Presenter.Report0003(settings, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0003ExportToExcel(Report0003SettingsViewModel settings)
        {
            try
            {
                return ExcelFile(report0003Presenter.Report0003ExportToExcel(settings, UserSession.CurrentUserInfo), "Report0003.xlsx");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Report0006

        public ActionResult Report0006Settings(string backURL)
        {
            try
            {
                return View("~/Views/Report/Report0006/Report0006Settings.aspx", report0006Presenter.Report0006Settings(backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0006(Report0006SettingsViewModel settings)
        {
            try
            {
                return View("~/Views/Report/Report0006/Report0006.cshtml", report0006Presenter.Report0006(settings, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0006PrintingFormSettings(string clientId, string clientOrganizationId)
        {
            try
            {
                return PartialView("~/Views/Report/Report0006/Report0006PrintingFormSettings.ascx", report0006Presenter.Report0006PrintingFormSettings(
                    !String.IsNullOrEmpty(clientId) ? ValidationUtils.TryGetInt(clientId) : (int?)null,
                    !String.IsNullOrEmpty(clientOrganizationId) ? ValidationUtils.TryGetInt(clientOrganizationId) : (int?)null,
                    UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0006PrintingForm(Report0006PrintingFormSettingsViewModel settings)
        {
            try
            {
                return View("~/Views/Report/Report0006/Report0006PrintingForm.aspx", report0006Presenter.Report0006PrintingForm(settings, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0006ExportToExcel(Report0006SettingsViewModel settings)
        {
            try
            {
                return ExcelFile(report0006Presenter.Report0006ExportToExcel(settings, UserSession.CurrentUserInfo), "Report0006.xlsx");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Report0007

        public ActionResult Report0007Settings(string backURL)
        {
            try
            {
                return View("~/Views/Report/Report0007/Report0007Settings.aspx", report0007Presenter.Report0007Settings(
                    backURL,
                    UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0007(Report0007SettingsViewModel settings)
        {
            try
            {
                return View("~/Views/Report/Report0007/Report0007.cshtml", report0007Presenter.Report0007(settings, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0007ExportToExcel(Report0007SettingsViewModel settings)
        {
            try
            {
                return ExcelFile(report0007Presenter.Report0007ExportToExcel(settings, UserSession.CurrentUserInfo), "Report0007.xlsx");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Report0010

        public ActionResult Report0010Settings(string backURL)
        {
            try
            {
                return View("~/Views/Report/Report0010/Report0010Settings.aspx", report0010Presenter.Report0010Settings(backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0010(Report0010SettingsViewModel settings)
        {
            try
            {
                return View("~/Views/Report/Report0010/Report0010.cshtml", report0010Presenter.Report0010(settings, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult Report0010ExportToExcel(Report0010SettingsViewModel settings)
        {
            try
            {
                return ExcelFile(report0010Presenter.Report0010ExportToExcel(settings, UserSession.CurrentUserInfo), "Report0010.xlsx");
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