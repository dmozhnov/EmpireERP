using System.ComponentModel;
using ERP.Wholesale.UI.ViewModels.BaseWaybill;

namespace ERP.Wholesale.UI.ViewModels.ReceiptWaybill
{
    public class ReceiptWaybillMainDetailsViewModel : BaseWaybillMainDetailsViewModel
    {
        /// <summary>
        /// Номер накладной
        /// </summary>
        [DisplayName("Номер")]
        public string Number { get; set; }

        /// <summary>
        /// Дата накладной
        /// </summary>
        [DisplayName("Дата")]
        public string Date { get; set; }

        [DisplayName("Место хранения")]
        public string StorageName { get; set; }
        public string StorageId { get; set; }
        public bool AllowToViewStorageDetails { get; set; }

        [DisplayName("Организация")]
        public string AccountOrganizationName { get; set; }
        public string AccountOrganizationId { get; set; }

        [DisplayName("Поставщик")]
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }
        public bool AllowToViewProviderDetails { get; set; }

        [DisplayName("Договор")]
        public string ContractInfo { get; set; }

        [DisplayName("Производитель")]
        public string ProducerName { get; set; }
        public string ProducerId { get; set; }
        public bool AllowToViewProducerDetails { get; set; }

        [DisplayName("Заказ")]
        public string ProductionOrderName { get; set; }
        public string ProductionOrderId { get; set; }
        public bool AllowToViewProductionOrderDetails { get; set; }

        [DisplayName("№ накл. постав.")]
        public string ProviderNumber { get; set; }

        [DisplayName("№ С-Ф. постав.")]
        public string ProviderInvoiceNumber { get; set; }

        [DisplayName("Сумма по позициям")]
        public string Sum { get; set; }

        [DisplayName("Ставка и сумма НДС")]
        public string ValueAddedTaxString { get; set; }

        [DisplayName("Кол-во позиций и отгрузка")]
        public string RowCount { get; set; }

        [DisplayName("Кол-во позиций (ожид. | прием.) и отгрузка")]
        public string PendingRowCount { get; set; }
        public string ReceiptedRowCount { get; set; }

        [DisplayName("Ставка НДС")]
        public string ShippingPercent { get; set; }

        [DisplayName("Сумма по документу")]
        public string PendingSum { get; set; }

        [DisplayName("Сумма по строкам (ожидание | приемка)")]
        public string ReceiptedSum { get; set; }

        [DisplayName("Сумма от поставщика (ожидание | приемка)")]
        public string ApprovedSum { get; set; }

        [DisplayName("Скидка поставщика")]
        public string DiscountPercent { get; set; }
        public string DiscountSum { get; set; }

        [DisplayName("Дата накл. постав.")]
        public string ProviderDate { get; set; }

        [DisplayName("Дата С-Ф постав.")]
        public string ProviderInvoiceDate { get; set; }

        /// <summary>
        /// Пользователь, осуществивший приемку
        /// </summary>
        [DisplayName("Приемка")]
        public string ReceiptedByName { get; set; }
        public string ReceiptedById { get; set; }
        public bool AllowToViewReceiptedByDetails { get; set; }
        public string ReceiptDate { get; set; }

        /// <summary>
        /// Пользователь, осуществивший согласование
        /// </summary>
        [DisplayName("Согласование")]
        public string ApprovedByName { get; set; }
        public string ApprovedById { get; set; }
        public bool AllowToViewApprovedByDetails { get; set; }
        public string ApprovementDate { get; set; }

        /// <summary>
        /// Признак того, что накладная создана (создается) из партии заказа
        /// </summary>
        public bool IsCreatedFromProductionOrderBatch { get; set; }

        /// <summary>
        /// есть расхождения по сумме
        /// </summary>
        public bool AreSumDivergences { get; set; }

        /// <summary>
        /// принято с расхождениями
        /// </summary>
        public bool ReceiptedWithDivergences { get; set; }

        /// <summary>
        /// принято без или после расхождений
        /// </summary>
        public bool ApprovedAfterDivergences { get; set; }

        /// <summary>
        /// принято после расхождений
        /// </summary>
        public bool ApprovedFinallyAfterDivergences { get; set; }

        public bool AllowToDelete { get; set; }

        public bool AllowToEdit { get; set; }
        public bool AllowToEditProviderDocuments { get; set; }

        public bool AllowToAccept { get; set; }
        public bool AllowToAcceptRetroactively { get; set; }
        public bool AllowToCancelAcceptance { get; set; }

        public bool AllowToReceipt { get; set; }

        public bool AllowToPrintForms { get; set; }

        public bool AllowToPrintDivergenceAct { get; set; }
    }
}