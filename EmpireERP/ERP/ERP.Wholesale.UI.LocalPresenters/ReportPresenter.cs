using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.Utils;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Report;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ReportPresenter : IReportPresenter
    {
        #region Внутренний класс "Отчет"

        private class Report
        {
            public string Id { get; set; }
            public string Name { get; set; }

            public Report(string id, string name)
            {
                Id = id;
                Name = name;
            }
        }

        #endregion

        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;  
        private readonly IUserService userService;

        #endregion

        #region Конструкторы

        public ReportPresenter(IUnitOfWorkFactory unitOfWorkFactory, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.userService = userService;           
        }


        #endregion

        #region Список

        #region Товары и цены
        
        /// <summary>
        /// Список отчетов по товарам и ценам.
        /// </summary>
        /// <param name="currentUser">Информация о текущем пользователе системы.</param>        
        public ArticlesAndPricesListViewModel ArticlesAndPricesList(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new ArticlesAndPricesListViewModel()
                {
                    ReportsGrid = GetArticlesAndPricesReportGridLocal(new GridState(), user),
                    ReportListName = "Отчеты - Товары и цены"
                };

                return model;
            }
        }

        /// <summary>
        /// Грид отчетов по товарам и ценам.
        /// </summary>
        public GridData GetArticlesAndPricesReportGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetArticlesAndPricesReportGridLocal(state, user);
            }
        }

        private GridData GetArticlesAndPricesReportGridLocal(GridState state, User user)
        {
            var model = new GridData();
            model.AddColumn("ReportId", "Код отчета", Unit.Pixel(63), align: GridColumnAlign.Center);
            model.AddColumn("Name", "Название", Unit.Percentage(100));

            // товары и цены
            var articlesAndPricesReports = new List<Report>();
            if (user.HasPermission(Permission.Report0001_View))
            {
                articlesAndPricesReports.Add(new Report("Report0001", "Наличие товаров на местах хранения"));
            }

            if (user.HasPermission(Permission.Report0004_View))
            {
                articlesAndPricesReports.Add(new Report("Report0004", "Движение товара за период"));
            }

            if (user.HasPermission(Permission.Report0005_View))
            {
                articlesAndPricesReports.Add(new Report("Report0005", "Карта движения товара"));
            }

            if (user.HasPermission(Permission.Report0008_View))
            {
                articlesAndPricesReports.Add(new Report("Report0008", "Реестр накладных"));
            }

            if (user.HasPermission(Permission.Report0009_View))
            {
                articlesAndPricesReports.Add(new Report("Report0009", "Поставки"));
            }

            foreach (var item in GridUtils.GetEntityRange(articlesAndPricesReports, state))
            {
                model.AddRow(new GridRow(
                    new GridLabelCell("ReportId") { Value = item.Id },
                    new GridLinkCell("Name") { Value = item.Name }
                ));
            }
            model.State = state;
            model.GridPartialViewAction = "/Report/ShowArticlesAndPricesList/";

            return model;
        } 
        #endregion

        #region Продажи
        
        /// <summary>
        /// Список отчетов по продажам.
        /// </summary>  
        public ArticlesAndPricesListViewModel ArticleSaleList(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new ArticlesAndPricesListViewModel()
                {
                    ReportsGrid = GetArticleSaleReportGridLocal(new GridState(), user),
                    ReportListName = "Отчеты - Продажи"
                };

                return model;
            }
        }

        /// <summary>
        /// Грид отчетов по продажам.
        /// </summary> 
        public GridData GetArticleSaleReportGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetArticleSaleReportGridLocal(state, user);
            }
        }

        private GridData GetArticleSaleReportGridLocal(GridState state, User user)
        {
            var model = new GridData();
            model.AddColumn("ReportId", "Код отчета", Unit.Pixel(63), align: GridColumnAlign.Center);
            model.AddColumn("Name", "Название", Unit.Percentage(100));

            var articleSaleReports = new List<Report>();
            if (user.HasPermission(Permission.Report0002_View))
            {
                articleSaleReports.Add(new Report("Report0002", "Реализация товаров"));
            }
            if (user.HasPermission(Permission.Report0003_View))
            {
                articleSaleReports.Add(new Report("Report0003", "Финансовый отчет"));
            }
            if (user.HasPermission(Permission.Report0006_View))
            {
                articleSaleReports.Add(new Report("Report0006", "Отчет по взаиморасчетам"));
            }
            if (user.HasPermission(Permission.Report0007_View))
            {
                articleSaleReports.Add(new Report("Report0007", "Отчет по взаиморасчетам по реализациям"));
            }
            if (user.HasPermission(Permission.Report0010_View))
            {
                articleSaleReports.Add(new Report("Report0010", "Принятые платежи"));
            }

            foreach (var item in GridUtils.GetEntityRange(articleSaleReports, state))
            {
                model.AddRow(new GridRow(
                    new GridLabelCell("ReportId") { Value = item.Id },
                    new GridLinkCell("Name") { Value = item.Name }
                ));
            }
            model.State = state;
            model.GridPartialViewAction = "/Report/ShowArticleSaleList/";

            return model;
        } 
        #endregion

        #endregion
    }
}