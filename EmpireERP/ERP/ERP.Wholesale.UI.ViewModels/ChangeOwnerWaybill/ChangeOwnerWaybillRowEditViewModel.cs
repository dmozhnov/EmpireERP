using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;
using ERP.Utils.Mvc;

namespace ERP.Wholesale.UI.ViewModels.ChangeOwnerWaybill
{
    public class ChangeOwnerWaybillRowEditViewModel
    {
        public Guid ChangeOwnerWaybillRowId { get; set; }

        public Guid ChangeOwnerWaybillId { get; set; }

        [DisplayName("Наименование товара")]
        public string ArticleName { get; set; }

        /// <summary>
        /// Название единицы измерения товара
        /// </summary>
        public string MeasureUnitName { get; set; }

        /// <summary>
        /// Количество допускаемых знаков после запятой в количестве товара (требуется только при начальном появлении формы)
        /// </summary>
        public string MeasureUnitScale { get; set; }

        [Required(ErrorMessage = "Укажите товар")]
        [Range(1, int.MaxValue, ErrorMessage = "Укажите товар")]
        public int ArticleId { get; set; }

        [DisplayName("Партия")]
        public string BatchName { get; set; }

        [DisplayName("Учетная цена")]
        public string AccountingPriceString { get; set; }
        public string AccountingPriceValue { get; set; }

        [DisplayName("Доступно к перемещению")]
        public string AvailableToReserveCount { get; set; }

        [DisplayName("Закупочная цена")]
        public string PurchaseCost { get; set; }

        [DisplayName("В наличии на складе")]
        public string AvailableToReserveFromStorageCount { get; set; }

        [DisplayName("Ожидается поступление")]
        public string AvailableToReserveFromPendingCount { get; set; }

        [DisplayName("Отгружаемое кол-во")]
        [Required(ErrorMessage = "Укажите кол-во")]
        [GreaterByConst(0, ErrorMessage = "Кол-во должно быть больше 0")]
        [GreaterOrEqualByProperty("TotallyReserved", ErrorMessage = "Количество товара не может быть меньше {0}, т.к. товар участвует в исходящих накладных.")]
        public string MovingCount { get; set; }

        [DisplayName("Ставка НДС")]
        [Required(ErrorMessage = "Укажите ставку НДС")]
        public short ValueAddedTaxId { get; set; }
        public IEnumerable<ParamDropDownListItem> ValueAddedTaxList;

        [DisplayName("Сумма НДС")]
        public string ValueAddedTaxSum { get; set; }

        //количество товара, участвующее в исходящих накладных
        public string TotallyReserved { get; set; }

        public string Title { get; set; }

        public string StorageId { get; set; }
        public string SenderId { get; set; }

        // Дата накладной перемещения
        public string ChangeOwnerWaybillDate { get; set; }

        // выбранная партия товара
        public Guid ReceiptWaybillRowId { get; set; }
        // сохраненная партия 
        public Guid CurrentReceiptWaybillRowId { get; set; }

        public bool AllowToEdit { get; set; }

        public string ManualSourcesInfo { get; set; }

        /// <summary>
        /// Разрешается ли просматривать закупочные цены
        /// </summary>
        public bool AllowToViewPurchaseCost { get; set; }

        public ChangeOwnerWaybillRowEditViewModel()
        {
            MovingCount = "";
            ManualSourcesInfo = "";
        }
    }
}
