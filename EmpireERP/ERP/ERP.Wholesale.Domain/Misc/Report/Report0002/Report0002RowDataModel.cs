using System;

namespace ERP.Wholesale.Domain.Misc.Report.Report0002
{
    /// <summary>
    /// Модель данных отчета 0002
    /// </summary>
    public class Report0002RowDataModel
    {
        /// <summary>
        /// Код товара
        /// </summary>
        public int ArticleId { get; set; }

        /// <summary>
        /// Артикул товара
        /// </summary>
        public string ArticleNumber { get; set; }

        /// <summary>
        /// Название товара
        /// </summary>
        public string ArticleName { get; set; }

        /// <summary>
        /// Код партии
        /// </summary>
        public Guid BatchId { get; set; }

        /// <summary>
        /// Номер партии
        /// </summary>
        public string BatchNumber { get; set; }

        /// <summary>
        /// Дата партии
        /// </summary>
        public DateTime BatchDate { get; set; }

        /// <summary>
        /// Количество товара
        /// </summary>
        public decimal Count { get; set; }

        /// <summary>
        /// Размер упаковки
        /// </summary>
        public decimal PackSize { get; set; }

        /// <summary>
        /// Название страны производителя
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Номер ГТД
        /// </summary>
        public string CustomsDeclarationNumber { get; set; }

        /// <summary>
        /// Код группы товаров
        /// </summary>
        public short ArticleGroupId { get; set; }

        /// <summary>
        /// Название группы товаров
        /// </summary>
        public string ArticleGroupName { get; set; }

        /// <summary>
        /// Код МХ
        /// </summary>
        public short StorageId { get; set; }

        /// <summary>
        /// Название МХ
        /// </summary>
        public string StorageName { get; set; }

        /// <summary>
        /// Тип МХ
        /// </summary>
        public byte StorageTypeId { get; set; }

        /// <summary>
        /// Код организации аккаунта
        /// </summary>
        public int AccountOrganizationId { get; set; }

        /// <summary>
        /// Название организации аккаунта
        /// </summary>
        public string AccountOrganizationName { get; set; }

        /// <summary>
        /// Код команды
        /// </summary>
        public short TeamId { get; set; }

        /// <summary>
        /// Название команды
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// Код пользователя
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Код клиента
        /// </summary>
        public int ClientId { get; set; }

        /// <summary>
        /// Имя клиента
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Код организации клиента
        /// </summary>
        public int ClientOrganizationId { get; set; }

        /// <summary>
        /// Название организации клиента
        /// </summary>
        public string ClientOrganizationName { get; set; }

        /// <summary>
        /// Код производителя/поставщика
        /// </summary>
        public int ProducerId { get; set; }

        /// <summary>
        /// Название производителя/поставщика
        /// </summary>
        public string ProducerName { get; set; }
        
        /// <summary>
        /// Признак, что строка является возвратом товара
        /// </summary>
        public bool IsReturn { get; set; }
        
        /// <summary>
        /// Остаток товара на МХ
        /// </summary>
        public decimal? ArticleAvailabilityCount { get; set; }

        /// <summary>
        /// УЦ остатка товаров
        /// </summary>
        public decimal? ArticleAvailabilityAccountingPrice { get; set; }

        /// <summary>
        /// Сумма в УЦ
        /// </summary>
        public decimal AccountingPriceSum { get; set; }

        /// <summary>
        /// Сумма в ЗЦ
        /// </summary>
        public decimal PurchaseCostSum { get; set; }

        /// <summary>
        /// Сумма в ОЦ
        /// </summary>
        public decimal SalePriceSum { get; set; }

        /// <summary>
        /// Средняя ЗЦ
        /// </summary>
        public decimal AveragePurchaseCost { get; set; }

        /// <summary>
        /// Средняя ОЦ
        /// </summary>
        public decimal AverageSalePrice { get; set; }
    }
}
