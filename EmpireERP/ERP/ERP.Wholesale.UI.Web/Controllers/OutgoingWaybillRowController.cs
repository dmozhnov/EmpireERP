using System;
using System.Web.Mvc;
using System.Web.UI;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.Web.Infrastructure;

namespace ERP.Wholesale.UI.Web.Controllers
{
    [OutputCache(Location = OutputCacheLocation.None)]
    public class OutgoingWaybillRowController : WholesaleController
    {
        #region Поля

        private readonly IOutgoingWaybillRowPresenter outgoingWaybillRowPresenter;

        #endregion

        #region Конструкторы

        public OutgoingWaybillRowController(IOutgoingWaybillRowPresenter outgoingWaybillRowPresenter)
        {
            this.outgoingWaybillRowPresenter = outgoingWaybillRowPresenter;
        }

        #endregion

        #region Методы

        [HttpPost]
        public ActionResult GetSourceWaybill(string type, string id, string articleName, string batchName)
        {
            try
            {
                var waybillRowId = ValidationUtils.TryGetGuid(id);
                WaybillType waybillType = GetOutgoingWaybillTypeFromString(type);

                return PartialView("IncomingWaybillRow", outgoingWaybillRowPresenter.GetSourceWaybill(waybillType, waybillRowId, articleName, batchName, UserSession.CurrentUserInfo));
               
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowSourceWaybillGrid(GridState state)
        {
            try
            {
                return PartialView("IncomingWaybillRowGrid", outgoingWaybillRowPresenter.ShowSourceWaybillGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpGet]
        public ActionResult GetAvailableToReserveWaybillRows(string type, string articleId, string organizationId, string storageId, string selectedSourcesInfo = null, string waybillRowId = null)
        {
            try
            {
                if (selectedSourcesInfo == null) selectedSourcesInfo = "";

                var _articleId = ValidationUtils.TryGetInt(articleId);
                var _organizationId = ValidationUtils.TryGetInt(organizationId);
                var _storageId = ValidationUtils.TryGetShort(storageId);

                Guid? waybillRowGuid = waybillRowId != null && waybillRowId != Guid.Empty.ToString() ? ValidationUtils.TryGetGuid(waybillRowId) : (Guid?)null;
                
                var waybillType = GetOutgoingWaybillTypeFromString(type);
                                
                return PartialView("IncomingWaybillRow", outgoingWaybillRowPresenter.GetAvailableToReserveWaybillRows(selectedSourcesInfo, waybillType, 
                    _articleId, _organizationId, _storageId, UserSession.CurrentUserInfo, waybillRowGuid));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        [HttpPost]
        public ActionResult ShowAvailableToReserveWaybillRowsGrid(GridState state)
        {
            try
            {
                return PartialView("IncomingWaybillRowGrid", outgoingWaybillRowPresenter.ShowAvailableToReserveWaybillRowsGrid(state, UserSession.CurrentUserInfo));
            }
            catch (Exception ex)
            {
                return Content(ProcessException(ex));
            }
        }

        #region Вспомогательные методы

        /// <summary>
        /// Для исходящих накладных возвращает WaybillType по названию накладной. Если передается название не-исходящей накладной, то будет эксепшн.
        /// </summary>
        /// <param name="waybillType"></param>
        /// <returns></returns>
        private WaybillType GetOutgoingWaybillTypeFromString(string waybillType)
        {
            switch (waybillType)
            {
                case "MovementWaybill":
                    return WaybillType.MovementWaybill;

                case "WriteoffWaybill":
                    return WaybillType.WriteoffWaybill;

                case "ChangeOwnerWaybill":
                    return WaybillType.ChangeOwnerWaybill;                    

                case "ExpenditureWaybill":
                    return WaybillType.ExpenditureWaybill;                    

                case "ReceiptWaybill":
                    throw new Exception(String.Format("Для накладной вида «Приходная накладная» строки-источники отсутствуют."));

                case "ReturnFromClientWaybill":
                    throw new Exception(String.Format("Для накладной вида «Возврат товара от клиента» строки-источники отсутствуют."));

                default:
                    throw new Exception("Не определен тип накладной для получения строк-источников.");
            }
        }

        #endregion

        #endregion

    }
}