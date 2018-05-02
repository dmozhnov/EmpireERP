using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.PrintingForm.Common
{
    public class CashMemoPrintingFormItemViewModel
    {
        [DisplayName("Наименование товара")]
        public string ArticleName { get; set; }

        [DisplayName("Уп.")]
        public string PackSize { get; set; }

        [DisplayName("Кол-во")]
        public string Count { get; set; }

        [DisplayName("Цена")]
        public string Price { get; set; }

        [DisplayName("Сумма")]
        public string Sum { get; set; }
          
    }
}
