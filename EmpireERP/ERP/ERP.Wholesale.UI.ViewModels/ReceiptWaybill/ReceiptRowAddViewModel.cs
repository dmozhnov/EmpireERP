using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.ReceiptWaybill
{
    public class ReceiptRowAddViewModel
    {
        [DisplayName("Товар")]
        [Required(ErrorMessage = "Укажите товар")]
        [Range(1, int.MaxValue, ErrorMessage = "Укажите товар")]
        public int ArticleId { get; set; }

        [DisplayName("ГТД")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(33, ErrorMessage = "Не более {1} символов")]
        public string CustomsDeclarationNumber { get; set; }

        public Guid WaybillId { get; set; }

        [DisplayName("Принимаемое количество")]
        [Required(ErrorMessage = "Укажите кол-во")]
        public string ReceiptedCount { get; set; }

        [DisplayName("Кол-во по документу")]
        [Required(ErrorMessage = "Укажите кол-во")]
        public string ProviderCount { get; set; }

        [DisplayName("Сумма по документу")]
        [Required(ErrorMessage = "Укажите сумму")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Не более 2 знаков после запятой")]
        public string ProviderSum { get; set; }

        [DisplayName("Страна производства")]
        [Required(ErrorMessage = "Укажите страну производства")]
        public short ProductionCountryId { get; set; }
        public IEnumerable<SelectListItem> ProductionCountryList { get; set; }

        [DisplayName("Фабрика-изготовитель")]
        [Required(ErrorMessage = "Укажите фабрику-изготовителя")]
        public string ManufacturerId { get; set; }

        /// <summary>
        /// Название производителя
        /// </summary>
        public string ManufacturerName { get; set; }

        public bool AllowToViewPurchaseCosts { get; set; }
    }
}