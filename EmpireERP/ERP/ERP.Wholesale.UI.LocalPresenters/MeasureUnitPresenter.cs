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
using ERP.Wholesale.UI.ViewModels.MeasureUnit;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class MeasureUnitPresenter : IMeasureUnitPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IMeasureUnitService measureUnitService;
        private readonly IUserService userService;
        
        #endregion

        #region Конструктор

        public MeasureUnitPresenter(IUnitOfWorkFactory unitOfWorkFactory, IMeasureUnitService measureUnitService, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            
            this.measureUnitService = measureUnitService;
            this.userService = userService;
        }

        #endregion

        #region Методы

        #region Список

        public MeasureUnitListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                
                var model = new MeasureUnitListViewModel()
                {
                    Data = GetMeasureUnitGridLocal(new GridState() { Sort = "FullName=Asc" }, user)
                };

                return model;
            }
        }

        public GridData GetMeasureUnitGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                
                return GetMeasureUnitGridLocal(state, user);
            }
        }

        private GridData GetMeasureUnitGridLocal(GridState state, User user)
        {
            if (state == null)
                state = new GridState() { Sort = "FullName=Asc" };

            bool allowToDelete = user.HasPermission(Permission.MeasureUnit_Delete);
            bool allowToEdit = user.HasPermission(Permission.MeasureUnit_Edit);

            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("FullName", "Полное наименование", Unit.Percentage(100));
            model.AddColumn("ShortName", "Краткое наименование", Unit.Pixel(133));
            model.AddColumn("Scale", "Знаков после запятой", Unit.Pixel(125), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), style: GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.MeasureUnit_Create);

            var rows = measureUnitService.GetFilteredList(state);
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
                    new GridLabelCell("FullName") { Value = item.FullName, Key = "FullName" },
                    new GridLabelCell("ShortName") { Value = item.ShortName.ToString() },
                    new GridLabelCell("Scale") { Value = item.Scale.ToString() },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "Id" }
                ));
            }

            return model;
        }

        #endregion

        #region Создание / редактирование / удаление
        
        public MeasureUnitEditViewModel Create(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.MeasureUnit_Create);

                var model = new MeasureUnitEditViewModel()
                {
                    AllowToEdit = true,
                    Title = "Добавление единицы измерения"
                };

                return model;
            }
        }

        public MeasureUnitEditViewModel Edit(short id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var measureUnit = measureUnitService.CheckMeasureUnitExistence(id);
                
                bool allowToEdit = user.HasPermission(Permission.MeasureUnit_Edit);

                var model = new MeasureUnitEditViewModel()
                {
                    AllowToEdit = allowToEdit,
                    Comment = measureUnit.Comment,
                    FullName = measureUnit.FullName,
                    Scale = measureUnit.Scale,
                    ShortName = measureUnit.ShortName,
                    Id = measureUnit.Id,
                    NumericCode = measureUnit.NumericCode,
                    Title = (allowToEdit ? "Редактирование единицы измерения" : "Детали единицы измерения")
                };

                return model;
            }
        }

        public object Save(MeasureUnitEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                
                MeasureUnit measureUnit;

                if (model.Id == 0)
                {
                    user.CheckPermission(Permission.MeasureUnit_Create);

                    measureUnit = new MeasureUnit(model.ShortName, model.FullName, model.NumericCode, model.Scale);
                }
                else
                {
                    user.CheckPermission(Permission.MeasureUnit_Edit);

                    measureUnit = measureUnitService.CheckMeasureUnitExistence(model.Id);

                    measureUnit.ShortName = model.ShortName;
                    measureUnit.FullName = model.FullName;
                    measureUnit.Scale = model.Scale;
                    measureUnit.NumericCode = model.NumericCode;
                }
                measureUnit.Comment = StringUtils.ToHtml(model.Comment);

                measureUnitService.Save(measureUnit);

                uow.Commit();

                return new
                {
                    Name = measureUnit.FullName,
                    ShortName = measureUnit.ShortName,
                    Scale = measureUnit.Scale,
                    Id = measureUnit.Id
                };
            }
        }

        public void Delete(short id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var measureUnit = measureUnitService.CheckMeasureUnitExistence(id);
                var user = userService.CheckUserExistence(currentUser.Id);

                measureUnitService.Delete(measureUnit, user);

                uow.Commit();
            }
        }
 
        #endregion

        #region Выбор
        
        public MeasureUnitSelectViewModel SelectMeasureUnit(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new MeasureUnitSelectViewModel()
                {
                    Title = "Выбор единицы измерения",
                    AllowToCreateMeasureUnit = user.HasPermission(Permission.MeasureUnit_Create),
                    Filter = new FilterData()
                    {
                        Items = new List<FilterItem>()
                    {
                        new FilterTextEditor("FullName", "Единица измерения")
                    }
                    },
                    Grid = GetMeasureUnitSelectGridLocal(new GridState() { PageSize = 5 })
                };

                return model;
            }
        }

        public GridData GetMeasureUnitSelectGrid(GridState state)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                return GetMeasureUnitSelectGridLocal(state);
            }
        }

        private GridData GetMeasureUnitSelectGridLocal(GridState state)
        {
            if (state == null) state = new GridState() { Sort = "FullName=Asc" };

            GridData model = new GridData();

            model.AddColumn("Action", "Действие", Unit.Pixel(55));
            model.AddColumn("FullName", "Полное наименование", Unit.Percentage(100));
            model.AddColumn("ShortName", "Краткое наименование", Unit.Pixel(133));
            model.AddColumn("Scale", "Знаков после запятой", Unit.Pixel(125), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), style: GridCellStyle.Hidden);

            var rows = measureUnitService.GetFilteredList(state);
            model.State = state;

            var actions = new GridActionCell("Action");
            actions.AddAction("Выбрать", "measureUnit_select_link");

            foreach (var item in rows)
            {
                model.AddRow(new GridRow(
                    actions,
                    new GridPseudoLinkCell("FullName") { Value = item.FullName, Key = "FullName" },
                    new GridLabelCell("ShortName") { Value = item.ShortName.ToString() },
                    new GridLabelCell("Scale") { Value = item.Scale.ToString() },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "Id" }
                ));
            }

            return model;
        } 

        #endregion

        #endregion
    }
}
