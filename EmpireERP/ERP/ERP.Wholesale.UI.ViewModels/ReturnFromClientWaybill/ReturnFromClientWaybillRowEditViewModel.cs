using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.ReturnFromClientWaybill
{
    public class ReturnFromClientWaybillRowEditViewModel
    {
        public Guid Id { get; set; }
        public Guid ReturnFromClientWaybillId { get; set; }

        [DisplayName("Наименование товара")]
        public string ArticleName { get; set; }

        [Required(ErrorMessage = "Укажите товар")]
        [Range(1, int.MaxValue, ErrorMessage = "Укажите товар")]
        public int ArticleId { get; set; }

        /// <summary>
        /// Название единицы измерения товара
        /// </summary>
        public string MeasureUnitName { get; set; }

        /// <summary>
        /// Количество допускаемых знаков после запятой в количестве товара (требуется только при начальном появлении формы)
        /// </summary>
        public string MeasureUnitScale { get; set; }

        [DisplayName("Документ реализации")]
        public string SaleWaybillName { get; set; }

        [DisplayName("Закупочная цена")]
        public string PurchaseCost { get; set; }

        [DisplayName("УЦ приемки")]
        public string AccountingPrice { get; set; }

        [DisplayName("Отпускная цена")]
        public string SalePrice { get; set; }

        [DisplayName("Всего реализовано")]
        public string TotalSoldCount { get; set; }

        [DisplayName("Возвращено по другим накл.")]
        public string ReturnedCount { get; set; }

        [DisplayName("Доступно к возврату")]
        public string AvailableToReturnCount { get; set; }

        [DisplayName("Возвращаемое кол-во")]
        [Required(ErrorMessage = "Укажите кол-во")]
        [GreaterByConst(0, ErrorMessage = "Кол-во должно быть больше 0")]
        public string ReturningCount { get; set; }

        public string Title { get; set; }
        
        /// <summary>
        /// Клиент, который возвращает
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Сделка, по который возвращается товар
        /// </summary>
        public string DealId { get; set; }

        /// <summary>
        /// Команда, по которой возвращается товар
        /// </summary>
        public string TeamId { get; set; }
        
        /// <summary>
        /// Организация получатель, по которой возврат
        /// </summary>
        public string RecipientId { get; set; }

        public string RecipientStorageId { get; set; }
        
        // Дата накладной списания
        public string ReturnFromClientWaybillDate { get; set; }

        // выбранная партия товара
        public Guid SaleWaybillRowId { get; set; }
        
        // сохраненная партия 
        public Guid CurrentSaleWaybillRowId { get; set; }

        public bool AllowToEdit { get; set; }

        /// <summary>
        /// Разрешается ли просматривать закупочные цены
        /// </summary>
        public bool AllowToViewPurchaseCost { get; set; }

        /// <summary>
        /// Разрешается ли просматривать учетные цены
        /// </summary>
        public bool AllowToViewAccountingPrice { get; set; }
    }
}