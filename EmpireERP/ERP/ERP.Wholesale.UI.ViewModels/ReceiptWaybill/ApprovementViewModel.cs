using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.ReceiptWaybill
{
    public class ApprovementViewModel
    {
        /// <summary>
        /// Идентификатор накладной
        /// </summary>
        public string WaybillId { get; set; }

        /// <summary>
        /// Ожидаемая сумма
        /// </summary>
        [DisplayName("Сумма накладной из ожидания")]
        public string PendingSum { get; set; }

        /// <summary>
        /// Сумма по приемке
        /// </summary>
        [DisplayName("Сумма накладной при приемке на склад")]
        public string ReceiptedSum { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Итоговая сумма накладной
        /// </summary>
        [DisplayName("Итоговая сумма накладной")]
        public string TotalApprovedSum { get; set; }

        /// <summary>
        /// Сумма накладной по согласованным стоимостям строк накладной
        /// </summary>
        [DisplayName("Сумма накладной по позициям")]
        public string ApprovedRowsSum { get; set; }

        /// <summary>
        /// Пришедшие товары
        /// </summary>
        public GridData ReceiptWaybillRowGrid { get; set; }

        /// <summary>
        /// Обратный адрес
        /// </summary>
        public string BackURL { get; set; }

        /// <summary>
        /// Можно ли просматривать ЗЦ
        /// </summary>
        public bool AllowToViewPurchaseCosts { get; set; }

        /// <summary>
        /// Разрешено ли согласование накладной задним числом
        /// </summary>
        public bool AllowToApproveRetroactively { get; set; }
        public bool IsPossibilityToApproveRetroactively { get; set; }
    }    
}