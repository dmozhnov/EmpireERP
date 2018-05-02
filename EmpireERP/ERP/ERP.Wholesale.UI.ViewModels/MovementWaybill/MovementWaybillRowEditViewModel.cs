using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;
using ERP.Utils.Mvc;

namespace ERP.Wholesale.UI.ViewModels.MovementWaybill
{
    public class MovementWaybillRowEditViewModel
    {
        public Guid Id { get; set; }

        public Guid MovementWaybillId { get; set; }        

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

        [DisplayName("Учетная цена отправителя")]
        public string SenderAccountingPriceString { get; set; }
        public string SenderAccountingPriceValue { get; set; }

        [DisplayName("Учетная цена получателя")]
        public string RecipientAccountingPriceString { get; set; }
        public string RecipientAccountingPriceValue { get; set; }

        [DisplayName("Наценка перемещения")]
        public string MovementMarkupPercent { get; set; }
        public string MovementMarkupSum { get; set; }

        [DisplayName("Наценка от закупки (новая)")]
        public string PurchaseMarkupPercent { get; set; }
        public string PurchaseMarkupSum { get; set; }

        [DisplayName("В наличии на складе")]
        public string AvailableToReserveFromStorageCount { get; set; }

        [DisplayName("Ожидается поступление")]
        public string AvailableToReserveFromPendingCount { get; set; }

        [DisplayName("Доступно к перемещению")]
        public string AvailableToReserveCount { get; set; }

        [DisplayName("Отгружаемое кол-во")]        
        [Required(ErrorMessage = "Укажите кол-во")]
        [GreaterByConst(0, ErrorMessage = "Кол-во должно быть больше 0")]
        [GreaterOrEqualByProperty("TotallyReserved", ErrorMessage = "Количество товара не может быть меньше {0}, т.к. товар участвует в исходящих накладных.")]
        public string MovingCount { get; set; }

        [DisplayName("Ставка НДС")]
        [Required(ErrorMessage = "Укажите ставку НДС")]
        public short ValueAddedTaxId { get; set; }
        public IEnumerable<ParamDropDownListItem> ValueAddedTaxList;

        [DisplayName("Сумма НДС отправ. | получ.")]
        public string SenderValueAddedTaxSum { get; set; }
        public string RecipientValueAddedTaxSum { get; set; }

        /// <summary>
        /// Количество товара, участвующее в исходящих накладных
        /// </summary>
        public string TotallyReserved { get; set; }

        public string Title { get; set; }

        public string SenderStorageId { get; set; }
        public string SenderId { get; set; }
        public string RecipientStorageId { get; set; }

        /// <summary>
        /// Дата накладной перемещения
        /// </summary>
        public string MovementWaybillDate { get; set; }

        /// <summary>
        /// Выбранная партия товара
        /// </summary>
        public Guid ReceiptWaybillRowId { get; set; }

        /// <summary>
        /// Сохраненная партия
        /// </summary>
        public Guid CurrentReceiptWaybillRowId { get; set; }

        public bool AllowToEdit { get; set; }

        /// <summary>
        /// Разрешается ли просматривать закупочные цены
        /// </summary>
        public bool AllowToViewPurchaseCost { get; set; }

        /// <summary>
        /// Разрешается ли менять ставку НДС
        /// </summary>
        public bool AllowToChangeValueAddedTax { get; set; }

        public string ManualSourcesInfo { get; set; }

        public MovementWaybillRowEditViewModel()
        {
            ManualSourcesInfo = "";
        }
    }
}