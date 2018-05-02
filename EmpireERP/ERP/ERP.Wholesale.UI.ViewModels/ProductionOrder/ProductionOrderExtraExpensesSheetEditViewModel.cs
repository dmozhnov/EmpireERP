using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderExtraExpensesSheetEditViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Код листа дополнительных расходов
        /// </summary>
        public string ExtraExpensesSheetId { get; set; }

        /// <summary>
        /// Код заказа, к которому относится лист дополнительных расходов
        /// </summary>
        public string ProductionOrderId { get; set; }

        /// <summary>
        /// Название заказа, к которому относится лист дополнительных расходов
        /// </summary>
        [DisplayName("Заказ")]
        public string ProductionOrderName { get; set; }

        /// <summary>
        /// Способ выбора валюты
        /// </summary>
        public byte ExtraExpensesSheetCurrencyDeterminationTypeId { get; set; }

        [DisplayName("Контрагент")]
        [Required(ErrorMessage = "Введите контрагента")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        public string ExtraExpensesContractorName { get; set; }

        [DisplayName("Дата")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите дату")]
        public string ExtraExpensesSheetDate { get; set; }

        public string ExtraExpensesSheetCurrencyLiteralCode { get; set; }

        [DisplayName("Валюта")]
        [Required(ErrorMessage = "Укажите валюту")]
        public short ExtraExpensesSheetCurrencyId { get; set; }

        /// <summary>
        /// Перечень валют
        /// </summary>
        public IEnumerable<SelectListItem> CurrencyList { get; set; }

        [DisplayName("Курс")]
        public string ExtraExpensesSheetCurrencyRateName { get; set; }
        public string ExtraExpensesSheetCurrencyRateForEdit { get; set; }
        public string ExtraExpensesSheetCurrencyRateForDisplay { get; set; }
        public string ExtraExpensesSheetCurrencyRateId { get; set; }

        [DisplayName("Сумма расходов")]
        [StringLength(19, ErrorMessage = "Не более {1} символов")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Неверный формат суммы")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Сумма расходов должна быть больше 0")]
        [Required(ErrorMessage = "Укажите сумму расходов")]
        public string CostInCurrency { get; set; }

        /// <summary>
        /// Сумма в рублях (при редактировании вычисляется, при создании ---)
        /// </summary>
        [DisplayName("в руб.")]
        public string CostInBaseCurrency { get; set; }

        [DisplayName("Назначение расходов")]
        [Required(ErrorMessage = "Введите назначение расходов")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        public string ExtraExpensesPurpose { get; set; }

        /// <summary>
        /// Всего оплачено в валюте
        /// </summary>
        [DisplayName("Всего оплачено")]
        public string PaymentSumInCurrency { get; set; }

        /// <summary>
        /// Всего оплачено в рублях
        /// </summary>
        [DisplayName("в руб. по курсам оплат")]
        public string PaymentSumInBaseCurrency { get; set; }

        /// <summary>
        /// Всего оплачено в процентах (считается по валюте)
        /// </summary>
        [DisplayName("% от стоимости")]
        public string PaymentPercent { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        public string Comment { get; set; }

        /// <summary>
        /// Можно ли редактировать поля, зависящие от наличия оплат по листу дополнительных расходов
        /// </summary>
        public bool AllowToEditPaymentDependentFields { get; set; }

        /// <summary>
        /// Можно ли редактировать поля
        /// </summary>
        public bool AllowToEdit { get; set; }

        /// <summary>
        /// Можно ли выбирать валюту
        /// </summary>
        public bool AllowToChangeCurrency { get; set; }

        /// <summary>
        /// Можно ли выбирать курс валюты
        /// </summary>
        public bool AllowToChangeCurrencyRate { get; set; }
    }
}