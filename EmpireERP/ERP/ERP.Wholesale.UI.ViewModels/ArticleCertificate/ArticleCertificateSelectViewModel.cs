using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;

namespace ERP.Wholesale.UI.ViewModels.ArticleCertificate
{
    public class ArticleCertificateSelectViewModel
    {
        /// <summary>
        /// Заголовок
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Фильтр
        /// </summary>
        public FilterData Filter { get; set; }

        /// <summary>
        /// Грид сертификатов товаров
        /// </summary>
        public GridData Grid { get; set; }

        /// <summary>
        /// Флаг разрешения создавать сертификаты товаров
        /// </summary>
        public bool AllowToCreateArticleCertificate { get; set; }
    }
}
