using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ERP.Wholesale.UI.ViewModels.Article
{
    public class ArticleEditViewModel
    {
        /// <summary>
        /// Код
        /// </summary>
        [DisplayName("Код")]
        public int Id { get; set; }
            
        /// <summary>
        /// Полное название
        /// </summary>
        [DisplayName("Наименование товара")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        [Required(ErrorMessage = "Укажите наименование товара")]
        public string FullArticleName { get; set; }

        /// <summary>
        /// Краткое название
        /// </summary>
        [DisplayName("Краткое наименование")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        [Required(ErrorMessage = "Укажите краткое наименование")]
        public string ShortName { get; set; }

        /// <summary>
        /// Артикул
        /// </summary>
        [DisplayName("Артикул")]
        [StringLength(30, ErrorMessage = "Не более {1} символов")]
        [Required(ErrorMessage = "Укажите артикул")]
        public string Number { get; set; }

        /// <summary>
        /// Заводской артикул
        /// </summary>
        [DisplayName("Заводской артикул")]
        [StringLength(30, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ManufacturerNumber { get; set; }

        /// <summary>
        /// Товарная группа
        /// </summary>
        [DisplayName("Группа товара")]        
        public string ArticleGroupName { get; set; }
        [Range(1, short.MaxValue, ErrorMessage = "Выберите группу товара")]
        public short ArticleGroupId { get; set; }

        [Range(1, 2, ErrorMessage = "Товар можно добавить только в группы второго уровня")]        
        public byte IsCurrentArticleGroupLevelCorrect { get; set; }

        /// <summary>
        /// Торговая марка
        /// </summary>
        [DisplayName("Торговая марка")]
        public string TrademarkName { get; set; }
        public short? TrademarkId { get; set; }

        /// <summary>
        /// Фабрика-изготовитель
        /// </summary>
        [DisplayName("Фабрика-изготовитель")]
        public short? ManufacturerId { get; set; }

        /// <summary>
        /// Название фабрики-изготовителя
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// Страна производства
        /// </summary>
        [DisplayName("Страна производства")]
        public short? ProductionCountryId { get; set; }

        /// <summary>
        /// Единица измерения
        /// </summary>
        [DisplayName("Единицы измерения")]
        [Required(ErrorMessage = "Укажите единицу измерения")]
        [Range(1, short.MaxValue, ErrorMessage = "Укажите единицу измерения")]
        public short MeasureUnitId { get; set; }
        public string MeasureUnitName { get; set; }
        public string MeasureUnitShortName { get; set; }
        public string MeasureUnitScale { get; set; }

        /// <summary>
        /// Количество в упаковке
        /// </summary>
        [DisplayName("Кол-во в упак.")]
        [Required(ErrorMessage = "Укажите кол-во в упаковке")]
        public string PackSize { get; set; }

        /// <summary>
        /// Вес упаковки
        /// </summary>
        [DisplayName("Вес упак.")]
        [RegularExpression("[0-9]{1,5}([,.][0-9]{1,3})?", ErrorMessage = "Не более 3 знаков после запятой")]
        [Required(ErrorMessage = "Укажите вес упаковки")]
        public string PackWeight { get; set; }

        /// <summary>
        /// Длина упаковки
        /// </summary>
        [DisplayName("Размеры упак. (мм)")]
        [RegularExpression("[0-9]{1,6}", ErrorMessage = "Введите целое число")]
        //[Required(ErrorMessage = "Укажите длину упаковки")]
        public string PackLength { get; set; }

        /// <summary>
        /// Ширина упаковки
        /// </summary>
        [DisplayName("Ширина упак.")]
        [RegularExpression("[0-9]{1,6}", ErrorMessage = "Введите целое число")]
        //[Required(ErrorMessage = "Укажите ширину упаковки")]
        public string PackWidth { get; set; }

        /// <summary>
        /// Высота упаковки
        /// </summary>
        [DisplayName("Высота упак.")]
        [RegularExpression("[0-9]{1,6}", ErrorMessage = "Введите целое число")]
        //[Required(ErrorMessage = "Укажите высоту упаковки")]
        public string PackHeight { get; set; }

        /// <summary>
        /// Объем упаковки
        /// </summary>
        [DisplayName("Объем упак.")]
        [RegularExpression("[0-9]{1,9}([,.][0-9]{1,6})?", ErrorMessage = "Не более 6 знаков после запятой")]
        [Required(ErrorMessage = "Укажите объем упаковки")]
        public string PackVolume { get; set; }

        /// <summary>
        /// Сертификат товара
        /// </summary>
        [DisplayName("Сертификат товара")]
        public int CertificateId { get; set; }
        public string CertificateName { get; set; }

        /// <summary>
        /// Процент, идущий в заработную плату
        /// </summary>
        [DisplayName("% з/п продавцу")]
        [RegularExpression("[0-9]{1,2}([,.][0-9]{1,2})?", ErrorMessage = "Не более 2 знаков до и после запятой")]
        [Required(AllowEmptyStrings = true)]
        public string SalaryPercent { get; set; }

        /// <summary>
        /// Признак необходимости брать значение процента, идущего в заработную плату, из группы
        /// </summary>
        [DisplayName("Брать значение % з/п продавцу из группы товара")]
        public string IsSalaryPercentFromGroup { get; set; }

        /// <summary>
        /// Берется ли значение процента, идущего в заработную плату, из текущей группы товара
        /// </summary>
        public string SalaryPercentFromGroup { get; set; }

        /// <summary>
        /// Признак того, что товар устарел (не появляется при добавлении товаров списком)
        /// </summary>
        [DisplayName("Товар устарел")]
        public string IsObsolete { get; set; }

        /// <summary>
        /// Процент наценки
        /// </summary>
        [DisplayName("% наценки")]
        [RegularExpression("[0-9]{1,4}([,.][0-9]{1,2})?", ErrorMessage = "Не более 4 знаков до и 2 после запятой")]
        [Required(ErrorMessage = "Укажите % наценки")]
        public string MarkupPercent { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        [DisplayName("Комментарий")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Comment { get; set; }

        /// <summary>
        /// Невидимое поле для валидации процента в зарплату продавцу
        /// </summary>
        [RegularExpression("0", ErrorMessage = "Не более 2 знаков после запятой")]  
        public string isSalaryPercentCorrect { get; set; }  
        
        public IEnumerable<SelectListItem> ProductionCountryList;

        public string Title { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToAddCountry { get; set; }
        public bool AllowToClearCertificate { get; set; }

        public ArticleEditViewModel()
        {
            PackSize = "1";
            PackWeight = "0";

            PackLength = "0";
            PackWidth = "0";
            PackHeight = "0";

            PackVolume = "0";
            SalaryPercent = "0";
            MarkupPercent = "0";
        }
    }
}