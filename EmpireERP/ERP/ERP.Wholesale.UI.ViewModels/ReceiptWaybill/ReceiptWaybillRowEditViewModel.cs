using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;
using ERP.Utils.Mvc;

namespace ERP.Wholesale.UI.ViewModels.ReceiptWaybill
{
    public class ReceiptWaybillRowEditViewModel
    {
        public Guid Id { get; set; }

        public Guid ReceiptWaybillId { get; set; }

        [DisplayName("Наименование товара")]
        public string ArticleName { get; set; }

        [Required(ErrorMessage = "Укажите товар")]
        [Range(1, int.MaxValue, ErrorMessage = "Укажите товар")]
        public int ArticleId { get; set; }

        [DisplayName("Кол-во, ожидаемое к поставке")]
        [Required(ErrorMessage = "Укажите кол-во")]
        [GreaterOrEqualByProperty("TotallyReserved", ErrorMessage = "Кол-во не может быть меньше, чем уже зарезервировано ({0})")]
        [GreaterByConst(0, ErrorMessage = "Кол-во должно быть больше 0")]
        public string PendingCount { get; set; }

        public string MeasureUnitName { get; set; }

        /// <summary>
        /// Количество допускаемых знаков после запятой в количестве товара (требуется только при начальном появлении формы)
        /// </summary>
        public string MeasureUnitScale { get; set; }

        [DisplayName("Сумма по накладной")]
        [Required(ErrorMessage = "Укажите сумму")]
        public string PendingSum { get; set; }

        [DisplayName("Номер ГТД")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(33, ErrorMessage = "Не более {1} символов")]
        public string CustomsDeclarationNumber { get; set; }

        [DisplayName("Закупочная цена")]
        [Required(ErrorMessage = "Укажите цену")]
        public string PurchaseCost { get; set; }

        /// <summary>
        /// Ставка НДС
        /// </summary>
        [DisplayName("Ставка НДС")]
        [Required(ErrorMessage = "Укажите ставку НДС")]
        public short PendingValueAddedTaxId { get; set; }

        /// <summary>
        /// Список ставок НДС
        /// </summary>
        public IEnumerable<ParamDropDownListItem> PendingValueAddedTaxList { get; set; }

        [DisplayName("Сумма НДС")]
        public string PendingValueAddedTaxSum { get; set; }

        public string Title { get; set; }

        [DisplayName("Страна производства")]
        [Required(ErrorMessage = "Укажите страну производства")]
        public short ProductionCountryId { get; set; }
        public IEnumerable<SelectListItem> ProductionCountryList { get; set; }

        [DisplayName("Фабрика-изготовитель")]
        [Required(ErrorMessage = "Укажите фабрику-изготовителя")]
        public string ManufacturerId { get; set; }

        /// <summary>
        /// Название фабрики-изготовителя
        /// </summary>
        public string ManufacturerName { get; set; }

        /// <summary>
        /// Флаг, отражающий, какой показатель (сумма, зак. цена) изменен последним (1 - изменена зак. цена)
        /// </summary>
        public string PendingSumIsChangedLast { get; set; }

        /// <summary>
        /// Признак возможности редактировать
        /// </summary>
        public bool AllowToEdit { get; set; }

        /// <summary>
        /// Можно ли добавлять в список страну
        /// </summary>
        public bool AllowToAddCountry { get; set; }

        /// <summary>
        /// Можно ли просматривать закупочные цены
        /// </summary>
        public bool AllowToViewPurchaseCosts { get; set; }

        /// <summary>
        /// Суммарно зарезервировано в исходящих накладных
        /// </summary>
        public string TotallyReserved { get; set; }
    }
}