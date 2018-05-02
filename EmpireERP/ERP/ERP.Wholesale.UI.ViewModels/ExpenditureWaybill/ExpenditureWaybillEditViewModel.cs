using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;
using ERP.Wholesale.UI.ViewModels.BaseWaybill;

namespace ERP.Wholesale.UI.ViewModels.ExpenditureWaybill
{
    public class ExpenditureWaybillEditViewModel : BaseWaybillEditViewModel
    {
        /// <summary>
        /// Код
        /// </summary>
        public Guid Id { get; set; }

        [DisplayName("Дата")]
        [Required(ErrorMessage = "Укажите дату")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string Date { get; set; }

        /// <summary>
        /// Разрешено ли изменение даты накладной
        /// </summary>
        public bool AllowToChangeDate { get; set; }

        /// <summary>
        /// Место хранения
        /// </summary>
        [DisplayName("Место хранения")]
        [Required(ErrorMessage = "Укажите место хранения")]
        public short SenderStorageId { get; set; }
        public IEnumerable<SelectListItem> StorageList { get; set; }

        /// <summary>
        /// Клиент
        /// </summary>
        [DisplayName("Клиент")]
        [Required(ErrorMessage = "Укажите клиента")]
        public string ClientId { get; set; }
        public string ClientName { get; set; }

        /// <summary>
        /// Организация клиента
        /// </summary>
        [DisplayName("Организация клиента")]
        [Required(ErrorMessage = "Укажите организацию клиента")]
        public string ClientOrganizationId { get; set; }
        public string ClientOrganizationName { get; set; }

        /// <summary>
        /// Сделка
        /// </summary>
        [DisplayName("Сделка")]
        [Required(ErrorMessage = "Укажите сделку")]
        public string DealId { get; set; }
        public string DealName { get; set; }

        /// <summary>
        /// Текущая квота
        /// </summary>
        [DisplayName("Квота")]
        [Required(ErrorMessage = "Укажите квоту")]
        public string DealQuotaId { get; set; }
        public string DealQuotaName { get; set; }

        /// <summary>
        /// Ставка НДС
        /// </summary>
        [DisplayName("Ставка НДС")]
        [Required(ErrorMessage = "Укажите ставку НДС")]
        public short ValueAddedTaxId { get; set; }
        public IEnumerable<SelectListItem> ValueAddedTaxList;

        [DisplayName("Комментарий")]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Comment { get; set; }

        /// <summary>
        /// Форма взаиморасчетов (1 - "Предоплата", 0 - "Отсрочка платежа")
        /// </summary>
        [DisplayName("Форма взаиморасчетов")]
        public byte IsPrepayment { get; set; }

        /// <summary>
        /// Округлять ли отпускную цену до целого (после наложения скидок)
        /// </summary>
        [DisplayName("Округлять цену товаров до целого")]
        public string RoundSalePrice { get; set; }

        /// <summary>
        /// Опция, соответствующая значению "false" радиокнопки выбора формы взаиморасчетов
        /// </summary>
        public readonly string IsPrepayment_false;

        /// <summary>
        /// Опция, соответствующая значению "true" радиокнопки выбора формы взаиморасчетов
        /// </summary>
        public readonly string IsPrepayment_true;

        /// <summary>
        /// Разрешено ли изменять форму взаиморасчетов
        /// </summary>
        public bool AllowToChangePaymentType;

        /// <summary>
        /// Разрешено ли изменять команду
        /// </summary>
        public bool AllowToEditTeam { get; set; }

        public bool AllowToEdit { get; set; }

        public string Title { get; set; }
        public string BackURL { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Адрес доставки
        /// </summary>
        [DisplayName("Адрес доставки")]
        [StringLength(250, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string CustomDeliveryAddress { get; set; }

        public string ClientDeliveryAddress { get; set; }
        public string OrganizationDeliveryAddress { get; set; }

        /// <summary>
        /// Тип адреса доставки
        /// </summary>
        [DisplayName("Тип адреса доставки")]
        [Required(ErrorMessage = "Укажите тип адреса доставки")]
        public string DeliveryAddressTypeId { get; set; }
        public IEnumerable<SelectListItem> DeliveryAddressTypeList { get; set; }

        /// <summary>
        /// Код команды
        /// </summary>
        [DisplayName("Команда")]        
        [Required(ErrorMessage = "Укажите команду")]
        public string TeamId { get; set; }
        public IEnumerable<SelectListItem> TeamList { get; set; }

        /// <summary>
        /// Сумма текущих взаиморасчетов за наличный расчет по выбранному договору
        /// </summary>
        [DisplayName("Сумма текущих взаиморасчетов за наличный расчет по договору, связанному с выбранной сделкой, составляет")]
        public string DealContractCashPaymentSum { get; set; }

        public ExpenditureWaybillEditViewModel()
        {
            IsPrepayment_false = "Отсрочка платежа";
            IsPrepayment_true = "Предоплата";

            StorageList = new List<SelectListItem>();
            TeamList = new List<SelectListItem>();
        }
    }
}