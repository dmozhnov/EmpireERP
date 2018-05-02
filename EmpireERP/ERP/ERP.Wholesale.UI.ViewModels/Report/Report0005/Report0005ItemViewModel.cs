using System;
using System.Collections.Generic;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0005
{
    public class Report0005ItemViewModel
    {
        public IEnumerable<Report0005ItemViewModel> SubItems { get; set; }

        public ERP.Wholesale.Domain.Entities.WaybillType WaybillType { get; set; }
        public string WaybillName { get; set; }
        public Guid WaybillId { get; set; }

        public string WaybillStateName { get; set; }
        
        /// <summary>
        /// Количество по накладной
        /// </summary>
        public string Count { get; set; }
        /// <summary>
        /// Используется ли позиция в дереве больше одного раза
        /// </summary>
        public bool IsUsedMoreThenOnce { get; set; }
        
        /// <summary>
        /// Отгруженное количество (исходящая накладная сформирована, отгружена, но не проведена)
        /// </summary>
        public string ShippedCount { get; set; }

        /// <summary>
        /// Зарезервированное количество (исходящая накладная сформирована, но не проведена)
        /// </summary>
        public string ReservedCount { get; set; }

        public string SalePrice { get; set; }

        public string SenderAccountingPrice { get; set; }

        public string RecipientAccountingPrice { get; set; }

        public string PurchaseCost { get; set; }

        public bool HiddenWaybill { get; set; }
        public bool MarkedWaybill { get; set; }
        public int ItemLevel { get; set; }

        /// <summary>
        /// Остаток
        /// </summary>
        public string RemainCount { get; set; }
        /// <summary>
        /// Возвращено
        /// </summary>
        public string ReturnedCount { get; set; }

        /// <summary>
        /// Создана ли приходная накладная партии товара по заказу
        /// </summary>
        public bool IsCreatedFromProductionOrderBatch { get; set; }

        public string BatchName { get; set; }
        public string ClientName { get; set; }
        public string ContractorName { get; set; }
        public string RecipientName { get; set; }
        
        public short RecipientStorageId { get; set; }
        public string RecipientStorageName { get; set; }
        public string SenderStorageName { get; set; }
        public short SenderStorageId { get; set; }
        public string SenderName { get; set; }
        public string Reason { get; set; }
        public string WaybillTypeName { get; set; }
        public DateTime Date { get; set; }

        public Report0005ItemViewModel()
        {
            SubItems = new List<Report0005ItemViewModel>();
            HiddenWaybill = false;
            MarkedWaybill = false;
            ItemLevel = 0;
            IsUsedMoreThenOnce = false;
        }      
    }
}
