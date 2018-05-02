using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.Report.Report0008;
using OfficeOpenXml;
using System.Drawing;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class Report0008Presenter : BaseReportPresenter, IReport0008Presenter
    {
        #region Внутрение типы данных

        /// <summary>
        /// Тип группировки
        /// </summary>
        private enum GroupingType
        {
            /// <summary>
            /// По МХ
            /// </summary>
            [EnumDisplayName("Место хранения")]
            ByStorage = 1,

            /// <summary>
            /// По МХ-приемщику
            /// </summary>
            [EnumDisplayName("Место хранения - приемщик")]
            ByRecipientStorage = 2,

            /// <summary>
            /// По МХ-отправителю
            /// </summary>
            [EnumDisplayName("Место хранения - отправитель")]
            BySenderStorage = 3,

            /// <summary>
            /// По куратору накладной
            /// </summary>
            [EnumDisplayName("Куратор накладной")]
            ByCurator = 4,

            /// <summary>
            /// По клиенту
            /// </summary>
            [EnumDisplayName("Клиент")]
            ByClient = 5,

            /// <summary>
            /// По поставщикам
            /// </summary>
            [EnumDisplayName("Поставщик")]
            ByProvider = 6,
        }

        /// <summary>
        /// Модель данных для всех типов накладных. Поля которых нет в текущем типе накладной не заполняются.
        /// </summary>
        /// <remarks>Наследуется от модели-представления т.к. ничем от нее не отличается</remarks>
        public class WaybillDataModel : Report0008_WaybillItemViewModel { }

        /// <summary>
        /// Делегат для функции заполнения модели-представления из модели-данных
        /// </summary>
        /// <param name="table">Список моделей-представления</param>
        /// <param name="data">Список данных</param>
        /// <remarks>Подобные функции необходимы, т.к. несмотря на то что типы данных одни и те же, 
        /// в зависимости от типа накладной используются разные поля</remarks>
        private delegate void FillTableRows(IList<Report0008_WaybillItemViewModel> table, IEnumerable<WaybillDataModel> data);

        #endregion

        #region Поля

        private readonly IStorageService storageService;
        private readonly IClientService clientService;
        private readonly IProviderService providerService;

        private readonly IReceiptWaybillService receiptWaybillService;
        private readonly IMovementWaybillService movementWaybillService;
        private readonly IChangeOwnerWaybillService changeOwnerWaybillService;
        private readonly IWriteoffWaybillService writeoffWaybillService;
        private readonly IExpenditureWaybillService expenditureWaybillService;
        private readonly IReturnFromClientWaybillService returnFromClientWaybillService;

        private readonly IReceiptWaybillMainIndicatorService receiptWaybillIndicatorService;
        private readonly IMovementWaybillMainIndicatorService movementWaybillIndicatorService;
        private readonly IChangeOwnerWaybillMainIndicatorService changeOwnerWaybillIndicatorService;
        private readonly IWriteoffWaybillMainIndicatorService writeoffWaybillIndicatorService;
        private readonly IReturnFromClientWaybillMainIndicatorService returnFromClientWaybillIndicatorService;

        #endregion

        #region Конструктор

        public Report0008Presenter(IUnitOfWorkFactory unitOfWorkFactory, IUserService userService, IStorageService storageService, IClientService clientService, IProviderService providerService,
            IReceiptWaybillService receiptWaybillService, IMovementWaybillService movementWaybillService, IChangeOwnerWaybillService changeOwnerWaybillService,
            IWriteoffWaybillService writeoffWaybillService, IExpenditureWaybillService expenditureWaybillService, IReturnFromClientWaybillService returnFromClientWaybillService,
            IReceiptWaybillMainIndicatorService receiptWaybillIndicatorService, IMovementWaybillMainIndicatorService movementWaybillIndicatorService, 
            IChangeOwnerWaybillMainIndicatorService changeOwnerWaybillIndicatorService, IWriteoffWaybillMainIndicatorService writeoffWaybillIndicatorService, 
            IReturnFromClientWaybillMainIndicatorService returnFromClientWaybillIndicatorService) 
            : base(unitOfWorkFactory, userService)
        {
            this.storageService = storageService;
            this.clientService = clientService;
            this.providerService = providerService;

            this.receiptWaybillService = receiptWaybillService;
            this.movementWaybillService = movementWaybillService;
            this.changeOwnerWaybillService = changeOwnerWaybillService;
            this.writeoffWaybillService = writeoffWaybillService;
            this.expenditureWaybillService = expenditureWaybillService;
            this.returnFromClientWaybillService = returnFromClientWaybillService;

            this.receiptWaybillIndicatorService = receiptWaybillIndicatorService;
            this.movementWaybillIndicatorService = movementWaybillIndicatorService;
            this.changeOwnerWaybillIndicatorService = changeOwnerWaybillIndicatorService;
            this.writeoffWaybillIndicatorService = writeoffWaybillIndicatorService;
            this.returnFromClientWaybillIndicatorService = returnFromClientWaybillIndicatorService;
        }

        #endregion

        #region Методы

        #region Настройка отчета

        /// <summary>
        /// Настройка отчета
        /// </summary>
        public Report0008SettingsViewModel Report0008Settings(string backUrl, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Report0008_View);

                var currentDate = DateTimeUtils.GetCurrentDateTime();

                if (!user.HasPermission(Permission.ReceiptWaybill_List_Details) && !user.HasPermission(Permission.MovementWaybill_List_Details) &&
                    !user.HasPermission(Permission.ChangeOwnerWaybill_List_Details) && !user.HasPermission(Permission.WriteoffWaybill_List_Details) &&
                    !user.HasPermission(Permission.ExpenditureWaybill_List_Details) && !user.HasPermission(Permission.ReturnFromClientWaybill_List_Details))
                {
                    throw new Exception("Недостаточно прав на просмотр накладных.");
                }
                
                var model = new Report0008SettingsViewModel()
                {
                    BackURL = backUrl,

                    StartDate = new DateTime(currentDate.Year, currentDate.Month, 1).ToShortDateString(),
                    EndDate = currentDate.ToShortDateString(),

                    WaybillTypeList = GetWaybillTypeList(user),
                    ShowAdditionInfo = "1", 
                    ExcludeDivergences = "0",
                    SortDateTypeList = GetWaybillSortDateTypeList((WaybillType?)null, (int?)null, true),
                    WaybillOptionList = GetWaybillOptionList((WaybillType?)null, (WaybillDateType?)null, true),
                    GroupByCollection = new List<SelectListItem>(),
                    DateTypeList = new List<SelectListItem>(),
                    PriorToDate = currentDate.ToShortDateString(),
                    StorageList = storageService.GetList(user, Permission.Report0008_Storage_List).OrderBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name),
                    CuratorList = userService.GetList(user).OrderBy(x => x.DisplayName).ToDictionary( x => x.Id.ToString(), x => x.DisplayName)
                };

                return model;
            }
        }

        /// <summary>
        /// Получение списка доступных типов накладных
        /// </summary>
        /// <param name="user">Пользователь</param>
        private IEnumerable<SelectListItem> GetWaybillTypeList(User user)
        {
            return ComboBoxBuilder.GetComboBoxItemList<WaybillType>(true, false);
        }

        /// <summary>
        /// Получение списка возможных вариантов задания статусов накладных для вывода в отчет
        /// </summary>
        public object GetWaybillOptionList(string waybillTypeId, string dateTypeId, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                object result;

                var user = userService.CheckUserExistence(currentUser.Id);
                ValidationUtils.NotNull(waybillTypeId, "Неверное значение входного параметра.");
                ValidationUtils.NotNull(dateTypeId, "Неверное значение входного параметра.");

                if (!String.IsNullOrEmpty(waybillTypeId) && !String.IsNullOrEmpty(dateTypeId))
                {
                    var value = ValidationUtils.TryGetEnum<WaybillType>(waybillTypeId, "Неверное значение типа накладной.");
                    var dateType = ValidationUtils.TryGetEnum<WaybillDateType>(dateTypeId, "Неверное значение поля «В диапазон должна попадать».");

                    var list = GetWaybillOptionList(value, dateType, false);

                    var selectedOption = "";
                    if (list.Count() == 1)
                        selectedOption = list.First().Value;

                    result = new { List = list, SelectedOption = selectedOption };
                }
                else
                {
                    result = new { List = GetWaybillOptionList((WaybillType?)null, (WaybillDateType?)null, false), SelectedOption = "" };
                }

                return result;
            }
        }

        /// <summary>
        /// Получение списка возможных вариантов задания сортировки для вывода в отчет
        /// </summary>
        /// <param name="waybillTypeId">Код типа накландой</param>
        /// <param name="waybillOptionId">Значение поля «Выводить накладные»</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Список возможных вариантов задания сортировки</returns>
        public object GetWaybillSortDateTypeList(string waybillTypeId, string waybillOptionId, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                object result;

                var user = userService.CheckUserExistence(currentUser.Id);
                ValidationUtils.NotNull(waybillTypeId, "Неверное значение входного параметра.");
                ValidationUtils.NotNull(waybillOptionId, "Неверное значение входного параметра.");

                if (!String.IsNullOrEmpty(waybillTypeId) && !String.IsNullOrEmpty(waybillOptionId))
                {
                    var waybillType = ValidationUtils.TryGetEnum<WaybillType>(waybillTypeId, "Неверное значение типа накладной.");

                    var logicState = ValidationUtils.TryGetInt(waybillOptionId, "Неверное значение поля «Выводить накладные».");

                    var list = GetWaybillSortDateTypeList(waybillType, logicState, false);

                    var selectedOption = "";
                    if (list.Count() == 1)
                        selectedOption = list.First().Value;

                    result = new { List = list, SelectedOption = selectedOption };
                }
                else
                {
                    result = new { List = GetWaybillSortDateTypeList((WaybillType?)null, (int?)null, false), SelectedOption = "" };
                }

                return result;
            }
        }

        /// <summary>
        /// Получение списка возможных группировок
        /// </summary>
        public object GetWaybillGroupingTypeList(string waybillTypeId, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                ValidationUtils.NotNull(waybillTypeId, "Неверное значение входного параметра.");

                var groupingTypeList = new List<GroupingType>();
                var waybillType = ValidationUtils.TryGetEnum<WaybillType>(waybillTypeId, "Неверное значение типа накладной.");

                //общие группировки для всех типов накладных            
                groupingTypeList.Add(GroupingType.ByCurator);

                //группировки в зависимости от типа накладных
                switch (waybillType)
                {
                    case WaybillType.ReceiptWaybill:
                        groupingTypeList.Add(GroupingType.ByRecipientStorage);
                        groupingTypeList.Add(GroupingType.ByProvider);
                        break;
                    case WaybillType.MovementWaybill:
                        groupingTypeList.Add(GroupingType.ByRecipientStorage);
                        groupingTypeList.Add(GroupingType.BySenderStorage);
                        break;
                    case WaybillType.WriteoffWaybill:
                        groupingTypeList.Add(GroupingType.ByStorage);
                        break;
                    case WaybillType.ExpenditureWaybill:
                        groupingTypeList.Add(GroupingType.BySenderStorage);
                        groupingTypeList.Add(GroupingType.ByClient);
                        break;
                    case WaybillType.ChangeOwnerWaybill:
                        groupingTypeList.Add(GroupingType.ByStorage);
                        break;
                    case WaybillType.ReturnFromClientWaybill:
                        groupingTypeList.Add(GroupingType.ByRecipientStorage);
                        groupingTypeList.Add(GroupingType.ByClient);
                        break;
                }

                return new
                {
                    List = ComboBoxBuilder.GetComboBoxItemList<GroupingType>(groupingTypeList, x => x.GetDisplayName(), x => x.ValueToString(),
                                                            false, false),
                    SelectedOption = "1"
                };
            }
        }

        /// <summary>
        /// Получение списка возможных типов дат
        /// </summary>
        public object GetWaybillDateTypeList(string waybillTypeId, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                ValidationUtils.NotNull(waybillTypeId, "Неверное значение входного параметра.");

                var dateTypeList = new List<WaybillDateType>();

                //Если выбрали пустую строчку в комбобоксе то отправляем пустой список
                if (String.IsNullOrEmpty(waybillTypeId))
                {
                    return new
                    {
                        List = ComboBoxBuilder.GetComboBoxItemList<WaybillDateType>(dateTypeList, x => x.GetDisplayName(), x => x.ValueToString(),
                                                   false, false),
                        SelectedOption = ""
                    };
                }

                var waybillType = ValidationUtils.TryGetEnum<WaybillType>(waybillTypeId, "Неверное значение типа накладной.");

                //общие типы дат для всех типов накладных
                dateTypeList.Add(WaybillDateType.Date);
                dateTypeList.Add(WaybillDateType.AcceptanceDate);

                //типы дат в зависимости от типа накладной
                switch (waybillType)
                {
                    case WaybillType.ReceiptWaybill:
                        dateTypeList.Add(WaybillDateType.ReceiptDate);
                        dateTypeList.Add(WaybillDateType.ApprovementDate);
                        break;
                    case WaybillType.MovementWaybill:
                        dateTypeList.Add(WaybillDateType.ShippingDate);
                        dateTypeList.Add(WaybillDateType.ReceiptDate);
                        break;
                    case WaybillType.WriteoffWaybill:
                        dateTypeList.Add(WaybillDateType.WriteoffDate);
                        break;
                    case WaybillType.ExpenditureWaybill:
                        dateTypeList.Add(WaybillDateType.ShippingDate);
                        break;
                    case WaybillType.ChangeOwnerWaybill:
                        dateTypeList.Add(WaybillDateType.ChangeOwnerDate);
                        break;
                    case WaybillType.ReturnFromClientWaybill:
                        dateTypeList.Add(WaybillDateType.ReceiptDate);
                        break;
                    default:
                        break;
                }

                return new
                {
                    List = ComboBoxBuilder.GetComboBoxItemList<WaybillDateType>(dateTypeList, x => x.GetDisplayName(), x => x.ValueToString(),
                                                   false, false),
                    SelectedOption = ""
                };
            }
        }


        /// <summary>
        /// Получение формы для выбора клиента
        /// </summary>
        public Report0008_ClientSelector GetClientSelector(UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return new Report0008_ClientSelector()
                {
                    ClientList = clientService.GetList(user).OrderBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name)
                };
            }
        }

        /// <summary>
        /// Получение формы для выбора поставщика
        /// </summary>
        public Report0008_ProviderSelectorViewModel GetProviderSelector(UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return new Report0008_ProviderSelectorViewModel
                {
                    ProviderList = providerService.GetList(user).OrderBy(x => x.Name).ToDictionary(x => x.Id.ToString(), x => x.Name)
                };
            }
        }

        #region Формирование списка доступных настроек вывода накладных по статусам

        /// <summary>
        /// Формирование списка доступных настроек вывода накладных по статусам
        /// </summary>
        /// <param name="type">Тип накладной</param>
        private IEnumerable<SelectListItem> GetWaybillOptionList(WaybillType? type, WaybillDateType? dateType, bool addEmptyItem)
        {
            if (type.HasValue && dateType.HasValue)
            {
                switch (type.Value)
                {
                    case WaybillType.ReceiptWaybill:
                        return GetReceiptWaybillOptionList(dateType.Value, addEmptyItem);

                    case WaybillType.MovementWaybill:
                        return GetMovementWaybillOptionList(dateType.Value, addEmptyItem);

                    case WaybillType.ChangeOwnerWaybill:
                        return GetChangeOwnerWaybillOptionList(dateType.Value, addEmptyItem);

                    case WaybillType.WriteoffWaybill:
                        return GetWriteoffWaybillOptionList(dateType.Value, addEmptyItem);

                    case WaybillType.ExpenditureWaybill:
                        return GetExpenditureWaybillOptionList(dateType.Value, addEmptyItem);

                    case WaybillType.ReturnFromClientWaybill:
                        return GetReturnFromClientWaybillOptionList(dateType.Value, addEmptyItem);

                    default:
                        throw new Exception("Неверный тип накладной.");
                }
            }

            return new List<SelectListItem>();
        }

        private IEnumerable<SelectListItem> GetReceiptWaybillOptionList(WaybillDateType dateType, bool addEmptyItem)
        {
            var logicStateList = new List<ReceiptWaybillLogicState>();

            switch (dateType)
            {
                case WaybillDateType.Date:
                    logicStateList.Add(ReceiptWaybillLogicState.All);
                    logicStateList.Add(ReceiptWaybillLogicState.NotAccepted);
                    logicStateList.Add(ReceiptWaybillLogicState.Accepted);
                    logicStateList.Add(ReceiptWaybillLogicState.AcceptedNotApproved);
                    logicStateList.Add(ReceiptWaybillLogicState.AcceptedNotReceipted);
                    logicStateList.Add(ReceiptWaybillLogicState.NotReceipted);
                    logicStateList.Add(ReceiptWaybillLogicState.Receipted);
                    logicStateList.Add(ReceiptWaybillLogicState.ReceiptedNotApproved);
                    logicStateList.Add(ReceiptWaybillLogicState.NotApproved);
                    logicStateList.Add(ReceiptWaybillLogicState.Approved);
                    break;
                case WaybillDateType.AcceptanceDate:
                    logicStateList.Add(ReceiptWaybillLogicState.Accepted);
                    logicStateList.Add(ReceiptWaybillLogicState.AcceptedNotApproved);
                    logicStateList.Add(ReceiptWaybillLogicState.AcceptedNotReceipted);
                    logicStateList.Add(ReceiptWaybillLogicState.Receipted);
                    logicStateList.Add(ReceiptWaybillLogicState.ReceiptedNotApproved);
                    logicStateList.Add(ReceiptWaybillLogicState.Approved);
                    break;
                case WaybillDateType.ReceiptDate:
                    logicStateList.Add(ReceiptWaybillLogicState.Receipted);
                    logicStateList.Add(ReceiptWaybillLogicState.ReceiptedNotApproved);
                    logicStateList.Add(ReceiptWaybillLogicState.Approved);
                    break;
                case WaybillDateType.ApprovementDate:
                    logicStateList.Add(ReceiptWaybillLogicState.Approved);
                    break;
                default:
                    throw new Exception("Неверное значение поля «В диапазон должна попадать»");
            }

            return ComboBoxBuilder.GetComboBoxItemList<ReceiptWaybillLogicState>(logicStateList, x => x.GetDisplayName(), x => x.ValueToString(),
                                               addEmptyItem, false);
        }

        private IEnumerable<SelectListItem> GetMovementWaybillOptionList(WaybillDateType dateType, bool addEmptyItem)
        {
            var logicStateList = new List<MovementWaybillLogicState>();

            switch (dateType)
            {
                case WaybillDateType.Date:
                    logicStateList.Add(MovementWaybillLogicState.All);
                    logicStateList.Add(MovementWaybillLogicState.NotAccepted);
                    logicStateList.Add(MovementWaybillLogicState.Accepted);
                    logicStateList.Add(MovementWaybillLogicState.AcceptedNotShipped);
                    logicStateList.Add(MovementWaybillLogicState.AcceptedNotReceipted);
                    logicStateList.Add(MovementWaybillLogicState.NotShipped);
                    logicStateList.Add(MovementWaybillLogicState.Shipped);
                    logicStateList.Add(MovementWaybillLogicState.ShippedNotReceipted);
                    logicStateList.Add(MovementWaybillLogicState.NotReceipted);
                    logicStateList.Add(MovementWaybillLogicState.Receipted);
                    break;
                case WaybillDateType.AcceptanceDate:
                    logicStateList.Add(MovementWaybillLogicState.Accepted);
                    logicStateList.Add(MovementWaybillLogicState.AcceptedNotShipped);
                    logicStateList.Add(MovementWaybillLogicState.AcceptedNotReceipted);
                    logicStateList.Add(MovementWaybillLogicState.Shipped);
                    logicStateList.Add(MovementWaybillLogicState.ShippedNotReceipted);
                    logicStateList.Add(MovementWaybillLogicState.Receipted);
                    break;
                case WaybillDateType.ShippingDate:
                    logicStateList.Add(MovementWaybillLogicState.Shipped);
                    logicStateList.Add(MovementWaybillLogicState.ShippedNotReceipted);
                    logicStateList.Add(MovementWaybillLogicState.Receipted);
                    break;
                case WaybillDateType.ReceiptDate:
                    logicStateList.Add(MovementWaybillLogicState.Receipted);
                    break;
                default:
                    throw new Exception("Неверное значение поля «В диапазон должна попадать»");
            }

            return ComboBoxBuilder.GetComboBoxItemList<MovementWaybillLogicState>(logicStateList, x => x.GetDisplayName(), x => x.ValueToString(),
                                               addEmptyItem, false);
        }

        private IEnumerable<SelectListItem> GetChangeOwnerWaybillOptionList(WaybillDateType dateType, bool addEmptyItem)
        {
            var logicStateList = new List<ChangeOwnerWaybillLogicState>();

            switch (dateType)
            {
                case WaybillDateType.Date:
                    logicStateList.Add(ChangeOwnerWaybillLogicState.All);
                    logicStateList.Add(ChangeOwnerWaybillLogicState.ExceptNotAccepted);
                    logicStateList.Add(ChangeOwnerWaybillLogicState.NotAccepted);
                    logicStateList.Add(ChangeOwnerWaybillLogicState.Accepted);
                    break;
                case WaybillDateType.AcceptanceDate:
                    logicStateList.Add(ChangeOwnerWaybillLogicState.Accepted);
                    break;
                case WaybillDateType.ChangeOwnerDate:
                    logicStateList.Add(ChangeOwnerWaybillLogicState.Accepted);
                    break;
                default:
                    throw new Exception("Неверное значение поля «В диапазон должна попадать»");
            }

            return ComboBoxBuilder.GetComboBoxItemList<ChangeOwnerWaybillLogicState>(logicStateList, x => x.GetDisplayName(), x => x.ValueToString(),
                                               addEmptyItem, false);
        }

        private IEnumerable<SelectListItem> GetWriteoffWaybillOptionList(WaybillDateType dateType, bool addEmptyItem)
        {
            var logicStateList = new List<WriteoffWaybillLogicState>();

            switch (dateType)
            {
                case WaybillDateType.Date:
                    logicStateList.Add(WriteoffWaybillLogicState.All);
                    logicStateList.Add(WriteoffWaybillLogicState.ExceptNotAccepted);
                    logicStateList.Add(WriteoffWaybillLogicState.NotAccepted);
                    logicStateList.Add(WriteoffWaybillLogicState.ReadyToWriteoff);
                    logicStateList.Add(WriteoffWaybillLogicState.Writtenoff);
                    break;
                case WaybillDateType.AcceptanceDate:
                    logicStateList.Add(WriteoffWaybillLogicState.ReadyToWriteoff);
                    logicStateList.Add(WriteoffWaybillLogicState.Writtenoff);
                    break;
                case WaybillDateType.WriteoffDate:
                    logicStateList.Add(WriteoffWaybillLogicState.Writtenoff);
                    break;
                default:
                    throw new Exception("Неверное значение поля «В диапазон должна попадать»");
            }

            return ComboBoxBuilder.GetComboBoxItemList<WriteoffWaybillLogicState>(logicStateList, x => x.GetDisplayName(), x => x.ValueToString(),
                                               addEmptyItem, false);
        }

        private IEnumerable<SelectListItem> GetExpenditureWaybillOptionList(WaybillDateType dateType, bool addEmptyItem)
        {
            var logicStateList = new List<ExpenditureWaybillLogicState>();

            switch (dateType)
            {
                case WaybillDateType.Date:
                    logicStateList.Add(ExpenditureWaybillLogicState.All);
                    logicStateList.Add(ExpenditureWaybillLogicState.NotAccepted);
                    logicStateList.Add(ExpenditureWaybillLogicState.Accepted);
                    logicStateList.Add(ExpenditureWaybillLogicState.AcceptedNotShipped);
                    logicStateList.Add(ExpenditureWaybillLogicState.NotShipped);
                    logicStateList.Add(ExpenditureWaybillLogicState.Shipped);
                    break;
                case WaybillDateType.AcceptanceDate:
                    logicStateList.Add(ExpenditureWaybillLogicState.Accepted);
                    logicStateList.Add(ExpenditureWaybillLogicState.AcceptedNotShipped);
                    logicStateList.Add(ExpenditureWaybillLogicState.Shipped);
                    break;
                case WaybillDateType.ShippingDate:
                    logicStateList.Add(ExpenditureWaybillLogicState.Shipped);
                    break;
                default:
                    throw new Exception("Неверное значение поля «В диапазон должна попадать»");
            }

            return ComboBoxBuilder.GetComboBoxItemList<ExpenditureWaybillLogicState>(logicStateList, x => x.GetDisplayName(), x => x.ValueToString(),
                                               addEmptyItem, false);
        }

        private IEnumerable<SelectListItem> GetReturnFromClientWaybillOptionList(WaybillDateType dateType, bool addEmptyItem)
        {
            var logicStateList = new List<ReturnFromClientWaybillLogicState>();

            switch (dateType)
            {
                case WaybillDateType.Date:
                    logicStateList.Add(ReturnFromClientWaybillLogicState.All);
                    logicStateList.Add(ReturnFromClientWaybillLogicState.ExceptNotAccepted);
                    logicStateList.Add(ReturnFromClientWaybillLogicState.NotAccepted);
                    logicStateList.Add(ReturnFromClientWaybillLogicState.Accepted);
                    logicStateList.Add(ReturnFromClientWaybillLogicState.Receipted);
                    break;
                case WaybillDateType.AcceptanceDate:
                    logicStateList.Add(ReturnFromClientWaybillLogicState.Accepted);
                    logicStateList.Add(ReturnFromClientWaybillLogicState.Receipted);
                    break;
                case WaybillDateType.ReceiptDate:
                    logicStateList.Add(ReturnFromClientWaybillLogicState.Receipted);
                    break;
                default:
                    throw new Exception("Неверное значение поля «В диапазон должна попадать»");
            }

            return ComboBoxBuilder.GetComboBoxItemList<ReturnFromClientWaybillLogicState>(logicStateList, x => x.GetDisplayName(), x => x.ValueToString(),
                                               addEmptyItem, false);
        }

        #endregion

        #region Формирование списка доступных для сортировки дат

        /// <summary>
        /// Формирование списка доступных для сортировки дат
        /// </summary>
        /// <param name="type">Тип накладной</param>
        /// <param name="logicState">Статус накладной</param>
        private IEnumerable<SelectListItem> GetWaybillSortDateTypeList(WaybillType? type, int? logicState, bool addEmptyItem)
        {
            if (type.HasValue && logicState.HasValue)
            {
                switch (type.Value)
                {
                    case WaybillType.ReceiptWaybill:
                        return GetReceiptWaybillSortDateTypeList((ReceiptWaybillLogicState)logicState.Value, addEmptyItem);

                    case WaybillType.MovementWaybill:
                        return GetMovementWaybillSortDateTypeList((MovementWaybillLogicState)logicState.Value, addEmptyItem);

                    case WaybillType.ChangeOwnerWaybill:
                        return GetChangeOwnerWaybillSortDateTypeList((ChangeOwnerWaybillLogicState)logicState.Value, addEmptyItem);

                    case WaybillType.WriteoffWaybill:
                        return GetWriteoffWaybillSortDateTypeList((WriteoffWaybillLogicState)logicState.Value, addEmptyItem);

                    case WaybillType.ExpenditureWaybill:
                        return GetExpenditureWaybillSortDateTypeList((ExpenditureWaybillLogicState)logicState.Value, addEmptyItem);

                    case WaybillType.ReturnFromClientWaybill:
                        return GetReturnFromClientWaybillSortDateTypeList((ReturnFromClientWaybillLogicState)logicState.Value, addEmptyItem);

                    default:
                        throw new Exception("Неверный тип накладной.");
                }
            }

            return new List<SelectListItem>();
        }

        /// <summary>
        /// Формирование списка доступных сортировок по дате
        /// </summary>
        /// <param name="logicState">Статус накладной</param>
        /// <param name="addEmptyItem">Признак добавления пустого элемента</param>
        /// <returns>Список элементов - возможных сортировок</returns>
        private IEnumerable<SelectListItem> GetReceiptWaybillSortDateTypeList(ReceiptWaybillLogicState logicState, bool addEmptyItem)
        {
            var sortDateTypeList = new List<WaybillDateType>();

            switch (logicState)
            {
                case ReceiptWaybillLogicState.All:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    sortDateTypeList.Add(WaybillDateType.ReceiptDate);
                    sortDateTypeList.Add(WaybillDateType.ApprovementDate);
                    break;
                case ReceiptWaybillLogicState.NotAccepted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    break;
                case ReceiptWaybillLogicState.Accepted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    break;
                case ReceiptWaybillLogicState.AcceptedNotApproved:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    break;
                case ReceiptWaybillLogicState.AcceptedNotReceipted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    break;
                case ReceiptWaybillLogicState.NotReceipted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    break;
                case ReceiptWaybillLogicState.Receipted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    sortDateTypeList.Add(WaybillDateType.ReceiptDate);
                    break;
                case ReceiptWaybillLogicState.ReceiptedNotApproved:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    sortDateTypeList.Add(WaybillDateType.ReceiptDate);
                    break;
                case ReceiptWaybillLogicState.NotApproved:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    break;
                case ReceiptWaybillLogicState.Approved:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    sortDateTypeList.Add(WaybillDateType.ReceiptDate);
                    sortDateTypeList.Add(WaybillDateType.ApprovementDate);
                    break;      
                default:
                    throw new Exception("Неверное значение поля «Выводить накладные»");
            }

            return ComboBoxBuilder.GetComboBoxItemList<WaybillDateType>(sortDateTypeList, x => x.GetDisplayName(), x => x.ValueToString(),
                                               addEmptyItem, false);
        }

        /// <summary>
        /// Формирование списка доступных сортировок по дате
        /// </summary>
        /// <param name="logicState">Статус накладной</param>
        /// <param name="addEmptyItem">Признак добавления пустого элемента</param>
        /// <returns>Список элементов - возможных сортировок</returns>
        private IEnumerable<SelectListItem> GetMovementWaybillSortDateTypeList(MovementWaybillLogicState logicState, bool addEmptyItem)
        {
            var sortDateTypeList = new List<WaybillDateType>();

            switch (logicState)
            {
                case MovementWaybillLogicState.All:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    sortDateTypeList.Add(WaybillDateType.ShippingDate);
                    sortDateTypeList.Add(WaybillDateType.ReceiptDate);
                    break;
                case MovementWaybillLogicState.NotAccepted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    break;
                case MovementWaybillLogicState.Accepted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    break;
                case MovementWaybillLogicState.AcceptedNotShipped:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    break;
                case MovementWaybillLogicState.AcceptedNotReceipted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    break;
                case MovementWaybillLogicState.NotShipped:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    break;
                case MovementWaybillLogicState.Shipped:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    sortDateTypeList.Add(WaybillDateType.ShippingDate);
                    break;
                case MovementWaybillLogicState.ShippedNotReceipted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    sortDateTypeList.Add(WaybillDateType.ShippingDate);
                    break;
                case MovementWaybillLogicState.NotReceipted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    break;
                case MovementWaybillLogicState.Receipted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    sortDateTypeList.Add(WaybillDateType.ShippingDate);
                    sortDateTypeList.Add(WaybillDateType.ReceiptDate);
                    break;
                default:
                    throw new Exception("Неверное значение поля «Выводить накладные»");
            }

            return ComboBoxBuilder.GetComboBoxItemList<WaybillDateType>(sortDateTypeList, x => x.GetDisplayName(), x => x.ValueToString(),
                                               addEmptyItem, false);
        }

        /// <summary>
        /// Формирование списка доступных сортировок по дате
        /// </summary>
        /// <param name="logicState">Статус накладной</param>
        /// <param name="addEmptyItem">Признак добавления пустого элемента</param>
        /// <returns>Список элементов - возможных сортировок</returns>
        private IEnumerable<SelectListItem> GetChangeOwnerWaybillSortDateTypeList(ChangeOwnerWaybillLogicState logicState, bool addEmptyItem)
        {
            var sortDateTypeList = new List<WaybillDateType>();

            switch (logicState)
            {
                case ChangeOwnerWaybillLogicState.All:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    sortDateTypeList.Add(WaybillDateType.ChangeOwnerDate);
                    break;
                case ChangeOwnerWaybillLogicState.ExceptNotAccepted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    break;
                case ChangeOwnerWaybillLogicState.NotAccepted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    break;
                case ChangeOwnerWaybillLogicState.Accepted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    sortDateTypeList.Add(WaybillDateType.ChangeOwnerDate);
                    break;
                default:
                    throw new Exception("Неверное значение поля «Выводить накладные»");
            }

            return ComboBoxBuilder.GetComboBoxItemList<WaybillDateType>(sortDateTypeList, x => x.GetDisplayName(), x => x.ValueToString(),
                                               addEmptyItem, false);
        }

        /// <summary>
        /// Формирование списка доступных сортировок по дате
        /// </summary>
        /// <param name="logicState">Статус накладной</param>
        /// <param name="addEmptyItem">Признак добавления пустого элемента</param>
        /// <returns>Список элементов - возможных сортировок</returns>
        private IEnumerable<SelectListItem> GetWriteoffWaybillSortDateTypeList(WriteoffWaybillLogicState logicState, bool addEmptyItem)
        {
            var sortDateTypeList = new List<WaybillDateType>();

            switch (logicState)
            {
                case WriteoffWaybillLogicState.All:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    sortDateTypeList.Add(WaybillDateType.WriteoffDate);
                    break;
                case WriteoffWaybillLogicState.ExceptNotAccepted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    break;
                case WriteoffWaybillLogicState.NotAccepted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    break;
                case WriteoffWaybillLogicState.ReadyToWriteoff:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    break;
                case WriteoffWaybillLogicState.Writtenoff:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    sortDateTypeList.Add(WaybillDateType.WriteoffDate);
                    break;
                default:
                    throw new Exception("Неверное значение поля «Выводить накладные»");
            }

            return ComboBoxBuilder.GetComboBoxItemList<WaybillDateType>(sortDateTypeList, x => x.GetDisplayName(), x => x.ValueToString(),
                                               addEmptyItem, false);
        }

        /// <summary>
        /// Формирование списка доступных сортировок по дате
        /// </summary>
        /// <param name="logicState">Статус накладной</param>
        /// <param name="addEmptyItem">Признак добавления пустого элемента</param>
        /// <returns>Список элементов - возможных сортировок</returns>
        private IEnumerable<SelectListItem> GetExpenditureWaybillSortDateTypeList(ExpenditureWaybillLogicState logicState, bool addEmptyItem)
        {
            var sortDateTypeList = new List<WaybillDateType>();

            switch (logicState)
            {
                case ExpenditureWaybillLogicState.All:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    sortDateTypeList.Add(WaybillDateType.ShippingDate);
                    break;
                case ExpenditureWaybillLogicState.NotAccepted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    break;
                case ExpenditureWaybillLogicState.Accepted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    break;
                case ExpenditureWaybillLogicState.AcceptedNotShipped:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    break;
                case ExpenditureWaybillLogicState.NotShipped:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    break;
                case ExpenditureWaybillLogicState.Shipped:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    sortDateTypeList.Add(WaybillDateType.ShippingDate);
                    break;
                default:
                    throw new Exception("Неверное значение поля «Выводить накладные»");
            }

            return ComboBoxBuilder.GetComboBoxItemList<WaybillDateType>(sortDateTypeList, x => x.GetDisplayName(), x => x.ValueToString(),
                                               addEmptyItem, false);
        }

        /// <summary>
        /// Формирование списка доступных сортировок по дате
        /// </summary>
        /// <param name="logicState">Статус накладной</param>
        /// <param name="addEmptyItem">Признак добавления пустого элемента</param>
        /// <returns>Список элементов - возможных сортировок</returns>
        private IEnumerable<SelectListItem> GetReturnFromClientWaybillSortDateTypeList(ReturnFromClientWaybillLogicState logicState, bool addEmptyItem)
        {
            var sortDateTypeList = new List<WaybillDateType>();

            switch (logicState)
            {
                case ReturnFromClientWaybillLogicState.All:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    sortDateTypeList.Add(WaybillDateType.ReceiptDate);
                    break;
                case ReturnFromClientWaybillLogicState.ExceptNotAccepted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    break;
                case ReturnFromClientWaybillLogicState.NotAccepted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    break;
                case ReturnFromClientWaybillLogicState.Accepted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    break;
                case ReturnFromClientWaybillLogicState.Receipted:
                    sortDateTypeList.Add(WaybillDateType.Date);
                    sortDateTypeList.Add(WaybillDateType.AcceptanceDate);
                    sortDateTypeList.Add(WaybillDateType.ReceiptDate);
                    break;
                default:
                    throw new Exception("Неверное значение поля «Выводить накладные»");
            }

            return ComboBoxBuilder.GetComboBoxItemList<WaybillDateType>(sortDateTypeList, x => x.GetDisplayName(), x => x.ValueToString(),
                                               addEmptyItem, false);
        }

        #endregion

        #endregion

        #region Построение отчета

        /// <summary>
        /// Построение отчета
        /// </summary>
        /// <param name="settings">Настройки отчета</param>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <returns>Модель отчета</returns>
        public Report0008ViewModel Report0008(Report0008SettingsViewModel settings, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                ValidationUtils.NotNull(settings, "Неверно задан входной параметр.");

                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.Report0008_View);   //Проверяем право на построение отчета
                
                var currentDate = DateTimeUtils.GetCurrentDateTime();

                DateTime startDate, endDate;
                base.ParseDatePeriod(settings.StartDate, settings.EndDate, currentDate, out startDate, out endDate);

                var showAdditionInfo = ValidationUtils.TryGetBool(settings.ShowAdditionInfo);
                var excludeDivergences = ValidationUtils.TryGetBool(settings.ExcludeDivergences);
                
                var waybillType = ValidationUtils.TryGetEnum<WaybillType>(settings.WaybillTypeId, "Тип накладных указан неверно.");
                var waybillDateType = ValidationUtils.TryGetEnum<WaybillDateType>(settings.DateTypeId, "Тип даты указан неверно.");
                var sortDateType = ValidationUtils.TryGetEnum<WaybillDateType>(settings.SortDateTypeId, "Тип даты указан неверно.");
                
                ValidateSettingsConsistency(waybillType, waybillDateType);
                ValidationUtils.Assert(!((waybillType != WaybillType.ReceiptWaybill) && excludeDivergences), "Настройка «Учитывать расхождения» может использоваться только для накладныхс типом «Приходная накладная»");

                var stateId = settings.WaybillOptionId;
                DateTime? priorToDate = null;

                if (waybillDateType == WaybillDateType.Date && stateId != null && stateId != "" && stateId != "0")
                {
                    priorToDate = ValidationUtils.TryGetDate(settings.PriorToDate);
                    priorToDate = priorToDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                    ValidationUtils.Assert(priorToDate >= endDate, "Дата в поле «До даты» не может быть меньше даты окончания периода.");
                   
                }

                var allStorages = settings.AllStorages == "1";
                var storageIds = !allStorages ? GetIdList(settings.StorageIDs).Select(x=>ValidationUtils.TryGetShort(x)) : null;

                var allCurators = settings.AllCurators == "1";
                var curatorIds = !allCurators ? GetIdList(settings.CuratorIDs).Select(x => ValidationUtils.TryGetInt(x)) : null;
                         
                var model = new Report0008ViewModel()
                    {
                        CreatedBy = user.DisplayName,
                        CreationDate = currentDate.ToFullDateTimeString(),
                        StartDate = startDate.ToShortDateString(),
                        EndDate = endDate.ToShortDateString(),
                        ReportName = "Отчет «Реестр накладных»",
                        WaybillType = (int)waybillType,
                        WaybillTypeName = waybillType.GetDisplayName(),
                        DateTypeName = waybillDateType.GetDisplayName(),
                        SortDateTypeName = sortDateType.GetDisplayName(),
                        PriorToDateString = priorToDate.HasValue ? priorToDate.Value.ToShortDateString() : null
                    };

                switch (waybillType)
                {
                    case WaybillType.ReceiptWaybill:
                        user.CheckPermission(Permission.ReceiptWaybill_List_Details);

                        var providerIds = !(settings.AllProviders == "1") ? GetIdList(settings.ProviderIDs).Select(x => ValidationUtils.TryGetInt(x)) : null;

                        FillInReceiptWaybillTable(model, startDate, endDate, showAdditionInfo, storageIds, curatorIds, providerIds, stateId, settings.GroupByCollectionIDs, 
                            waybillDateType, priorToDate, excludeDivergences, sortDateType, user);
                        break;

                    case WaybillType.MovementWaybill:
                        user.CheckPermission(Permission.MovementWaybill_List_Details);
                        ValidateMovementWaybillGroupingConsistency(settings.GroupByCollectionIDs);
                        FillInMovementWaybillTable(model, startDate, endDate, showAdditionInfo, storageIds, curatorIds, stateId, settings.GroupByCollectionIDs, waybillDateType, 
                            priorToDate, sortDateType, user);
                        break;

                    case WaybillType.ChangeOwnerWaybill:
                        user.CheckPermission(Permission.ChangeOwnerWaybill_List_Details);
                        FillInChangeOwnerWaybillTable(model, startDate, endDate, showAdditionInfo, storageIds, curatorIds, stateId, settings.GroupByCollectionIDs, waybillDateType, 
                            priorToDate, sortDateType, user);
                        break;

                    case WaybillType.WriteoffWaybill:
                        user.CheckPermission(Permission.WriteoffWaybill_List_Details);
                        FillInWriteoffWaybillTable(model, startDate, endDate, showAdditionInfo, storageIds, curatorIds, stateId, settings.GroupByCollectionIDs, waybillDateType, 
                            priorToDate, sortDateType, user);
                        break;

                    case WaybillType.ExpenditureWaybill:
                        {
                            user.CheckPermission(Permission.ExpenditureWaybill_List_Details);

                            var clientIds = !(settings.AllClients == "1") ? GetIdList(settings.ClientIDs).Select(x => ValidationUtils.TryGetInt(x)) : null;

                            FillInExpenditureWaybillTable(model, startDate, endDate, showAdditionInfo, storageIds, curatorIds, clientIds, stateId, settings.GroupByCollectionIDs, 
                                waybillDateType, priorToDate, sortDateType, user);
                        }
                        break;

                    case WaybillType.ReturnFromClientWaybill:
                        {
                            user.CheckPermission(Permission.ReturnFromClientWaybill_List_Details);

                            var clientIds = !(settings.AllClients == "1")? GetIdList(settings.ClientIDs).Select(x => ValidationUtils.TryGetInt(x)) : null;

                            FillInReturnFromClientWaybillTable(model, startDate, endDate, showAdditionInfo, storageIds, curatorIds, clientIds, stateId, settings.GroupByCollectionIDs, 
                                waybillDateType, priorToDate, sortDateType, user);
                        }
                        break;

                    default:
                        throw new Exception("Неизвестный тип накладной.");
                }

                return model;
            }
        }

        #region Вспомогательные методы

        /// <summary>
        /// Проверка на согласованность типа даты и статусов накладной
        /// </summary>
        private void ValidateSettingsConsistency(WaybillType waybillType, WaybillDateType waybillDateType)
        {
            var errorMessage = String.Format("Тип «{0}» не согласуется с параметром «{1}».", waybillType.GetDisplayName(),
                                                            waybillDateType.GetDisplayName());
            switch (waybillDateType)
            {
                //первые два типа даты есть у всех накладных
                case WaybillDateType.Date:
                case WaybillDateType.AcceptanceDate:
                    break;
                case WaybillDateType.ReceiptDate:
                    switch (waybillType)
	                {
		                case WaybillType.ReceiptWaybill:
                        case WaybillType.MovementWaybill:
                        case WaybillType.ReturnFromClientWaybill:
                             break;
                        default:
                             throw new Exception(errorMessage);
	                }
                    break;
                case WaybillDateType.ApprovementDate:
                    ValidationUtils.Assert(waybillType == WaybillType.ReceiptWaybill, errorMessage);
                    break;
                case WaybillDateType.ShippingDate:
                    ValidationUtils.Assert(waybillType == WaybillType.ExpenditureWaybill || waybillType == WaybillType.MovementWaybill, errorMessage);
                    break;
                case WaybillDateType.ChangeOwnerDate:
                    ValidationUtils.Assert(waybillType == WaybillType.ChangeOwnerWaybill, errorMessage);
                    break;
                case WaybillDateType.WriteoffDate:
                    ValidationUtils.Assert(waybillType == WaybillType.WriteoffWaybill, errorMessage);
                    break;
                default:
                    throw new Exception("Данный тип даты не предусмотрен.");
            }
        }

        void ValidateMovementWaybillGroupingConsistency(string groupByCollectionIDs)
        {
            var list = StringUtils.GetShortIdList(groupByCollectionIDs);
            ValidationUtils.Assert(!(list.Contains((short)GroupingType.ByRecipientStorage) && list.Contains((short)GroupingType.BySenderStorage)),
                                   "Одновременная группировка по МХ-приемщика и МХ-отправителя невозможна.");
        }

        /// <summary>
        /// Получение коллекции ID из строки приходящей от клиента
        /// </summary>
        /// <param name="ids">строка с идентификаторами вида ID1_ID2_...IDN</param>
        /// <returns>Список ID</returns>
        private IEnumerable<string> GetIdList(string ids)
        {
            IList<string> list = new List<string>();

            if (!String.IsNullOrEmpty(ids))
            {
                var splitIds = ids.Split('_');

                foreach (var id in splitIds)
                {
                    ValidationUtils.TryGetShort(id);    // Провереям что строка является корректным числом
                    list.Add(id);
                }
            }

            return list;
        }

        /// <summary>
        /// Формирование перечня статусов накладной для вывода
        /// </summary>
        /// <param name="stateId">Код статуса для вывода</param>
        /// <param name="type">Тип накланой</param>
        private string GetStatesForOut(string stateId, WaybillType type)
        {
            var result = "";

            switch (type)
            {
                case WaybillType.ReceiptWaybill:
                    {
                        var state = ValidationUtils.TryGetEnum<ReceiptWaybillLogicState>(stateId);
                        result = state.GetDisplayName();
                    }
                    break;

                case WaybillType.MovementWaybill:
                    {
                        var state = ValidationUtils.TryGetEnum<MovementWaybillLogicState>(stateId);
                        result = state.GetDisplayName();
                    }
                    break;

                case WaybillType.ChangeOwnerWaybill:
                    {
                        var state = ValidationUtils.TryGetEnum<ChangeOwnerWaybillLogicState>(stateId);
                        result = state.GetDisplayName();
                    }
                    break;

                case WaybillType.WriteoffWaybill:
                    {
                        var state = ValidationUtils.TryGetEnum<WriteoffWaybillLogicState>(stateId);
                        result = state.GetDisplayName();
                    }
                    break;

                case WaybillType.ExpenditureWaybill:
                    {
                        var state = ValidationUtils.TryGetEnum<ExpenditureWaybillLogicState>(stateId);
                        result = state.GetDisplayName();
                    }
                    break;

                case WaybillType.ReturnFromClientWaybill:
                    {
                        var state = ValidationUtils.TryGetEnum<ReturnFromClientWaybillLogicState>(stateId);
                        result = state.GetDisplayName();
                    }
                    break;

                default:
                    throw new Exception("Неизвестный тип накладной.");
            }

            return result;
        }

        #endregion


        /// <summary>
        /// Заполнить модель-представление для таблицы.
        /// </summary>
        private IList<Report0008_WaybillItemViewModel> FillTableViewModel(IEnumerable<WaybillDataModel> data, List<int> groupFields,
            FillTableRows funcFillTableRows)
        {
            var viewModel = new List<Report0008_WaybillItemViewModel>();
            GroupByTableRow(viewModel, data, funcFillTableRows, groupFields, 1);
            return viewModel;
        }

        #region Группировка

        /// <summary>
        /// Сделать группировки. Рекурсивный спуск
        /// </summary>
        /// <param name="table">Список с моделями-представления</param>
        /// <param name="data">Список с моделями данных</param>
        /// <param name="funcFillTableRows">Функция заполнения модели-представления из модели-данных</param>
        /// <param name="groupFields">Список группировок</param>
        /// <param name="groupLevel">Текущий уровень группировки</param>
        private void GroupByTableRow(IList<Report0008_WaybillItemViewModel> table, IEnumerable<WaybillDataModel> data,
                FillTableRows funcFillTableRows, List<int> groupFields, int groupLevel)
        {
            if (groupFields.Count < groupLevel)
            {
                funcFillTableRows(table, data);
                return;
            }

            switch (groupFields[groupLevel - 1])
            {
                case (byte)GroupingType.ByStorage:
                    TableRowsGrouping<Storage>(data, table, x => x.StorageName, funcFillTableRows,
                       "Место хранения", groupFields, groupLevel);
                    break;
                case (byte)GroupingType.ByRecipientStorage:
                    TableRowsGrouping<Storage>(data, table, x => x.RecipientStorageName, funcFillTableRows,
                       "Место хранения - приемщик", groupFields, groupLevel);
                    break;
                case (byte)GroupingType.BySenderStorage:
                    TableRowsGrouping<Storage>(data, table, x => x.SenderStorageName, funcFillTableRows,
                       "Место хранения - отправитель", groupFields, groupLevel);
                    break;
                case (byte)GroupingType.ByCurator:
                    TableRowsGrouping<User>(data, table, x => x.CuratorName, funcFillTableRows,
                        "Куратор накладной", groupFields, groupLevel);
                    break;
                case (byte)GroupingType.ByClient:
                    TableRowsGrouping<User>(data, table, x => x.ClientName, funcFillTableRows,
                        "Клиент", groupFields, groupLevel);
                    break;
                case (byte)GroupingType.ByProvider:
                    TableRowsGrouping<Storage>(data, table, x => x.ProviderName, funcFillTableRows,
                        "Поставщик", groupFields, groupLevel);
                    break;
                default:
                    throw new Exception("Неизвестная группировка данных.");
            }

        }

        /// <summary>
        /// Группировка
        /// </summary>
        /// <typeparam name="TGroupingObject">Объект по которому группируем</typeparam>
        /// <param name="list">Данные</param>
        /// <param name="viewModel">Представление</param>
        /// <param name="groupingObjectName">Лямбда возвращающая поле по которому производится группировка</param>
        /// <param name="funcFillTableRows">Функция заполнения модели-представления из модели данных</param>
        /// <param name="title">Заголовок группировки</param>
        /// <param name="groupFields">Список группировок</param>
        /// <param name="groupLevel">Текущий уровень группировки</param>
        private void TableRowsGrouping<TGroupingObject>(IEnumerable<WaybillDataModel> list, IList<Report0008_WaybillItemViewModel> viewModel,
                Func<WaybillDataModel, string> groupingObjectName, FillTableRows funcFillTableRows, string title,
                List<int> groupFields, int groupLevel)
        {
            foreach (var group in list.GroupBy(x => groupingObjectName(x)).OrderBy(x => groupingObjectName(x.First())))
            {
                viewModel.Add(new Report0008_WaybillItemViewModel()
                {
                    IsGroup = true,
                    GroupTitle = String.Format("{0}: {1}", title, groupingObjectName(group.First())),
                    GroupLevel = groupLevel,

                    AccountingPriceSum = group.Any(x => x.AccountingPriceSum.HasValue) ? group.Sum(x => x.AccountingPriceSum) : null,
                    PurchaseCostSum =  group.Any(x => x.PurchaseCostSum.HasValue) ? group.Sum(x => x.PurchaseCostSum) : null,
                    SalePriceSum = group.Any(x => x.SalePriceSum.HasValue) ? group.Sum(x => x.SalePriceSum) : null,
                    ReturnFromClientSum = group.Any(x => x.ReturnFromClientSum.HasValue) ? group.Sum(x => x.ReturnFromClientSum) : null,
                    SenderAccountingPriceSum = group.Any(x => x.SenderAccountingPriceSum.HasValue) ? group.Sum(x => x.SenderAccountingPriceSum) : null,
                    RecipientAccountingPriceSum = group.Any(x => x.RecipientAccountingPriceSum.HasValue) ? 
                                                                group.Sum(x => x.RecipientAccountingPriceSum) : null
                }); // Добавляем заголовок группы

                GroupByTableRow(viewModel, group, funcFillTableRows, groupFields, groupLevel + 1);
            }
        }

        /// <summary>
        /// Получить список группировок
        /// </summary>
        /// <param name="groupByCollectionIds">Строка с Id группировок</param>
        private List<int> GetGroupFields(string groupByCollectionIds)
        {
            var result = new List<int>();
            if (String.IsNullOrEmpty(groupByCollectionIds))
                return result;

            foreach (var val in groupByCollectionIds.Split('_'))
            {
                var value = ValidationUtils.TryGetInt(val);
                var enumValue = ValidationUtils.TryGetEnum<GroupingType>(val, "Неверный код группировки.");
                result.Add(value);
            }

            return result;
        }

        #endregion

        #region Приход

        /// <summary>
        /// Заполнение таблицы по приходным накладным
        /// </summary>
        private void FillInReceiptWaybillTable(Report0008ViewModel model, DateTime startDate, DateTime endDate, bool showAdditionInfo, IEnumerable<short> storageIds, 
            IEnumerable<int> curatorIds, IEnumerable<int> providerIds, string stateId, string groupByCollectionIds, WaybillDateType dateType, DateTime? priorToDate, 
            bool excludeDivergences, WaybillDateType sortDateType, User user)
        {
            IEnumerable<ReceiptWaybill> list;
            var dataModel = new List<WaybillDataModel>();
            int pageNumber = 0;
            var logicState = ValidationUtils.TryGetEnum<ReceiptWaybillLogicState>(stateId);

            //итоговые суммы
            decimal? purchaseCostSumTotal = null;
            decimal? accountingPriceSumTotal = null;

            do
            {
                list = receiptWaybillService.GetList(logicState, storageIds, Permission.Report0008_Storage_List, curatorIds,
                    Permission.User_List_Details, providerIds, Permission.Provider_List_Details, startDate, endDate, ++pageNumber, dateType, priorToDate, user);

                list = SortReceiptWaybillList(list, sortDateType);

                FillInReceiptWaybillDataModelListByBatch(dataModel, list, showAdditionInfo, ref purchaseCostSumTotal, ref accountingPriceSumTotal, excludeDivergences, user);
                
            } while (list.Count() > 0);

            var groupFields = GetGroupFields(groupByCollectionIds);
            model.ReceiptWaybillModel.Rows = FillTableViewModel(dataModel, groupFields, FillReceiptWaybillTableRows);

            model.ReceiptWaybillModel.PurchaseCostSumTotal = purchaseCostSumTotal.ForDisplay(ValueDisplayType.Money);
            model.ReceiptWaybillModel.AccountingPriceSumTotal = accountingPriceSumTotal.ForDisplay(ValueDisplayType.Money);

            model.ReceiptWaybillModel.ShowAdditionInfo = showAdditionInfo;
            model.ShownStates = GetStatesForOut(stateId, WaybillType.ReceiptWaybill);
        }

       
        /// <summary>
        /// Заполнение модели-представления из модели данных
        /// </summary>
        private void FillReceiptWaybillTableRows(IList<Report0008_WaybillItemViewModel> table, IEnumerable<WaybillDataModel> data)
        {
            foreach (var row in data)
            {
                table.Add(new Report0008_WaybillItemViewModel()
                {
                    AccountingPriceSum = row.AccountingPriceSum,
                    Date = row.Date,
                    Number = row.Number,
                    ProviderName = row.ProviderName,
                    ProviderInvoice = row.ProviderInvoice,
                    ProviderWaybillName = row.ProviderWaybillName,
                    PurchaseCostSum = row.PurchaseCostSum,
                    RecipientAccountOrganizationName = row.RecipientAccountOrganizationName,
                    RecipientStorageName = row.RecipientStorageName,
                    StateName = row.StateName,
                    WaybillStateHistory = row.WaybillStateHistory,
                    Comment = row.Comment
                });
            }
        }

        /// <summary>
        /// Движение приходной накладной
        /// </summary>
        /// <param name="waybill"></param>
        /// <returns></returns>
        private string GetWaybillStateHistory(ReceiptWaybill waybill)
        {
            var result = "Проводка: " + waybill.AcceptanceDate.ForDisplay(true);
            result += ", Приемка: " + waybill.ReceiptDate.ForDisplay(true);

            if (waybill.AreDivergencesAfterReceipt)
            {
                result += ", Согласование: " + waybill.ApprovementDate.ForDisplay(true);
            }

            return result;
        }

        /// <summary>
        /// Обработка пакета приходных накладных
        /// </summary>
        /// <param name="purchaseCostSumTotal">Итоговая сумма в ЗЦ, та что пойдет в "Итого"</param>
        /// <param name="accountingPriceSumTotal">Итоговая сумма в УЦ</param>
        private void FillInReceiptWaybillDataModelListByBatch(IList<WaybillDataModel> dataModel, IEnumerable<ReceiptWaybill> list, bool showAdditionInfo, 
            ref decimal? purchaseCostSumTotal, ref decimal? accountingPriceSumTotal, bool excludeDivergences, User user)
        {
            foreach (var item in list)
            {
                decimal? accountingPriceSum = receiptWaybillIndicatorService.CalcAccountingPriceSum(item, user, excludeDivergences);
                decimal? purchaseCostSum = receiptWaybillIndicatorService.CalcPurchaseCostSum(item, user, excludeDivergences);

                var allowToViewAccountingPrice = user.HasPermissionToViewStorageAccountingPrices(item.ReceiptStorage);
                var allowToViewPurchaseCost = receiptWaybillService.IsPossibilityToViewPurchaseCosts(item, user);

                dataModel.Add(new WaybillDataModel()
                {
                    Number = item.Number,
                    Date = item.Date.ToShortDateString(),
                    PurchaseCostSum = allowToViewPurchaseCost ? purchaseCostSum : null,
                    AccountingPriceSum = allowToViewAccountingPrice? accountingPriceSum : null,
                    StateName = item.State.GetDisplayName(),
                    ProviderName = item.Contractor.Name,
                    RecipientStorageName = item.ReceiptStorage.Name,
                    RecipientAccountOrganizationName = item.AccountOrganization.ShortName,
                    ProviderWaybillName = item.ProviderWaybillName,
                    ProviderInvoice = item.ProviderInvoice,
                    WaybillStateHistory = showAdditionInfo ? GetWaybillStateHistory(item) : "",
                    CuratorName = item.Curator.DisplayName,
                    Comment = item.Comment
                });

                //Изменение итоговых сумм
                if (allowToViewPurchaseCost)
                    purchaseCostSumTotal = purchaseCostSumTotal.HasValue ? purchaseCostSumTotal + purchaseCostSum : purchaseCostSum;

                if (allowToViewAccountingPrice && accountingPriceSum.HasValue)
                    accountingPriceSumTotal = accountingPriceSumTotal.HasValue ? accountingPriceSumTotal + accountingPriceSum : accountingPriceSum;
            }
        }

        /// <summary>
        /// Сортировка списка приходных накладных
        /// </summary>
        /// <param name="list">Список приходных накладных</param>
        /// <param name="sortDateType">Дата, по которой сортируется список</param>
        private IEnumerable<ReceiptWaybill> SortReceiptWaybillList(IEnumerable<ReceiptWaybill> list, WaybillDateType sortDateType)
        {
            switch (sortDateType)
            {
                case WaybillDateType.Date:
                    list = list.OrderBy(x => x.Date).ToList();
                    break;
                case WaybillDateType.AcceptanceDate:
                    list = list.OrderBy(x => x.AcceptanceDate).ToList();
                    break;
                case WaybillDateType.ReceiptDate:
                    list = list.OrderBy(x => x.ReceiptDate).ToList();
                    break;
                case WaybillDateType.ApprovementDate:
                    list = list.OrderBy(x => x.ApprovementDate).ToList();
                    break;
                default:
                    throw new Exception("Неверный тип сортировки.");
            }

            return list;
        }

        #endregion

        #region Внутреннее перемещение

        /// <summary>
        /// Заполнение таблицы по внутреннему перемещению
        /// </summary>
        private void FillInMovementWaybillTable(Report0008ViewModel model, DateTime startDate, DateTime endDate, bool showAdditionInfo, IEnumerable<short> storageIds, 
            IEnumerable<int> curatorIds, string stateId, string groupByCollectionIds, WaybillDateType dateType, DateTime? priorToDate, WaybillDateType sortDateType, User user)
        {
            IEnumerable<MovementWaybill> list;
            var dataModel = new List<WaybillDataModel>();
            var logicState = ValidationUtils.TryGetEnum<MovementWaybillLogicState>(stateId);

            int pageNumber = 0;

            //итоговые суммы
            decimal? purchaseCostSumTotal = null;
            decimal? senderAccountingPriceSumTotal = null;
            decimal? recipientAccountingPriceSumTotal = null;

            do
            {
                list = movementWaybillService.GetList(logicState, storageIds, Permission.Report0008_Storage_List,
                    curatorIds, Permission.User_List_Details, startDate, endDate, ++pageNumber,dateType, priorToDate, user);

                list = SortMovementWaybillList(list, sortDateType);

                FillInMovementWaybillDataModelListByBatch(dataModel, list, showAdditionInfo, ref purchaseCostSumTotal, ref senderAccountingPriceSumTotal,
                    ref recipientAccountingPriceSumTotal, user);
            } while (list.Count() > 0);

            var groupFields = GetGroupFields(groupByCollectionIds);
            model.MovementWaybillModel.Rows = FillTableViewModel(dataModel, groupFields, FillMovementWaybillTableRows);

            model.MovementWaybillModel.PurchaseCostSumTotal = purchaseCostSumTotal.ForDisplay(ValueDisplayType.Money);
            model.MovementWaybillModel.RecipientAccountingPriceSumTotal = recipientAccountingPriceSumTotal.ForDisplay(ValueDisplayType.Money);
            model.MovementWaybillModel.SenderAccountingPriceSumTotal = senderAccountingPriceSumTotal.ForDisplay(ValueDisplayType.Money);

            model.MovementWaybillModel.ShowAdditionInfo = showAdditionInfo;
            model.ShownStates = GetStatesForOut(stateId, WaybillType.MovementWaybill);
        }

        /// <summary>
        /// Заполнение модели-представления из модели данных
        /// </summary>
        private void FillMovementWaybillTableRows(IList<Report0008_WaybillItemViewModel> table, IEnumerable<WaybillDataModel> data)
        {
            foreach (var row in data)
            {
                table.Add(new Report0008_WaybillItemViewModel()
                {
                    Number = row.Number,
                    Date = row.Date,
                    PurchaseCostSum = row.PurchaseCostSum,
                    SenderAccountingPriceSum = row.SenderAccountingPriceSum,
                    RecipientAccountingPriceSum = row.RecipientAccountingPriceSum,
                    StateName = row.StateName,
                    SenderStorageName = row.SenderStorageName,
                    SenderAccountOrganizationName = row.SenderAccountOrganizationName,
                    RecipientStorageName = row.RecipientStorageName,
                    RecipientAccountOrganizationName = row.RecipientAccountOrganizationName,                  
                    WaybillStateHistory = row.WaybillStateHistory,
                    Comment = row.Comment
                });
            }
        }

        /// <summary>
        /// Движение накладной внутреннего перемещения
        /// </summary>
        private string GetWaybillStateHistory(MovementWaybill waybill)
        {
            var result = "Проводка: " + waybill.AcceptanceDate.ForDisplay(true);
            result += ", Отгрузка: " + waybill.ShippingDate.ForDisplay(true);
            result += ", Приемка: " + waybill.ReceiptDate.ForDisplay(true);

            return result;
        }

        /// <summary>
        /// Обработка пакета накладных перемещения
        /// </summary>
        /// <param name="purchaseCostSumTotal">Итоговая сумма в закупочных ценах</param>
        /// <param name="senderAccountingPriceSumTotal">Итоговая сумма в учетных ценах отправителя</param>
        /// <param name="recipientAccountingPriceSumTotal">Итоговая сумма в учетных ценах получателя</param>
        private void FillInMovementWaybillDataModelListByBatch(IList<WaybillDataModel> dataModel, IEnumerable<MovementWaybill> list, bool showAdditionInfo,
            ref decimal? purchaseCostSumTotal, ref decimal? senderAccountingPriceSumTotal, ref decimal? recipientAccountingPriceSumTotal, User user)
        {

            var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

            foreach (var item in list)
            {
                var indicators = movementWaybillIndicatorService.GetMainIndicators(item, user, false, false);
                decimal? senderAccountingPriceSum = indicators.SenderAccountingPriceSum;
                decimal? recipientAccountingPriceSum =  indicators.RecipientAccountingPriceSum;

                var allowToViewSenderAccountingPrice = user.HasPermissionToViewStorageAccountingPrices(item.SenderStorage);
                var allowToViewRecipientAccountingPrice = user.HasPermissionToViewStorageAccountingPrices(item.RecipientStorage);

                dataModel.Add(new WaybillDataModel()
                {
                    Number = item.Number,
                    Date = item.Date.ToShortDateString(),
                    PurchaseCostSum = allowToViewPurchaseCost? (decimal?)item.PurchaseCostSum : null,
                    SenderAccountingPriceSum = allowToViewSenderAccountingPrice? senderAccountingPriceSum : null,
                    RecipientAccountingPriceSum = allowToViewRecipientAccountingPrice? recipientAccountingPriceSum : null,
                    StateName = item.State.GetDisplayName(),
                    SenderStorageName = item.SenderStorage.Name, 
                    SenderAccountOrganizationName = item.Sender.ShortName,
                    RecipientStorageName = item.RecipientStorage.Name,
                    RecipientAccountOrganizationName = item.Recipient.ShortName,
                    WaybillStateHistory = showAdditionInfo ? GetWaybillStateHistory(item) : "",
                    CuratorName = item.Curator.DisplayName,
                    Comment = item.Comment
                });

                //Изменение итоговых сумм
                if (allowToViewPurchaseCost)
                    purchaseCostSumTotal = purchaseCostSumTotal.HasValue ? purchaseCostSumTotal + item.PurchaseCostSum : item.PurchaseCostSum;

                if (allowToViewSenderAccountingPrice)
                    senderAccountingPriceSumTotal = senderAccountingPriceSumTotal.HasValue ? senderAccountingPriceSumTotal + (senderAccountingPriceSum ?? 0)
                                                                                           : senderAccountingPriceSum;
                if (allowToViewRecipientAccountingPrice)
                    recipientAccountingPriceSumTotal = recipientAccountingPriceSumTotal.HasValue ? 
                                                                    recipientAccountingPriceSumTotal + (recipientAccountingPriceSum ?? 0)
                                                                    : recipientAccountingPriceSum;
            }
        }

        /// <summary>
        /// Сортировка списка накладных перемещения
        /// </summary>
        /// <param name="list">Список накладных перемещения</param>
        /// <param name="sortDateType">Дата, по которой сортируется список</param>
        private IEnumerable<MovementWaybill> SortMovementWaybillList(IEnumerable<MovementWaybill> list, WaybillDateType sortDateType)
        {
            switch (sortDateType)
            {
                case WaybillDateType.Date:
                    list = list.OrderBy(x => x.Date).ToList();
                    break;
                case WaybillDateType.AcceptanceDate:
                    list = list.OrderBy(x => x.AcceptanceDate).ToList();
                    break;
                case WaybillDateType.ShippingDate:
                    list = list.OrderBy(x => x.ShippingDate).ToList();
                    break;
                case WaybillDateType.ReceiptDate:
                    list = list.OrderBy(x => x.ReceiptDate).ToList();
                    break;
                default:
                    throw new Exception("Неверный тип сортировки");
            }

            return list;
        }

        #endregion

        #region Смена собственника

        /// <summary>
        /// Заполнение таблицы по смене собственника
        /// </summary>
        private void FillInChangeOwnerWaybillTable(Report0008ViewModel model, DateTime startDate, DateTime endDate, bool showAdditionInfo, IEnumerable<short> storageIds, 
            IEnumerable<int> curatorIds, string stateId, string groupByCollectionIds, WaybillDateType dateType, DateTime? priorToDate, WaybillDateType sortDateType, User user)
        {
            IEnumerable<ChangeOwnerWaybill> list;
            var dataModel = new List<WaybillDataModel>();
            var logicState = ValidationUtils.TryGetEnum<ChangeOwnerWaybillLogicState>(stateId);
            int pageNumber = 0;

            //итоговые суммы
            decimal? purchaseCostSumTotal = null;
            decimal? accountingPriceSumTotal = null;

            do
            {
                list = changeOwnerWaybillService.GetList(logicState, storageIds, Permission.Report0008_Storage_List,
                    curatorIds, Permission.User_List_Details, startDate, endDate, ++pageNumber, dateType, priorToDate, user);

                list = SortChangeOwnerWaybillList(list, sortDateType);

                FillInChangeOwnerWaybillDataModelListByBatch(dataModel, list, showAdditionInfo, ref purchaseCostSumTotal, ref accountingPriceSumTotal, user);
            } while (list.Count() > 0);

            var groupFields = GetGroupFields(groupByCollectionIds);
            model.ChangeOwnerWaybillModel.Rows = FillTableViewModel(dataModel, groupFields, FillChangeOwnerWaybillTableRows);

            model.ChangeOwnerWaybillModel.AccountingPriceSumTotal = accountingPriceSumTotal.ForDisplay(ValueDisplayType.Money);
            model.ChangeOwnerWaybillModel.PurchaseCostSumTotal = purchaseCostSumTotal.ForDisplay(ValueDisplayType.Money);

            model.ChangeOwnerWaybillModel.ShowAdditionInfo = showAdditionInfo;
            model.ShownStates = GetStatesForOut(stateId, WaybillType.ChangeOwnerWaybill);
        }

        /// <summary>
        /// Заполнение модели-представления из модели данных
        /// </summary>
        private void FillChangeOwnerWaybillTableRows(IList<Report0008_WaybillItemViewModel> table, IEnumerable<WaybillDataModel> data)
        {
            foreach (var row in data)
            {
                table.Add(new Report0008_WaybillItemViewModel()
                {
                    Number = row.Number,
                    Date = row.Date,
                    PurchaseCostSum = row.PurchaseCostSum,
                    AccountingPriceSum = row.AccountingPriceSum,
                    StateName = row.StateName,
                    StorageName = row.StorageName,
                    SenderAccountOrganizationName = row.SenderAccountOrganizationName,
                    RecipientAccountOrganizationName = row.RecipientAccountOrganizationName,
                    WaybillStateHistory = row.WaybillStateHistory,
                    Comment = row.Comment
                });
            }
        }

        /// <summary>
        /// Движение накладной смены собственника
        /// </summary>
        private string GetWaybillStateHistory(ChangeOwnerWaybill waybill)
        {
            var result = "Проводка: " + waybill.AcceptanceDate.ForDisplay(true);
            result += ", Перемещение: " + waybill.ChangeOwnerDate.ForDisplay(true);

            return result;
        }

        /// <summary>
        /// Обработка пакета накладных смены собственника
        /// </summary>
        /// <param name="purchaseCostSumTotal">Итоговая сумма в ЗЦ</param>
        /// <param name="accountingPriceSumTotal">Итоговая сумма в УЦ</param>   
        private void FillInChangeOwnerWaybillDataModelListByBatch(IList<WaybillDataModel> dataModel, IEnumerable<ChangeOwnerWaybill> list, bool showAdditionInfo,
            ref decimal? purchaseCostSumTotal, ref decimal? accountingPriceSumTotal, User user)
        {
            var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);
            
            foreach (var item in list)
            {
                var indicators = changeOwnerWaybillIndicatorService.GetMainIndicators(item, user, false);
                decimal? accountingPriceSum = indicators.AccountingPriceSum;

                var allowToViewAccountingPrice = user.HasPermissionToViewStorageAccountingPrices(item.Storage);

                dataModel.Add(new WaybillDataModel()
                {
                    Number = item.Number,
                    Date = item.Date.ToShortDateString(),
                    PurchaseCostSum = allowToViewPurchaseCost? (decimal?)item.PurchaseCostSum : null,
                    AccountingPriceSum = allowToViewAccountingPrice? accountingPriceSum : null,
                    StateName = item.State.GetDisplayName(),
                    StorageName = item.Storage.Name,
                    SenderAccountOrganizationName = item.Sender.ShortName,
                    RecipientAccountOrganizationName = item.Recipient.ShortName,
                    WaybillStateHistory = showAdditionInfo ? GetWaybillStateHistory(item) : "",
                    CuratorName = item.Curator.DisplayName,
                    Comment = item.Comment
                });

                //Изменение итоговых сумм
                if (allowToViewPurchaseCost)
                    purchaseCostSumTotal = purchaseCostSumTotal.HasValue ? purchaseCostSumTotal + item.PurchaseCostSum : item.PurchaseCostSum;

                if (allowToViewAccountingPrice && accountingPriceSum.HasValue)
                    accountingPriceSumTotal = accountingPriceSumTotal.HasValue ? accountingPriceSumTotal + accountingPriceSum : accountingPriceSum;
            }
        }

        /// <summary>
        /// Сортировка списка накладных смены собственника
        /// </summary>
        /// <param name="list">Список накладных смены собственника</param>
        /// <param name="sortDateType">Дата, по которой сортируется список</param>
        private IEnumerable<ChangeOwnerWaybill> SortChangeOwnerWaybillList(IEnumerable<ChangeOwnerWaybill> list, WaybillDateType sortDateType)
        {
            switch (sortDateType)
            {
                case WaybillDateType.Date:
                    list = list.OrderBy(x => x.Date).ToList();
                    break;
                case WaybillDateType.AcceptanceDate:
                    list = list.OrderBy(x => x.AcceptanceDate).ToList();
                    break;
                case WaybillDateType.ChangeOwnerDate:
                    list = list.OrderBy(x => x.ChangeOwnerDate).ToList();
                    break;
                default:
                    throw new Exception("Неверный тип сортировки");
            }

            return list;
        }

        #endregion

        #region Списание

        /// <summary>
        /// Заполнение таблицы по списанию
        /// </summary>
        private void FillInWriteoffWaybillTable(Report0008ViewModel model, DateTime startDate, DateTime endDate, bool showAdditionInfo, IEnumerable<short> storageIds, 
            IEnumerable<int> curatorIds, string stateId, string groupByCollectionIds, WaybillDateType dateType, DateTime? priorToDate, WaybillDateType sortDateType, User user)
        {
            IEnumerable<WriteoffWaybill> list;
            var dataModel = new List<WaybillDataModel>();
            var logicState = ValidationUtils.TryGetEnum<WriteoffWaybillLogicState>(stateId);
            int pageNumber = 0;

            //итоговые суммы
            decimal? purchaseCostSumTotal = null;
            decimal? accountingPriceSumTotal = null;

            do
            {
                list = writeoffWaybillService.GetList(logicState, storageIds, Permission.Report0008_Storage_List,
                    curatorIds, Permission.User_List_Details, startDate, endDate, ++pageNumber, dateType, priorToDate, user);

                list = SortWriteoffWaybillList(list, sortDateType);

                FillInWriteoffWaybillDataModelListByBatch(dataModel, list, showAdditionInfo, ref purchaseCostSumTotal, ref accountingPriceSumTotal, user);
            } while (list.Count() > 0);

            var groupFields = GetGroupFields(groupByCollectionIds);
            model.WriteoffWaybillModel.Rows = FillTableViewModel(dataModel, groupFields, FillWriteoffWaybillTableRows);

            model.WriteoffWaybillModel.AccountingPriceSumTotal = accountingPriceSumTotal.ForDisplay(ValueDisplayType.Money);
            model.WriteoffWaybillModel.PurchaseCostSumTotal = purchaseCostSumTotal.ForDisplay(ValueDisplayType.Money);

            model.WriteoffWaybillModel.ShowAdditionInfo = showAdditionInfo;
            model.ShownStates = GetStatesForOut(stateId, WaybillType.WriteoffWaybill);
        }

        /// <summary>
        /// Заполнение модели-представления из модели данных
        /// </summary>
        private void FillWriteoffWaybillTableRows(IList<Report0008_WaybillItemViewModel> table, IEnumerable<WaybillDataModel> data)
        {
            foreach (var row in data)
            {
                table.Add(new Report0008_WaybillItemViewModel()
                {
                    Number = row.Number,
                    Date = row.Date,
                    PurchaseCostSum = row.PurchaseCostSum,
                    AccountingPriceSum = row.AccountingPriceSum,
                    StateName = row.StateName,
                    StorageName = row.StorageName,
                    AccountOrganizationName = row.AccountOrganizationName,
                    WriteoffReasonName = row.WriteoffReasonName,
                    WaybillStateHistory = row.WaybillStateHistory,
                    CuratorName = row.CuratorName,
                    Comment = row.Comment
                });
            }
        }

        /// <summary>
        /// Движение накладной списания
        /// </summary>
        private string GetWaybillStateHistory(WriteoffWaybill waybill)
        {
            var result = "Проводка: " + waybill.AcceptanceDate.ForDisplay(true);
            result += ", Списание: " + waybill.WriteoffDate.ForDisplay(true);

            return result;
        }

        /// <summary>
        /// Обработка пакета накладных списания
        /// </summary>
        /// <param name="purchaseCostSumTotal">Итоговая сумма в ЗЦ</param>
        /// <param name="accountingPriceSumTotal">Итоговая сумма в УЦ</param>   
        private void FillInWriteoffWaybillDataModelListByBatch(IList<WaybillDataModel> dataModel, IEnumerable<WriteoffWaybill> list, bool showAdditionInfo,
            ref decimal? purchaseCostSumTotal, ref decimal? accountingPriceSumTotal, User user)
        {
            var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

            foreach (var item in list)
            {
                var indicators = writeoffWaybillIndicatorService.GetMainIndicators(item, user);
                decimal? accountingPriceSum = indicators.SenderAccountingPriceSum;

                var allowToViewAccountingPrice = user.HasPermissionToViewStorageAccountingPrices(item.SenderStorage);

                dataModel.Add(new WaybillDataModel()
                {
                    Number = item.Number,
                    Date = item.Date.ToShortDateString(),
                    PurchaseCostSum = allowToViewPurchaseCost? (decimal?)item.PurchaseCostSum : null,
                    AccountingPriceSum = allowToViewAccountingPrice? accountingPriceSum : null,
                    StateName = item.State.GetDisplayName(),
                    StorageName = item.SenderStorage.Name,
                    AccountOrganizationName = item.Sender.ShortName,
                    WriteoffReasonName = item.WriteoffReason.Name,
                    WaybillStateHistory = showAdditionInfo ? GetWaybillStateHistory(item) : "",
                    CuratorName = item.Curator.DisplayName,
                    Comment = item.Comment
                });

                //Изменение итоговых сумм
                if(allowToViewPurchaseCost)
                    purchaseCostSumTotal = purchaseCostSumTotal.HasValue ? purchaseCostSumTotal + item.PurchaseCostSum : item.PurchaseCostSum;

                if(allowToViewAccountingPrice)
                    accountingPriceSumTotal = accountingPriceSumTotal.HasValue ? accountingPriceSumTotal + (accountingPriceSum ?? 0) : accountingPriceSum;
            }
        }

        /// <summary>
        /// Сортировка списка накладных списания
        /// </summary>
        /// <param name="list">Список накладных списания</param>
        /// <param name="sortDateType">Дата, по которой сортируется список</param>
        private IEnumerable<WriteoffWaybill> SortWriteoffWaybillList(IEnumerable<WriteoffWaybill> list, WaybillDateType sortDateType)
        {
            switch (sortDateType)
            {
                case WaybillDateType.Date:
                    list = list.OrderBy(x => x.Date).ToList();
                    break;
                case WaybillDateType.AcceptanceDate:
                    list = list.OrderBy(x => x.AcceptanceDate).ToList();
                    break;
                case WaybillDateType.WriteoffDate:
                    list = list.OrderBy(x => x.WriteoffDate).ToList();
                    break;
                default:
                    throw new Exception("Неверный тип сортировки");
            }

            return list;
        }

        #endregion

        #region Реализация

        /// <summary>
        /// Заполнение таблицы по реализации
        /// </summary>
        private void FillInExpenditureWaybillTable(Report0008ViewModel model, DateTime startDate, DateTime endDate, bool showAdditionInfo, IEnumerable<short> storageIds, 
            IEnumerable<int> curatorIds, IEnumerable<int> clientIds, string stateId, string groupByCollectionIds, WaybillDateType dateType, DateTime? priorToDate, 
            WaybillDateType sortDateType, User user)
        {
            IEnumerable<ExpenditureWaybill> list;
            var dataModel = new List<WaybillDataModel>();
            var logicState = ValidationUtils.TryGetEnum<ExpenditureWaybillLogicState>(stateId);
            int pageNumber = 0;

            //Итоговые цены
            decimal? accountingPriceSumTotal = null;
            decimal? salePriceSumTotal = null;
            decimal? purchaseCostSumTotal = null;
            decimal? returnFromClientSumTotal = null;

            do
            {
                list = expenditureWaybillService.GetList(logicState, storageIds, Permission.Report0008_Storage_List,
                    curatorIds, Permission.User_List_Details, clientIds, Permission.Client_List_Details, startDate, endDate, ++pageNumber, 
                    dateType, priorToDate, user);

                list = SortExpenditureWaybillList(list, sortDateType);

                FillInExpenditureWaybillDataModelListByBatch(dataModel, list, showAdditionInfo, ref purchaseCostSumTotal, ref accountingPriceSumTotal,
                    ref salePriceSumTotal, ref returnFromClientSumTotal, user);
            } while (list.Count() > 0);

            var groupFields = GetGroupFields(groupByCollectionIds);
            model.ExpenditureWaybillModel.Rows = FillTableViewModel(dataModel, groupFields, FillExpenditureWaybillTableRows);

            model.ExpenditureWaybillModel.PurchaseCostSumTotal = purchaseCostSumTotal.ForDisplay(ValueDisplayType.Money);
            model.ExpenditureWaybillModel.AccountingPriceSumTotal = accountingPriceSumTotal.ForDisplay(ValueDisplayType.Money);
            model.ExpenditureWaybillModel.SalePriceSumTotal = salePriceSumTotal.ForDisplay(ValueDisplayType.Money);
            model.ExpenditureWaybillModel.ReturnFromClientSumTotal = returnFromClientSumTotal.ForDisplay(ValueDisplayType.Money);

            model.ExpenditureWaybillModel.ShowAdditionInfo = showAdditionInfo;
            model.ShownStates = GetStatesForOut(stateId, WaybillType.ExpenditureWaybill);
        }

        /// <summary>
        /// Заполнение модели-представления из модели данных
        /// </summary>
        private void FillExpenditureWaybillTableRows(IList<Report0008_WaybillItemViewModel> table, IEnumerable<WaybillDataModel> data)
        {
            foreach (var row in data)
            {
                table.Add(new Report0008_WaybillItemViewModel()
                {
                    Number = row.Number,
                    Date = row.Date,
                    PurchaseCostSum = row.PurchaseCostSum,
                    AccountingPriceSum = row.AccountingPriceSum,
                    SalePriceSum = row.SalePriceSum,
                    ReturnFromClientSum = row.ReturnFromClientSum,
                    SenderStorageName = row.SenderStorageName,
                    AccountOrganizationName = row.AccountOrganizationName,
                    ClientName = row.ClientName,
                    Contract = row.Contract,
                    Quota = row.Quota,
                    WaybillStateHistory = row.WaybillStateHistory,
                    Comment = row.Comment
                });
            }
        }

        /// <summary>
        /// Движение накладной списания
        /// </summary>
        private string GetWaybillStateHistory(ExpenditureWaybill waybill)
        {
            var result = "Проводка: " + waybill.AcceptanceDate.ForDisplay(true);
            result += ", Отгрузка: " + waybill.ShippingDate.ForDisplay(true);

            return result;
        }

        /// <summary>
        /// Обработка пакета накладных реалиазции
        /// </summary>
        /// <param name="purchaseCostSumTotal">Итоговая сумма в закупочных ценах</param>
        /// <param name="accountingPriceSumTotal">Итоговая сумма в учетных ценах</param>
        /// <param name="salePriceSumTotal">Итоговая сумма в отпускных ценах</param>
        /// <param name="returnFromClientSumTotal">Итоговая сумма возвратов</param>
        private void FillInExpenditureWaybillDataModelListByBatch(IList<WaybillDataModel> dataModel, IEnumerable<ExpenditureWaybill> list, 
            bool showAdditionInfo, ref decimal? purchaseCostSumTotal, ref decimal? accountingPriceSumTotal, 
            ref decimal?  salePriceSumTotal, ref decimal? returnFromClientSumTotal, User user)
        {
            var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

            foreach (var item in list)
            {
                var indicators = expenditureWaybillService.GetMainIndicators(item, calculateSenderAccountingPriceSum: true, calculateSalePriceSum: true,
                calculatePaymentSum: false, calculatePaymentPercent: false, calculateVatInfoList: false, calculateProfit: false, calculateTotalDiscount: false,
                calculateLostProfit: false, calculateTotalReservedByReturnSum: false, calculateTotalReturnedSum: true);

                var allowToViewAccountingPrice = user.HasPermissionToViewStorageAccountingPrices(item.SenderStorage);

                dataModel.Add(new WaybillDataModel()
                {
                    Number = item.Number,
                    Date = item.Date.ToShortDateString(),
                    PurchaseCostSum = allowToViewPurchaseCost? (decimal?)item.PurchaseCostSum : null,
                    AccountingPriceSum = allowToViewAccountingPrice ? (decimal?)indicators.SenderAccountingPriceSum : null,
                    SalePriceSum = indicators.SalePriceSum,
                    ReturnFromClientSum = indicators.TotalReturnedSum,
                    SenderStorageName = item.SenderStorage.Name,
                    AccountOrganizationName = item.Sender.ShortName,
                    ClientName = item.Deal.Client.Name,
                    Contract = showAdditionInfo ? item.Deal.Contract.FullName : "",
                    Quota = showAdditionInfo ? item.Quota.Name : "",
                    WaybillStateHistory = showAdditionInfo ? GetWaybillStateHistory(item) : "",
                    CuratorName = item.Curator.DisplayName,
                    Comment = item.Comment
                });

                //Изменение итоговых цен
                salePriceSumTotal = salePriceSumTotal.HasValue ? salePriceSumTotal + indicators.SalePriceSum : indicators.SalePriceSum;
                returnFromClientSumTotal = returnFromClientSumTotal.HasValue ? returnFromClientSumTotal + indicators.TotalReturnedSum : indicators.TotalReturnedSum;
                
                if (allowToViewAccountingPrice)
                    accountingPriceSumTotal = accountingPriceSumTotal.HasValue ? accountingPriceSumTotal + indicators.SenderAccountingPriceSum
                                                                               : indicators.SenderAccountingPriceSum;
                if (allowToViewPurchaseCost)
                    purchaseCostSumTotal = purchaseCostSumTotal.HasValue ? purchaseCostSumTotal + item.PurchaseCostSum : item.PurchaseCostSum;

            }
        }

        /// <summary>
        /// Сортировка списка накладных реализации
        /// </summary>
        /// <param name="list">Список накладных реализации</param>
        /// <param name="sortDateType">Дата, по которой сортируется список</param>
        private IEnumerable<ExpenditureWaybill> SortExpenditureWaybillList(IEnumerable<ExpenditureWaybill> list, WaybillDateType sortDateType)
        {
            switch (sortDateType)
            {
                case WaybillDateType.Date:
                    list = list.OrderBy(x => x.Date).ToList();
                    break;
                case WaybillDateType.AcceptanceDate:
                    list = list.OrderBy(x => x.AcceptanceDate).ToList();
                    break;
                case WaybillDateType.ShippingDate:
                    list = list.OrderBy(x => x.ShippingDate).ToList();
                    break;
                default:
                    throw new Exception("Неверный тип сортировки");
            }

            return list;
        }

        #endregion

        #region Возврат

        /// <summary>
        /// Заполнение таблицы по возвратам
        /// </summary>
        private void FillInReturnFromClientWaybillTable(Report0008ViewModel model, DateTime startDate, DateTime endDate, bool showAdditionInfo, IEnumerable<short> storageIds, 
            IEnumerable<int> curatorIds, IEnumerable<int> clientIds, string stateId, string groupByCollectionIds, WaybillDateType dateType, DateTime? priorToDate, 
            WaybillDateType sortDateType, User user)
        {
            IEnumerable<ReturnFromClientWaybill> list;
            var dataModel = new List<WaybillDataModel>();
            var logicState = ValidationUtils.TryGetEnum<ReturnFromClientWaybillLogicState>(stateId);
            int pageNumber=0;

            //Итоговые цены
            decimal? accountingPriceSumTotal = null;
            decimal? salePriceSumTotal = null;
            decimal? purchaseCostSumTotal = null;

            do
            {
                list = returnFromClientWaybillService.GetList(logicState, storageIds, Permission.Report0008_Storage_List,
                    curatorIds, Permission.User_List_Details, clientIds, Permission.Client_List_Details, startDate, endDate, ++pageNumber,
                    dateType, priorToDate, user);

                list = SortReturnFromClientWaybillList(list, sortDateType);

                FillInReturnFromClientWaybillDataModelListByBatch(dataModel, list, showAdditionInfo, ref purchaseCostSumTotal, ref accountingPriceSumTotal,
                        ref salePriceSumTotal, user);
            } while (list.Count() > 0);

            var groupFields = GetGroupFields(groupByCollectionIds);
            model.ReturnFromClientWaybillModel.Rows = FillTableViewModel(dataModel, groupFields, FillReturnFromClientWaybillTableRows);

            model.ReturnFromClientWaybillModel.PurchaseCostSumTotal = purchaseCostSumTotal.ForDisplay(ValueDisplayType.Money);
            model.ReturnFromClientWaybillModel.AccountingPriceSumTotal = accountingPriceSumTotal.ForDisplay(ValueDisplayType.Money);
            model.ReturnFromClientWaybillModel.SalePriceSumTotal = salePriceSumTotal.ForDisplay(ValueDisplayType.Money);

            model.ReturnFromClientWaybillModel.ShowAdditionInfo = showAdditionInfo;
            model.ShownStates = GetStatesForOut(stateId, WaybillType.ReturnFromClientWaybill);
        }

        /// <summary>
        /// Заполнение модели-представления из модели данных
        /// </summary>
        private void FillReturnFromClientWaybillTableRows(IList<Report0008_WaybillItemViewModel> table, IEnumerable<WaybillDataModel> data)
        {
            foreach (var row in data)
            {
                table.Add(new Report0008_WaybillItemViewModel()
                {
                    Number = row.Number,
                    Date = row.Date,
                    PurchaseCostSum = row.PurchaseCostSum,
                    AccountingPriceSum = row.AccountingPriceSum,
                    SalePriceSum = row.SalePriceSum,
                    RecipientStorageName = row.RecipientStorageName,
                    RecipientAccountOrganizationName = row.RecipientAccountOrganizationName,
                    ClientName = row.ClientName,
                    Contract = row.Contract,
                    ReturnFromClientReasonName = row.ReturnFromClientReasonName,
                    WaybillStateHistory = row.WaybillStateHistory,
                    CuratorName = row.CuratorName,
                    Comment = row.Comment
                });
            }
        }

        /// <summary>
        /// Движение накладной возврата
        /// </summary>
        private string GetWaybillStateHistory(ReturnFromClientWaybill waybill)
        {
            var result = "Проводка: " + waybill.AcceptanceDate.ForDisplay(true);
            result += ", Приемка: " + waybill.ReceiptDate.ForDisplay(true);

            return result;
        }

        /// <summary>
        /// Обработка пакета накладных возврата
        /// </summary>
        /// <param name="purchaseCostSumTotal">Итоговая сумма в закупочных ценах</param>
        /// <param name="accountingPriceSumTotal">Итоговая сумма в учетных ценах</param>
        /// <param name="salePriceSumTotal">Итоговая сумма в отпускных ценах</param>
        private void FillInReturnFromClientWaybillDataModelListByBatch(IList<WaybillDataModel> dataModel, IEnumerable<ReturnFromClientWaybill> list, bool showAdditionInfo,
            ref decimal? purchaseCostSumTotal, ref decimal? accountingPriceSumTotal, ref decimal? salePriceSumTotal, User user)
        {
            var allowToViewPurchaseCost = user.HasPermission(Permission.PurchaseCost_View_ForEverywhere);

            foreach (var item in list)
            {
                var allowToViewAccountingPrice = user.HasPermissionToViewStorageAccountingPrices(item.RecipientStorage);

                decimal accountingPriceSum = allowToViewAccountingPrice? returnFromClientWaybillIndicatorService.CalculateAccountingPriceSum(item) : 0;

                dataModel.Add(new WaybillDataModel()
                {
                    Number = item.Number,
                    Date = item.Date.ToShortDateString(),
                    PurchaseCostSum = allowToViewPurchaseCost? (decimal?)item.PurchaseCostSum : null,
                    SalePriceSum = item.SalePriceSum,
                    AccountingPriceSum = allowToViewAccountingPrice? (decimal?)accountingPriceSum : null,
                    RecipientStorageName = item.RecipientStorage.Name,
                    RecipientAccountOrganizationName = item.Recipient.ShortName,
                    ClientName = item.Client.Name,
                    Contract = item.Deal.Contract.FullName, 
                    WaybillStateHistory = showAdditionInfo ? GetWaybillStateHistory(item) : "",
                    ReturnFromClientReasonName = item.ReturnFromClientReason.Name,
                    CuratorName = item.Curator.DisplayName,
                    Comment = item.Comment
                });

                //Изменение итоговых цен
                salePriceSumTotal = salePriceSumTotal.HasValue ? salePriceSumTotal + item.SalePriceSum : item.SalePriceSum;

                if(allowToViewPurchaseCost)
                    purchaseCostSumTotal = purchaseCostSumTotal.HasValue ? purchaseCostSumTotal + item.PurchaseCostSum : item.PurchaseCostSum;

                if (allowToViewAccountingPrice)
                    accountingPriceSumTotal = accountingPriceSumTotal.HasValue ? accountingPriceSumTotal + accountingPriceSum : accountingPriceSum;
            }
        }

        /// <summary>
        /// Сортировка списка накладных возврата
        /// </summary>
        /// <param name="list">Список накладных возврата</param>
        /// <param name="sortDateType">Дата, по которой сортируется список</param>
        private IEnumerable<ReturnFromClientWaybill>  SortReturnFromClientWaybillList(IEnumerable<ReturnFromClientWaybill> list, WaybillDateType sortDateType)
        {
            switch (sortDateType)
            {
                case WaybillDateType.Date:
                    list = list.OrderBy(x => x.Date).ToList();
                    break;
                case WaybillDateType.AcceptanceDate:
                    list = list.OrderBy(x => x.AcceptanceDate).ToList();
                    break;
                case WaybillDateType.ReceiptDate:
                    list = list.OrderBy(x => x.ReceiptDate).ToList();
                    break;
                default:
                    throw new Exception("Неверный тип сортировки");
            }

            return list;
        }

        #endregion

        #region Выгрузка в Excel
        /// <summary>
        /// Экспорт отчета в Excel
        /// </summary>
        /// <param name="settings">Настройки отчета</param>
        /// <param name="currentUser">Текущий пользователь</param>
        public byte[] Report0008ExportToExcel(Report0008SettingsViewModel settings, UserInfo currentUser)
        {
            var viewModel = Report0008(settings, currentUser);
            
            string reportHeader = viewModel.ReportName + " \r\nза период с " + viewModel.StartDate + " по " + viewModel.EndDate + "\r\n";
            string sign = "Форма: Report0008" + "\r\nАвтор: " + viewModel.CreatedBy + "\r\nСоставлен: " + viewModel.CreationDate;
            
            string tableTitle = "В диапазон попадает: " + viewModel.DateTypeName + ". Тип накладных: " + viewModel.WaybillTypeName + ". Статусы накладных: " + viewModel.ShownStates;  
            if (viewModel.PriorToDateString != null)
            {
                tableTitle += " до даты " + viewModel.PriorToDateString;
            }
            tableTitle += ". Отсортировано по: " +viewModel.SortDateTypeName + ".";

            int columnsCount = GetColumnCount(viewModel);

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet sheet;
                switch (viewModel.WaybillType)
                {
                    case 1:
                        sheet = pck.Workbook.Worksheets.Add("Реестр приходных накладных");
                        FillReceiptWaybillTable(sheet, columnsCount, viewModel.ReceiptWaybillModel, sheet.PrintHeader(columnsCount, reportHeader, sign, tableTitle, 1));
                        break;
                    case 2:
                        sheet = pck.Workbook.Worksheets.Add("Реестр накладных перемещения");
                        FillMovementWaybillTable(sheet, columnsCount, viewModel.MovementWaybillModel, sheet.PrintHeader(columnsCount, reportHeader, sign, tableTitle, 1));
                        break;
                    case 3:
                        sheet = pck.Workbook.Worksheets.Add("Реестр накладных списания");
                        FillWriteoffWaybillTable(sheet, columnsCount, viewModel.WriteoffWaybillModel, sheet.PrintHeader(columnsCount, reportHeader, sign, tableTitle, 1));
                        break;
                    case 4:
                        sheet = pck.Workbook.Worksheets.Add("Реестр накладных реализации");
                        FillExpenditureWaybillTable(sheet, columnsCount, viewModel.ExpenditureWaybillModel, sheet.PrintHeader(columnsCount, reportHeader, sign, tableTitle, 1));
                        break;
                    case 5:
                        sheet = pck.Workbook.Worksheets.Add("Реестр накладных смены собственника");
                        FillChangeOwnerWaybillTable(sheet, columnsCount, viewModel.ChangeOwnerWaybillModel, sheet.PrintHeader(columnsCount, reportHeader, sign, tableTitle, 1));
                        break;
                    case 6:
                        sheet = pck.Workbook.Worksheets.Add("Реестр накладных возврата");
                        FillReturnFromClientWaybillTable(sheet, columnsCount, viewModel.ReturnFromClientWaybillModel, sheet.PrintHeader(columnsCount, reportHeader, sign, tableTitle, 1));
                        break;
                }       

                if (pck.Workbook.Worksheets.Any())
                {
                    pck.Workbook.Worksheets[1].View.TabSelected = true;
                }

                //Возвращаем файл
                return pck.GetAsByteArray();
            }
        }

        /// <summary>
        /// Формирует реестр накладных для приходных накладных 
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillReceiptWaybillTable(ExcelWorksheet workSheet, int columns, Report0008_ReceiptWaybillTableViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region шапка отчета
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Number));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Date));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.StateName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.ProviderName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.RecipientStorageName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.RecipientAccountOrganizationName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.ProviderWaybillName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.ProviderInvoice));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.PurchaseCostSumString));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.AccountingPriceSumString));
            currentCol++;
            if (viewModel.ShowAdditionInfo)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.WaybillStateHistory));
                currentCol++;
            }
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Comment));
            currentRow++;
            currentCol = 1;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            bool flag = false;

            #region Строки с данными
		    for(int i = 0; i < viewModel.Rows.Count; i++)     
            {
                var row = viewModel.Rows[i];

                if (row.IsGroup)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableSubTotalRowStyle());

                    workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 7].MergeRange()
                        .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, indent: row.GroupLevel).SetFormattedValue(row.GroupTitle);
                    currentCol += 8;

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PurchaseCostSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceSumString, ValueDisplayType.Money);
                }
                else
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableUnEvenRowStyle() : ExcelUtils.GetTableEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Number).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Date).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.StateName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ProviderName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.RecipientStorageName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.RecipientAccountOrganizationName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ProviderWaybillName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ProviderInvoice).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PurchaseCostSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceSumString, ValueDisplayType.Money);
                    currentCol++;
                    if (viewModel.ShowAdditionInfo)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.WaybillStateHistory).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(StringUtils.GetStringWithoutHTML(row.Comment)).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, textWrap:true);
                }
                currentRow++;
                currentCol = 1;
                flag = !flag;
            } 
	        #endregion

            #region Итого
            if (viewModel.Rows.Count == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle()).MergeRange()
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center).SetFormattedValue("Нет данных");
                currentRow++;
                currentCol = 1;
            }

            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());

            workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 7].MergeRange()
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right).SetFormattedValue("Итого по столбцу:");
            currentCol += 8;

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.PurchaseCostSumTotal, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.AccountingPriceSumTotal, ValueDisplayType.Money);
            currentRow++;
            currentCol = 1;
            #endregion

            currentRow++;

            workSheet.Cells[currentRow, currentCol, currentRow, columns].MergeRange().SetFormattedValue("Итого: " + viewModel.RowCountString + " шт.", ExcelUtils.GetDefaultStyle())
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, fontStyle: FontStyle.Bold);

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(100);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));
            
            currentRow++;

            return currentRow;
        }

        /// <summary>
        /// Формирует реестр накладных для  накладных перемещения
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillMovementWaybillTable(ExcelWorksheet workSheet, int columns, Report0008_MovementWaybillTableViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region шапка отчета
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Number));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Date));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.StateName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.SenderStorageName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.SenderAccountOrganizationName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.RecipientStorageName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.RecipientAccountOrganizationName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.PurchaseCostSumString));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.SenderAccountingPriceSumString));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.RecipientAccountingPriceSumString));
            currentCol++;
            if (viewModel.ShowAdditionInfo)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.WaybillStateHistory));
                currentCol++;
            }
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Comment));
            currentRow++;
            currentCol = 1;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            bool flag = false;

            #region Строки с данными
            for (int i = 0; i < viewModel.Rows.Count; i++)
            {
                var row = viewModel.Rows[i];

                if (row.IsGroup)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableSubTotalRowStyle());

                    workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 6].MergeRange()
                        .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, indent: row.GroupLevel).SetFormattedValue(row.GroupTitle);
                    currentCol += 7;

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PurchaseCostSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SenderAccountingPriceSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.RecipientAccountingPriceSumString, ValueDisplayType.Money);
                }
                else
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableUnEvenRowStyle() : ExcelUtils.GetTableEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Number).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Date).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.StateName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SenderStorageName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SenderAccountOrganizationName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.RecipientStorageName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.RecipientAccountOrganizationName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PurchaseCostSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SenderAccountingPriceSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.RecipientAccountingPriceSumString, ValueDisplayType.Money);
                    currentCol++;
                    if (viewModel.ShowAdditionInfo)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.WaybillStateHistory).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(StringUtils.GetStringWithoutHTML(row.Comment)).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, textWrap: true);
                }
                currentRow++;
                currentCol = 1;
                flag = !flag;
            }
            #endregion

            #region Итого
            if (viewModel.Rows.Count == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle()).MergeRange()
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center).SetFormattedValue("Нет данных");
                currentRow++;
                currentCol = 1;
            }

            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());

            workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 6].MergeRange()
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right).SetFormattedValue("Итого по столбцу:");
            currentCol += 7;

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.PurchaseCostSumTotal, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.SenderAccountingPriceSumTotal, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.RecipientAccountingPriceSumTotal, ValueDisplayType.Money);
            currentRow++;
            currentCol = 1;
            #endregion

            currentRow++;

            workSheet.Cells[currentRow, currentCol, currentRow, columns].MergeRange().SetFormattedValue("Итого: " + viewModel.RowCountString + " шт.", ExcelUtils.GetDefaultStyle())
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, fontStyle: FontStyle.Bold);

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(100);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            currentRow++;

            return currentRow;
        }

        /// <summary>
        /// Формирует реестр накладных для  накладных списания
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillWriteoffWaybillTable(ExcelWorksheet workSheet, int columns, Report0008_WriteoffWaybillTableViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region шапка отчета
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Number));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Date));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.StateName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.StorageName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.AccountOrganizationName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.WriteoffReasonName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.PurchaseCostSumString));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.AccountingPriceSumString));
            currentCol++;
            if (viewModel.ShowAdditionInfo)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.WaybillStateHistory));
                currentCol++;
            }
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.CuratorName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Comment));
            currentRow++;
            currentCol = 1;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            bool flag = false;

            #region Строки с данными
            for (int i = 0; i < viewModel.Rows.Count; i++)
            {
                var row = viewModel.Rows[i];

                if (row.IsGroup)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableSubTotalRowStyle());

                    workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 5].MergeRange()
                        .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, indent: row.GroupLevel).SetFormattedValue(row.GroupTitle);
                    currentCol += 6;

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PurchaseCostSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceSumString, ValueDisplayType.Money);
                }
                else
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableUnEvenRowStyle() : ExcelUtils.GetTableEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Number).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Date).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.StateName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.StorageName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountOrganizationName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.WriteoffReasonName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PurchaseCostSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceSumString, ValueDisplayType.Money);
                    currentCol++;
                    if (viewModel.ShowAdditionInfo)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.WaybillStateHistory).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.CuratorName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(StringUtils.GetStringWithoutHTML(row.Comment)).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, textWrap: true);
                }
                currentRow++;
                currentCol = 1;
                flag = !flag;
            }
            #endregion

            #region Итого
            if (viewModel.Rows.Count == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle()).MergeRange()
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center).SetFormattedValue("Нет данных");
                currentRow++;
                currentCol = 1;
            }

            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());

            workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 5].MergeRange()
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right).SetFormattedValue("Итого по столбцу:");
            currentCol += 6;

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.PurchaseCostSumTotal, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.AccountingPriceSumTotal, ValueDisplayType.Money);
            currentRow++;
            currentCol = 1;
            #endregion

            currentRow++;

            workSheet.Cells[currentRow, currentCol, currentRow, columns].MergeRange().SetFormattedValue("Итого: " + viewModel.RowCountString + " шт.", ExcelUtils.GetDefaultStyle())
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, fontStyle: FontStyle.Bold);

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(100);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            currentRow++;

            return currentRow;
        }

        /// <summary>
        /// Формирует реестр накладных для  накладных реализации
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillExpenditureWaybillTable(ExcelWorksheet workSheet, int columns, Report0008_ExpenditureWaybillTableViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region шапка отчета
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Number));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Date));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.SenderStorageName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.AccountOrganizationName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.ClientName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.PurchaseCostSumString));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.AccountingPriceSumString));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.SalePriceSumString));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.ReturnFromClientSumString));
            currentCol++;
            if (viewModel.ShowAdditionInfo)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Contract));
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Quota));
                currentCol++;
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.WaybillStateHistory));
                currentCol++;
            }
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Comment));
            currentRow++;
            currentCol = 1;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            bool flag = false;

            #region Строки с данными
            for (int i = 0; i < viewModel.Rows.Count; i++)
            {
                var row = viewModel.Rows[i];

                if (row.IsGroup)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableSubTotalRowStyle());

                    workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 4].MergeRange()
                        .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, indent: row.GroupLevel).SetFormattedValue(row.GroupTitle);
                    currentCol += 5;

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PurchaseCostSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SalePriceSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ReturnFromClientSumString, ValueDisplayType.Money);
                    currentCol++;
                }
                else
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableUnEvenRowStyle() : ExcelUtils.GetTableEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Number).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Date).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SenderStorageName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountOrganizationName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ClientName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PurchaseCostSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SalePriceSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ReturnFromClientSumString, ValueDisplayType.Money);
                    currentCol++;
                    if (viewModel.ShowAdditionInfo)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Contract).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Quota).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.WaybillStateHistory).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(StringUtils.GetStringWithoutHTML(row.Comment)).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, textWrap: true);
                }
                currentRow++;
                currentCol = 1;
                flag = !flag;
            }
            #endregion

            #region Итого
            if (viewModel.Rows.Count == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle()).MergeRange()
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center).SetFormattedValue("Нет данных");
                currentRow++;
                currentCol = 1;
            }

            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());

            workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 4].MergeRange()
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right).SetFormattedValue("Итого по столбцу:");
            currentCol += 5;

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.PurchaseCostSumTotal, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.AccountingPriceSumTotal, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.SalePriceSumTotal, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.ReturnFromClientSumTotal, ValueDisplayType.Money);
            currentRow++;
            currentCol = 1;
            #endregion

            currentRow++;

            workSheet.Cells[currentRow, currentCol, currentRow, columns].MergeRange().SetFormattedValue("Итого: " + viewModel.RowCountString + " шт.", ExcelUtils.GetDefaultStyle())
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, fontStyle: FontStyle.Bold);

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(100);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            currentRow++;

            return currentRow;
        }

        /// <summary>
        /// Формирует реестр накладных для  накладных смены собственника
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillChangeOwnerWaybillTable(ExcelWorksheet workSheet, int columns, Report0008_ChangeOwnerWaybillTableViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region шапка отчета
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Number));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Date));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.StateName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.StorageName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.SenderAccountOrganizationName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.RecipientAccountOrganizationName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.PurchaseCostSumString));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.AccountingPriceSumString));
            currentCol++;
            if (viewModel.ShowAdditionInfo)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.WaybillStateHistory));
                currentCol++;
            }
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Comment));
            currentRow++;
            currentCol = 1;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            bool flag = false;

            #region Строки с данными
            for (int i = 0; i < viewModel.Rows.Count; i++)
            {
                var row = viewModel.Rows[i];

                if (row.IsGroup)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableSubTotalRowStyle());

                    workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 5].MergeRange()
                        .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, indent: row.GroupLevel).SetFormattedValue(row.GroupTitle);
                    currentCol += 6;

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PurchaseCostSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceSumString, ValueDisplayType.Money);
                }
                else
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableUnEvenRowStyle() : ExcelUtils.GetTableEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Number).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Date).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.StateName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.StorageName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SenderAccountOrganizationName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.RecipientAccountOrganizationName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PurchaseCostSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceSumString, ValueDisplayType.Money);
                    currentCol++;
                    if (viewModel.ShowAdditionInfo)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.WaybillStateHistory).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(StringUtils.GetStringWithoutHTML(row.Comment)).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, textWrap: true);
                }
                currentRow++;
                currentCol = 1;
                flag = !flag;
            }
            #endregion

            #region Итого
            if (viewModel.Rows.Count == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle()).MergeRange()
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center).SetFormattedValue("Нет данных");
                currentRow++;
                currentCol = 1;
            }

            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());

            workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 5].MergeRange()
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right).SetFormattedValue("Итого по столбцу:");
            currentCol += 6;

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.PurchaseCostSumTotal, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.AccountingPriceSumTotal, ValueDisplayType.Money);
            currentRow++;
            currentCol = 1;
            #endregion

            currentRow++;

            workSheet.Cells[currentRow, currentCol, currentRow, columns].MergeRange().SetFormattedValue("Итого: " + viewModel.RowCountString + " шт.", ExcelUtils.GetDefaultStyle())
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, fontStyle: FontStyle.Bold);

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(100);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            currentRow++;

            return currentRow;
        }

        /// <summary>
        /// Формирует реестр накладных для  накладных возврата
        /// </summary>
        /// <param name="workSheet">Лист Excel</param>
        /// <param name="columns">Количество столбцов в отчете</param>
        /// <param name="viewModel">Данные таблицы</param>
        /// <param name="startRow">Начальная строка</param>
        /// <returns> Следующая начальная строка</returns>
        private int FillReturnFromClientWaybillTable(ExcelWorksheet workSheet, int columns, Report0008_ReturnFromClientWaybillTableViewModel viewModel, int startRow)
        {
            int currentRow = startRow;
            int currentCol = 1;

            #region шапка отчета
            //Устанавливаем стиль для ячеек шапки
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableHeaderRowStyle());

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Number));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Date));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.RecipientStorageName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.RecipientAccountOrganizationName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.ClientName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Contract));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.PurchaseCostSumString));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.SalePriceSumString));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.AccountingPriceSumString));
            currentCol++;
            if (viewModel.ShowAdditionInfo)
            {
                workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.WaybillStateHistory));
                currentCol++;
            }
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.ReturnFromClientReasonName));
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(ReflectionUtils.GetPropertyDisplayName<Report0008_WaybillItemViewModel>(x => x.Comment));
            currentRow++;
            currentCol = 1;
            workSheet.View.FreezePanes(currentRow, currentCol);
            #endregion

            bool flag = false;

            #region Строки с данными
            for (int i = 0; i < viewModel.Rows.Count; i++)
            {
                var row = viewModel.Rows[i];

                if (row.IsGroup)
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableSubTotalRowStyle());

                    workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 5].MergeRange()
                        .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, indent: row.GroupLevel).SetFormattedValue(row.GroupTitle);
                    currentCol += 6;

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PurchaseCostSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SalePriceSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceSumString, ValueDisplayType.Money);
                }
                else
                {
                    workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(flag ? ExcelUtils.GetTableUnEvenRowStyle() : ExcelUtils.GetTableEvenRowStyle());

                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Number).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Date).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.RecipientStorageName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.RecipientAccountOrganizationName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ClientName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.Contract).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.PurchaseCostSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.SalePriceSumString, ValueDisplayType.Money);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.AccountingPriceSumString, ValueDisplayType.Money);
                    currentCol++;
                    if (viewModel.ShowAdditionInfo)
                    {
                        workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.WaybillStateHistory).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                        currentCol++;
                    }
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(row.ReturnFromClientReasonName).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left);
                    currentCol++;
                    workSheet.Cells[currentRow, currentCol].SetFormattedValue(StringUtils.GetStringWithoutHTML(row.Comment)).ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, textWrap: true);
                }
                currentRow++;
                currentCol = 1;
                flag = !flag;
            }
            #endregion

            #region Итого
            if (viewModel.Rows.Count == 0)
            {
                workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableEvenRowStyle()).MergeRange()
                    .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Center).SetFormattedValue("Нет данных");
                currentRow++;
                currentCol = 1;
            }
            workSheet.Cells[currentRow, currentCol, currentRow, columns].ApplyStyle(ExcelUtils.GetTableTotalRowStyle());

            workSheet.Cells[currentRow, currentCol, currentRow, currentCol + 5].MergeRange()
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Right).SetFormattedValue("Итого по столбцу:");
            currentCol += 6;

            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.PurchaseCostSumTotal, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.SalePriceSumTotal, ValueDisplayType.Money);
            currentCol++;
            workSheet.Cells[currentRow, currentCol].SetFormattedValue(viewModel.AccountingPriceSumTotal, ValueDisplayType.Money);
            currentRow++;
            currentCol = 1;
            #endregion

            currentRow++;

            workSheet.Cells[currentRow, currentCol, currentRow, columns].MergeRange().SetFormattedValue("Итого: " + viewModel.RowCountString + " шт.", ExcelUtils.GetDefaultStyle())
                .ChangeRangeStyle(horizontalAlignment: OfficeOpenXml.Style.ExcelHorizontalAlignment.Left, fontStyle: FontStyle.Bold);

            workSheet.Cells[startRow, 1, currentRow, columns].AutofitRangeColumns(100);
            workSheet.Select(new ExcelAddress(startRow, columns + 3, startRow, columns + 3));

            currentRow++;

            return currentRow;
        }

        /// <summary>
        /// Подсчет количества столбцов в таблицах отчета
        /// </summary>
        /// <param name="viewModel">Данные</param>
        private int GetColumnCount(Report0008ViewModel viewModel)
        {
            int columns = 0;
            switch (viewModel.WaybillType)
            {
                case 1:
                    if (viewModel.ReceiptWaybillModel.ShowAdditionInfo)
                        columns = 12;
                    else
                        columns = 11;
                    break;
                case 2:
                    if (viewModel.MovementWaybillModel.ShowAdditionInfo)
                        columns = 12;
                    else
                        columns = 11;
                    break;
                case 3:
                    if (viewModel.WriteoffWaybillModel.ShowAdditionInfo)
                        columns = 11;
                    else
                        columns = 10;
                    break;
                case 4:
                    if (viewModel.ExpenditureWaybillModel.ShowAdditionInfo)
                        columns = 13;
                    else
                        columns = 10;
                    break;
                case 5:
                    if (viewModel.ChangeOwnerWaybillModel.ShowAdditionInfo)
                        columns = 10;
                    else
                        columns = 9;
                    break;
                case 6:
                    if (viewModel.ReturnFromClientWaybillModel.ShowAdditionInfo)
                        columns = 12;
                    else
                        columns = 11;
                    break;
            }
            return columns;
        }
        #endregion

        #endregion

        #endregion
    }
}