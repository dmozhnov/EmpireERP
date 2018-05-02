
namespace ERP.Wholesale.UI.ViewModels.PrintingForm.Common.Т1
{
    public class T1ProductSectionPrintingFormViewModel
    {
        /// <summary>
        ///  Номер ТТН
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Отправитель товара
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// Получатель товара
        /// </summary>
        public string Recipient { get; set; }

        /// <summary>
        /// Плательщик
        /// </summary>
        public string Payer { get; set; }

        /// <summary>
        /// Дата составления документа
        /// </summary>
        public string CreationDay { get; set; }
        public string CreationMonth { get; set; }
        public string CreationYear { get; set; }

        /// <summary>
        /// Количество позиций по накладной
        /// </summary>
        public string RowCountString { get; set; }

        /// <summary>
        /// Количество наименований
        /// </summary>
        public string ArticleCountString { get; set; }

        /// <summary>
        /// Масса брутто
        /// </summary>
        public string GrousWeightString { get; set; }
        public string GrousWeightValue { get; set; }

        /// <summary>
        /// Отпущено на сумму (целая цасть прописью)
        /// </summary>
        public string TotalSumSeniorString { get; set; }
        
        /// <summary>
        /// Отпущено на сумму (дробная часть числом)
        /// </summary>
        public string TotalSumJuniorString { get; set; }

        /// <summary>
        /// Отпущено на сумму (числом)
        /// </summary>
        public string TotalSumValue { get; set; }

        /// <summary>
        /// Сумма НДС
        /// </summary>
        public string ValueAddedTaxSum { get; set; }

        /// <summary>
        /// Процент НДС
        /// </summary>
        /// <remarks>Если по позициям накладной ставка НДС различна, то данное поле не заполняется</remarks>
        public string ValueAddedTaxPercentage { get; set; }

        /// <summary>
        /// Адрес получения позиций 
        /// </summary>
        public string RowsContentURL { get; set; }

        /// <summary>
        /// Код накладной
        /// </summary>
        public string WaybillId { get; set; }

        /// <summary>
        /// Общее количество товара
        /// </summary>
        public string TotalCount { get; set; }

        /// <summary>
        /// Количество дробных разрядов при выводе количества товара
        /// </summary>
        public string CountScale { get; set; }

        /// <summary>
        /// Общая масса товара
        /// </summary>
        public string TotalWeight { get; set; }

        /// <summary>
        /// Общая сумма
        /// </summary>
        public string TotalSum { get; set; }

        /// <summary>
        /// Тип учетных цен
        /// </summary>
        public string PriceTypeId { get; set; }
    }
}
