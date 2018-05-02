using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.ArticleCertificate
{
    public class ArticleCertificateListViewModel
    {
        /// <summary>
        /// Фильтр
        /// </summary>
        public FilterData Filter { get; set; }

        /// <summary>
        /// Грид сертификатов товаров
        /// </summary>
        public GridData Data { get; set; }
    }
}