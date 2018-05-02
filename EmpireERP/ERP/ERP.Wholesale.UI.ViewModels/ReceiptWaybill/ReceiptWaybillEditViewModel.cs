using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;
using ERP.Wholesale.UI.ViewModels.BaseWaybill;

namespace ERP.Wholesale.UI.ViewModels.ReceiptWaybill
{
    public class ReceiptWaybillEditViewModel : BaseWaybillEditViewModel
    {
        public string Title { get; set; }
        public string Name { get; set; }

        public Guid Id { get; set; }

        /// <summary>
        /// Признак того, что накладная создана (создается) из партии заказа
        /// </summary>
        public bool IsCreatedFromProductionOrderBatch { get; set; }

        /// <summary>
        /// Код партии заказа, из которой создана накладная
        /// </summary>
        public string ProductionOrderBatchId { get; set; }

        /// <summary>
        /// Название заказа на производство, по партии которого создана накладная
        /// </summary>
        [DisplayName("Заказ")]
        public string ProductionOrderName { get; set; }

        /// <summary>
        /// Название производителя, по партии заказа которого создана накладная
        /// </summary>
        [DisplayName("Производитель")]
        public string ProducerName { get; set; }

        /// <summary>
        /// Признак новой накладной
        /// </summary>
        public bool IsNew
        {
            get
            {
                return Id == Guid.Empty;
            }
        }

        /// <summary>
        /// Признак того, что накладная ожидается
        /// </summary>
        public bool IsPending { get; set; }

        [DisplayName("Дата")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите дату")]
        public string Date { get; set; }

        /// <summary>
        /// Разрешено ли изменение даты накладной
        /// </summary>
        public bool AllowToChangeDate { get; set; }


        [DisplayName("Место хранения")]
        [Required(ErrorMessage = "Укажите место хранения")]
        public short ReceiptStorageId { get; set; }
        public IEnumerable<SelectListItem> ReceiptStorageList;

        [DisplayName("Организация")]
        [Required(ErrorMessage = "Укажите организацию")]
        public int AccountOrganizationId { get; set; }
        public IEnumerable<SelectListItem> AccountOrganizationList;

        [DisplayName("Поставщик")]
        [Required(ErrorMessage = "Укажите поставщика")]
        public int ProviderId { get; set; }
        public IEnumerable<SelectListItem> ProviderList;

        [DisplayName("Номер накл. постав.")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(25, ErrorMessage = "Не более {1} символов")]
        public string ProviderNumber { get; set; }

        [DisplayName("Дата накл. постав.")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string ProviderDate { get; set; }

        [DisplayName("Номер С-Ф постав.")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(25, ErrorMessage = "Не более {1} символов")]
        public string ProviderInvoiceNumber { get; set; }

        [DisplayName("Дата С-Ф постав.")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string ProviderInvoiceDate { get; set; }

        [DisplayName("Договор")]
        [Required(ErrorMessage = "Укажите договор")]
        public short ContractId { get; set; }
        public IEnumerable<SelectListItem> ContractList;

        [DisplayName("Номер ГТД")]
        [StringLength(23, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string CustomsDeclarationNumber { get; set; }

        /// <summary>
        /// Признак того, откуда берется номер ГТД
        /// "1" - из приходной накладной, "0" - последний номер по позиции
        /// </summary>
        public string IsCustomsDeclarationNumberFromReceiptWaybill { get; set; }
        
        /// <summary>
        /// Использовать номер ГТД из данной приходной накладной для всех позиций
        /// </summary>
        public string IsCustomsDeclarationNumberFromReceiptWaybill_true { get; set; }

        /// <summary>
        /// Использовать последний номер ГТД по данному товару
        /// </summary>        
        public string IsCustomsDeclarationNumberFromReceiptWaybill_false { get; set; }

        [DisplayName("Сумма накладной")]
        [Required(ErrorMessage = "Укажите общую сумму накладной")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Не более 2 знаков после запятой")]
        public string PendingSum { get; set; }

        [DisplayName("Ставка НДС")]
        [Required(ErrorMessage = "Укажите ставку НДС")]
        public short PendingValueAddedTaxId { get; set; }
        public IEnumerable<SelectListItem> ValueAddedTaxList;

        [DisplayName("% скидки")]
        [RegularExpression(@"[0-9]{1,3}([,.][0-9]{1,2})?", ErrorMessage = "Значение должно быть не больше 100 и содержать не более 2 знаков после запятой")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string DiscountPercent { get; set; }

        [DisplayName("Сумма скидки")]
        [Required(ErrorMessage = "Укажите сумму скидки")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Не более 2 знаков после запятой")]
        public string DiscountSum { get; set; }

        [DisplayName("Комментарий")]
        [StringLength(4000, ErrorMessage = "Не более {1} символов")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Comment { get; set; }

        /// <summary>
        /// Флаг разрешения изменения поставщика
        /// </summary>
        public bool AllowToChangeProvider { get; set; }

        /// <summary>
        /// Флаг разрешения смены места хранения и организации получателя
        /// </summary>
        public bool AllowToChangeStorageAndOrganization { get; set; }

        public bool AllowToEdit { get; set; }

        public bool AllowToEditProviderDocuments { get; set; }

        public bool AllowToViewPurchaseCosts { get; set; }

        /// <summary>
        /// Обратный адрес
        /// </summary>
        public string BackURL { get; set; }

        public ReceiptWaybillEditViewModel()
        {
            ReceiptStorageList = new List<SelectListItem>();
            AccountOrganizationList = new List<SelectListItem>();
            ProviderList = new List<SelectListItem>();
            ContractList = new List<SelectListItem>();
            ValueAddedTaxList = new List<SelectListItem>();
            BackURL = string.Empty;
            IsPending = false;
            IsCustomsDeclarationNumberFromReceiptWaybill = "0";
            IsCustomsDeclarationNumberFromReceiptWaybill_true = "Использовать указанный: ";
            IsCustomsDeclarationNumberFromReceiptWaybill_false = "Использовать последний по позиции";
        }
    }
}