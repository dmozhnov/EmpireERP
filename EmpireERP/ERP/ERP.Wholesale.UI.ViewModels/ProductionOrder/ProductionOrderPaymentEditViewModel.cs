using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderPaymentEditViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Код оплаты
        /// </summary>
        public string ProductionOrderPaymentId { get; set; }

        /// <summary>
        /// Код заказа, к которому относится оплата
        /// </summary>
        public string ProductionOrderId { get; set; }

        /// <summary>
        /// Название заказа, к которому относится оплата
        /// </summary>
        [DisplayName("Заказ")]
        public string ProductionOrderName { get; set; }

        /// <summary>
        /// Тип документа, на который относится оплата
        /// </summary>
        public string ProductionOrderPaymentTypeId { get; set; }

        /// <summary>
        /// Код документа, на который относится оплата
        /// </summary>
        public string ProductionOrderPaymentDocumentId { get; set; }

        /// <summary>
        /// Код планируемого платежа, на который относится оплата
        /// </summary>
        public string ProductionOrderPlannedPaymentId { get; set; }

        [DisplayName("№ платежного документа")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PaymentDocumentNumber { get; set; }

        [DisplayName("Дата оплаты")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите дату")]
        public string PaymentDate { get; set; }

        [DisplayName("Валюта оплаты")]
        public string PaymentCurrencyLiteralCode { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PaymentCurrencyId { get; set; }

        [DisplayName("Применяемый курс")]
        public string PaymentCurrencyRateName { get; set; }
        public string PaymentCurrencyRateValue { get; set; }
        public string PaymentCurrencyRateString { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PaymentCurrencyRateId { get; set; }

        [DisplayName("Сумма в валюте")]
        [Required(ErrorMessage = "Укажите сумму")]
        [StringLength(19, ErrorMessage = "Не более {1} символов")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Неверный формат суммы")]
        [CompareWithProperty("DebtRemainderInCurrencyValue", Utils.CompareOperationType.Le, ErrorMessage = "Сумма оплаты не может быть больше неоплаченного остатка ({0})")]
        [GreaterByConst(0, ErrorMessage = "Сумма оплаты должна быть больше 0")]
        public string SumInCurrency { get; set; }

        /// <summary>
        /// Неоплаченный остаток в валюте
        /// </summary>
        [DisplayName("Неоплаченный остаток")]
        public string DebtRemainderInCurrencyString { get; set; }
        public string DebtRemainderInCurrencyValue { get; set; }

        /// <summary>
        /// Сумма в рублях (при редактировании вычисляется, при создании ---)
        /// </summary>
        public string SumInBaseCurrency { get; set; }

        /// <summary>
        /// Назначение оплаты
        /// </summary>
        [DisplayName("Назначение оплаты")]
        public string ProductionOrderPaymentPurpose { get; set; }

        /// <summary>
        /// Форма оплаты
        /// </summary>
        [DisplayName("Форма оплаты")]
        [Required(ErrorMessage = "Укажите форму оплаты")]
        public byte ProductionOrderPaymentForm { get; set; }

        /// <summary>
        /// Перечень возможных форм оплаты
        /// </summary>
        public IEnumerable<SelectListItem> ProductionOrderPaymentFormList { get; set; }

        /// <summary>
        /// Сумма планового платежа
        /// </summary>
        [DisplayName("Планируемый платеж")]
        public string ProductionOrderPlannedPaymentSumInCurrency { get; set; }

        /// <summary>
        /// Валюта планового платежа
        /// </summary>
        public string ProductionOrderPlannedPaymentCurrencyLiteralCode { get; set; }

        /// <summary>
        /// Оплаченная сумма планового платежа
        /// </summary>
        [DisplayName("оплачено")]
        public string ProductionOrderPlannedPaymentPaidSumInBaseCurrency { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [DisplayName("Комментарий")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        public string Comment { get; set; }

        /// <summary>
        /// Можно ли редактировать плановый платеж
        /// </summary>
        public bool AllowToEditPlannedPayment { get; set; }

        /// <summary>
        /// Можно ли редактировать поля
        /// </summary>
        public bool AllowToEdit { get; set; }

        /// <summary>
        /// Можно ли выбирать курс валюты
        /// </summary>
        public bool AllowToChangeCurrencyRate { get; set; }
    }
}