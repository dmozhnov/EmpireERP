using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ProductionOrder;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class ProductionOrderController : WholesaleController
    {
        #region Поля

        private readonly IProductionOrderPresenter productionOrderPresenter;

        #endregion

        #region Конструкторы

        public ProductionOrderController(IProductionOrderPresenter productionOrderPresenter)
        {
            this.productionOrderPresenter = productionOrderPresenter;
        }

        #endregion

        #region Методы

        #region Список заказов

        public ActionResult List()
        {
            try
            {
                return View(productionOrderPresenter.List(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowActiveProductionOrderGrid(GridState state)
        {
            try
            {
                return PartialView("ActiveProductionOrderGrid", productionOrderPresenter.GetActiveProductionOrderGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowClosedProductionOrderGrid(GridState state)
        {
            try
            {
                return PartialView("ClosedProductionOrderGrid", productionOrderPresenter.GetClosedProductionOrderGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / редактирование

        public ActionResult Create(string backURL, string producerId = "")
        {
            try
            {
                int? id = null;
                if (!String.IsNullOrEmpty(producerId))
                {
                    id = ValidationUtils.TryGetInt(producerId);
                }

                return View("Edit", productionOrderPresenter.Create(backURL, id, UserSession.CurrentUserInfo));
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
                return View(productionOrderPresenter.Edit(ValidationUtils.TryGetGuid(id), backURL, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Save(ProductionOrderEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Content(productionOrderPresenter.Save(model, UserSession.CurrentUserInfo).ToString());
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult Close(string productionOrderId)
        {
            try
            {
                TempData["Message"] = productionOrderPresenter.Close(ValidationUtils.TryGetGuid(productionOrderId), UserSession.CurrentUserInfo);
                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult Open(string productionOrderId)
        {
            try
            {
                productionOrderPresenter.Open(ValidationUtils.TryGetGuid(productionOrderId), UserSession.CurrentUserInfo);
                TempData["Message"] = "Заказ открыт.";
                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Позиции

        #region Добавление / редактирование

        [HttpGet]
        public ActionResult AddRow(string batchId)
        {
            try
            {
                var batchGuid = ValidationUtils.TryGetNotEmptyGuid(batchId);

                var model = productionOrderPresenter.AddRow(batchGuid, UserSession.CurrentUserInfo);

                return PartialView("ProductionOrderBatchRowEdit", model);                
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult EditRow(string batchId, string rowId)
        {
            try
            {
                var batchGuid = ValidationUtils.TryGetNotEmptyGuid(batchId);
                var rowGuid = ValidationUtils.TryGetNotEmptyGuid(rowId);

                var model = productionOrderPresenter.EditRow(batchGuid, rowGuid, UserSession.CurrentUserInfo);

                return PartialView("ProductionOrderBatchRowEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult EditRow(ProductionOrderBatchRowEditViewModel model)
        {
            try 
            {
                return Json(productionOrderPresenter.SaveRow(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Получение информации о товаре по товару
        /// </summary>
        /// <param name="articleId">Идентификатор товара</param>
        /// <returns>Json</returns>
        [HttpGet]
        public ActionResult GetArticleInfo(string articleId, string producerId)
        {
            try
            {
                var result = productionOrderPresenter.GetArticleInfo(ValidationUtils.TryGetInt(articleId), ValidationUtils.TryGetInt(producerId));

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult DeleteRow(string batchId, string rowId)
        {
            try
            {
                var result = productionOrderPresenter.DeleteRow(ValidationUtils.TryGetGuid(batchId), ValidationUtils.TryGetGuid(rowId), UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }


        #endregion

        #endregion

        #region Детали

        #region Детали общие

        public ActionResult Details(string id, string backUrl)
        {
            try
            {
                var model = productionOrderPresenter.Details(ValidationUtils.TryGetNotEmptyGuid(id), backUrl, UserSession.CurrentUserInfo);
                model.TaskGrid.GridPartialViewAction = "/ProductionOrder/ShowTaskGrid/";
                model.TaskGrid.HelpContentUrl = "/Help/GetHelp_ProductionOrder_Details_TaskGrid";

                return View(model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
 
        #endregion

        #region Гриды

        [HttpPost]
        public ActionResult ShowProductionOrderBatchGrid(string id)
        {
            try
            {
                return PartialView("ProductionOrderBatchGrid", productionOrderPresenter.GetBatchGrid(ValidationUtils.TryGetNotEmptyGuid(id), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowProductionOrderTransportSheetGrid(GridState state)
        {
            try
            {
                return PartialView("ProductionOrderTransportSheetGrid", productionOrderPresenter.GetProductionOrderTransportSheetGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowProductionOrderExtraExpensesSheetGrid(GridState state)
        {
            try
            {
                return PartialView("ProductionOrderExtraExpensesSheetGrid", productionOrderPresenter.GetProductionOrderExtraExpensesSheetGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowProductionOrderCustomsDeclarationGrid(GridState state)
        {
            try
            {
                return PartialView("ProductionOrderCustomsDeclarationGrid", productionOrderPresenter.GetProductionOrderCustomsDeclarationGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowProductionOrderPaymentGrid(GridState state)
        {
            try
            {
                return PartialView("ProductionOrderPaymentGrid", productionOrderPresenter.GetProductionOrderPaymentGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }
        
        [HttpPost]
        public ActionResult ShowProductionOrderDocumentPackageGrid(GridState state)
        {
            try
            {
                return PartialView("ProductionOrderDocumentPackageGrid", productionOrderPresenter.GetDocumentPackageGrid(state, UserSession.CurrentUserInfo));
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
                var model = productionOrderPresenter.GetTaskGrid(state, UserSession.CurrentUserInfo);
                model.GridPartialViewAction = "/ProductionOrder/ShowTaskGrid/";

                return PartialView("~/Views/Task/NewTaskGrid.ascx", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region График исполнения заказа

        [HttpGet]
        public ActionResult ShowOrderExecutionGraph(string id)
        {
            try
            {
                var guid = ValidationUtils.TryGetNotEmptyGuid(id);
                var result = productionOrderPresenter.GetExecutionGraphData(guid, UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Плановый

        public ActionResult EditPlannedExpenses(string id)
        {
            try
            {
                var productionOrderId = ValidationUtils.TryGetGuid(id);

                var model = productionOrderPresenter.EditPlannedExpenses(productionOrderId, UserSession.CurrentUserInfo);

                return PartialView("ProductionOrderPlannedExpensesEdit", model);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SavePlannedExpenses(ProductionOrderPlannedExpensesEditViewModel model)
        {
            try
            {
                var obj = productionOrderPresenter.SavePlannedExpenses(model, UserSession.CurrentUserInfo);

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Работа с партиями заказа

        [HttpGet]
        public ActionResult GetProductionOrderBatchName(string productionOrderBatchId)
        {
            try
            {
                return Content(productionOrderPresenter.GetProductionOrderBatchName(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId),
                                                                                    UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveProductionOrderBatch(ProductionOrderBatchEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Content(productionOrderPresenter.SaveProductionOrderBatch(model, UserSession.CurrentUserInfo).ToString());
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult AddProductionOrderBatch(string productionOrderId)
        {
            try
            {
                return PartialView("ProductionOrderAddBatch",productionOrderPresenter.AddProductionOrderBatch(ValidationUtils.TryGetNotEmptyGuid(productionOrderId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult RenameProductionOrderBatch(string productionOrderBatchId)
        {
            try
            {
                return PartialView("ProductionOrderAddBatch", productionOrderPresenter.RenameProductionOrderBatch(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult DeleteProductionOrderBatch(string productionOrderBatchId)
        {
            try
            {
                productionOrderPresenter.DeleteProductionOrderBatch(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo);
                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Детали партии заказа

        #region Детали общие

        public ActionResult ProductionOrderBatchDetails(string id, string backUrl)
        {
            try
            {
                return View(productionOrderPresenter.ProductionOrderBatchDetails(ValidationUtils.TryGetNotEmptyGuid(id), backUrl, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Грид позиций

        [HttpPost]
        public ActionResult ShowProductionOrderBatchRowGrid(GridState state)
        {
            try
            {
                GridData data = productionOrderPresenter.GetProductionOrderBatchRowGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ProductionOrderBatchRowGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Изменение курса валюты

        /// <summary>
        /// Установка нового курса валюты
        /// </summary>
        [HttpPost]
        public ActionResult ChangeCurrencyRate(string productionOrderId, string currencyId, string currencyRateId)
        {
            try
            {
                int? _currencyRateId = !String.IsNullOrEmpty(currencyRateId) ? ValidationUtils.TryGetInt(currencyRateId) : (int?)null;

                return Json(productionOrderPresenter.ChangeCurrencyRate(ValidationUtils.TryGetNotEmptyGuid(productionOrderId), ValidationUtils.TryGetShort(currencyId),
                    _currencyRateId, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / редактирование контракта

        /// <summary>
        /// Создание контракта
        /// </summary>
        /// <param name="dealId"></param>
        [HttpGet]
        public ActionResult CreateContract(string productionOrderId)
        {
            try
            {
                return PartialView("ProductionOrderContractEdit", productionOrderPresenter.CreateContract(ValidationUtils.TryGetNotEmptyGuid(productionOrderId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Создание / редактирование контракта
        /// </summary>
        [HttpGet]
        public ActionResult EditContract(string productionOrderId)
        {
            try
            {
                return PartialView("ProductionOrderContractEdit", productionOrderPresenter.EditContract(ValidationUtils.TryGetNotEmptyGuid(productionOrderId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Создание / редактирование контракта
        /// </summary>
        [HttpPost]
        public ActionResult SaveContract(ProductionOrderContractEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Json(productionOrderPresenter.SaveContract(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Создание приходной накладной, связанной с партией заказа

        [HttpGet]
        public ActionResult CheckPossibilityToCreateReceiptWaybill(string productionOrderBatchId)
        {
            try
            {
                productionOrderPresenter.CheckPossibilityToCreateReceiptWaybill(ValidationUtils.TryGetGuid(productionOrderBatchId), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Выбор типа выбора валюты для транспортных листов и листов дополнительных расходов

        /// <summary>
        /// Выбор способа выбора валюты к транспортному листу
        /// </summary>
        /// <param name="productionOrderId">Код заказа</param>
        [HttpGet]
        public ActionResult ProductionOrderCurrencyDeterminationTypeSelect(string productionOrderId, string productionOrderCurrencyDocumentType)
        {
            try
            {
                return PartialView("ProductionOrderCurrencyDeterminationTypeSelector",
                    productionOrderPresenter.ProductionOrderCurrencyDeterminationTypeSelect(ValidationUtils.TryGetNotEmptyGuid(productionOrderId),
                    ValidationUtils.TryGetByte(productionOrderCurrencyDocumentType), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / редактирование / удаление транспортного листа

        /// <summary>
        /// Добавление транспортного листа
        /// </summary>
        [HttpPost]
        public ActionResult AddProductionOrderTransportSheet(ProductionOrderCurrencyDeterminationTypeSelectorViewModel model)
        {
            try
            {
                return PartialView("ProductionOrderTransportSheetEdit", productionOrderPresenter.AddProductionOrderTransportSheet(model, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование транспортного листа
        /// </summary>
        /// <param name="productionOrderId">Код заказа</param>
        /// <param name="transportSheetId">Код транспортного листа</param>
        [HttpGet]
        public ActionResult EditProductionOrderTransportSheet(string productionOrderId, string transportSheetId)
        {
            try
            {
                return PartialView("ProductionOrderTransportSheetEdit",
                    productionOrderPresenter.EditProductionOrderTransportSheet(ValidationUtils.TryGetNotEmptyGuid(productionOrderId),
                    ValidationUtils.TryGetNotEmptyGuid(transportSheetId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Сохранение транспортного листа
        /// </summary>
        /// <param name="model">Модель</param>
        [HttpPost]
        public ActionResult SaveProductionOrderTransportSheet(ProductionOrderTransportSheetEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Json(productionOrderPresenter.SaveProductionOrderTransportSheet(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Удаление транспортного листа
        /// </summary>
        /// <param name="productionOrderId">Код заказа</param>
        /// <param name="transportSheetId">Код транспортного листа</param>
        [HttpPost]
        public ActionResult DeleteProductionOrderTransportSheet(string productionOrderId, string transportSheetId)
        {
            try
            {
                return Json(productionOrderPresenter.DeleteProductionOrderTransportSheet(ValidationUtils.TryGetNotEmptyGuid(productionOrderId),
                    ValidationUtils.TryGetNotEmptyGuid(transportSheetId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / редактирование / удаление листа дополнительных расходов

        /// <summary>
        /// Добавление листа дополнительных расходов
        /// </summary>
        [HttpPost]
        public ActionResult AddProductionOrderExtraExpensesSheet(ProductionOrderCurrencyDeterminationTypeSelectorViewModel model)
        {
            try
            {
                return PartialView("ProductionOrderExtraExpensesSheetEdit", productionOrderPresenter.AddProductionOrderExtraExpensesSheet(model, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование листа дополнительных расходов
        /// </summary>
        /// <param name="productionOrderId">Код заказа</param>
        /// <param name="extraExpensesSheetId">Код листа дополнительных расходов</param>
        [HttpGet]
        public ActionResult EditProductionOrderExtraExpensesSheet(string productionOrderId, string extraExpensesSheetId)
        {
            try
            {
                return PartialView("ProductionOrderExtraExpensesSheetEdit",
                    productionOrderPresenter.EditProductionOrderExtraExpensesSheet(ValidationUtils.TryGetNotEmptyGuid(productionOrderId),
                    ValidationUtils.TryGetNotEmptyGuid(extraExpensesSheetId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Сохранение листа дополнительных расходов
        /// </summary>
        /// <param name="model">Модель</param>
        [HttpPost]
        public ActionResult SaveProductionOrderExtraExpensesSheet(ProductionOrderExtraExpensesSheetEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Json(productionOrderPresenter.SaveProductionOrderExtraExpensesSheet(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Удаление листа дополнительных расходов
        /// </summary>
        /// <param name="productionOrderId">Код заказа</param>
        /// <param name="extraExpensesSheetId">Код листа дополнительных расходов</param>
        [HttpPost]
        public ActionResult DeleteProductionOrderExtraExpensesSheet(string productionOrderId, string extraExpensesSheetId)
        {
            try
            {
                return Json(productionOrderPresenter.DeleteProductionOrderExtraExpensesSheet(ValidationUtils.TryGetNotEmptyGuid(productionOrderId),
                    ValidationUtils.TryGetNotEmptyGuid(extraExpensesSheetId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / разнесение по партиям / редактирование / удаление таможенного листа

        /// <summary>
        /// Добавление таможенного листа
        /// </summary>
        /// <param name="productionOrderId">Код заказа</param>
        [HttpGet]
        public ActionResult AddProductionOrderCustomsDeclaration(string productionOrderId)
        {
            try
            {
                return PartialView("ProductionOrderCustomsDeclarationEdit",
                    productionOrderPresenter.AddProductionOrderCustomsDeclaration(ValidationUtils.TryGetNotEmptyGuid(productionOrderId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование таможенного листа
        /// </summary>
        /// <param name="productionOrderId">Код заказа</param>
        /// <param name="customsDeclarationId">Код таможенного листа</param>
        [HttpGet]
        public ActionResult EditProductionOrderCustomsDeclaration(string productionOrderId, string customsDeclarationId)
        {
            try
            {
                return PartialView("ProductionOrderCustomsDeclarationEdit",
                    productionOrderPresenter.EditProductionOrderCustomsDeclaration(ValidationUtils.TryGetNotEmptyGuid(productionOrderId),
                    ValidationUtils.TryGetNotEmptyGuid(customsDeclarationId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Сохранение таможенного листа
        /// </summary>
        /// <param name="model">Модель</param>
        [HttpPost]
        public ActionResult SaveProductionOrderCustomsDeclaration(ProductionOrderCustomsDeclarationEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Json(productionOrderPresenter.SaveProductionOrderCustomsDeclaration(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Удаление таможенного листа
        /// </summary>
        /// <param name="productionOrderId">Код заказа</param>
        /// <param name="customsDeclarationId">Код таможенного листа</param>
        [HttpPost]
        public ActionResult DeleteProductionOrderCustomsDeclaration(string productionOrderId, string customsDeclarationId)
        {
            try
            {
                return Json(productionOrderPresenter.DeleteProductionOrderCustomsDeclaration(ValidationUtils.TryGetNotEmptyGuid(productionOrderId),
                    ValidationUtils.TryGetNotEmptyGuid(customsDeclarationId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Добавление / удаление оплаты

        /// <summary>
        /// Выбор типа оплаты
        /// </summary>
        [HttpGet]
        public ActionResult ProductionOrderPaymentTypeSelect(string productionOrderId)
        {
            try
            {
                return PartialView("ProductionOrderPaymentTypeSelector", productionOrderPresenter.ProductionOrderPaymentTypeSelect(
                    ValidationUtils.TryGetNotEmptyGuid(productionOrderId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Выбор оплачиваемого документа
        /// </summary>
        [HttpGet]
        public ActionResult ProductionOrderPaymentDocumentSelect(string productionOrderId, string productionOrderPaymentTypeId)
        {
            try
            {
                return PartialView("ProductionOrderPaymentDocumentSelector",
                    productionOrderPresenter.ProductionOrderPaymentDocumentSelect(ValidationUtils.TryGetNotEmptyGuid(productionOrderId),
                    ValidationUtils.TryGetByte(productionOrderPaymentTypeId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Показать грид документов, на которые относится оплата по заказу
        /// </summary>
        [HttpPost]
        public ActionResult ShowProductionOrderPaymentDocumentGrid(GridState state)
        {
            try
            {
                return PartialView("ProductionOrderPaymentDocumentGrid",
                    productionOrderPresenter.GetProductionOrderPaymentDocumentGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Добавление новой оплаты
        /// </summary>
        /// <param name="productionOrderId">Код заказа</param>
        [HttpGet]
        public ActionResult CreateProductionOrderPayment(string productionOrderId, string productionOrderPaymentTypeId, string productionOrderPaymentDocumentId)
        {
            try
            {
                return PartialView("ProductionOrderPaymentEdit", productionOrderPresenter.CreateProductionOrderPayment(ValidationUtils.TryGetNotEmptyGuid(productionOrderId),
                    ValidationUtils.TryGetByte(productionOrderPaymentTypeId), ValidationUtils.TryGetGuid(productionOrderPaymentDocumentId),
                    UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Сохранение оплаты
        /// </summary>
        /// <param name="model">Модель оплаты</param>
        [HttpPost]
        public ActionResult SaveProductionOrderPayment(ProductionOrderPaymentEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Json(productionOrderPresenter.SaveProductionOrderPayment(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Удаление оплаты
        /// </summary>
        [HttpPost]
        public ActionResult DeleteProductionOrderPayment(string productionOrderId, string paymentId)
        {
            try
            {
                return Json(productionOrderPresenter.DeleteProductionOrderPayment(ValidationUtils.TryGetNotEmptyGuid(productionOrderId),
                    ValidationUtils.TryGetNotEmptyGuid(paymentId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Работа со статусами и утверждениями

        [HttpPost]
        public ActionResult Accept(string productionOrderBatchId)
        {
            try
            {
                return Json(productionOrderPresenter.Accept(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelAcceptance(string productionOrderBatchId)
        {
            try
            {
                return Json(productionOrderPresenter.CancelAcceptance(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult Approve(string productionOrderBatchId)
        {
            try
            {
                return Json(productionOrderPresenter.Approve(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelApprovement(string productionOrderBatchId)
        {
            try
            {
                return Json(productionOrderPresenter.CancelApprovement(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ApproveByLineManager(string productionOrderBatchId)
        {
            try
            {
                return Json(productionOrderPresenter.ApproveByLineManager(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelApprovementByLineManager(string productionOrderBatchId)
        {
            try
            {
                return Json(productionOrderPresenter.CancelApprovementByLineManager(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ApproveByFinancialDepartment(string productionOrderBatchId)
        {
            try
            {
                return Json(productionOrderPresenter.ApproveByFinancialDepartment(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelApprovementByFinancialDepartment(string productionOrderBatchId)
        {
            try
            {
                return Json(productionOrderPresenter.CancelApprovementByFinancialDepartment(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ApproveBySalesDepartment(string productionOrderBatchId)
        {
            try
            {
                return Json(productionOrderPresenter.ApproveBySalesDepartment(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelApprovementBySalesDepartment(string productionOrderBatchId)
        {
            try
            {
                return Json(productionOrderPresenter.CancelApprovementBySalesDepartment(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ApproveByAnalyticalDepartment(string productionOrderBatchId)
        {
            try
            {
                return Json(productionOrderPresenter.ApproveByAnalyticalDepartment(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelApprovementByAnalyticalDepartment(string productionOrderBatchId)
        {
            try
            {
                return Json(productionOrderPresenter.CancelApprovementByAnalyticalDepartment(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ApproveByProjectManager(string productionOrderBatchId)
        {
            try
            {
                return Json(productionOrderPresenter.ApproveByProjectManager(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult CancelApprovementByProjectManager(string productionOrderBatchId)
        {
            try
            {
                return Json(productionOrderPresenter.CancelApprovementByProjectManager(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Редактирование этапов

        /// <summary>
        /// Получение модальной формы редактирования этапов
        /// </summary>
        [HttpGet]
        public ActionResult EditStages(string productionOrderBatchId)
        {
            try
            {
                return PartialView("ProductionOrderBatchEditStages", productionOrderPresenter.EditStages(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #region Грид этапов

        [HttpPost]
        public ActionResult ShowProductionOrderBatchStageGrid(GridState state)
        {
            try
            {
                GridData data = productionOrderPresenter.GetProductionOrderBatchStageGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ProductionOrderBatchStageGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        public ActionResult ClearCustomStages(string productionOrderBatchId)
        {
            try
            {
                return Json(productionOrderPresenter.ClearCustomStages(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), 0, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult AddStage(string productionOrderBatchId, string idPreviousStage, string position)
        {
            try
            {
                return PartialView("ProductionOrderBatchStageEdit",
                    productionOrderPresenter.AddStage(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId),
                    ValidationUtils.TryGetNotEmptyGuid(idPreviousStage), ValidationUtils.TryGetShort(position), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult EditStage(string productionOrderBatchId, string id)
        {
            try
            {
                return PartialView("ProductionOrderBatchStageEdit",
                    productionOrderPresenter.EditStage(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId),
                    ValidationUtils.TryGetNotEmptyGuid(id), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult SaveStage(ProductionOrderBatchStageEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                return Json(productionOrderPresenter.SaveStage(model, UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult DeleteStage(string productionOrderBatchId, string id, string isReturnBatchDetails = "0")
        {
            try
            {
                return Json(productionOrderPresenter.DeleteStage(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId),
                    ValidationUtils.TryGetNotEmptyGuid(id), ValidationUtils.TryGetByte(isReturnBatchDetails), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult MoveStageUp(string productionOrderBatchId, string id, string isReturnBatchDetails = "0")
        {
            try
            {
                return Json(productionOrderPresenter.MoveStageUp(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId),
                    ValidationUtils.TryGetNotEmptyGuid(id), ValidationUtils.TryGetByte(isReturnBatchDetails), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult MoveStageDown(string productionOrderBatchId, string id, string isReturnBatchDetails = "0")
        {
            try
            {
                return Json(productionOrderPresenter.MoveStageDown(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId),
                    ValidationUtils.TryGetNotEmptyGuid(id), ValidationUtils.TryGetByte(isReturnBatchDetails), UserSession.CurrentUserInfo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult LoadStagesFromTemplate(string productionOrderBatchId, string productionOrderBatchLifeCycleTemplateId, string isReturnBatchDetails = "0")
        {
            try
            {
                return Json(productionOrderPresenter.LoadStagesFromTemplate(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId),
                    ValidationUtils.TryGetShort(productionOrderBatchLifeCycleTemplateId), ValidationUtils.TryGetByte(isReturnBatchDetails), UserSession.CurrentUserInfo),
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Переходы между этапами

        public ActionResult ChangeStage(string productionOrderBatchId, string isSingleBatch)
        {
            try
            {
                return PartialView("ProductionOrderBatchChangeStage",
                    productionOrderPresenter.ChangeStage(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult MoveToNextStage(string productionOrderBatchId, string currentStageId, string isSingleBatch, string isReturnBatchDetails = "0")
        {
            try
            {
                var result = productionOrderPresenter.MoveToNextStage(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), ValidationUtils.TryGetNotEmptyGuid(currentStageId), ValidationUtils.TryGetByte(isReturnBatchDetails), UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult MoveToPreviousStage(string productionOrderBatchId, string currentStageId, string isSingleBatch, string isReturnBatchDetails = "0")
        {
            try
            {
                string message;
                var result = productionOrderPresenter.MoveToPreviousStage(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), ValidationUtils.TryGetNotEmptyGuid(currentStageId), ValidationUtils.TryGetByte(isReturnBatchDetails), out message, UserSession.CurrentUserInfo);

                if (!String.IsNullOrEmpty(message))
                {
                    TempData["Message"] = message;
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult MoveToUnsuccessfulClosingStage(string productionOrderBatchId, string currentStageId, string isSingleBatch, string isReturnBatchDetails = "0")
        {
            try
            {
                var result = productionOrderPresenter.MoveToUnsuccessfulClosingStage(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId),
                    ValidationUtils.TryGetNotEmptyGuid(currentStageId), ValidationUtils.TryGetBool(isSingleBatch),
                    ValidationUtils.TryGetByte(isReturnBatchDetails), UserSession.CurrentUserInfo);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region План платежей по заказу

        #region Модальная форма редактирования плана

        /// <summary>
        /// Получение модальной формы редактирования плана платежей
        /// </summary>
        [HttpGet]
        public ActionResult EditPlannedPayments(string productionOrderId)
        {
            try
            {
                return PartialView("ProductionOrderEditPlannedPayments", productionOrderPresenter.EditPlannedPayments(
                    ValidationUtils.TryGetNotEmptyGuid(productionOrderId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowProductionOrderPlannedPaymentGrid(GridState state)
        {
            try
            {
                GridData data = productionOrderPresenter.GetProductionOrderPlannedPaymentGrid(state, UserSession.CurrentUserInfo);

                return PartialView("ProductionOrderPlannedPaymentGrid", data);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Создание / редактирование / сохранение / удаление плановых платежей

        /// <summary>
        /// Добавление планового платежа
        /// </summary>
        [HttpGet]
        public ActionResult CreateProductionOrderPlannedPayment(string productionOrderId)
        {
            try
            {
                return PartialView("ProductionOrderPlannedPaymentEdit",
                    productionOrderPresenter.CreateProductionOrderPlannedPayment(ValidationUtils.TryGetNotEmptyGuid(productionOrderId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Редактирование планового платежа
        /// </summary>
        /// <param name="productionOrderPlannedPaymentId">Код планового платежа</param>
        [HttpGet]
        public ActionResult EditProductionOrderPlannedPayment(string productionOrderPlannedPaymentId)
        {
            try
            {
                return PartialView("ProductionOrderPlannedPaymentEdit",
                    productionOrderPresenter.EditProductionOrderPlannedPayment(ValidationUtils.TryGetNotEmptyGuid(productionOrderPlannedPaymentId),
                    UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Сохранение планового платежа
        /// </summary>
        /// <param name="model">Модель планового платежа</param>
        [HttpPost]
        public ActionResult SaveProductionOrderPlannedPayment(ProductionOrderPlannedPaymentEditViewModel model)
        {
            try
            {
                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");

                productionOrderPresenter.SaveProductionOrderPlannedPayment(model, UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        /// <summary>
        /// Удаление планового платежа
        /// </summary>
        [HttpPost]
        public ActionResult DeleteProductionOrderPlannedPayment(string productionOrderPlannedPaymentId)
        {
            try
            {
                productionOrderPresenter.DeleteProductionOrderPayment(ValidationUtils.TryGetNotEmptyGuid(productionOrderPlannedPaymentId),
                    UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Выбор планового платежа

        public ActionResult SelectPlannedPayment(string productionOrderId, string productionOrderPaymentTypeId, string selectFunctionName)
        {
            try
            {
                return PartialView("ProductionOrderPlannedPaymentSelector",
                    productionOrderPresenter.SelectPlannedPayment(ValidationUtils.TryGetGuid(productionOrderId),
                    ValidationUtils.TryGetByte(productionOrderPaymentTypeId), selectFunctionName, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ShowPlannedPaymentSelectGrid(GridState state)
        {
            try
            {
                ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

                return PartialView("ProductionOrderPlannedPaymentSelectGrid",
                    productionOrderPresenter.GetSelectPlannedPaymentGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Получение показателей планового платежа

        [HttpPost]
        public ActionResult GetPlannedPaymentInfo(string productionOrderPlannedPaymentId)
        {
            try
            {
                return Json(productionOrderPresenter.GetPlannedPaymentInfo(ValidationUtils.TryGetGuid(productionOrderPlannedPaymentId), UserSession.CurrentUserInfo),
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #endregion

        #region Расчет себестоимости

        public ActionResult ArticlePrimeCostSettingsForm(string productionOrderId)
        {
            try
            {
                return PartialView("ProductionOrderArticlePrimeCostSettingsForm",
                    productionOrderPresenter.ArticlePrimeCostSettingsForm(ValidationUtils.TryGetNotEmptyGuid(productionOrderId), UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult ArticlePrimeCostForm(string productionOrderId,string articlePrimeCostCalculationTypeId, string divideCustomsExpenses,
            string showArticleVolumeAndWeight, string articleTransportingPrimeCostCalculationTypeId, string includeUnsuccessfullyClosedBatches,
            string includeUnapprovedBatches)
        {
            try
            {
                return View("~/Views/ProductionOrder/ProductionOrderArticlePrimeCostForm.cshtml",
                    productionOrderPresenter.ArticlePrimeCostForm(ValidationUtils.TryGetNotEmptyGuid(productionOrderId),
                    ValidationUtils.TryGetByte(articlePrimeCostCalculationTypeId),
                    ValidationUtils.TryGetBool(divideCustomsExpenses),
                    ValidationUtils.TryGetBool(showArticleVolumeAndWeight),
                    ValidationUtils.TryGetByte(articleTransportingPrimeCostCalculationTypeId),
                    ValidationUtils.TryGetBool(includeUnsuccessfullyClosedBatches),
                    ValidationUtils.TryGetBool(includeUnapprovedBatches),
                    UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Разделение партии

        [HttpPost]
        public ActionResult CheckPossibilityToSplitBatch(string productionOrderBatchId)
        {
            try
            {
                productionOrderPresenter.CheckPossibilityToSplitBatch(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), UserSession.CurrentUserInfo);

                return Content("");
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult SplitBatch(string productionOrderBatchId, string backUrl)
        {
            try
            {
                return View("ProductionOrderBatchSplit", productionOrderPresenter.SplitBatch(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId), backUrl, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult PerformBatchSplit(string productionOrderBatchId, string splitInfo)
        {
            try
            {
                return Content(productionOrderPresenter.PerformBatchSplit(ValidationUtils.TryGetNotEmptyGuid(productionOrderBatchId),
                    splitInfo, UserSession.CurrentUserInfo).ToString());
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #endregion

        #region Модальная форма выбора заказа

        public ActionResult SelectProductionOrder(bool? activeOnly)
        {
            try
            {
                var isActive = activeOnly == null || activeOnly == true;
                return PartialView("ProductionOrderSelector", productionOrderPresenter.SelectProductionOrder(isActive, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult SelectProductionOrderByProducer(int producerId)
        {
            try
            {
                return PartialView("ProductionOrderSelector", productionOrderPresenter.SelectProductionOrderByProducer(producerId, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        //public ActionResult SelectProductionOrder(bool activeOnly)
        //{
        //    try
        //    {
        //        return PartialView("ProductionOrderSelector", productionOrderPresenter.SelectProductionOrder(activeOnly, UserSession.CurrentUserInfo));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Content(ProcessException(ex));
        //    }
        //}

        public ActionResult SelectProductionOrderByTeam(short teamId)
        {
            try
            {
                return PartialView("ProductionOrderSelector", productionOrderPresenter.SelectProductionOrderByTeam(teamId, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        public ActionResult SelectProductionOrderForMaterialsPackageAdding()
        {
            try
            {
                return PartialView("ProductionOrderSelector", productionOrderPresenter.SelectProductionOrderForMaterialsPackageAdding(UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowProductionOrderSelectGrid(GridState state)
        {
            try
            {
                ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

                return PartialView("ProductionOrderSelectGrid", productionOrderPresenter.GetProductionOrderSelectGrid(state, UserSession.CurrentUserInfo));
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
