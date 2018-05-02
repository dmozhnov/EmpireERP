using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0009
{
    /// <summary>
    /// Строка в детальной таблице
    /// </summary>
    public class Report0009DetailTableItemViewModel
    {
        /// <summary>
        /// Товар
        /// </summary>
        [DisplayName("Наименование")]
        public string ArticleName { get; set; }

        /// <summary>
        /// Код товара
        /// </summary>
        [DisplayName("Код")]
        public string ArticleId { get; set; }

        /// <summary>
        /// Артикул товара
        /// </summary>
        [DisplayName("Артикул")]
        public string ArticleNumber { get; set; }

        /// <summary>
        /// Партия
        /// </summary>
        [DisplayName("Партия")]
        public string BatchName { get; set; }

        /// <summary>
        /// Количество товара
        /// </summary>
        [DisplayName("Кол-во")]
        public string ArticleCount { get; set; }

        /// <summary>
        /// Количество товара в упаковке
        /// </summary>
        [DisplayName("Кол-во в упак.")]
        public string CountArticleInPack { get; set; }

        /// <summary>
        /// Страна-производитель
        /// </summary>
        [DisplayName("Страна-производитель")]
        public string CountryOfProduction { get; set; }

        /// <summary>
        /// Фабрика
        /// </summary>
        [DisplayName("Фабрика-изготовитель")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// ГТД
        /// </summary>
        [DisplayName("ГТД")]
        public string CustomsDeclarationNumber { get; set; }

        /// <summary>
        /// Наценка
        /// </summary>
        [DisplayName("Наценка")]
        public string Markup { get; set; }

        /// <summary>
        /// Закупочные цены
        /// </summary>
        [DisplayName("ЗЦ")]
        public string PurchaseCost { get; set; }
        
        /// <summary>
        /// Учетные цены прихода
        /// </summary>
        [DisplayName("УЦ прихода")]
        public string RecipientWaybillAccountingPrice { get; set; }

        /// <summary>
        /// Текущие учетные цены
        /// </summary>
        [DisplayName("УЦ текущие")]
        public string CurrentAccountingPrice { get; set; }

        /// <summary>
        /// Это заголовок для группировки?
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// Заголовок
        /// </summary>
        public string GroupTitle { get; set; }

        /// <summary>
        /// Уровень группировки
        /// </summary>
        public int GroupLevel { get; set; }

    }
}
