using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ArticleCertificate;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ArticleCertificatePresenter : IArticleCertificatePresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IArticleCertificateService articleCertificateService;
        private readonly IUserService userService;
        
        #endregion

        #region Конструктор

        public ArticleCertificatePresenter(IUnitOfWorkFactory unitOfWorkFactory, IArticleCertificateService articleCertificateService, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            
            this.articleCertificateService = articleCertificateService;
            this.userService = userService;            
        } 

        #endregion

        #region Методы

        #region Список
        
        public ArticleCertificateListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                
                var model = new ArticleCertificateListViewModel()
                {
                    Filter = new FilterData()
                    {
                        Items = new List<FilterItem>()
                        {
                            new FilterDateRangePicker("StartDate", "Дата начала"),
                            new FilterTextEditor("Name", "Название"),
                            new FilterDateRangePicker("EndDate", "Дата окончания")
                        }
                    },
                    Data = GetArticleCertificateGridLocal(new GridState() { Sort = "Id=Desc", PageSize = 25 }, user)
                };

                return model;
            }
        }

        public GridData GetArticleCertificateGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetArticleCertificateGridLocal(state, user);
            }
        }
        
        private GridData GetArticleCertificateGridLocal(GridState state, User user)
        {
            if (state == null)
            {
                state = new GridState() { Sort = "Id=Desc" };
            }

            bool allowToDelete = user.HasPermission(Permission.ArticleCertificate_Delete);
            bool allowToEdit = user.HasPermission(Permission.ArticleCertificate_Edit);

            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("Name", "Полное наименование", Unit.Percentage(100));
            model.AddColumn("StartDate", "Дата начала", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("EndDate", "Дата завершения", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), style: GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.ArticleCertificate_Create);

            var rows = articleCertificateService.GetFilteredList(state);
            model.State = state;

            var actions = new GridActionCell("Action");
            if (allowToEdit)
            {
                actions.AddAction("Ред.", "edit_link");
            }
            else
            {
                actions.AddAction("Дет.", "edit_link");
            }

            if (allowToDelete)
            {
                actions.AddAction("Удал.", "delete_link");
            }

            foreach (var item in rows)
            {
                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Name") { Value = item.Name },
                    new GridLabelCell("StartDate") { Value = item.StartDate.ToShortDateString() },
                    new GridLabelCell("EndDate") { Value = item.EndDate.ForDisplay() },
                    new GridHiddenCell("Id") { Value = item.Id.ToString() }
                ));
            }

            return model;
        } 
        #endregion

        #region Создание / редактирование / удаление
        
        public ArticleCertificateEditViewModel Create(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ArticleCertificate_Create);

                var model = new ArticleCertificateEditViewModel()
                {
                    AllowToEdit = true,
                    Title = "Добавление сертификата товара"
                };

                return model;
            }
        }

        public ArticleCertificateEditViewModel Edit(int id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var articleCertificate = articleCertificateService.CheckArticleCertificateExistence(id);
                var user = userService.CheckUserExistence(currentUser.Id);

                bool allowToEdit = user.HasPermission(Permission.ArticleCertificate_Edit);

                var model = new ArticleCertificateEditViewModel()
                {
                    AllowToEdit = allowToEdit,

                    Name = articleCertificate.Name,
                    StartDate = articleCertificate.StartDate.ToShortDateString(),
                    EndDate = allowToEdit ? articleCertificate.EndDate.ForEdit() : articleCertificate.EndDate.ForDisplay(),
                    Id = articleCertificate.Id,
                    Title = (allowToEdit ? "Редактирование сертификата товара" : "Детали сертификата товара")
                };

                return model;
            }
        }

        public object Save(ArticleCertificateEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                ArticleCertificate articleCertificate;

                var user = userService.CheckUserExistence(currentUser.Id);
                DateTime startDate;
                DateTime dateForParse;
                DateTime? endDate = null;

                if (!DateTime.TryParse(model.StartDate, out startDate))
                {
                    throw new Exception("Введите начальную дату в правильном формате или выберите из списка.");
                }

                if (!String.IsNullOrEmpty(model.EndDate))
                {
                    if (!DateTime.TryParse(model.EndDate, out dateForParse))
                    {
                        throw new Exception("Введите конечную дату в правильном формате или выберите из списка.");
                    }
                    endDate = dateForParse;
                }

                if (model.Id == 0)
                {
                    user.CheckPermission(Permission.ArticleCertificate_Create);

                    articleCertificate = new ArticleCertificate(model.Name, startDate, endDate);
                }
                else
                {
                    user.CheckPermission(Permission.ArticleCertificate_Edit);

                    articleCertificate = articleCertificateService.CheckArticleCertificateExistence(model.Id);

                    articleCertificate.Name = model.Name;
                    articleCertificate.SetDates(startDate, endDate);
                }

                articleCertificateService.Save(articleCertificate);

                uow.Commit();

                return new
                {
                    Name = articleCertificate.Name,
                    Id = articleCertificate.Id
                };
            }
        }

        public void Delete(int id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var articleCertificate = articleCertificateService.CheckArticleCertificateExistence(id);
                var user = userService.CheckUserExistence(currentUser.Id);

                articleCertificateService.Delete(articleCertificate, user);

                uow.Commit();
            }
        } 
        #endregion

        #region Выбор сертификата товара

        public ArticleCertificateSelectViewModel SelectArticleCertificate(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new ArticleCertificateSelectViewModel()
                {
                    Title = "Выбор сертификата товара",
                    AllowToCreateArticleCertificate = user.HasPermission(Permission.ArticleCertificate_Create),
                    Filter = new FilterData()
                    {
                        Items = new List<FilterItem>()
                    {
                        new FilterDateRangePicker("StartDate", "Дата начала"),
                        new FilterTextEditor("Name", "Название"),
                        new FilterDateRangePicker("EndDate", "Дата окончания")
                    }
                    },
                    Grid = GetArticleCertificateSelectGridLocal(new GridState() { Sort = "Id=Desc", PageSize = 5 })
                };

                return model;
            }
        }

        public GridData GetArticleCertificateSelectGrid(GridState state)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                return GetArticleCertificateSelectGridLocal(state);
            }
        }

        private GridData GetArticleCertificateSelectGridLocal(GridState state)
        {
            if (state == null)
            {
                state = new GridState() { Sort = "Id=Desc" };
            }

            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(55));
            model.AddColumn("Name", "Полное наименование", Unit.Percentage(100));
            model.AddColumn("StartDate", "Дата начала", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("EndDate", "Дата завершения", Unit.Pixel(60), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), style: GridCellStyle.Hidden);

            var rows = articleCertificateService.GetFilteredList(state);
            model.State = state;

            var actions = new GridActionCell("Action");
            actions.AddAction("Выбрать", "articleCertificate_select_link");

            foreach (var item in rows)
            {
                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Name") { Value = item.Name },
                    new GridLabelCell("StartDate") { Value = item.StartDate.ToShortDateString() },
                    new GridLabelCell("EndDate") { Value = item.EndDate.ForDisplay() },
                    new GridHiddenCell("Id") { Value = item.Id.ToString() }
                ));
            }

            return model;
        }

        #endregion

        #endregion
    }
}
