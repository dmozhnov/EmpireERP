
namespace ERP.Wholesale.UI.ViewModels.PrintingForm.Common.Т1
{
    /// <summary>
    /// Позиция формы Т1 (ТТН)
    /// </summary>
    public class T1ProductSectionPrintingFormItemViewModel
    {
        /// <summary>
        /// Код продукции
        /// </summary>
        public string ItemNumber{ get; set; }

        /// <summary>
        /// Номер прейскуранта
        /// </summary>
        public string ListPriseNumber { get; set; }
        
        /// <summary>
        /// Артикул
        /// </summary>
        public string Number{ get; set; }
        
        /// <summary>
        /// Количество
        /// </summary>
        public string Count{ get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public string Price{ get; set; }

        /// <summary>
        /// Название
        /// </summary>
        public string Name{ get; set; }
        
        /// <summary>
        /// Единица измерения
        /// </summary>
        public string MeasureUnit{ get; set; }
        
        /// <summary>
        /// Вес
        /// </summary>
        public string Weight{ get; set; }
        
        /// <summary>
        /// Сумма
        /// </summary>
        public string Sum{ get; set; }
    }
}
