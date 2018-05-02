using System;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0001
{
    public class Report0001_1ItemViewModel
    {
        public short StorageId { get; set; }
        public string StorageName { get; set; }
        public byte StorageType { get; set; }

        public int AccountOrganizationId { get; set; }
        public string AccountOrganizationName { get; set; }

        public short ArticleGroupId { get; set; }
        public string ArticleGroupName { get; set; }

        public int ArticleId { get; set; }
        public Guid ArticleBatchId { get; set; }
        public string ArticleBatchName { get; set; }
        public string ArticleNumber { get; set; }
        public string ArticleName { get; set; }

        public decimal PendingCount { get; set; }
        public decimal AvailableInStorageCount { get; set; }
        public decimal ReservedCount { get; set; }
        public decimal AvailableToReserveCount { get; set; }

        public decimal PurchaseCost { get; set; }
        public decimal? AccountingPrice { get; set; }
    }
}
