using System;
using System.Collections.Generic;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0001
{
    public class Report0001_2ItemViewModel
    {
        public short ArticleGroupId { get; set; }
        public string ArticleGroupName { get; set; }

        public int ArticleId { get; set; }
        public Guid ArticleBatchId { get; set; }
        public string ArticleBatchName { get; set; }
        public string ArticleNumber { get; set; }
        public string ArticleName { get; set; }

        public decimal PurchaseCost { get; set; }

        public List<Report0001_2SubitemViewModel> Subitems { get; set; }

        public Report0001_2ItemViewModel()
        {
            Subitems = new List<Report0001_2SubitemViewModel>();
        }
    }
}
