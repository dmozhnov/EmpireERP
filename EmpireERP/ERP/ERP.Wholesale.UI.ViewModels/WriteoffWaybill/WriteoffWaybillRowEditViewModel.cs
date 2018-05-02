using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ERP.Utils.Mvc.Validators;

namespace ERP.Wholesale.UI.ViewModels.WriteoffWaybill
{
    public class WriteoffWaybillRowEditViewModel
    {
        public Guid Id { get; set; }
        public Guid WriteoffWaybillId { get; set; }

        [DisplayName("Наименование товара")]
        public string ArticleName { get; set; }

        [Required(ErrorMessage = "Укажите товар")]
        [Range(1, int.MaxValue, ErrorMessage = "Укажите товар")]
        public int ArticleId { get; set; }

        /// <summary>
        /// Название единицы измерения товара
        /// </summary>
        public string MeasureUnitName { get; set; }

        /// <summary>
        /// Количество допускаемых знаков после запятой в количестве товара (требуется только при начальном появлении формы)
        /// </summary>
        public string MeasureUnitScale { get; set; }

        [DisplayName("Партия")]
        public string BatchName { get; set; }

        [DisplayName("Закупочная цена")]
        public string PurchaseCost { get; set; }

        [DisplayName("Учетная цена")]
        public string SenderAccountingPrice { get; set; }

        [DisplayName("Текущая наценка")]
        public string MarkupPercent { get; set; }
        public string MarkupSum { get; set; }

        [DisplayName("В наличии на складе")]
        public string AvailableToReserveFromStorageCount { get; set; }

        [DisplayName("Ожидается поступление")]
        public string AvailableToReserveFromPendingCount { get; set; }

        [DisplayName("Доступно к списанию")]
        public string AvailableToReserveCount { get; set; }

        [DisplayName("Списываемое кол-во")]
        [Required(ErrorMessage = "Укажите кол-во")]
        [GreaterByConst(0, ErrorMessage = "Кол-во должно быть больше 0")]
        public string WritingoffCount { get; set; }

        public string Title { get; set; }
        public string SenderStorageId { get; set; }
        public string SenderId { get; set; }
        
        // Дата накладной списания
        public string WriteoffWaybillDate { get; set; }

        // выбранная партия товара
        public Guid ReceiptWaybillRowId { get; set; }
        // сохраненная партия 
        public Guid CurrentReceiptWaybillRowId { get; set; }

        public bool AllowToEdit { get; set; }

        public bool AllowToViewPurchaseCost { get; set; }

        public string ManualSourcesInfo { get; set; }        
    }
}