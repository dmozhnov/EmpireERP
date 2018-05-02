
namespace ERP.Wholesale.UI.ViewModels.Report.Report0003
{
    public class Report0003ItemViewModel
    {
        /// <summary>
        /// Наименование строчки отчета
        /// </summary>
        public string IndicatorName { get; set; }

        /// <summary>
        /// Сумма в УЦ
        /// </summary>
        public decimal AccountingPriceSum { get; set; }
        
        /// <summary>
        /// Сумма в ЗЦ
        /// </summary>
        public decimal PurchaseCostSum { get; set;}

        /// <summary>
        /// Процент наценки в УЦ
        /// </summary>
        public decimal? AccountingPriceMarkupPercent { get; set; }

        /// <summary>
        /// Сумма наценки в УЦ
        /// </summary>
        public decimal? AccountingPriceMarkupSum { get; set; }

        /// <summary>
        /// Сумма в отпускных ценах
        /// </summary>
        public decimal? SalePriceSum { get; set; }

        /// <summary>
        /// Процент наценки в ОЦ
        /// </summary>
        public decimal? SalePriceMarkupPercent{ get; set; }

        /// <summary>
        /// Сумма наценки в ОЦ
        /// </summary>
        public decimal? SalePriceMarkupSum{ get; set; }


        public Report0003ItemViewModel(string indicatorName, decimal purchaseCostSum, decimal accountingPriceSum, decimal? salePriceSum)
        {
            IndicatorName = indicatorName;
            PurchaseCostSum = purchaseCostSum;
            AccountingPriceSum = accountingPriceSum;
            SalePriceSum = salePriceSum;
        }

        /// <summary>
        /// Вычитает элемент из текущего 
        /// </summary>
        /// <param name="item"></param>
        public void Substract(Report0003ItemViewModel item)
        {
            AccountingPriceSum -= item.AccountingPriceSum;
            PurchaseCostSum -= item.PurchaseCostSum;
            SalePriceSum -= item.SalePriceSum;
        }

    }
}
