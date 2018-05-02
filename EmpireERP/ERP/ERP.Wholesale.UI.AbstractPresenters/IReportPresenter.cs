using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.Report;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IReportPresenter
    {
        /// <summary>
        /// Список отчетов по товарам и ценам.
        /// </summary>        
        ArticlesAndPricesListViewModel ArticlesAndPricesList(UserInfo currentUser);

        /// <summary>
        /// Грид отчетов по товарам и ценам.
        /// </summary>
        GridData GetArticlesAndPricesReportGrid(GridState state, UserInfo currentUser);

        /// <summary>
        /// Список отчетов по продажам.
        /// </summary>        
        ArticlesAndPricesListViewModel ArticleSaleList(UserInfo currentUser);

        /// <summary>
        /// Грид отчетов по продажам.
        /// </summary>        
        GridData GetArticleSaleReportGrid(GridState state, UserInfo currentUser);
    }
}
