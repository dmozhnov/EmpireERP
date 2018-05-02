using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.OutgoingWaybillRow
{
    public class OutgoingWaybillRowViewModel
    {
        public FilterData FilterData { get; set; }

        public GridData IncomingWaybillRowGrid { get; set; }
        public string Title { get; set; }

        public string ArticleId { get; set; }
        public string ArticleName { get; set; }
        public string BatchName { get; set; }

        public string SenderStorageId { get; set; }
        public string RecipientStorageId { get; set; }
        public string SenderId { get; set; }

        public string SelectedSources { get; set; }
        public string SelectedBatchId { get; set; }

        public bool AllowToSave { get; set; }

    }
}