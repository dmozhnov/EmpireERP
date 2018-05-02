using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderPlannedPaymentEditViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Код оплаты
        /// </summary>
        public string ProductionOrderPlannedPaymentId { get; set; }

        /// <summary>
        /// Код заказа, к которому относится оплата
        /// </summary>
        public string ProductionOrderId { get; set; }

        [DisplayName("Период")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите начальную дату")]
        public string PlannedPaymentStartDate { get; set; }

        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите конечную дату")]
        public string PlannedPaymentEndDate { get; set; }

        /// <summary>
        /// Назначение
        /// </summary>
        [DisplayName("Назначение")]
        [Required(ErrorMessage = "Укажите назначение")]
        [StringLength(50, ErrorMessage = "Не более {1} символов")]
        public string PlannedPaymentPurpose { get; set; }

        /// <summary>
        /// Тип назначения
        /// </summary>
        [DisplayName("Тип назначения")]
        [Required(ErrorMessage = "Укажите тип назначения")]
        public byte ProductionOrderPaymentTypeId { get; set; }
        public IEnumerable<SelectListItem> ProductionOrderPaymentTypeList { get; set; }

        [DisplayName("Валюта")]
        [Required(ErrorMessage = "Укажите валюту")]
        public short PlannedPaymentCurrencyId { get; set; }
        public IEnumerable<SelectListItem> PlannedPaymentCurrencyList { get; set; }

        public string PlannedPaymentCurrencyLiteralCode { get; set; }

        [DisplayName("Курс для расчета")]
        public string PlannedPaymentCurrencyRateName { get; set; }
        public string PlannedPaymentCurrencyRateValue { get; set; }
        public string PlannedPaymentCurrencyRateString { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PlannedPaymentCurrencyRateId { get; set; }

        [DisplayName("Плановая сумма")]
        [Required(ErrorMessage = "Укажите сумму")]
        [StringLength(19, ErrorMessage = "Не более {1} символов")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Неверный формат суммы")]
        [GreaterByConst(0, ErrorMessage = "Сумма оплаты должна быть больше 0")]
        public string SumInCurrency { get; set; }

        /// <summary>
        /// Всего оплачено (в валюте планируемого платежа)
        /// </summary>
        [DisplayName("Всего оплачено")]
        public string CurrentPaymentSumInCurrency { get; set; }

        /// <summary>
        /// Всего оплачено (в базовой валюте по курсам оплат)
        /// </summary>
        [DisplayName("в руб. по курсам оплат")]
        public string CurrentPaymentSumInBaseCurrency { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        public string Comment { get; set; }

        /// <summary>
        /// Можно ли редактировать поля
        /// </summary>
        public bool AllowToEdit { get; set; }

        /// <summary>
        /// Можно ли редактировать сумму оплаты и менять валюту
        /// </summary>
        public bool AllowToEditSum { get; set; }

        /// <summary>
        /// Можно ли выбирать курс валюты
        /// </summary>
        public bool AllowToChangeCurrencyRate { get; set; }
    }
}