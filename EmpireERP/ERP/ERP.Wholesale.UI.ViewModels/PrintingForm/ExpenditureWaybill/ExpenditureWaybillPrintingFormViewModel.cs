using System.Collections.Generic;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.ExpenditureWaybill
{
    public class ExpenditureWaybillPrintingFormViewModel
    {
        #region Поля
        
        [DisplayName("Организация")]
        public string OrganizationName { get; set; }
        
        [DisplayName("Получатель")]
        public string RecepientName { get; set; }

        [DisplayName("Отпечатано")]
        public string Date { get; set; }

        [DisplayName("Название")]
        public string Title { get; set; }
        
        #endregion

        #region Свойства
        
        /// <summary>
        /// Количество позиций
        /// </summary>
        public decimal RowsCount { get { return Rows.Count; } }

        /// <summary>
        /// строки таблицы
        /// </summary>
        public IList<ExpenditureWaybillPrintingFormItemViewModel> Rows { get; set; }

        /// <summary>
        /// Общее количество в единицах измерения
        /// </summary>
        public decimal TotalCount { get; set; }

        /// <summary>
        /// Общая сумма закупки
        /// </summary>
        public decimal TotalSalePrice { get; set; }

        /// <summary>
        /// Общая сумма НДС
        /// </summary>
        public string ValueAddedTaxSum { get; set; }

        /// <summary>
        /// Общая сумма НДС
        /// </summary>
        public string TotalSumWithoutValueAddedTax { get; set; }
        
        #endregion

        #region Конструкторы
        
        public ExpenditureWaybillPrintingFormViewModel()
        {
            Rows = new List<ExpenditureWaybillPrintingFormItemViewModel>();
            TotalCount = 0;
            TotalSalePrice = 0;
        }
        
        #endregion
    }
}
