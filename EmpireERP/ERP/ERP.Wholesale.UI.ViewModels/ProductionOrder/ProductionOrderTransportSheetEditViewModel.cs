using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderTransportSheetEditViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Код транспортного листа
        /// </summary>
        public string TransportSheetId { get; set; }

        /// <summary>
        /// Код заказа, к которому относится транспортный лист
        /// </summary>
        public string ProductionOrderId { get; set; }

        /// <summary>
        /// Название заказа, к которому относится транспортный лист
        /// </summary>
        [DisplayName("Заказ")]
        public string ProductionOrderName { get; set; }

        /// <summary>
        /// Способ выбора валюты
        /// </summary>
        public byte TransportSheetCurrencyDeterminationTypeId { get; set; }

        [DisplayName("Экспедитор")]
        [Required(ErrorMessage = "Введите экспедитора")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        public string ForwarderName { get; set; }

        [DisplayName("Дата заявки")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите дату заявки")]
        public string RequestDate { get; set; }

        [DisplayName("Дата погрузки")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string ShippingDate { get; set; }

        [DisplayName("Ожидаемая дата прибытия")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string PendingDeliveryDate { get; set; }

        [DisplayName("Фактическая дата прибытия")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string ActualDeliveryDate { get; set; }

        public string TransportSheetCurrencyLiteralCode { get; set; }

        [DisplayName("Валюта")]
        [Required(ErrorMessage = "Укажите валюту")]
        public short TransportSheetCurrencyId { get; set; }

        /// <summary>
        /// Перечень валют
        /// </summary>
        public IEnumerable<SelectListItem> CurrencyList { get; set; }

        [DisplayName("Курс")]
        public string TransportSheetCurrencyRateName { get; set; }
        public string TransportSheetCurrencyRateForEdit { get; set; }
        public string TransportSheetCurrencyRateForDisplay { get; set; }
        public string TransportSheetCurrencyRateId { get; set; }

        [DisplayName("Стоимость")]
        [StringLength(19, ErrorMessage = "Не более {1} символов")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Неверный формат стоимости")]
        [GreaterByConst(0, ErrorMessage = "Стоимость должна быть больше 0")]
        [Required(ErrorMessage = "Укажите стоимость")]
        public string CostInCurrency { get; set; }

        /// <summary>
        /// Сумма в рублях (при редактировании вычисляется, при создании ---)
        /// </summary>
        [DisplayName("в руб.")]
        public string CostInBaseCurrency { get; set; }

        [DisplayName("Номер коносамента")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string BillOfLadingNumber { get; set; }

        [DisplayName("Шипинговая линия")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ShippingLine { get; set; }

        [DisplayName("Портовый документ №")]
        [StringLength(100, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PortDocumentNumber { get; set; }

        [DisplayName("Дата портового документа")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string PortDocumentDate { get; set; }

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
        /// Можно ли редактировать поля, зависящие от наличия оплат по транспортному листу
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