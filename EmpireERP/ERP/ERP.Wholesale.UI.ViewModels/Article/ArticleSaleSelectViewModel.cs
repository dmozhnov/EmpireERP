using System.ComponentModel;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.Article
{
    public class ArticleSaleSelectViewModel
    {
        public string ArticleId { get; set; }

        public string ArticleNumber { get; set; }

        public string ArticleName { get; set; }

        [DisplayName("Текущая учетная цена")]
        public string AccountingPrice { get; set; }

        [DisplayName("Всего доступно к возврату")]
        public string AvailableToReturnTotalCount { get; set; }

        public GridData SaleGrid { get; set; }
    }
}