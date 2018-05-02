using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderArticlePrimeCostSettingsViewModel
    {
        /// <summary>
        /// Заголовок модальной формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Код заказа
        /// </summary>
        public string ProductionOrderId { get; set; }

        /// <summary>
        /// По каким значениям считать себестоимость
        /// </summary>
        [DisplayName("По каким значениям считать себестоимость")]
        [Required(ErrorMessage = "Укажите, по каким значениям считать себестоимость")]
        public byte ArticlePrimeCostCalculationTypeId { get; set; }

        /// <summary>
        /// Перечень возможных способов подсчета себестоимости
        /// </summary>
        public IEnumerable<SelectListItem> ArticlePrimeCostCalculationTypeList { get; set; }

        /// <summary>
        /// Выбранный способ подсчета себестоимости
        /// </summary>
        public string ArticlePrimeCostCalculationType { get; set; }

        /// <summary>
        /// Разделить ли таможенные затраты
        /// </summary>
        [DisplayName("Разделить таможенные затраты")]
        public string DivideCustomsExpenses { get; set; }

        /// <summary>
        /// Отобразить ли объем и вес позиций
        /// </summary>
        [DisplayName("Отобразить объем и вес позиций")]
        public string ShowArticleVolumeAndWeight { get; set; }

        /// <summary>
        /// Как считать себестоимость транспортировки
        /// </summary>
        [DisplayName("Как считать себестоимость транспортировки")]
        [Required(ErrorMessage = "Укажите, как считать себестоимость транспортировки")]
        public byte ArticleTransportingPrimeCostCalculationTypeId { get; set; }

        /// <summary>
        /// Перечень возможных способов подсчета себестоимости транспортировки
        /// </summary>
        public IEnumerable<SelectListItem> ArticleTransportingPrimeCostCalculationTypeList { get; set; }

        /// <summary>
        /// Выбранный способ подсчета себестоимости транспортировки
        /// </summary>
        public string ArticleTransportingPrimeCostCalculationType { get; set; }

        /// <summary>
        /// Включить ли неуспешно закрытые партии
        /// </summary>
        [DisplayName("Включить неуспешно закрытые партии")]
        public string IncludeUnsuccessfullyClosedBatches { get; set; }

        /// <summary>
        /// Включить ли неподготовленные партии
        /// </summary>
        [DisplayName("Включить неподготовленные партии")]
        public string IncludeUnapprovedBatches { get; set; }

        /// <summary>
        /// Отображать ли наценку
        /// </summary>
        public bool IsMarkupPercentEnabled { get; set; }

        /// <summary>
        /// Величина наценки для создания приходной накладной
        /// </summary>
        [DisplayName("Добавить наценку")]
        [Required(ErrorMessage = "Укажите величину наценки")]
        [RegularExpression("[0-9]{1,4}([,.][0-9]{1,2})?", ErrorMessage = "Не более 4 знаков до и 2 после запятой")]
        public string MarkupPercent { get; set; }
    }
}
