using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderBatchRowEditViewModel
    {        
        public string Id { get; set; }

        public string BatchId { get; set; }

        public string Title { get; set; }

        public string ProducerId { get; set; }

        [DisplayName("Наименование товара")]
        public string ArticleName { get; set; }

        [Required(ErrorMessage = "Укажите товар")]
        [Range(1, int.MaxValue, ErrorMessage = "Укажите товар")]
        public string ArticleId { get; set; }

        [DisplayName("Фабрика-изготовитель")]
        public string ManufacturerName { get; set; }

        [Required(ErrorMessage = "Укажите фабрику-изготовителя")]
        [Range(1, int.MaxValue, ErrorMessage = "Укажите товар")]
        public string ManufacturerId { get; set; }

        [DisplayName("Страна производства")]
        [Required(ErrorMessage = "Укажите страну производства")]
        public string ProductionCountryId { get; set; }
        public IEnumerable<SelectListItem> ProductionCountryList { get; set; }

        [DisplayName("Размеры упак. (мм)")]
        [RegularExpression("[0-9]{1,6}", ErrorMessage = "Введите целое число")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PackLength { get; set; }

        [DisplayName("Размеры упак. (мм)")]
        [RegularExpression("[0-9]{1,6}", ErrorMessage = "Введите целое число")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PackWidth { get; set; }

        [DisplayName("Размеры упак. (мм)")]
        [RegularExpression("[0-9]{1,6}", ErrorMessage = "Введите целое число")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PackHeight { get; set; }

        [DisplayName("Кол-во в упак.")]
        public string PackSize { get; set; }

        [DisplayName("Вес упак.")]        
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PackWeight { get; set; }

        [DisplayName("Объем упак.")]
        [RegularExpression("[0-9]{1,9}([,.][0-9]{1,6})?", ErrorMessage = "Не более 6 знаков после запятой")]
        [Required(ErrorMessage = "Укажите объем упаковки")]
        public string PackVolume { get; set; }

        [DisplayName("Цена производства")]
        [RegularExpression("[0-9]{1,12}([,.][0-9]{1,6})?", ErrorMessage = "Не более 6 знаков после запятой")]
        [Required(ErrorMessage = "Укажите цену производства")]
        public string ProductionCost { get; set; }

        [DisplayName("Кол-во")]
        [RegularExpression("[0-9]{1,12}([,.][0-9]{1,6})?", ErrorMessage = "Не более 6 знаков после запятой")]
        [Required(ErrorMessage = "Укажите количество")]
        public string Count { get; set; }

        [DisplayName("Кол-во упак.")]
        [RegularExpression("[0-9]+", ErrorMessage = "Введите целое число от 1 до 32000")]
        [Range(1, 32000, ErrorMessage = "Введите целое число от 1 до 32000")]
        [Required(ErrorMessage = "Укажите кол-во упаковок")]
        public string PackCount { get; set; }

        [DisplayName("Стоимость")]
        [RegularExpression("[0-9]{1,12}([,.][0-9]{1,6})?", ErrorMessage = "Не более 6 знаков после запятой")]
        [Required(ErrorMessage = "Укажите стоимость")]
        public string TotalCost { get; set; }

        [DisplayName("Валюта")]
        public string CurrencyName { get; set; }

        [DisplayName("Курс")]        
        public string CurrencyRate { get; set; }

        [DisplayName("Общий вес позиции")]
        public string TotalWeight { get; set; }

        [DisplayName("Общий объем позиции")]
        public string TotalVolume { get; set; }

        [DisplayName("Вес всей партии")]
        public string BatchWeight { get; set; }

        [DisplayName("Объем всей партии")]
        public string BatchVolume { get; set; }

        [DisplayName("Оптимальное разм-е")]
        public string OptimalPlacement { get; set; }

        [DisplayName("Свободный объем")]
        public string FreeVolume { get; set; }

        /// <summary>
        /// Название единицы измерения
        /// </summary>
        public string MeasureUnitName { get; set; }

        /// <summary>
        /// Количество знаков после запятой
        /// </summary>
        public string MeasureUnitScale { get; set; }

        /// <summary>
        /// Признак возможности редактировать позицию
        /// </summary>
        public bool AllowToEditRow { get; set; }
        public bool AllowToAddCountry { get; set; }

        /// <summary>
        /// Можно ли менять страну производства
        /// </summary>
        public bool AllowToChangeCountry { get; set; }

        public ProductionOrderBatchRowEditViewModel()
        {
            PackHeight = "0";
            PackLength = "0";
            PackWidth = "0";
            Id = Guid.Empty.ToString();
        }
    }


}
