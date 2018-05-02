using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.Utils;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ContractorOrganization;
using ERP.Wholesale.UI.ViewModels.EconomicAgent;
using ERP.Wholesale.UI.ViewModels.Provider;
using ERP.Wholesale.UI.AbstractPresenters.Mediators;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ProviderPresenter : IProviderPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IProviderService providerService;
        private readonly IProviderTypeService providerTypeService;
        private readonly IProviderOrganizationService providerOrganizationService;
        private readonly IAccountOrganizationService accountOrganizationService;
        private readonly ILegalFormService legalFormService;
        private readonly IReceiptWaybillService receiptWaybillService;
        private readonly IUserService userService;
        private readonly IArticlePurchaseService articlePurchaseService;
        private readonly ITaskPresenterMediator taskPresenterMediator;

        #endregion

        #region Конструктор

        public ProviderPresenter(IUnitOfWorkFactory unitOfWorkFactory, IProviderService providerService, IProviderTypeService providerTypeService,
            IProviderOrganizationService providerOrganizationService, IAccountOrganizationService accountOrganizationService,
            ILegalFormService legalFormService, IReceiptWaybillService receiptWaybillService, IUserService userService, IArticlePurchaseService articlePurchaseService,
            ITaskPresenterMediator taskPresenterMediator)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.providerService = providerService;
            this.providerTypeService = providerTypeService;
            this.providerOrganizationService = providerOrganizationService;
            this.accountOrganizationService = accountOrganizationService;
            this.legalFormService = legalFormService;
            this.userService = userService;
            this.receiptWaybillService = receiptWaybillService;
            this.articlePurchaseService = articlePurchaseService;
            this.taskPresenterMediator = taskPresenterMediator;
        }

        #endregion

        #region Методы

        #region Список

        public ProviderListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Provider_List_Details);

                ProviderListViewModel model = new ProviderListViewModel();
                model.ProviderGrid = GetProviderGridLocal(new GridState() { PageSize = 10, Sort = "Name=Asc" }, user);

                model.Filter.Items.Add(new FilterTextEditor("Name", "Наименование"));
                model.Filter.Items.Add(new FilterComboBox("Rating", "Рейтинг", Enumerable.Range(0, 11).GetComboBoxItemList<int>(x => x.ToString(), x => x.ToString(), sort: false)));
                model.Filter.Items.Add(new FilterComboBox("Type", "Тип", providerTypeService.GetList().GetComboBoxItemList<ProviderType>(x => x.Name, x => x.Id.ToString())));
                model.Filter.Items.Add(new FilterComboBox("Reliability", "Надежность", ComboBoxBuilder.GetComboBoxItemList<ProviderReliability>(sort: false)));

                return model;
            }
        }

        public GridData GetProviderGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProviderGridLocal(state, user);
            }
        }

        private GridData GetProviderGridLocal(GridState state, User user)
        {
            if (state == null)
            {
                state = new GridState();
            }

            GridData model = new GridData();
            model.AddColumn("Name", "Наименование", Unit.Percentage(70));
            model.AddColumn("Type", "Тип", Unit.Percentage(30));
            model.AddColumn("Reliability", "Надежность", Unit.Pixel(100));
            model.AddColumn("Rating", "Рейтинг", Unit.Pixel(57), align: GridColumnAlign.Center);
            model.AddColumn("PurchaseCostSum", "Сумма закупок", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreate"] = user.HasPermission(Permission.Provider_Create);

            var list = providerService.GetFilteredList(state);

            var allowToViewPurchaseCosts = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

            var purchaseCostSums = new DynamicDictionary<int, decimal>();
            if (allowToViewPurchaseCosts)
            {
                purchaseCostSums = articlePurchaseService.GetTotalPurchaseCostSum(list, user);
            }

            foreach (Provider provider in list)
            {
                model.AddRow(new GridRow(
                    new GridLinkCell("Name") { Value = provider.Name },
                    new GridLabelCell("Type") { Value = provider.Type.Name },
                    new GridLabelCell("Reliability") { Value = provider.Reliability.GetDisplayName() },
                    new GridLabelCell("Rating") { Value = provider.Rating.ToString() },
                    new GridLabelCell("PurchaseCostSum") { Value = allowToViewPurchaseCosts ? purchaseCostSums[provider.Id].ForDisplay(ValueDisplayType.Money) : "---" },
                    new GridLabelCell("Id") { Value = provider.Id.ToString() }));
            }

            model.State = state;

            return model;
        }

        #endregion 
      
        #region Добавление / редактирование поставщика

        public ProviderEditViewModel Create(string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Provider_Create);

                ProviderEditViewModel model = new ProviderEditViewModel();
                model.BackURL = backURL;
                model.Title = "Добавление поставщика";
                model.RatingList = Enumerable.Range(0, 11).GetComboBoxItemList<int>(x => x.ToString(), x => x.ToString(), false, sort: false);
                model.TypeList = ComboBoxBuilder.GetComboBoxItemList(providerTypeService.GetList(), x => x.Name, x => x.Id.ToString());
                model.ReliabilityList = ComboBoxBuilder.GetComboBoxItemList<ProviderReliability>(sort: false);

                model.AllowToCreateProviderType = user.HasPermission(Permission.ProviderType_Create);

                return model;
            }
        }

        public ProviderEditViewModel Edit(int id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Provider_Edit);

                var provider = providerService.CheckProviderExistence(id);

                ProviderEditViewModel model = new ProviderEditViewModel();
                model.Name = provider.Name;
                model.Type = provider.Type.Id;
                model.Reliability = (byte)provider.Reliability;
                model.Rating = provider.Rating;
                model.Comment = provider.Comment;

                model.BackURL = backURL;
                model.Title = "Редактирование поставщика";
                model.RatingList = Enumerable.Range(0, 11).GetComboBoxItemList<int>(x => x.ToString(), x => x.ToString(), false, sort: false);
                model.TypeList = ComboBoxBuilder.GetComboBoxItemList(providerTypeService.GetList(), x => x.Name, x => x.Id.ToString());
                model.ReliabilityList = ComboBoxBuilder.GetComboBoxItemList<ProviderReliability>(sort: false);

                model.AllowToCreateProviderType = user.HasPermission(Permission.ProviderType_Create);

                return model;
            }
        }

        public int Save(ProviderEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                Provider provider = null;

                var provType = providerTypeService.CheckExistence(model.Type);

                if (model.Id == 0)
                {
                    user.CheckPermission(Permission.Provider_Create);

                    provider = new Provider(model.Name, provType, (ProviderReliability)model.Reliability, model.Rating);
                }
                else
                {
                    user.CheckPermission(Permission.Provider_Edit);

                    provider = providerService.CheckProviderExistence(model.Id);

                    provider.Name = model.Name;
                    provider.Reliability = (ProviderReliability)model.Reliability;
                    provider.Rating = model.Rating;
                    provider.Type = provType;
                }

                provider.Comment = StringUtils.ToHtml(model.Comment);

                providerService.Save(provider);

                uow.Commit();

                return provider.Id;
            }
        }

        public object GetProviderTypes()
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var types = providerTypeService.GetList().GetComboBoxItemList(p => p.Name, p => p.Id.ToString(), false);

                return new { List = types };
            }
        }

        #endregion

        #region Детали поставщика

        #region Детали общие

        public ProviderDetailsViewModel Details(int id, string backURL, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Provider_List_Details);

                var provider = providerService.CheckProviderExistence(id);

                var allowToViewPurchaseCosts = user.HasPermission(Permission.PurchaseCost_View_ForReceipt);

                var model = new ProviderDetailsViewModel();
                model.MainDetails.Id = provider.Id.ToString();
                model.MainDetails.Name = provider.Name;
                model.MainDetails.TypeName = provider.Type.Name;
                model.MainDetails.ReliabilityName = provider.Reliability.GetDisplayName();
                model.MainDetails.Rating = provider.Rating.ToString();
                model.MainDetails.CreationDate = provider.CreationDate.ToShortDateString();
                model.MainDetails.Comment = provider.Comment;
                model.MainDetails.ProviderOrganizationCount = provider.OrganizationCount.ToString();
                model.MainDetails.ContractCount = provider.Contracts.Count().ToString();
                model.MainDetails.PurchaseCostSum = (allowToViewPurchaseCosts ? articlePurchaseService.GetTotalPurchaseCostSum(provider, user).ForDisplay(ValueDisplayType.Money) : "---");
                model.MainDetails.PendingPurchaseCostSum = (allowToViewPurchaseCosts ? articlePurchaseService.GetPendingPurchaseCostSum(provider, user).ForDisplay(ValueDisplayType.Money) : "---");

                // Заполнение гридов
                if (user.HasPermission(Permission.ReceiptWaybill_List_Details))
                {
                    model.ReceiptWaybillGrid = GetReceiptWaybillGridLocal(new GridState() { Parameters = "providerId=" + provider.Id, PageSize = 10, Sort = "Date=Desc;CreationDate=Desc" }, user);
                    model.AllowToViewReceiptWaybillList = true;
                }

                if (user.HasPermission(Permission.ProviderOrganization_List_Details))
                {
                    model.ProviderOrganizationGrid = GetProviderOrganizationGridLocal(new GridState() { Parameters = "providerId=" + provider.Id, PageSize = 10 }, user);
                    model.AllowToViewProviderOrganizationList = true;
                }
                model.ProviderContractGrid = GetProviderContractGridLocal(new GridState() { Parameters = "providerId=" + provider.Id, PageSize = 10 }, user);
                model.TaskGrid = taskPresenterMediator.GetTaskGridForProvider(provider, user);

                model.AllowToEdit = user.HasPermission(Permission.Provider_Edit);
                model.AllowToDelete = user.HasPermission(Permission.Provider_Delete);

                return model;
            }
        }

        private object GetMainChangeableIndicators(Provider provider, ContractorOrganization contractorOrganization = null)
        {
            if (contractorOrganization != null)
            {
                return new
                {
                    ProviderOrganizationCount = provider.OrganizationCount.ToString(),
                    ContractCount = provider.Contracts.Count().ToString(),
                    ContractorOrganizationId = contractorOrganization.Id.ToString(),
                    ContractorOrganizationShortName = contractorOrganization.ShortName,
                };
            }
            else
            {
                return new
                {
                    ProviderOrganizationCount = provider.OrganizationCount.ToString(),
                    ContractCount = provider.Contracts.Count().ToString(),
                };
            }
        }

        #endregion

        #region Грид "Закупки у поставщика"

        /// <summary>
        /// Формирование модели грида "Закупки у поставщика"
        /// </summary>
        /// <param name="state">Состояние грида</param>
        public GridData GetReceiptWaybillGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetReceiptWaybillGridLocal(state, user);
            }
        }

        /// <summary>
        /// Формирование модели грида "Закупки у поставщика"
        /// </summary>
        /// <param name="state">Состояние грида</param>
        private GridData GetReceiptWaybillGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");

            GridData model = new GridData();
            model.AddColumn("ProviderNumber", "Номер документа", Unit.Pixel(115));
            model.AddColumn("Number", "Внутренний номер", Unit.Pixel(85));
            model.AddColumn("Date", "Дата", Unit.Pixel(60));
            model.AddColumn("CurrentSum", "Сумма", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("State", "Статус", Unit.Percentage(40));
            model.AddColumn("ReceiptStorageName", "Место хранения", Unit.Percentage(60));
            model.AddColumn("ReceiptWaybillId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ReceiptStorageId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreateReceiptWaybill"] = user.HasPermission(Permission.ReceiptWaybill_Create_Edit);

            ParameterString deriveParams = new ParameterString(state.Parameters);

            int providerId;
            if (deriveParams["providerId"].Value == null || !Int32.TryParse(deriveParams["providerId"].Value as string, out providerId))
            {
                throw new Exception("Неверное значение входного параметра.");
            }
            
            string filter = state.Filter;
            ParameterString deriveFilter = new ParameterString(state.Filter);            
            deriveFilter.Add("Provider", ParameterStringItem.OperationType.Eq, providerId.ToString());
            state.Filter = deriveFilter.ToString();

            var receiptWaybillList = receiptWaybillService.GetFilteredList(state, user);

            state.Filter = filter;
            
            foreach (var item in receiptWaybillList)
            {
                var allowToViewPurchaseCost = receiptWaybillService.IsPossibilityToViewPurchaseCosts(item, user);

                model.AddRow(new GridRow(
                    new GridLabelCell("ProviderNumber") { Value = item.ProviderNumber },
                    new GridLinkCell("Number") { Value = StringUtils.PadLeftZeroes(item.Number, 8) },
                    new GridLabelCell("Date") { Value = item.Date.ToShortDateString() },
                    new GridLabelCell("CurrentSum") { Value = allowToViewPurchaseCost ? item.CurrentSum.ForDisplay(ValueDisplayType.Money) : "---" },
                    new GridLabelCell("State") { Value = item.State.GetDisplayName() },
                    new GridLinkCell("ReceiptStorageName") { Value = item.ReceiptStorage.Name },
                    new GridHiddenCell("ReceiptWaybillId") { Value = item.Id.ToString() },
                    new GridHiddenCell("ReceiptStorageId") { Value = item.ReceiptStorage.Id.ToString() }
                ) { Style = (!item.IsApproved && item.AreSumDivergences) ? GridRowStyle.Error : GridRowStyle.Normal });
            }

            model.State = state;

            return model;
        }

        #endregion

        #region Грид "Организации поставщика"

        /// <summary>
        /// Формирование грида "Организации поставщика"
        /// </summary>
        /// <param name="state">Состояние грида</param>
        public GridData GetProviderOrganizationGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProviderOrganizationGridLocal(state, user);
            }
        }

        /// <summary>
        /// Формирование грида "Организации поставщика"
        /// </summary>
        /// <param name="state">Состояние грида</param>
        private GridData GetProviderOrganizationGridLocal(GridState state, User user)
        {
            ValidationUtils.NotNull(state, "Неверное значение входного параметра.");            
            
            var allowToRemoveOrganization = user.HasPermission(Permission.Provider_ProviderOrganization_Remove);

            GridData model = new GridData();
            if (allowToRemoveOrganization) { model.AddColumn("Action", "Действие", Unit.Pixel(90)); }
            model.AddColumn("INN", "ИНН", Unit.Pixel(75));
            model.AddColumn("ShortName", "Краткое наименование", Unit.Percentage(100));
            model.AddColumn("LegalForm", "Юр. форма", Unit.Pixel(75));
            model.AddColumn("ContractCount", "Кол-во договоров", Unit.Pixel(70), align: GridColumnAlign.Right);
            model.AddColumn("PurchaseCostSum", "Сумма закупок", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToAddProviderOrganization"] = user.HasPermission(Permission.Provider_ProviderOrganization_Add);

            ParameterString deriveParams = new ParameterString(state.Parameters);
            var provider = providerService.CheckProviderExistence(Int32.Parse(deriveParams["providerId"].Value as string));

            var providerOrganizationList = provider.Organizations.OrderBy(x => x.ShortName).Cast<ProviderOrganization>();

            model.State = state;

            var actions = new GridActionCell("Action");
            if (allowToRemoveOrganization)
            {
                actions.AddAction("Удал. из списка", "linkProviderOrganizationDelete");
            }

            providerOrganizationList = GridUtils.GetEntityRange(providerOrganizationList , state);

            var allowToViewPurchaseCostsEverywhere = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);
            var purchaseCostSums = new DynamicDictionary<int, decimal>();
            if (allowToViewPurchaseCostsEverywhere)
            {
                purchaseCostSums = articlePurchaseService.GetTotalPurchaseCostSum(provider, providerOrganizationList, user);
            }

            foreach (var providerOrganization in providerOrganizationList)
            {
                string innString = "";
                switch (providerOrganization.EconomicAgent.As<EconomicAgent>().Type)
                {
                    case EconomicAgentType.JuridicalPerson:
                        var juridicalPerson = providerOrganization.EconomicAgent.As<JuridicalPerson>();
                        innString = juridicalPerson.INN;
                        break;

                    case EconomicAgentType.PhysicalPerson:
                        var physicalPerson = providerOrganization.EconomicAgent.As<PhysicalPerson>();
                        innString = physicalPerson.INN;
                        break;
                    default:
                        throw new Exception(String.Format("Обнаружен экономический агент ({0}) неизвестного типа ({1}).", providerOrganization.Id, providerOrganization.EconomicAgent.As<EconomicAgent>().Type));
                };

                model.AddRow(new GridRow(
                    actions,
                    new GridLabelCell("INN") { Value = innString },
                    new GridLinkCell("ShortName") { Value = providerOrganization.ShortName },
                    new GridLabelCell("LegalForm") { Value = providerOrganization.EconomicAgent.LegalForm.Name },
                    new GridLabelCell("ContractCount") { Value = providerOrganization.ContractCount.ToString() },
                    new GridLabelCell("PurchaseCostSum") { Value = (user.HasPermission(Permission.PurchaseCost_View_ForEverywhere) ?
                         purchaseCostSums[providerOrganization.Id].ForDisplay(ValueDisplayType.Money) : "---") },
                    new GridHiddenCell("Id") { Value = providerOrganization.Id.ToString() }
                ));
            }

            return model;
        }

        #endregion

        #region Грид "Договоры"

        public GridData GetProviderContractGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetProviderContractGridLocal(state, user);
            }
        }

        private GridData GetProviderContractGridLocal(GridState state, User user)
        {
           if (state == null)
            {
                throw new Exception("Неверное значение входного параметра.");
            }

            GridData model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(70));
            model.AddColumn("Number", "Номер", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Date", "Дата", Unit.Pixel(65));
            model.AddColumn("Name", "Название", Unit.Percentage(30));
            model.AddColumn("AccountOrganizationName", "Собственная организация", Unit.Percentage(35));
            model.AddColumn("ProviderOrganizationName", "Организация поставщика", Unit.Percentage(35));
            model.AddColumn("PurchaseCostSum", "Сумма закупок", Unit.Pixel(90), align: GridColumnAlign.Right);
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ContractorId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("AccountOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);
            model.AddColumn("ProviderOrganizationId", "", Unit.Pixel(0), GridCellStyle.Hidden);

            model.ButtonPermissions["AllowToCreateContract"] = user.HasPermission(Permission.Provider_ProviderContract_Create);

            ParameterString deriveParams = new ParameterString(state.Parameters);

            var provider = providerService.CheckProviderExistence(Int32.Parse(deriveParams["providerId"].Value as string));
            
            model.State = state;

            var contractList = GridUtils.GetEntityRange(provider.Contracts.OrderByDescending(x => x.Date).ToList<Contract>(), state);

            var allowToViewPurchaseCostEverywhere = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);
            var purchaseCostSums = new DynamicDictionary<short, decimal>();
            if (allowToViewPurchaseCostEverywhere)
            {
                purchaseCostSums = articlePurchaseService.GetTotalPurchaseCostSum(provider, contractList, user);
            }
            
            foreach (var item in contractList)
            {
                ProviderContract providerContract = item != null ? item.As<ProviderContract>() : null;

                if (providerContract != null)
                {
                    var actions = new GridActionCell("Action");
                    actions.AddAction(user.HasPermission(Permission.Provider_ProviderContract_Edit) ? "Ред." : "Дет.", "linkProviderContractEdit");

                    if (providerService.IsPossibilityToDeleteContract(provider, providerContract, user))
                    {
                        actions.AddAction("Удал.", "linkProviderContractDelete");
                    }

                    model.AddRow(new GridRow(
                        actions,
                        new GridLabelCell("Number") { Value = providerContract.Number },
                        new GridLabelCell("Date") { Value = providerContract.Date.ToShortDateString() },
                        new GridLabelCell("Name") { Value = providerContract.Name },
                        new GridLinkCell("AccountOrganizationName") { Value = providerContract.AccountOrganization.ShortName },
                        user.HasPermission(Permission.ProviderOrganization_List_Details) ?
                            (GridCell)new GridLinkCell("ProviderOrganizationName") { Value = providerContract.ContractorOrganization.ShortName } :
                            (GridCell)new GridLabelCell("ProviderOrganizationName") { Value = providerContract.ContractorOrganization.ShortName },
                        new GridLabelCell("PurchaseCostSum") { Value = allowToViewPurchaseCostEverywhere ?
                            purchaseCostSums[item.Id].ForDisplay(ValueDisplayType.Money) : "---"
                        },
                        new GridLabelCell("Id") { Value = providerContract.Id.ToString(), Key = "providerContractId" },
                        new GridLabelCell("ContractorId") { Value = item.Id.ToString() },
                        new GridLabelCell("AccountOrganizationId") { Value = item.AccountOrganization.Id.ToString() },
                        new GridLabelCell("ProviderOrganizationId") { Value = item.ContractorOrganization.Id.ToString() }
                        )
                    );
                }
            }

            return model;
        }

        #endregion

        #region Грид "Задачи"

        /// <summary>
        /// Получение модели для грида задач
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public GridData GetTaskGrid(GridState state, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetTaskGridLocal(state, user);
            }
        }

        /// <summary>
        /// Формирование грида задач
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private GridData GetTaskGridLocal(GridState state, User user)
        {
            return taskPresenterMediator.GetTaskGridForProvider(state, user);
        }

        #endregion

        #endregion

        #region Создание, редактирование, удаление организаций поставщика

        #region Добавление организации поставщика

        #region Модальная форма для создания новой организации

        public EconomicAgentTypeSelectorViewModel CreateContractorOrganization(int contractorId)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = new EconomicAgentTypeSelectorViewModel();
                model.Title = "Добавление новой организации";
                model.ActionNameForJuridicalPerson = "EditJuridicalPerson";
                model.ActionNameForPhysicalPerson = "EditPhysicalPerson";
                model.ControllerName = "Provider";
                model.SuccessFunctionName = "OnSuccessOrganizationEdit";
                model.ContractorId = contractorId.ToString();

                return model;
            }
        }

        public object SaveJuridicalPerson(JuridicalPersonEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProviderOrganization_Create);

                var legalForm = legalFormService.CheckExistence(ValidationUtils.TryGetShort(model.LegalFormId));
                var provider = providerService.CheckProviderExistence(ValidationUtils.TryGetInt(model.ContractorId));

                ProviderOrganization providerOrganization;
                JuridicalPerson jp = null;

                jp = new JuridicalPerson(legalForm);
                jp.INN = model.INN;
                jp.KPP = model.KPP;
                jp.OGRN = model.OGRN;
                jp.OKPO = model.OKPO;
                jp.LegalForm = legalForm;
                jp.DirectorName = model.DirectorName;
                jp.DirectorPost = model.DirectorPost;
                jp.MainBookkeeperName = model.Bookkeeper;
                jp.CashierName = model.Cashier;

                providerOrganization = new ProviderOrganization(model.ShortName, model.FullName, jp);

                providerOrganization.Address = model.Address;
                providerOrganization.Comment = StringUtils.ToHtml(model.Comment);
                providerOrganization.Phone = model.Phone;
                providerOrganization.Fax = model.Fax;

                providerOrganizationService.Save(providerOrganization);
                providerService.AddContractorOrganization(provider, providerOrganization);

                uow.Commit();

                return GetMainChangeableIndicators(provider, providerOrganization);
            }
        }

        public object SavePhysicalPerson(PhysicalPersonEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ProviderOrganization_Create);

                short legalFormId;
                int providerId;

                if (!Int16.TryParse(model.LegalFormId, out legalFormId))
                {
                    throw new Exception("Правовая форма указана неверно.");
                }
                if (!Int32.TryParse(model.ContractorId, out providerId))
                {
                    throw new Exception("Контрагент указан неверно.");
                }

                var legalForm = legalFormService.CheckExistence(legalFormId);
                var provider = providerService.CheckProviderExistence(providerId);

                ProviderOrganization providerOrganization;
                PhysicalPerson pp = null;

                pp = new PhysicalPerson(legalForm);
                pp.OwnerName = model.FIO;
                pp.INN = model.INN;
                pp.Passport.Series = model.Series;
                pp.Passport.Number = model.Number;
                pp.Passport.IssuedBy = model.IssuedBy;
                pp.Passport.IssueDate = model.IssueDate == null ? (DateTime?)null : Convert.ToDateTime(model.IssueDate);
                pp.Passport.DepartmentCode = model.DepartmentCode;
                pp.OGRNIP = model.OGRNIP;
                pp.LegalForm = legalForm;

                providerOrganization = new ProviderOrganization(model.ShortName, model.FullName, pp);

                providerOrganization.Address = model.Address;
                providerOrganization.Comment = StringUtils.ToHtml(model.Comment);
                providerOrganization.Phone = model.Phone;
                providerOrganization.Fax = model.Fax;

                providerOrganizationService.Save(providerOrganization);
                providerService.AddContractorOrganization(provider, providerOrganization);

                uow.Commit();

                return GetMainChangeableIndicators(provider, providerOrganization);
            }
        }

        #endregion

        #region Добавление организации контрагента

        public object AddContractorOrganization(int providerId, int organizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {

                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Provider_ProviderOrganization_Add);

                var provider = providerService.CheckProviderExistence(providerId);
                var providerOrganization = providerOrganizationService.CheckProviderOrganizationExistence(organizationId);

                providerService.AddContractorOrganization(provider, providerOrganization);

                uow.Commit();

                return GetMainChangeableIndicators(provider);
            }
        }

        #endregion

        #endregion

        #region Удаление организации поставщика

        public object RemoveProviderOrganization(int providerId, int providerOrganizationId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Provider_ProviderOrganization_Remove);

                var provider = providerService.CheckProviderExistence(providerId);

                ContractorOrganization contractorOrganization = provider.Organizations.SingleOrDefault(x => x.Id == providerOrganizationId);
                ValidationUtils.NotNull(contractorOrganization, "Организация поставщика не найдена. Возможно, она была удалена или больше не принадлежит данному поставщику.");

                ProviderOrganization providerOrganization = contractorOrganization.As<ProviderOrganization>();

                providerService.RemoveProviderOrganization(provider, providerOrganization);

                uow.Commit();

                return GetMainChangeableIndicators(provider);
            }
        }

        #endregion

        #endregion

        #region Создание, редактирование, удаление договоров

        #region Добавление и редактирование договора

        public ProviderContractEditViewModel CreateContract(int providerId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Provider_ProviderContract_Create);

                var model = new ProviderContractEditViewModel();
                model.Title = "Добавление договора";
                model.ProviderId = providerId.ToString();
                model.Date = DateTime.Now.ToShortDateString();
                model.AllowToEdit = true;
                model.AllowToChangeOrganizations = true;

                return model;
            }
        }

        public ProviderContractEditViewModel EditContract(int providerId, short contractId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var provider = providerService.CheckProviderExistence(providerId);

                var allowToEdit = user.HasPermission(Permission.Provider_ProviderContract_Edit);

                Contract contract = provider.Contracts.SingleOrDefault(x => x.Id == contractId);
                if (contract == null)
                {
                    throw new Exception("Выбранный договор удален или больше не связан с данным поставщиком.");
                }

                var providerContract = contract.As<ProviderContract>();

                var model = new ProviderContractEditViewModel(contract);
                model.Title = (allowToEdit ? "Редактирование договора" : "Детали договора");
                model.ProviderId = providerId.ToString();
                model.AllowToEdit = allowToEdit;
                model.AllowToChangeOrganizations = providerService.IsPossibilityToEditOrganization(provider, providerContract, user);

                return model;
            }
        }

        public object SaveContract(ProviderContractEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var providerId = ValidationUtils.TryGetInt(model.ProviderId);
                var accountOrganizationId = ValidationUtils.TryGetInt(model.AccountOrganizationId);
                var providerOrganizationId = ValidationUtils.TryGetInt(model.ProviderOrganizationId);
                var id = ValidationUtils.TryGetInt(model.Id);

                DateTime date;
                if (!DateTime.TryParse(model.Date, out date))
                {
                    throw new Exception("Введите дату в правильном формате или выберите из списка.");
                }

                var provider = providerService.CheckProviderExistence(providerId);
                var accountOrganization = accountOrganizationService.CheckAccountOrganizationExistence(accountOrganizationId);

                ProviderOrganization providerOrganization = providerOrganizationService.CheckProviderOrganizationExistence(providerOrganizationId);

                if (!provider.Organizations.Contains(providerOrganization))
                {
                    throw new Exception("Выбранная организация поставщика больше не принадлежит данному поставщику.");
                }

                if (id == 0)
                {
                    user.CheckPermission(Permission.Provider_ProviderContract_Create);

                    var providerContract = new ProviderContract(accountOrganization, providerOrganization, model.Name, model.Number, date, date);
                    providerContract.Comment = StringUtils.ToHtml(model.Comment);

                    providerService.AddProviderContract(provider, providerContract);
                }
                else
                {
                    user.CheckPermission(Permission.Provider_ProviderContract_Edit);

                    Contract contract = provider.Contracts.SingleOrDefault(x => x.Id == id);
                    var providerContract = contract.As<ProviderContract>();
                    if (contract == null)
                    {
                        throw new Exception("Выбранный договор удален или больше не принадлежит данному поставщику.");
                    }

                    contract.Comment = StringUtils.ToHtml(model.Comment);
                    contract.Date = date;
                    contract.Name = model.Name;
                    contract.Number = model.Number;
                    contract.StartDate = date;

                    if (contract.AccountOrganization != accountOrganization || contract.ContractorOrganization != providerOrganization)
                    {
                        providerService.CheckPossibilityToEditOrganization(provider, providerContract, user);

                        contract.AccountOrganization = accountOrganization;
                        contract.ContractorOrganization = providerOrganization;
                    }

                    providerService.Save(provider);
                }

                uow.Commit();

                return GetMainChangeableIndicators(provider);
            }
        }

        #endregion

        #region Удаление договора

        public object DeleteContract(int providerId, short contractId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var provider = providerService.CheckProviderExistence(providerId);

                Contract contract = provider.Contracts.SingleOrDefault(x => x.Id == contractId);
                if (contract == null)
                {
                    throw new Exception("Выбранный договор удален или больше не связан с данным поставщиком.");
                }

                ProviderContract providerContract = contract.As<ProviderContract>();

                providerService.DeleteProviderContract(provider, providerContract, user);

                uow.Commit();

                return GetMainChangeableIndicators(provider);
            }
        }

        #endregion

        #endregion

        #region Модальная форма выбора организации поставщика

        public ContractorOrganizationSelectViewModel SelectContractorOrganization(int providerId, string mode, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {

                var user = userService.CheckUserExistence(currentUser.Id);

                mode = mode.ToLower();

                if (mode != "includeprovider" && mode != "excludeprovider")
                {
                    throw new Exception("Неверное значение входного параметра.");
                }

                var state = new GridState { Parameters = "providerId=" + providerId + ";mode=" + mode };

                var model = new ContractorOrganizationSelectViewModel();

                model.Title = "Добавление связанной организации";
                model.NewOrganizationLinkName = "Создать новую организацию и связать ее с данным поставщиком";

                model.ContractorId = providerId.ToString();
                model.ControllerName = "Provider";
                model.ActionName = "CreateContractorOrganization";
                model.AllowToCreateNewOrganization = user.HasPermission(Permission.ProviderOrganization_Create);

                model.GridData = GetProviderOrganizationSelectGridLocal(state);
                model.Filter = new FilterData()
                {
                    Items = new List<FilterItem>()
                {
                    new FilterTextEditor("ShortName", "Название"),
                    new FilterTextEditor("INN", "ИНН"),
                    new FilterTextEditor("Address", "Адрес"),
                    new FilterTextEditor("OGRN", "ОГРН"),
                    new FilterTextEditor("OKPO", "ОКПО"),
                    new FilterTextEditor("KPP", "КПП")
                }
                };

                return model;
            }
        }

                /// <summary>
        /// Формирование модели грида доступных организаций контрагента
        /// </summary>
        /// <param name="state">состояние грида</param>
        public GridData GetProviderOrganizationSelectGrid(GridState state = null)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {                
                return GetProviderOrganizationSelectGridLocal(state);
            }
        }

        /// <summary>
        /// Формирование модели грида доступных организаций контрагента
        /// </summary>
        /// <param name="state">состояние грида</param>
        private GridData GetProviderOrganizationSelectGridLocal(GridState state = null)
        {
            GridData model = new GridData();
            model.AddColumn("Action", "Действие", Unit.Pixel(55));
            model.AddColumn("INN", "ИНН", Unit.Pixel(75));
            model.AddColumn("ShortName", "Название", Unit.Percentage(45));
            model.AddColumn("LegalForm", "Юр. форма", Unit.Pixel(80));
            model.AddColumn("Address", "Адрес", Unit.Percentage(55));
            model.AddColumn("Id", "", Unit.Pixel(0), GridCellStyle.Hidden);

            ParameterString deriveParams = new ParameterString(state.Parameters);

            string mode = deriveParams["mode"].Value as string;

            if (mode == null)
            {
                throw new Exception("Неверное значение входного параметра.");
            }

            int providerId;

            if (deriveParams["providerId"] == null || !Int32.TryParse(deriveParams["providerId"].Value as string, out providerId))
            {
                throw new Exception("Неверное значение входного параметра.");
            }

            var provider = providerService.CheckProviderExistence(providerId);

            // Список кодов организаций, принадлежавших данному поставщику
            List<int> providerOrganizationIdList = provider.Organizations.ToList().ConvertAll<int>(x => x.Id);

            IEnumerable<ProviderOrganization> providerOrganizationsOnThisPage;

            switch (mode)
            {
                case "includeprovider":
                    model.Title = "Организации поставщика";

                    var providerOrganizationList = provider.Organizations.Cast<ProviderOrganization>().OrderBy(x => x.ShortName).ToList<ProviderOrganization>();
                    providerOrganizationsOnThisPage = GridUtils.GetEntityRange(providerOrganizationList, state);

                    break;

                case "excludeprovider":
                    model.Title = "Организации, доступные для поставщиков";
                    deriveParams = new ParameterString("");
                    deriveParams.Add("Id", ParameterStringItem.OperationType.NotOneOf);
                    deriveParams["Id"].Value = providerOrganizationIdList.ToList().ConvertAll<string>(x => x.ToString());
                    state.Sort = "ShortName=Asc;";

                    providerOrganizationsOnThisPage = providerOrganizationService.GetFilteredList(state, deriveParams);
                    break;

                default:
                    throw new Exception("Неверное значение входного параметра.");
            }
            
            model.State = state;

            foreach (var item in providerOrganizationsOnThisPage)
            {
                var providerOrganization = item.As<ProviderOrganization>();

                string innString = "";
                switch (providerOrganization.EconomicAgent.As<EconomicAgent>().Type)
                {
                    case EconomicAgentType.JuridicalPerson:
                        JuridicalPerson juridicalPerson = providerOrganization.EconomicAgent.As<JuridicalPerson>();
                        innString = juridicalPerson.INN;
                        break;
                    case EconomicAgentType.PhysicalPerson:
                        PhysicalPerson physicalPerson = providerOrganization.EconomicAgent.As<PhysicalPerson>();
                        innString = physicalPerson.INN;
                        break;
                    default:
                        throw new Exception(String.Format("Обнаружена организация ({0}) неизвестного типа.", providerOrganization.Id));
                };

                model.AddRow(new GridRow(
                    new GridActionCell("Action", new GridActionCell.Action("Выбрать", "linkOrganizationSelect")),
                    new GridLabelCell("INN") { Value = innString },
                    new GridLabelCell("ShortName") { Value = providerOrganization.ShortName, Key = "organizationShortName" },
                    new GridLabelCell("LegalForm") { Value = providerOrganization.EconomicAgent.As<EconomicAgent>().LegalForm.Name },
                    new GridLabelCell("Address") { Value = providerOrganization.Address },
                    new GridHiddenCell("Id") { Value = providerOrganization.Id.ToString(), Key = "organizationId" }
                ));
            }

            model.GridPartialViewAction = "/Provider/ShowProviderOrganizationSelectGrid/";            

            return model;
        }

        #endregion

        #region Удаление поставщика

        public void Delete(int providerId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var provider = providerService.CheckProviderExistence(providerId);

                providerService.Delete(provider, user);

                uow.Commit();
            }
        }

        #endregion      
                
        #endregion
    }
}
