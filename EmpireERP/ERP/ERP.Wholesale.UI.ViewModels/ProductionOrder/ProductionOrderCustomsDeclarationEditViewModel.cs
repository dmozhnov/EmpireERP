using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.ProductionOrder
{
    public class ProductionOrderCustomsDeclarationEditViewModel
    {
        /// <summary>
        /// Заголовок формы
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Код таможенного листа
        /// </summary>
        public string CustomsDeclarationId { get; set; }

        /// <summary>
        /// Код заказа, к которому относится таможенный лист
        /// </summary>
        public string ProductionOrderId { get; set; }

        /// <summary>
        /// Название заказа, к которому относится таможенный лист
        /// </summary>
        [DisplayName("Заказ")]
        public string ProductionOrderName { get; set; }

        [DisplayName("Номер ГТД")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(33, ErrorMessage = "Не более {1} символов")]
        public string CustomsDeclarationNumber { get; set; }

        [DisplayName("Название")]
        [Required(ErrorMessage = "Введите название")]
        [StringLength(200, ErrorMessage = "Не более {1} символов")]
        public string Name { get; set; }

        [DisplayName("Дата")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите дату")]
        public string CustomsDeclarationDate { get; set; }

        /// <summary>
        /// Сумма ввозных таможенных пошлин
        /// </summary>
        [DisplayName("Ввозные тамож. пошлины")]
        [StringLength(19, ErrorMessage = "Не более {1} символов")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Неверный формат суммы")]
        public string ImportCustomsDutiesSum { get; set; }

        /// <summary>
        /// Сумма вывозных таможенных пошлин
        /// </summary>
        [DisplayName("Вывозные тамож. пошлины")]
        [StringLength(19, ErrorMessage = "Не более {1} символов")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Неверный формат суммы")]
        public string ExportCustomsDutiesSum { get; set; }

        /// <summary>
        /// Сумма НДС (допускает отрицательные значения)
        /// </summary>
        [DisplayName("НДС")]
        [StringLength(19, ErrorMessage = "Не более {1} символов")]
        [RegularExpression(@"(-)?[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Неверный формат суммы")]
        public string ValueAddedTaxSum { get; set; }

        /// <summary>
        /// Акциз
        /// </summary>
        [DisplayName("Акциз")]
        [StringLength(19, ErrorMessage = "Не более {1} символов")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Неверный формат суммы")]
        public string ExciseSum { get; set; }

        /// <summary>
        /// Сумма таможенных сборов
        /// </summary>
        [DisplayName("Тамож. сборы")]
        [StringLength(19, ErrorMessage = "Не более {1} символов")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Неверный формат суммы")]
        public string CustomsFeesSum { get; set; }

        /// <summary>
        /// Сумма КТС (корректировка таможенной стоимости). Допускает отрицательные значения
        /// </summary>
        [DisplayName("КТС")]
        [StringLength(19, ErrorMessage = "Не более {1} символов")]
        [RegularExpression(@"(-)?[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Неверный формат суммы")]
        public string CustomsValueCorrection { get; set; }

        /// <summary>
        /// Всего оплачено (в рублях)
        /// </summary>
        [DisplayName("Всего оплачено")]
        public string PaymentSum { get; set; }

        /// <summary>
        /// Всего оплачено в процентах
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
        /// Можно ли редактировать поля
        /// </summary>
        public bool AllowToEdit { get; set; }
    }
}