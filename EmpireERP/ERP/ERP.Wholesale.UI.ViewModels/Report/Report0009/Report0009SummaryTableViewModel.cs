using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0009
{
    public class Report0009SummaryTableViewModel
    {
        /// <summary>
        /// Строки таблицы
        /// </summary>
        public IEnumerable<Report0009SummaryTableItemViewModel> Rows;

        /// <summary>
        /// Итоговая сумма в закупочных ценах
        /// </summary>
        public string PurchaseCostSumTotal { get; set; }

        /// <summary>
        /// Итоговая сумма в учетных ценах прихода
        /// </summary>
        public string RecipientWaybillAccountingPriceSumTotal { get; set; }

        /// <summary>
        /// Итоговая сумма в текущих учетных ценах
        /// </summary>
        public string CurrentAccountingPriceSumTotal { get; set; }

        /// <summary>
        /// Итоговая сумма наценки
        /// </summary>
        public string MarkupSumTotal { get; set; }
        
        /// <summary>
        /// Можно ли просматривать ЗЦ
        /// </summary>
        public bool AllowToViewPurchaseCost;

        /// <summary>
        /// Показывать ли наценку
        /// </summary>
        public bool ShowMarkup;

        /// <summary>
        /// Показывать закупочные цены
        /// </summary>
        public bool InPurchaseCost { get; set; }

        /// <summary>
        /// Показывать в учетных ценах получателя
        /// </summary>
        public bool InRecipientWaybillAccountingPrice { get; set; }

        /// <summary>
        /// Показывать в текущих учетных ценах
        /// </summary>
        public bool InCurrentAccountingPrice { get; set; }


        /// <summary>
        /// Заголовок таблицы
        /// </summary>
        public string TableTitle;

        /// <summary>
        /// Заголовок столбца с названием
        /// </summary>
        public string NameColumnTitle;
    }
}
