using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels.AccountingPriceList
{
    public class ArticleAccountingPriceEditViewModel
    {
        /// <summary>
        /// Идентификатор товара (элемента)
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Идентификатор реестра (родительского объекта)
        /// </summary>
        public Guid AccountingPriceListId { get; set; }

        [DisplayName("Наименование товара")]        
        public string ArticleName { get; set; }

        [Required(ErrorMessage="Укажите товар")]
        [Range(1, 4294967296, ErrorMessage = "Укажите товар")]
        public int ArticleId { get; set; }

        /// <summary>
        /// Код товара для отображения. Равен пустой строке при создании и строковому представлению ArticleId иначе
        /// </summary>
        [DisplayName("Код")]
        public string ArticleIdForDisplay { get; set; }

        [DisplayName("Артикул")]
        public string ArticleNumber { get; set; }

        [DisplayName("Рассчитанная учетная цена")]
        public string CalculatedAccountingPrice { get; set; }

        [DisplayName("Учетная цена")]
        [Required(ErrorMessage = "Укажите учетную цену")]
        [RegularExpression(@"[0-9]{1,16}([,.][0-9]{1,2})?", ErrorMessage = "Не более 2 знаков после запятой")]
        public string AccountingPrice { get; set; }

        [DisplayName("Средняя закуп. стоимость")]
        public string AveragePurchaseCost { get; set; }

        [DisplayName("Средняя учетная стоимость")]
        public string AverageAccountingPrice { get; set; }

        [DisplayName("Максимальная ЗЦ")]
        public string MaxPurchaseCost { get; set; }

        [DisplayName("Максимальная УЦ")]
        public string MaxAccountingPrice { get; set; }

        [DisplayName("Минимальная ЗЦ")]
        public string MinPurchaseCost { get; set; }

        [DisplayName("Минимальная УЦ")]
        public string MinAccountingPrice { get; set; }

        [DisplayName("Последняя ЗЦ")]
        public string LastPurchaseCost { get; set; }

        [DisplayName("% наценки по умолчанию")]
        public string DefaultMarkupPercent { get; set; }

        [DisplayName("Правило формирования УЦ")]
        public string AccountingPriceRule { get; set; }

        public string UsedDefaultRule { get; set; }

        public string DefaultRuleErrorCaption { get; private set; }

        public string Title { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToEditPrice { get; set; }

        public ArticleAccountingPriceEditViewModel()
        {
            ArticleName = "";
            ArticleId = 0;
            AccountingPrice = "0";
            Id = Guid.Empty;
            AveragePurchaseCost = AverageAccountingPrice = MaxPurchaseCost = MaxAccountingPrice =
                MinPurchaseCost = MinAccountingPrice = LastPurchaseCost = DefaultMarkupPercent = AccountingPriceRule = "---";
            UsedDefaultRule = "0";
            DefaultRuleErrorCaption = "Не удалось использовать заданное правило";
        }
    }
}