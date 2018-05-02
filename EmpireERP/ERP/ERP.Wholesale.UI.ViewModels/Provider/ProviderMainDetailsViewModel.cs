using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.Provider
{
    /// <summary>
    /// Модель основных деталей поставщика
    /// </summary>
    public class ProviderMainDetailsViewModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [DisplayName("Наименование")]
        public string Name { get; set; }

        /// <summary>
        /// Тип
        /// </summary>
        [DisplayName("Тип")]
        public string TypeName { get; set; }

        /// <summary>
        /// Надежность
        /// </summary>
        [DisplayName("Надежность")]
        public string ReliabilityName { get; set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        [DisplayName("Дата создания")]
        public string CreationDate { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        public string Comment { get; set; }

        /// <summary>
        /// Рейтинг
        /// </summary>
        [DisplayName("Рейтинг")]
        public string Rating { get; set; }

        /// <summary>
        /// Общая сумма закупок (в том числе и "Ожидается поставка")
        /// </summary>
        [DisplayName("Общая сумма закупок")]
        public string PurchaseCostSum { get; set; }

        /// <summary>
        /// Ожидается поставка
        /// </summary>
        [DisplayName("Ожидается поставка")]
        public string PendingPurchaseCostSum { get; set; }

        /// <summary>
        /// Количество организаций
        /// </summary>
        [DisplayName("Кол-во организаций")]
        public string ProviderOrganizationCount { get; set; }

        /// <summary>
        /// Количество договоров
        /// </summary>
        [DisplayName("Кол-во договоров")]
        public string ContractCount { get; set; }
    }
}