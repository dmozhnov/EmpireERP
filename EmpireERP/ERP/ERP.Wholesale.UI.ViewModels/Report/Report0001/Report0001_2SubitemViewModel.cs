
namespace ERP.Wholesale.UI.ViewModels.Report.Report0001
{
    public class Report0001_2SubitemViewModel
    {
        public short StorageId { get; set; }
        public string StorageName { get; set; }
        public byte StorageType { get; set; }

        public decimal AvailableInStorageCount { get; set; }
        public decimal PendingCount { get; set; }
        public decimal ReservedCount { get; set; }
        public decimal AvailableToReserveCount { get; set; }
        
        public decimal? AccountingPrice { get; set; }

        /// <summary>
        /// Сумма в акцептованных учетных ценах
        /// </summary>
        public decimal? AvailableInStorageAccountingPriceSum { get; set; }
        public decimal? PendingAccountingPriceSum { get; set; }
        public decimal? ReservedAccountingPriceSum { get; set; }
        public decimal? AvailableToReserveAccountingPriceSum { get; set; } 

    }
}
