using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Contractor;
using ERP.Utils.Mvc;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ContractorPresenter : IContractorPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IUserService userService;
        private readonly IContractorService contractorService;

        #endregion

        #region Конструктор

        public ContractorPresenter(IUnitOfWorkFactory unitOfWorkFactory, IUserService userService,IContractorService contractorService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.userService = userService;
            this.contractorService = contractorService;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Получение модели для окна выбора контрагента
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ContractorSelectViewModel SelectContractor(UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var model = new ContractorSelectViewModel()
                {
                    Filter = new FilterData()
                    {
                        Items = new List<FilterItem>()
                        {
                            new FilterTextEditor("Name", "Название"),
                            new FilterComboBox("ContractorType", "Тип контрагента", ComboBoxBuilder.GetComboBoxItemList<ContractorType>())
                        }
                    },
                    Grid = GetContractorGridLocal(new GridState(), user),
                    Title = "Выбор контрагента"
                };

                return model;
            }
        }

        /// <summary>
        /// Получение модели грида выбора контрагента
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public GridData GetContractorGrid(GridState state, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetContractorGridLocal(state, user);
            }
        }

        /// <summary>
        /// Формирование модели грида выбора контрагента
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private GridData GetContractorGridLocal(GridState state, User user)
        {
            var model = new GridData();
            model.AddColumn("Action", "Выбрать", Unit.Pixel(40));
            model.AddColumn("Name", "Название", Unit.Percentage(100));
            model.AddColumn("Type", "Тип", Unit.Pixel(100));
            model.AddColumn("TypeId", "", GridCellStyle.Hidden);
            model.AddColumn("Id", "", GridCellStyle.Hidden);

            var list = contractorService.GetContractorByUser(state, user);
            
            var actions = new GridActionCell("Action");
            actions.AddAction("Выбрать", "select_contractor");

            foreach (var item in list)
            {
                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("Name") { Value = item.Name },
                    new GridLabelCell("Type") { Value = item.ContractorType.GetDisplayName() },
                    new GridLabelCell("TypeId") { Value = item.ContractorType.ValueToString() },
                    new GridHiddenCell("Id") { Value = item.Id.ToString() }));
            }

            model.State = state;

            return model;
        }

        #endregion
    }
}
