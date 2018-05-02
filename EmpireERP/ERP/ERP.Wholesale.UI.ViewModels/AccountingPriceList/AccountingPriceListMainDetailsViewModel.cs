using System;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.AccountingPriceList
{
    public class AccountingPriceListMainDetailsViewModel
    {
        public Guid Id { get; set; }

        [DisplayName("Номер")]
        public string Number { get; set; }
        public string Name { get; set; }

        [DisplayName("Статус документа")]
        public string State { get; set; }

        [DisplayName("Статус документа")]
        public string StateDescription { get; set; }
        
        [DisplayName("Основание")]
        public string ReasonDescription { get; set; }
        public string Reason { get; set; }
        public string ReasonUrl { get; set; }
        public string ReasonReceiptWaybillId { get; set; }

        [DisplayName("Дата начала действия")]
        public string StartDate { get; set; }

        [DisplayName("Время начала действия")]
        public string StartTime { get; set; }

        [DisplayName("Дата завер. действия")]
        public string EndDate { get; set; }

        [DisplayName("Время завер. действия")]
        public string EndTime { get; set; }

        [DisplayName("Сумма в ЗЦ")]
        public string PurchaseCostSum { get; set; }

        [DisplayName("Сумма в старых УЦ")]
        public string OldAccountingPriceSum { get; set; }

        [DisplayName("Сумма в новых УЦ")]
        public string NewAccountingPriceSum { get; set; }

        [DisplayName("Изменения цен от старого")]
        public string AccountingPriceDifPercent { get; set; }

        [DisplayName("Изменения цен от старого")]
        public string AccountingPriceDifSum { get; set; }

        [DisplayName("Новая наценка от закупки")]
        public string PurchaseMarkupPercent { get; set; }

        [DisplayName("Новая наценка от закупки")]
        public string PurchaseMarkupSum { get; set; }
        
        [DisplayName("Кол-во позиций")]
        public string RowCount { get; set; }

        /// <summary>
        /// Куратор
        /// </summary>
        [DisplayName("Куратор")]
        public string CuratorName { get; set; }
        public string CuratorId { get; set; }
        public bool AllowToViewCuratorDetails { get; set; }

        public bool AllowToViewReceiptWaybillDetails { get; set; }
    }
}
