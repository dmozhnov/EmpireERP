using System.Collections.Generic;
using System.Linq;
using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0010
{
    public class Report0010DetailsTableViewModel
    {
        /// <summary>
        /// Строки таблицы
        /// </summary>
        public IEnumerable<Report0010DetailsTableRowViewModel> Rows { get; set; }

        /// <summary>
        /// Итоговая сумма платежа
        /// </summary>        
        public string TotalPaymentSumString { get { return Rows.Where(x => !x.IsGroup).Sum(x => x.PaymentSum).ForDisplay(ValueDisplayType.Money); } }

        /// <summary>
        /// Итоговая разнесенная сумма платежа
        /// </summary>
        public string TotalDistributedSumString { get { return Rows.Where(x => !x.IsGroup).Sum(x => x.DistributedSum).ForDisplay(ValueDisplayType.Money); } }

        /// <summary>
        /// Разнесенная сумма на накладные
        /// </summary>
        public string TotalDistributedToSaleWaybillPaymentSumString { get { return Rows.Where(x => !x.IsGroup).Sum(x => x.DistributedToSaleWaybillPaymentSum).ForDisplay(ValueDisplayType.Money); } }

        /// <summary>
        /// Разнесенная сумма на корректировки сальдо
        /// </summary>
        public string TotalDistributedToBalanceCorrectionPaymentSumString { get { return Rows.Where(x => !x.IsGroup).Sum(x => x.DistributedToBalanceCorrectionPaymentSum).ForDisplay(ValueDisplayType.Money); } }

        /// <summary>
        /// Возвращено из данной оплаты
        /// </summary>
        public string TotalPaymentToClientSumString { get { return Rows.Where(x => !x.IsGroup).Sum(x => x.PaymentToClientSum).ForDisplay(ValueDisplayType.Money); } }

        /// <summary>
        /// Неразнесенная сумма платежа
        /// </summary>
        public string TotalUndistributedSumString { get { return Rows.Where(x => !x.IsGroup).Sum(x => x.UndistributedSum).ForDisplay(ValueDisplayType.Money); } }

        /// <summary>
        /// Заголовок таблицы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Отображать ли форму оплаты (скрывается при выводе трех развернутых таблиц)
        /// </summary>
        public bool ShowPaymentForm { get; set; }

        /// <summary>
        /// Настройки отчета
        /// </summary>
        public Report0010SettingsViewModel Settings { get; set; }

        public Report0010DetailsTableViewModel()
        {
            Rows = new List<Report0010DetailsTableRowViewModel>();
        }
    }
}
