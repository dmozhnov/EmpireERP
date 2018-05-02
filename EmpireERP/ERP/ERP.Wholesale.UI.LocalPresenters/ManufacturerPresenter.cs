using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
using ERP.Wholesale.UI.ViewModels.BaseDictionary;
using ERP.Wholesale.UI.ViewModels.Manufacturer;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ManufacturerPresenter : BaseDictionaryPresenter<Manufacturer>, IManufacturerPresenter
    {
        #region Свойства

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IManufacturerService manufacturerService;
        private readonly IProducerService producerService;

        #endregion

        #region Конструкторы

        public ManufacturerPresenter(IUnitOfWorkFactory unitOfWorkFactory, IManufacturerService manufacturerService, IProducerService producerService, IUserService userService)
                : base(manufacturerService, userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.manufacturerService = manufacturerService;
            this.producerService = producerService;
        }

        #endregion

        #region Методы

        #region Создание/Редактирование

        public ManufacturerEditViewModel Create(int? producerId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Manufacturer_Create);

                var model = new ManufacturerEditViewModel()
                {
                    Title = "Добавление фабрики-изготовителя",
                    AllowToEdit = true,
                    ProducerId = producerId.HasValue ? producerId.Value.ToString() : "0"
                };

                return model;
            }
        }

        public ManufacturerEditViewModel Edit(short id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var manufacturer = manufacturerService.CheckExistence(id);
                var allowToEdit = user.HasPermission(Permission.Manufacturer_Edit);

                var model = new ManufacturerEditViewModel()
                {
                    ProducerId = "0",
                    Id = manufacturer.Id,
                    Name = manufacturer.Name,
                    AllowToEdit = allowToEdit,
                    Title = allowToEdit ? "Редактирование фабрики-изготовителя" : "Детали фабрики-изготовителя"
                };

                return model;
            }
        }

        public object Save(ManufacturerEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                int? producerId = !String.IsNullOrEmpty(model.ProducerId) ? ValidationUtils.TryGetInt(model.ProducerId) : (int?)null;

                Manufacturer manufacturer;

                if (model.Id == 0)
                {
                    user.CheckPermission(Permission.Manufacturer_Create);

                    manufacturerService.CheckNameUniqueness(0, model.Name);

                    manufacturer = new Manufacturer(model.Name);
                }
                else
                {
                    user.CheckPermission(Permission.Manufacturer_Edit);

                    manufacturerService.CheckNameUniqueness(model.Id, model.Name);

                    manufacturer = manufacturerService.CheckExistence(model.Id, user);

                    manufacturer.Name = model.Name;
                }

                // Если существует организация производителя с таким именем, связываем ее с созданным / отредактированным изготовителем (очевидно, она еще ни с кем
                // не связана, т.к. иначе был бы конфликт имен при создании / редактировании изготовителя)
                var producerOrganization = producerService.GetProducerOrganizationByName(model.Name);
                if (producerOrganization != null)
                {
                    producerOrganization.SetManufacturer(manufacturer);
                    producerService.Save(producerOrganization.Producer);
                }

                manufacturerService.Save(manufacturer);

                // Если требуется добавить изготовителя производителю, добавляем
                if (producerId.HasValue && producerId.Value != 0)
                {
                    var producer = producerService.CheckProducerExistence(producerId.Value);
                    if (!producer.Manufacturers.Contains(manufacturer))
                    {
                        producerService.AddManufacturer(producer, manufacturer, user);
                    }
                }

                uow.Commit();

                return new
                {
                    Id = manufacturer.Id,
                    Name = manufacturer.Name
                };
            }
        }

        public void Delete(short id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                base.DeleteBaseDictionary(id, currentUser);

                uow.Commit();
            }
        }

        #endregion

        #region Список

        public BaseDictionaryListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = base.ListBaseDictionary(currentUser);

                return model;
            }
        }

        public GridData GetManufacturerGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = base.GetBaseDictionaryGrid(state, currentUser);

                return model;
            }
        }

        #endregion

        #region Выбор фабрики-изготовителя

        /// <summary>
        /// Выбор фабрики-производителя для добавления в производителя
        /// </summary>
        /// <param name="producerId">Идентификатор производителя</param>
        /// <returns></returns>
        public ManufacturerSelectorViewModel SelectManufacturer(int producerId, UserInfo currentUser, string mode = "exclude")
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var producer = producerService.CheckProducerExistence(producerId);

                var model = new ManufacturerSelectorViewModel();

                model.Title = mode == "exclude" ? "Добавление фабрики-изготовителя производителю" : "Выбор фабрики-изготовителя";
                model.ProducerId = producerId.ToString();

                model.Filter = new ERP.UI.ViewModels.GridFilter.FilterData();
                model.Filter.Items.Add(new FilterTextEditor("Name", "Название"));

                model.ManufacturerGrid = GetManufacturerGridLocal(new GridState()
                {
                    Parameters = "Id=" + GetListOfExcludeId(producer.Manufacturers) + ";Mode=" + mode,
                    PageSize = 5,
                });

                model.AllowToCreate = user.HasPermission(Permission.Manufacturer_Create);

                return model;
            }
        }

        /// <summary>
        /// Выбор фабрики-производителя для добавления в производителя
        /// </summary>
        /// <returns></returns>
        public ManufacturerSelectorViewModel SelectManufacturer(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new ManufacturerSelectorViewModel();

                model.Title = "Выбор фабрики-изготовителя";

                model.Filter = new ERP.UI.ViewModels.GridFilter.FilterData();
                model.Filter.Items.Add(new FilterTextEditor("Name", "Название"));

                model.ManufacturerGrid = GetManufacturerGridLocal(new GridState());

                model.AllowToCreate = user.HasPermission(Permission.Manufacturer_Create);

                return model;
            }
        }

        /// <summary>
        /// Формирование строки, содержащая Id фабрик производителей
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string GetListOfExcludeId(IEnumerable<Manufacturer> list)
        {
            string result = "";
            bool first = true;

            foreach (var val in list)
            {
                if (!first)
                {
                    result += "," + val.Id.ToString();
                }
                else
                {
                    result = val.Id.ToString();
                    first = false;
                }
            }

            if (result == "") result = "0";

            return result;
        }

        public GridData GetManufacturerGrid(GridState state)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                return GetManufacturerGridLocal(state);
            }
        }

        private GridData GetManufacturerGridLocal(GridState state)
        {
            if (state == null)
            {
                state = new GridState();
            }

            var deriveParameters = new ParameterString(state.Parameters);
            ParameterString ps = new ParameterString(state.Filter);

            // Есть 2 варианта: 1) state.Parameters содержат Id, это от одного до N идентификаторов изготовителей (один элемент "0", если их нет);
            // тогда он обязательно содержит и Mode ("only", если берем только эти Id, и "exclude", если берем все Id, кроме них);
            // 2) state.Parameters не содержит ни Id, ни Mode
            if (deriveParameters.Keys.Contains("Id"))
            {
                string mode = deriveParameters["Mode"].Value as string;
                ValidationUtils.Assert(mode == "exclude" || mode == "only", "Неверное значение входного параметра.");

                // Различаем одно число и несколько чисел через запятую
                if (deriveParameters["Id"].Value is List<string>)
                {
                    ps.Add("Id", mode == "only" ? ParameterStringItem.OperationType.OneOf : ParameterStringItem.OperationType.NotOneOf,
                        deriveParameters["Id"].Value as List<string>);
                }
                else
                {
                    ps.Add("Id", mode == "only" ? ParameterStringItem.OperationType.Eq : ParameterStringItem.OperationType.NotEq,
                        deriveParameters["Id"].Value as string);
                }
            }

            var model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("ManufacturerName", "Название", Unit.Percentage(100));
            model.AddColumn("IsProducer", "Является производителем", Unit.Pixel(100), GridCellStyle.Label, GridColumnAlign.Center);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            var list = manufacturerService.GetFilteredList(state, ps);
            var action = new GridActionCell("Action");
            action.AddAction("Выбрать", "select");

            foreach (var val in list)
            {
                model.AddRow(new GridRow(
                    action,
                    new GridLabelCell("ManufacturerName") { Value = val.Name },
                    new GridLabelCell("IsProducer") { Value = manufacturerService.IsProducer(val) ? "Да" : "Нет" },
                    new GridHiddenCell("Id") { Value = val.Id.ToString() }));
            }

            model.State = state;

            return model;
        }

        #endregion

        #endregion
    }
}