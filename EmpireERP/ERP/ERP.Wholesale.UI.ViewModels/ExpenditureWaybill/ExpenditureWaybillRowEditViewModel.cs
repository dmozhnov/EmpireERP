using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.Utils.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.ExpenditureWaybill
{
    public class ExpenditureWaybillRowEditViewModel
    {
        public Guid Id { get; set; }
        public Guid ExpenditureWaybillId { get; set; }

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

        [DisplayName("Партия")]
        public string BatchName { get; set; }

        [DisplayName("Закупочная цена")]
        public string PurchaseCost { get; set; }

        [DisplayName("Учетная цена")]
        public string SenderAccountingPrice { get; set; }

        [DisplayName("Отпускная цена")]
        public string SalePriceString { get; set; }
        public string SalePriceValue { get; set; }

        [DisplayName("Наценка от закупки")]
        public string MarkupPercent { get; set; }
        public string MarkupSum { get; set; }

        [DisplayName("В наличии на складе")]
        public string AvailableToReserveFromStorageCount { get; set; }

        [DisplayName("Ожидается поступление")]
        public string AvailableToReserveFromPendingCount { get; set; }

        [DisplayName("Доступно к отгрузке")]
        public string AvailableToReserveCount { get; set; }

        [DisplayName("Скидка по квоте")]
        public string DealQuotaDiscountPercent { get; set; }

        [DisplayName("Отгружаемое кол-во")]
        [Required(ErrorMessage = "Укажите кол-во")]
        [GreaterByConst(0, ErrorMessage = "Кол-во должно быть больше 0")]
        public string SellingCount { get; set; }

        [DisplayName("Ставка НДС")]
        [Required(ErrorMessage = "Укажите ставку НДС")]
        public short ValueAddedTaxId { get; set; }
        public IEnumerable<ParamDropDownListItem> ValueAddedTaxList;

        [DisplayName("Сумма НДС")]
        public string ValueAddedTaxSum { get; set; }

        public string Title { get; set; }
        public string SenderStorageId { get; set; }
        public string SenderId { get; set; }

        // Дата накладной списания
        public string ExpenditureWaybillDate { get; set; }

        // выбранная партия товара
        public Guid ReceiptWaybillRowId { get; set; }
        // сохраненная партия 
        public Guid CurrentReceiptWaybillRowId { get; set; }

        /// <summary>
        /// Округлять ли отпускную цену до целого (после наложения скидок)
        /// </summary>
        public bool RoundSalePrice { get; set; }

        public bool AllowToEdit { get; set; }

        /// <summary>
        /// Разрешается ли просматривать закупочные цены
        /// </summary>
        public bool AllowToViewPurchaseCost { get; set; }

        public string ManualSourcesInfo { get; set; }
    }
}