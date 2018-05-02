using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderPlannedExpensesEditViewModel
    {
        /// <summary>
        /// Затраты на производство
        /// </summary>
        [DisplayName("Затраты на производство")]
        [RegularExpression("[0-9]{1,12}([.,][0-9]{1,2})?", ErrorMessage="Укажите корректную сумму")]
        [Required(ErrorMessage = "Укажите сумму")]
        public string ProductionExpensesInCurrency { get; set; }

        /// <summary>
        /// Затраты на транспортировку
        /// </summary>
        [DisplayName("Затраты на транспортировку")]
        [RegularExpression("[0-9]{1,12}([.,][0-9]{1,2})?", ErrorMessage = "Укажите корректную сумму")]
        [Required(ErrorMessage = "Укажите сумму")]
        public string TransportationExpensesInCurrency { get; set; }

        /// <summary>
        /// Дополнительные затраты
        /// </summary>
        [DisplayName("Дополнительные затраты")]
        [RegularExpression("[0-9]{1,12}([.,][0-9]{1,2})?", ErrorMessage = "Укажите корректную сумму")]
        [Required(ErrorMessage = "Укажите сумму")]
        public string ExtraExpensesInCurrency { get; set; }

        /// <summary>
        /// Таможенные затраты
        /// </summary>
        [DisplayName("Таможенные затраты")]
        [RegularExpression("[0-9]{1,12}([.,][0-9]{1,2})?", ErrorMessage = "Укажите корректную сумму")]
        [Required(ErrorMessage = "Укажите сумму")]
        public string CustomsExpensesInCurrency { get; set; }

        /// <summary>
        /// Плановые оплаты за производство
        /// </summary>
        [DisplayName("Затраты на производство")]
        public string PlannedProductionPaymentsInBaseCurrency { get; set; }

        /// <summary>
        /// Плановые оплаты за транспортировку
        /// </summary>
        [DisplayName("Затраты на транспортировку")]
        public string PlannedTransportationPaymentsInBaseCurrency { get; set; }

        /// <summary>
        /// Плановые дополнительные оплаты
        /// </summary>
        [DisplayName("Дополнительные затраты")]
        public string PlannedExtraExpensesPaymentsInBaseCurrency { get; set; }

        /// <summary>
        /// Плановые таможенные оплаты
        /// </summary>
        [DisplayName("Таможенные затраты")]
        public string PlannedCustomsPaymentsInBaseCurrency { get; set; }

        /// <summary>
        /// Курс валюты
        /// </summary>
        public string CurrencyRate { get; set; }

        /// <summary>
        /// Валюта, в которой указываются суммы (должна соответствовать валюте заказа)
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        public string ProductionOrderId { get; set; }

        public bool AllowToEdit { get; set; }
    }
}
