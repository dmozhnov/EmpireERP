using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.AccountingPriceList
{
    public class AccountingPriceListEditViewModel
    {       
        [DisplayName("Номер")]
        [Required(ErrorMessage = "Укажите номер")]
        [StringLength(25, ErrorMessage = "Не более {1} символов")]
        public string Number { get; set; }

        /// <summary>
        /// Для скрытого поля, отображающего, уникален ли введенный номер. 1 - уникален, остальное - нет.
        /// </summary>
        [RegularExpression("1", ErrorMessage = "Введите уникальный номер")]
        public byte NumberIsUnique { get; set; }

        [DisplayName("Дата и время начала действия")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        [Required(ErrorMessage = "Укажите дату")]
        public string StartDate { get; set; }

        [DisplayName("Время начала действия")]
        [RegularExpression("([0-1][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]", ErrorMessage = "Укажите корректное время")]
        [Required(ErrorMessage = "Укажите время начала действия")]
        public string StartTime { get; set; }

        [DisplayName("Дата и время завершения действия")]
        [IsDate(ErrorMessage = "Неверный формат даты")]
        public string EndDate { get; set; }

        [DisplayName("Время начала действия")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [RegularExpression("([0-1][0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]?", ErrorMessage = "Укажите корректное время")]
        public string EndTime { get; set; }

        /// <summary>
        /// Дополнительный идентификатор. При основании создания "Приход" содержит Id приходной накладной.
        /// При основании "По месту хранения" содержит Id места хранения.
        /// </summary>
        public string AdditionalId { get; set; }

        /// <summary>
        /// Идентификатор редактируемого реестра цен. Guid.Empty (все нули), если реестр создается
        /// </summary>
        public Guid AccountingPriceListId { get; set; }

        [DisplayName("Основание")]        
        //[Required(ErrorMessage = "Выберите основание")]
        public string Reason { get; set; }

        public string ReasonId { get; set; }                
      
        public string BackURL { get; set; }

        public string Title { get; set; }
        public string Name { get; set; }

        [DisplayName("Правила расчета учетной цены по умолчанию")]
        public short AccountingPriceCalcRuleType { get; set; }
        public string AccountingPriceCalcRuleType_caption1 { get; set; }
        public string AccountingPriceCalcRuleType_caption2 { get; set; }

        [DisplayName("Правила определения закупочной цены")]
        public short PurchaseCostDeterminationRuleType { get; set; }
        public string PurchaseCostDeterminationRuleType_caption1 { get; set; }
        public string PurchaseCostDeterminationRuleType_caption2 { get; set; }
        public string PurchaseCostDeterminationRuleType_caption3 { get; set; }
        public string PurchaseCostDeterminationRuleType_caption4 { get; set; }

        [DisplayName("Правила определения % наценки")]
        public short MarkupPercentDeterminationRuleType { get; set; }
        public string MarkupPercentDeterminationRuleType_caption1 { get; set; }
        public string MarkupPercentDeterminationRuleType_caption2 { get; set; }
        public string MarkupPercentDeterminationRuleType_caption3 { get; set; }

        [RequiredByRadioButton("MarkupPercentDeterminationRuleType", 3, ErrorMessage = "Введите значение")]
        [RegularExpressionByRadioButton("MarkupPercentDeterminationRuleType", 3, "^[0-9]+([.,][0-9]?[0-9])?$", ErrorMessage = "Неверный формат числа")]        
        public string CustomMarkupValue { get; set; }

        [DisplayName("Правила определения учетной цены")]
        public short AccountingPriceDeterminationRuleType { get; set; }
        public string AccountingPriceDeterminationRuleType_caption1 { get; set; }
        public string AccountingPriceDeterminationRuleType_caption2 { get; set; }
        public string AccountingPriceDeterminationRuleType_caption3 { get; set; }
        public string AccountingPriceDeterminationRuleType_caption4 { get; set; }

        [DisplayName("Назначение % наценки / скидки")]
        public short MarkupValueRuleType { get; set; }
        public string MarkupValueRuleType_caption1 { get; set; }
        public string MarkupValueRuleType_caption2 { get; set; }

        [RequiredByRadioButton("MarkupValueRuleType", 1, ErrorMessage="Введите значение")]
        [RegularExpressionByRadioButton("MarkupValueRuleType", 1, "^[0-9]+([.,][0-9]?[0-9])?$", ErrorMessage="Неверный формат числа")]   
        public string MarkupValuePercent { get; set; }

        [RequiredByRadioButton("MarkupValueRuleType", 2, ErrorMessage = "Введите значение")]
        [RegularExpressionByRadioButton("MarkupValueRuleType", 2, "^[0-9]+([.,][0-9]?[0-9])?$", ErrorMessage = "Неверный формат числа")]
        [RangeByRadioButton("MarkupValueRuleType", 2, 0, 100, ErrorMessage="Скидка не может быть больше 100%")]
        public string DiscountValuePercent { get; set; }

        public IEnumerable<SelectListItem> StorageTypeList { get; set; }

        [RequiredByRadioButton("AccountingPriceDeterminationRuleType", 1, ErrorMessage = "Выберите значение")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StorageTypeId1 { get; set; }

        [RequiredByRadioButton("AccountingPriceDeterminationRuleType", 2, ErrorMessage = "Выберите значение")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StorageTypeId2 { get; set; }

        [RequiredByRadioButton("AccountingPriceDeterminationRuleType", 3, ErrorMessage = "Выберите значение")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StorageTypeId3 { get; set; }

        /// <summary>
        /// Те МХ, которыми будут заполняться комбобоксы для установки правила по умолчанию
        /// </summary>
        public IEnumerable<SelectListItem> StorageList { get; set; }
        [RequiredByRadioButton("AccountingPriceDeterminationRuleType", 4, ErrorMessage = "Выберите значение")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string StorageId { get; set; }
        public string StorageName { get; set; }


        // Добавочка
        [DisplayName("Правила формирования последней цифры в учетной цене")]
        public short LastDigitCalcRuleType { get; set; }
        public string LastDigitCalcRuleType_caption1 { get; set; }
        public string LastDigitCalcRuleType_caption2 { get; set; }
        public string LastDigitCalcRuleType_caption3 { get; set; }
        public string LastDigitCalcRuleType_caption4 { get; set; }
        public string LastDigitCalcRuleType_caption5 { get; set; }


        public int LastDigitCalcRuleAsFirstIn { get; set; }    // Возможно надо сменить тип данных!!!
        [RequiredByRadioButton("LastDigitCalcRuleType", 3, ErrorMessage = "Выберите значение")]
        public short StorageTypeId4 { get; set; }
        public byte? LastDigitCalcRuleNumber { get; set; }
        public short? LastDigitCalcRulePenny { get; set; }

        /// <summary>
        /// Те МХ, которые будут в списке наверху, из которых будет выбираться распространение реестра
        /// Список мест хранения, ключ - id, значение - название МХ        
        /// </summary>
        public Dictionary<string, string> Storages { get; set; }

        /// <summary>
        /// Строка кодов выбранных мест хранения
        /// </summary>
        public string StorageIDs { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToEditStorages { get; set; }

        /// <summary>
        /// Можно ли выбирать правило "по закупочной цене"
        /// </summary>
        public bool AllowToSetByPurchaseCost { 
            get{
                return allowToSetByPurchaseCost;
            } 
            set{
                if (value == false)
                {
                    AccountingPriceCalcRuleType = 2;
                }
                allowToSetByPurchaseCost = value;
            } 
        }
        private bool allowToSetByPurchaseCost;

        public AccountingPriceListEditViewModel()
        {            
            StorageTypeList = new List<SelectListItem>();
            
            AccountingPriceCalcRuleType = PurchaseCostDeterminationRuleType = MarkupPercentDeterminationRuleType = AccountingPriceDeterminationRuleType = MarkupValueRuleType = LastDigitCalcRuleType = 1;

            StorageTypeId1 = StorageTypeId2 = StorageTypeId3 = "";
            StorageId = "";
        }
    }
}