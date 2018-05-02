using System;
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
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ContractorOrganization;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ContractorOrganizationPresenter : IContractorOrganizationPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IContractorOrganizationService contractorOrganizationService;
        private readonly IUserService userService;

        #endregion

        #region Конструкторы

        public ContractorOrganizationPresenter(IUnitOfWorkFactory unitOfWorkFactory, IContractorOrganizationService contractorOrganizationService, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.contractorOrganizationService = contractorOrganizationService;
            this.userService = userService;
        }

        #endregion

        #region Методы

        public ContractorOrganizationListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                ContractorOrganizationListViewModel model = new ContractorOrganizationListViewModel();
                model.ContractorOrganizationGrid = GetContractorOrganizationGridLocal(new GridState() { PageSize = 10, Sort = "ShortName=Asc" }, user);

                model.Filter.Items.Add(new FilterTextEditor("Name", "Наименование"));
                model.Filter.Items.Add(new FilterTextEditor("INN", "ИНН"));
                model.Filter.Items.Add(new FilterTextEditor("Address", "Адрес"));
                model.Filter.Items.Add(new FilterTextEditor("OGRN", "ОГРН"));
                model.Filter.Items.Add(new FilterTextEditor("OKPO", "ОКПО"));
                model.Filter.Items.Add(new FilterTextEditor("KPP", "КПП"));
                model.Filter.Items.Add(new FilterComboBox("Type", "Тип организации", new List<SelectListItem>() 
                {
                    new SelectListItem(){ Text = "", Value = ""},
                    new SelectListItem(){ Text = OrganizationType.ClientOrganization.GetDisplayName(), Value = OrganizationType.ClientOrganization.ValueToString()},
                    new SelectListItem(){ Text = OrganizationType.ProviderOrganization.GetDisplayName(), Value = OrganizationType.ProviderOrganization.ValueToString()},
                }));

                return model;
            }
        }

        /// <summary>
        /// Формирование грида организаций
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <returns></returns>
        public GridData GetContractorOrganizationGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetContractorOrganizationGridLocal(state, user);
            }
        }

        /// <summary>
        /// Формирование грида организаций
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <returns></returns>
        private GridData GetContractorOrganizationGridLocal(GridState state, User user)
        {
            if (state == null) { state = new GridState(); }

            GridData grid = new GridData();
            grid.AddColumn("INN", "ИНН", Unit.Pixel(80));
            grid.AddColumn("ShortName", "Краткое название", Unit.Percentage(40));
            grid.AddColumn("Type", "Тип организации", Unit.Percentage(20));
            grid.AddColumn("LegalForm", "Юр. форма", Unit.Pixel(80));
            grid.AddColumn("Address", "Адрес", Unit.Percentage(40));
            grid.AddColumn("Id", "", GridCellStyle.Hidden);
            grid.AddColumn("TypeId", "", GridCellStyle.Hidden);

            // Отфильтровываем все типы организаций, кроме ProviderOrganization и ClientOrganization
            ParameterString parameterString = new ParameterString("");
            parameterString.Add("Type", ParameterStringItem.OperationType.OneOf);
            
            var availableOrganizationTypes = new List<string>();                       
                        
            if(user.HasPermission(Permission.ProviderOrganization_List_Details))
            {
                availableOrganizationTypes.Add(OrganizationType.ProviderOrganization.ValueToString());
            }
            if(user.HasPermission(Permission.ClientOrganization_List_Details))
            {
                availableOrganizationTypes.Add(OrganizationType.ClientOrganization.ValueToString());
            };

            if (availableOrganizationTypes.Count == 0) availableOrganizationTypes.Add("0");

            parameterString["Type"].Value = availableOrganizationTypes;

            var fps = new ParameterString(state.Filter);
            if (fps["Type"]!= null && fps["Type"].Value as string == "")
            {
                fps.Delete("Type");
            }
            parameterString.MergeWith(fps);

            var list = contractorOrganizationService.GetFilteredList(state, parameterString);

            foreach (var org in list)
            {
                string type = "", typeId = "";

                bool addRow = true;

                if (org.Is<ProviderOrganization>())
                {                   
                    type = OrganizationType.ProviderOrganization.GetDisplayName();
                    typeId = OrganizationType.ProviderOrganization.ValueToString();
                }
                else if (org.Is<ClientOrganization>())
                {                 
                    type = OrganizationType.ClientOrganization.GetDisplayName();
                    typeId = OrganizationType.ClientOrganization.ValueToString();
                }
                else if (org.Is<ProducerOrganization>())
                {
                    addRow = false;
                }
                else
                {
                    throw new Exception(String.Format("Обнаружена организация ({0}) неизвестного типа.", org.Id));
                }

                if (addRow)
                {
                    string inn = (org.EconomicAgent.Is<JuridicalPerson>() ? (org.EconomicAgent.As<JuridicalPerson>()).INN : (org.EconomicAgent.As<PhysicalPerson>()).INN);
                    
                    grid.AddRow(new GridRow(
                        new GridLabelCell("INN") { Value = inn },
                        new GridLinkCell("ShortName") { Value = org.As<ContractorOrganization>().ShortName },
                        new GridLabelCell("Type") { Value = type },
                        new GridLabelCell("LegalForm") { Value = org.EconomicAgent.LegalForm.Name },
                        new GridLabelCell("Address") { Value = org.As<ContractorOrganization>().Address },
                        new GridHiddenCell("Id") { Value = org.Id.ToString() },
                        new GridHiddenCell("TypeId") { Value = typeId }
                        ));
                }
            }

            grid.State = state;

            return grid;
        }

        #endregion
    }
}
