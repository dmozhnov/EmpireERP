using System.ComponentModel;
using ERP.UI.ViewModels.Grid;

namespace ERP.Wholesale.UI.ViewModels.Article
{
    public class ArticleBatchSelectViewModel
    {
        public string ArticleId { get; set; }

        public string ArticleNumber { get; set; }

        public string ArticleName { get; set; }

        [DisplayName("Учетная цена отправителя")]
        public string SenderAccountingPrice { get; set; }
        public string SenderAccountingPriceValue { get; set; }

        [DisplayName("Учетная цена получателя")]
        public string RecipientAccountingPrice { get; set; }
        public string RecipientAccountingPriceValue { get; set; }

        [DisplayName("Наценка перемещения")]
        public string MovementMarkupSum { get; set; }
        public string MovementMarkupPercent { get; set; }

        [DisplayName("Доступно по данным партиям")]
        public string AvailableToMoveTotalCount { get; set; }

        public GridData BatchGrid { get; set; }
    }
}