using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.ViewModels.Grid;
using ERP.UI.ViewModels.GridFilter;
using ERP.Utils;
using ERP.Utils.Mvc;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class TaskPresenter : ITaskPresenter
    {
        #region Дополнительные структуры

        /// <summary>
        /// Предыдущие значения по истории
        /// </summary>
        private struct PreviousTaskHistoryItems
        {
            public Contractor Contractor { get; set; }
            public DateTime? DeadLine { get; set; }
            public Deal Deal { get; set; }
            public DateTime? DeletionDate { get; set; }
            public string Description { get; set; }
            public User ExecutedBy { get; set; }
            public DateTime? FactualCompletionDate { get; set; }
            public int? FactualSpentTime { get; set; }
            public TaskPriority TaskPriority { get; set; }
            public ProductionOrder ProductionOrder { get; set; }
            public DateTime? StartDate { get; set; }
            public TaskExecutionState TaskExecutionState { get; set; }
            public string Topic { get; set; }
            public TaskType TaskType { get; set; }
        };

        #endregion

        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly ITaskService taskService;
        private readonly ITaskPriorityService taskPriorityService;
        private readonly ITaskTypeService taskTypeService;
        private readonly IUserService userService;
        private readonly IContractorService contractorService;
        private readonly IDealService dealService;
        private readonly IProductionOrderService productionOrderService;
        private readonly ITaskExecutionItemService taskExecutionItemService;

        #endregion

        #region Конструкторы

        public TaskPresenter(IUnitOfWorkFactory unitOfWorkFactory, ITaskService taskService, ITaskPriorityService taskPriorityService, ITaskTypeService taskTypeService,
            IUserService userService, IContractorService contractorService, IDealService dealService, IProductionOrderService productionOrderService,
            ITaskExecutionItemService taskExecutionItemService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.taskService = taskService;
            this.taskPriorityService = taskPriorityService;
            this.taskTypeService = taskTypeService;
            this.userService = userService;
            this.contractorService = contractorService;
            this.dealService = dealService;
            this.productionOrderService = productionOrderService;
            this.taskExecutionItemService = taskExecutionItemService;
        }

        #endregion

        #region Методы

        #region Список

        /// <summary>
        /// Список задач
        /// </summary>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        public TaskListViewModel List(UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var paramNewTask = "ExecutionState.ExecutionStateType=" + EnumUtils.ValueToString(TaskExecutionStateType.New);
                var paramExecuteTask = "ExecutionState.ExecutionStateType=" + EnumUtils.ValueToString(TaskExecutionStateType.Executing);
                var paramCompletedTask = "ExecutionState.ExecutionStateType=" + EnumUtils.ValueToString(TaskExecutionStateType.Completed);

                var model = new TaskListViewModel();
                model.Filter = new FilterData()
                {
                    Items = new List<FilterItem>()
                        {
                            new FilterTextEditor("Topic", "Название"),
                            new FilterHyperlink("CreatedBy", "Автор","Выберите автора"),
                            new FilterDateRangePicker("StartDate", "Начало"),
                            new FilterHyperlink("ExecutedBy","Ответственный", "Выберите ответственного исполнителя"),
                            new FilterDateRangePicker("DeadLine", "Завершение"),
                            new FilterHyperlink("Contractor","Контрагент", "Выберите контрагента"),
                            new FilterComboBox("Priority", "Приоритет", 
                                taskPriorityService.GetAll().OrderBy(x => x.OrdinalNumber).GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), sort: false)),
                            new FilterHyperlink("Deal","Сделка", "Выберите сделку"),
                            new FilterComboBox("Type", "Тип", 
                                taskTypeService.GetAll().OrderBy(x => x.Name).GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), sort: false)),
                            new FilterHyperlink("ProductionOrder","Заказ", "Выберите заказ"),
                            new FilterComboBox("ExecutionState", "Состояние", new List<SelectListItem>()),
                            new FilterTextEditor("Id", "Код задачи"),
                        }
                };
                model.NewTaskGrid = GetNewTaskGridLocal(new GridState() { Parameters = paramNewTask, Sort = "DeadLine=Desc;CreationDate=Desc;" }, user);
                model.ExecutingTaskGrid = GetExecutingTaskGridLocal(new GridState() { Parameters = paramExecuteTask, Sort = "DeadLine=Desc;StartDate=Desc;" }, user);
                model.CompletedTaskGrid = GetCompletedTaskGridLocal(new GridState() { Parameters = paramCompletedTask, Sort = "FactualCompletionDate=Asc;" }, user);

                return model;
            }
        }

        /// <summary>
        /// Получение модели для грида новых задач
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public GridData GetNewTaskGrid(GridState state, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetNewTaskGridLocal(state, user);
            }
        }

        /// <summary>
        /// Получение модели для грида задач в работе
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public GridData GetExecutionTaskGrid(GridState state, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetExecutingTaskGridLocal(state, user);
            }
        }

        /// <summary>
        /// Получение модели для грида завершенных задач
        /// </summary>
        /// <param name="state"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public GridData GetCompletedTaskGrid(GridState state, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                return GetCompletedTaskGridLocal(state, user);
            }
        }

        /// <summary>
        /// Определение статуса строки грида (цвет строки)
        /// </summary>
        /// <param name="task"></param>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        private GridRowStyle GetRowStyle(Task task, DateTime currentDate)
        {
            GridRowStyle result = GridRowStyle.Normal;
            if (task.DeadLine != null && task.ExecutionState.ExecutionStateType != TaskExecutionStateType.Completed)  // Если указан срок завершения и задача не закрыта, то
            {
                // раскрашиваем строки в соответствии с оставшимся на выполнение временем
                if (task.DeadLine.Value <= currentDate)
                {
                    result = GridRowStyle.Error;  // Задача просрочена
                }
                else if ((task.DeadLine.Value - currentDate).Days == 0)
                {
                    result = GridRowStyle.Warning;  // На выполнение задачи осталось меньше одного дня
                }
            }
            if (task.DeadLine != null && task.ExecutionState.ExecutionStateType == TaskExecutionStateType.Completed)  // Если указан срок завершения и задача закрыта, то
            {
                // раскрашиваем строки в соответствии с тем, была ли просрочена задача
                if (task.DeadLine.Value <= task.FactualCompletionDate.Value)
                {
                    result = GridRowStyle.Error;  // Задача была просрочена
                }
            }

            return result;
        }

        /// <summary>
        /// Формирование грида новых задач
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private GridData GetNewTaskGridLocal(GridState state, User user)
        {
            if (state == null) state = new GridState();
            GridData model = new GridData();
            var currentDate = DateTimeUtils.GetCurrentDateTime();

            var ps = new ParameterString(state.Parameters);
            model.ButtonPermissions["AllowToCreateNewTask"] = taskService.IsPossibilityToCreateTask(user);
            model.Title = "Новые задачи и мероприятия";

            model.AddColumn("Topic", "Тема", Unit.Percentage(50));
            model.AddColumn("Type", "Тип", Unit.Pixel(80));
            model.AddColumn("Priority", "Приоритет", Unit.Pixel(80));
            model.AddColumn("StartDate", "Дата начала исполнения", Unit.Pixel(80));
            model.AddColumn("DeadLine", "Требуемое завершение", Unit.Pixel(80));
            model.AddColumn("CreatedBy", "Автор", Unit.Percentage(25));
            model.AddColumn("ExecutedBy", "Ответственный", Unit.Percentage(25));
            model.AddColumn("Id", "", GridCellStyle.Hidden);
            model.AddColumn("CreatedById", "", GridCellStyle.Hidden);
            model.AddColumn("ExecutedById", "", GridCellStyle.Hidden);

            var list = taskService.GetFilteredList(state, ps, user);

            foreach (var item in list)
            {
                var allowToViewCreatedBy = userService.IsPossibilityToViewDetails(item.CreatedBy, user);
                var allowToViewExecutedBy = item.ExecutedBy != null ? userService.IsPossibilityToViewDetails(item.ExecutedBy, user) : false;

                model.AddRow(new GridRow(
                    new GridLinkCell("Topic") { Value = item.Topic },
                    new GridLabelCell("Type") { Value = item.Type.Name },
                    new GridLabelCell("Priority") { Value = item.Priority.Name },
                    new GridLabelCell("StartDate") { Value = item.StartDate.ForDisplay() },
                    new GridLabelCell("DeadLine") { Value = item.DeadLine.ForDisplay() },
                    allowToViewCreatedBy ?
                        (GridCell)new GridLinkCell("CreatedBy") { Value = item.CreatedBy.DisplayName } :
                        (GridCell)new GridLabelCell("CreatedBy") { Value = item.CreatedBy.DisplayName },
                    allowToViewExecutedBy ?
                        (GridCell)new GridLinkCell("ExecutedBy") { Value = item.ExecutedBy.DisplayName } :
                        (GridCell)new GridLabelCell("ExecutedBy") { Value = item.ExecutedBy != null ? item.ExecutedBy.DisplayName : "---" },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "Id" },
                    new GridHiddenCell("CreatedById") { Value = item.CreatedBy.Id.ToString(), Key = "CreatedById" },
                    new GridHiddenCell("ExecutedById") { Value = item.ExecutedBy != null ? item.ExecutedBy.Id.ToString() : "", Key = "ExecutedById" })
                    {
                        Style = GetRowStyle(item, currentDate)
                    });
            }
            model.State = state;

            return model;
        }

        /// <summary>
        /// Формирование грида задач в работе 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private GridData GetExecutingTaskGridLocal(GridState state, User user)
        {
            if (state == null) state = new GridState();
            GridData model = new GridData();
            var currentDate = DateTimeUtils.GetCurrentDateTime();

            var ps = new ParameterString(state.Parameters);
            model.Title = "Задачи и мероприятия в работе";

            model.AddColumn("Topic", "Тема", Unit.Percentage(50));
            model.AddColumn("Type", "Тип", Unit.Pixel(80));
            model.AddColumn("Priority", "Приоритет", Unit.Pixel(80));
            model.AddColumn("CompletionPercentage", "% выполнения", Unit.Pixel(75), align: GridColumnAlign.Right);
            model.AddColumn("StartDate", "Дата начала исполнения", Unit.Pixel(80));
            model.AddColumn("DeadLine", "Требуемое завершение", Unit.Pixel(80));
            model.AddColumn("CreatedBy", "Автор", Unit.Percentage(25));
            model.AddColumn("ExecutedBy", "Ответственный", Unit.Percentage(25));
            model.AddColumn("Id", "", GridCellStyle.Hidden);
            model.AddColumn("CreatedById", "", GridCellStyle.Hidden);
            model.AddColumn("ExecutedById", "", GridCellStyle.Hidden);

            var list = taskService.GetFilteredList(state, ps, user);

            foreach (var item in list)
            {
                var allowToViewCreatedBy = userService.IsPossibilityToViewDetails(item.CreatedBy, user);
                var allowToViewExecutedBy = item.ExecutedBy != null ? userService.IsPossibilityToViewDetails(item.ExecutedBy, user) : false;

                model.AddRow(new GridRow(
                    new GridLinkCell("Topic") { Value = item.Topic },
                    new GridLabelCell("Type") { Value = item.Type.Name },
                    new GridLabelCell("Priority") { Value = item.Priority.Name },
                    new GridLabelCell("CompletionPercentage") { Value = item.CompletionPercentage.ForDisplay() + "%" },
                    new GridLabelCell("StartDate") { Value = item.StartDate.ForDisplay() },
                    new GridLabelCell("DeadLine") { Value = item.DeadLine.ForDisplay() },
                    allowToViewCreatedBy ?
                        (GridCell)new GridLinkCell("CreatedBy") { Value = item.CreatedBy.DisplayName } :
                        (GridCell)new GridLabelCell("CreatedBy") { Value = item.CreatedBy.DisplayName },
                    allowToViewExecutedBy ?
                        (GridCell)new GridLinkCell("ExecutedBy") { Value = item.ExecutedBy.DisplayName } :
                        (GridCell)new GridLabelCell("ExecutedBy") { Value = item.ExecutedBy != null ? item.ExecutedBy.DisplayName : "---" },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "Id" },
                    new GridHiddenCell("CreatedById") { Value = item.CreatedBy.Id.ToString(), Key = "CreatedById" },
                    new GridHiddenCell("ExecutedById") { Value = item.ExecutedBy != null ? item.ExecutedBy.Id.ToString() : "", Key = "ExecutedById" })
                    {
                        Style = GetRowStyle(item, currentDate)
                    });
            }
            model.State = state;

            return model;
        }

        /// <summary>
        /// Формирование грида завершенных задач
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private GridData GetCompletedTaskGridLocal(GridState state, User user)
        {
            if (state == null) state = new GridState();
            GridData model = new GridData();
            var currentDate = DateTimeUtils.GetCurrentDateTime();

            var ps = new ParameterString(state.Parameters);
            model.Title = "Завершенные задачи и мероприятия";

            model.AddColumn("Topic", "Тема", Unit.Percentage(50));
            model.AddColumn("Type", "Тип", Unit.Pixel(80));
            model.AddColumn("Priority", "Приоритет", Unit.Pixel(80));
            model.AddColumn("StartDate", "Дата начала исполнения", Unit.Pixel(80));
            model.AddColumn("FactualCompletionDate", "Дата завершения", Unit.Pixel(80));
            model.AddColumn("CreatedBy", "Автор", Unit.Percentage(25));
            model.AddColumn("ExecutedBy", "Ответственный", Unit.Percentage(25));
            model.AddColumn("Id", "", GridCellStyle.Hidden);
            model.AddColumn("CreatedById", "", GridCellStyle.Hidden);
            model.AddColumn("ExecutedById", "", GridCellStyle.Hidden);

            var list = taskService.GetFilteredList(state, ps, user);

            foreach (var item in list)
            {
                var allowToViewCreatedBy = userService.IsPossibilityToViewDetails(item.CreatedBy, user);
                var allowToViewExecutedBy = item.ExecutedBy != null ? userService.IsPossibilityToViewDetails(item.ExecutedBy, user) : false;

                model.AddRow(new GridRow(
                    new GridLinkCell("Topic") { Value = item.Topic },
                    new GridLabelCell("Type") { Value = item.Type.Name },
                    new GridLabelCell("Priority") { Value = item.Priority.Name },
                    new GridLabelCell("StartDate") { Value = item.StartDate.ForDisplay() },
                    new GridLabelCell("FactualCompletionDate") { Value = item.FactualCompletionDate.ForDisplay() },
                    allowToViewCreatedBy ?
                        (GridCell)new GridLinkCell("CreatedBy") { Value = item.CreatedBy.DisplayName } :
                        (GridCell)new GridLabelCell("CreatedBy") { Value = item.CreatedBy.DisplayName },
                    allowToViewExecutedBy ?
                        (GridCell)new GridLinkCell("ExecutedBy") { Value = item.ExecutedBy.DisplayName } :
                        (GridCell)new GridLabelCell("ExecutedBy") { Value = item.ExecutedBy != null ? item.ExecutedBy.DisplayName : "---" },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "Id" },
                    new GridHiddenCell("CreatedById") { Value = item.CreatedBy.Id.ToString(), Key = "CreatedById" },
                    new GridHiddenCell("ExecutedById") { Value = item.ExecutedBy != null ? item.ExecutedBy.Id.ToString() : "", Key = "ExecutedById" })
                    {
                        Style = GetRowStyle(item, currentDate)
                    });
            }
            model.State = state;

            return model;
        }

        /// <summary>
        /// Получение модели для заполнения combobox статусов
        /// </summary>
        /// <param name="taskTypeId">Идентификатор типа задачи</param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public object GetStates(short? taskTypeId, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                IEnumerable<TaskExecutionState> list = new List<TaskExecutionState>();
                if (taskTypeId != null)
                {
                    var taskType = taskTypeService.CheckTaskTypeExistence(taskTypeId.Value);
                    list = taskType.States.OrderBy(x => x.OrdinalNumber);
                }

                return new { List = ComboBoxBuilder.GetComboBoxItemList(list, x => x.Name, x => x.Id.ToString(), false, false), SelectedOption = list.ElementAt(0) };
            }
        }


        #endregion

        #region Добавление/редактирование/удаление задачи

        /// <summary>
        /// Создание новой задачи
        /// </summary>
        /// <param name="backUrl">Адрес возврата</param>
        /// <param name="executedById">Идентификатор ответственного за выполнение задачи</param>
        /// <param name="dealId">Идентификатор связанной сделки</param>
        /// <param name="contractorId">Идентификатор связанного контагента</param>
        /// <param name="productionOrderId">Идентификатор связанного заказа на производство</param>
        /// <param name="currentUser">Текущий пользователь</param>
        /// <returns></returns>
        public TaskEditViewModel Create(string backUrl, int? executedById, int? dealId, int? contractorId, Guid? productionOrderId, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                taskService.CheckPossibilityToCreateTask(user);

                User executedBy = executedById != null ? userService.CheckUserExistence(executedById.Value) : null;
                Deal deal = dealId != null ? dealService.CheckDealExistence(dealId.Value, user) : null;
                Contractor contractor = contractorId != null ? contractorService.CheckContractorExistence(contractorId.Value) : null;
                ProductionOrder productionOrder = productionOrderId != null ? productionOrderService.CheckProductionOrderExistence(productionOrderId.Value, user) : null;

                ValidationUtils.Assert(deal == null || productionOrder == null, "Неверные входные данные.");    // Невозможно указать сделку и заказ одновременно
                if (deal != null)
                {
                    if (contractor == null) // Если указана только сделка, то ...
                    {
                        contractor = deal.Client;   // ...берем клиента
                    }
                    else
                    {
                        // Иначе проверяем связь контрагента и сделки
                        ValidationUtils.Assert(contractor == deal.Client, String.Format("Сделка «{0}» не связана с контрагентом «{1}».", deal.Name, contractor.Name));
                    }
                }
                if (productionOrder != null)
                {
                    if (contractor == null)  // Если указан только заказ на производство, то ...
                    {
                        contractor = productionOrder.Producer;  // ... берем производителя
                    }
                    else
                    {
                        // Иначе проверяем связь контрагента и заказа на производство
                        ValidationUtils.Assert(contractor == productionOrder.Producer, String.Format("Заказ на производство «{0}» не связан с контрагентом «{1}».", productionOrder.Name, contractor.Name));
                    }
                }

                var contractorType = contractor != null ? contractor.ContractorType : (ContractorType?)null;
                var model = new TaskEditViewModel();

                model.Title = "Создание новой задачи";
                model.DeadLineTime = "23:59:59";
                model.ContractorName = contractor != null ? contractor.Name : "Выберите контрагента";
                model.ContractorId = contractor != null ? contractor.Id.ToString() : "";
                model.DealName = deal != null ? deal.Name : (contractorType == null || contractorType == ContractorType.Client ? "Выберите сделку" : "---");
                model.DealId = deal != null ? deal.Id.ToString() : "";
                model.ExecutedByName = executedBy != null ? executedBy.DisplayName : "Выберите ответственное лицо";
                model.ExecutedById = executedBy != null ? executedBy.Id.ToString() : "";
                model.ProductionOrderName = productionOrder != null ? productionOrder.Name : (contractorType == null || contractorType == ContractorType.Producer ? "Выберите заказ на производство" : "---");
                model.ProductionOrderId = productionOrder != null ? productionOrder.Id.ToString() : "";
                model.TaskExecutionStateList = new List<SelectListItem>();  // Т.к. статусы исполнения зависят от типа задачи, а он еще пока не известен
                model.TaskPriorityList = taskPriorityService.GetAll().OrderBy(x => x.OrdinalNumber).GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), sort: false);
                model.TaskTypeList = taskTypeService.GetAll().OrderBy(x => x.Name).GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), sort: false);
                model.CreatedBy = user.DisplayName;
                model.CreationDate = DateTimeUtils.GetCurrentDateTime().ToShortDateString();
                model.BackURL = backUrl;

                return model;
            }
        }

        /// <summary>
        /// Редактирование задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="backUrl"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public TaskEditViewModel Edit(int taskId, string backUrl, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var task = taskService.CheckTaskExistence(taskId, user);

                taskService.CheckPossibilityToEditTask(task, user);

                var contractorType = task.Contractor != null ? task.Contractor.ContractorType : (ContractorType?)null;

                return new TaskEditViewModel()
                {
                    Title = "Редактирование задачи",
                    ContractorId = task.Contractor != null ? task.Contractor.Id.ToString() : "",
                    ContractorName = task.Contractor != null ? task.Contractor.Name : "Выберите контрагента",
                    ContractorType = contractorType != null ? contractorType.ValueToString() : "",
                    DealId = task.Deal != null ? task.Deal.Id.ToString() : "",
                    DeadLineDate = task.DeadLine != null ? task.DeadLine.Value.ToShortDateString() : "",
                    DeadLineTime = task.DeadLine != null ? task.DeadLine.Value.ToFullTimeString() : "",
                    DealName = task.Deal != null ? task.Deal.Name : (contractorType == null || contractorType == ContractorType.Client) ? "Выберите сделку" : "---",
                    ExecutedById = task.ExecutedBy != null ? task.ExecutedBy.Id.ToString() : "",
                    ExecutedByName = task.ExecutedBy != null ? task.ExecutedBy.DisplayName : "Выберите ответственное лицо",
                    ProductionOrderId = task.ProductionOrder != null ? task.ProductionOrder.Id.ToString() : "",
                    ProductionOrderName = task.ProductionOrder != null ? task.ProductionOrder.Name : (contractorType == null || contractorType == ContractorType.Producer) ? "Выберите заказ на производство" : "---",
                    TaskTypeList = taskTypeService.GetAll().OrderBy(x => x.Name).GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), sort: false),
                    TaskTypeId = task.Type.Id.ToString(),
                    TaskExecutionStateId = task.ExecutionState.Id.ToString(),
                    TaskExecutionStateList = ComboBoxBuilder.GetComboBoxItemList(task.Type.States.OrderBy(x => x.OrdinalNumber), x => x.Name, x => x.Id.ToString(), false, false),
                    TaskPriorityId = task.Priority.Id.ToString(),
                    TaskPriorityList = taskPriorityService.GetAll().OrderBy(x => x.OrdinalNumber).GetComboBoxItemList(x => x.Name, x => x.Id.ToString(), sort: false),
                    Topic = task.Topic,
                    Description = StringUtils.FromHtml(task.Description),
                    CreatedBy = task.CreatedBy.DisplayName,
                    CreationDate = task.CreationDate.ToFullDateTimeString(),
                    Id = task.Id,
                    BackURL = backUrl
                };
            }
        }

        /// <summary>
        /// Сохранение задачи
        /// </summary>
        /// <param name="model">Модель</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Идентификатор сохраненной задачи</returns>
        public string Save(TaskEditViewModel model, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                Task task = null;
                TaskExecutionItem executionItem = null;

                ValidationUtils.NotNull(model, "Неверное значение входного параметра.");
                var user = userService.CheckUserExistence(currentUser.Id);
                var currentDate = DateTimeUtils.GetCurrentDateTime();   //Текущее время

                // Проверка прав
                if (model.Id == 0)
                {
                    // Новая задача
                    taskService.CheckPossibilityToCreateTask(user);
                }
                else
                {
                    //Редактируется имеющаяся
                    task = taskService.CheckTaskExistence(model.Id, user);
                    taskService.CheckPossibilityToEditTask(task, user);
                }

                ValidationUtils.Assert(!String.IsNullOrEmpty(model.Topic), "Указано неверное значение заголовка задачи.");

                var taskPriorityId = ValidationUtils.TryGetShort(model.TaskPriorityId, "Указан неверный идентификатор приоритета задачи.");
                var taskPriority = taskPriorityService.CheckTaskPriorityExistence(taskPriorityId);

                var taskTypeId = ValidationUtils.TryGetShort(model.TaskTypeId, "Указан неверный идентификатор типа задачи.");
                var taskType = taskTypeService.CheckTaskTypeExistence(taskTypeId);

                var taskExecutionStateId = ValidationUtils.TryGetShort(model.TaskExecutionStateId, "Указан неверный идентификатор статуса исполнения задачи.");
                var taskExecutionState = taskType.States.Where(x => x.Id == taskExecutionStateId).FirstOrDefault();
                ValidationUtils.NotNull(taskExecutionState, "Статус не найден. Возможно, он был удален.");

                User executedBy = null;
                if (!String.IsNullOrEmpty(model.ExecutedById))
                {
                    var executedById = ValidationUtils.TryGetInt(model.ExecutedById, "Указан неверный идентификатор ответственного лица.");
                    var permission = taskService.GetPermissionForExecutedByList(user);
                    executedBy = userService.CheckUserExistence(executedById, user, permission, "Ответственное лицо не найдено. Возможно, оно было удалено.");
                }
                Contractor contractor = null;
                if (!String.IsNullOrEmpty(model.ContractorId))
                {
                    var contractorId = ValidationUtils.TryGetInt(model.ContractorId, "Указан неверный идентификатор контрагента.");
                    contractor = contractorService.CheckContractorExistence(contractorId);
                }
                Deal deal = null;
                if (!String.IsNullOrEmpty(model.DealId))
                {
                    var dealId = ValidationUtils.TryGetInt(model.DealId, "Указан неверный идентификатор сделки.");
                    deal = dealService.CheckDealExistence(dealId, user);
                }
                ProductionOrder productionOrder = null;
                if (!String.IsNullOrEmpty(model.ProductionOrderId))
                {
                    var productionOrderId = ValidationUtils.TryGetGuid(model.ProductionOrderId, "Указан неверный идентификатор заказа на производство.");
                    productionOrder = productionOrderService.CheckProductionOrderExistence(productionOrderId, user);
                }
                DateTime? deadLine = null;
                var deadLineDateTime = model.DeadLineDate + ' ' + model.DeadLineTime;
                if (!String.IsNullOrWhiteSpace(deadLineDateTime))
                {
                    deadLine = ValidationUtils.TryGetDate(deadLineDateTime, "Дата требуемого завершения указана не верно.");
                }
                var description = StringUtils.ToHtml(model.Description);

                if (model.Id == 0)
                {
                    taskService.CheckPossibilityToCreateTask(user);

                    // Сохраняется новая задача
                    task = new Task(model.Topic, taskType, taskPriority, taskExecutionState, currentDate, user);

                    //--- Необязательная часть
                    if (contractor != null) { task.Contractor = contractor; }
                    if (deal != null) { task.Deal = deal; }
                    if (productionOrder != null) { task.ProductionOrder = productionOrder; }

                    if (executedBy != null) { task.ExecutedBy = executedBy; }
                    if (deadLine != null)
                    {
                        ValidationUtils.Assert(deadLine >= currentDate,
                               String.Format("Невозможно установить требуемую дату исполнения ранее текущей даты ({0}).", currentDate.ToFullDateTimeString()));
                        task.DeadLine = deadLine;
                    }
                    task.Description = StringUtils.ToHtml(model.Description);
                    //---

                    executionItem = new TaskExecutionItem(task, taskType, taskExecutionState, task.CompletionPercentage, user, false, currentDate, currentDate);
                }
                else
                {
                    // Редактируется имеющаяся
                    if (task.Contractor != contractor) { task.Contractor = contractor; }
                    if (task.Deal != deal) { task.Deal = deal; }
                    if (task.ProductionOrder != productionOrder) { task.ProductionOrder = productionOrder; }

                    if (task.DeadLine != deadLine)
                    {
                        ValidationUtils.Assert(deadLine >= currentDate,
                            String.Format("Невозможно установить требуемую дату исполнения ранее текущей даты ({0}).", currentDate.ToFullDateTimeString()));
                        task.DeadLine = deadLine;
                    }
                    if (task.Description != description) { task.Description = description; }
                    if (task.ExecutedBy != executedBy) { task.ExecutedBy = executedBy; }
                    if (task.Topic != model.Topic) { task.Topic = model.Topic; }
                    if (task.Priority != taskPriority) { task.Priority = taskPriority; }

                    if (task.ExecutionState != taskExecutionState || task.Type != taskType)
                    {
                        executionItem = new TaskExecutionItem(task, taskType, taskExecutionState, task.CompletionPercentage, user, false, currentDate, currentDate);
                    }
                }

                taskService.Save(task, currentDate, user);
                if (executionItem != null)
                {
                    taskExecutionItemService.Save(executionItem, currentDate, user);
                }

                uow.Commit();

                return task.Id.ToString();
            }

        }

        /// <summary>
        /// Удаление задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="currentUser"></param>
        public void Delete(int taskId, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var task = taskService.CheckTaskExistence(taskId, user);
                var currentDate = DateTimeUtils.GetCurrentDateTime();

                taskService.CheckPossibilityToDeleteTask(task, user);

                taskService.Delete(task, currentDate, user);

                uow.Commit();
            }
        }

        /// <summary>
        /// Получение данных о клиенте по сделке
        /// </summary>
        /// <param name="dealId">Идентификатор сделки</param>
        /// <param name="currentUser">Полдьзователь</param>
        /// <returns>имя и код клиента</returns>
        public object GetClientByDeal(int dealId, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var deal = dealService.CheckDealExistence(dealId, user);

                return new { ClientName = deal.Client.Name, ClientId = deal.Client.Id };
            }
        }

        #endregion

        #region Детали

        /// <summary>
        /// Детали задачи
        /// </summary>
        /// <param name="taskId">Идентификатор задачи</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        public TaskDetailsViewModel Details(int taskId, string backUrl, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var task = taskService.CheckTaskExistence(taskId, user);

                var model = new TaskDetailsViewModel();
                model.Title = "Детали задачи";
                model.AllowToEdit = taskService.IsPossibilityToEditTask(task, user);
                model.AllowToDelete = taskService.IsPossibilityToDeleteTask(task, user);
                model.AllowToCreateTaskExecutuion = taskExecutionItemService.IsPossibilityToCreateTaskExecution(task, user);
                model.AllowToCompleteTask = model.AllowToCreateTaskExecutuion;

                // Детали задачи
                model.MainDetails.Id = task.Id;
                model.MainDetails.Topic = task.Topic;
                model.MainDetails.CreatedByName = task.CreatedBy.DisplayName;
                model.MainDetails.CreatedById = task.CreatedBy.Id.ToString();
                model.MainDetails.TaskTypeName = task.Type.Name;
                model.MainDetails.TaskPriorityName = task.Priority.Name;
                model.MainDetails.ExecutedByName = task.ExecutedBy != null ? task.ExecutedBy.DisplayName : "---";
                model.MainDetails.ExecutedById = task.ExecutedBy != null ? task.ExecutedBy.Id.ToString() : "";
                model.MainDetails.StartDate = task.StartDate != null ? task.StartDate.Value.ToFullDateTimeString() : "---";
                model.MainDetails.DeadLine = task.DeadLine != null ? task.DeadLine.Value.ToFullDateTimeString() : "---";
                model.MainDetails.TaskExecutionStateName = task.ExecutionState.Name;
                model.MainDetails.CompletionPercentage = task.CompletionPercentage.ForDisplay();
                model.MainDetails.FactualCompletionDate = task.FactualCompletionDate != null ? task.FactualCompletionDate.Value.ToFullDateTimeString() : "---";
                model.MainDetails.FactualSpentTime = task.FactualSpentTime > 0 ? MinutesToTimeDisplay(task.FactualSpentTime) : "---";
                model.MainDetails.ContractorName = task.Contractor != null ? task.Contractor.Name : "---";
                model.MainDetails.ContractorId = task.Contractor != null ? task.Contractor.Id.ToString() : "";
                model.MainDetails.ContractorType = task.Contractor != null ? task.Contractor.ContractorType.ValueToString() : "";
                model.MainDetails.DealName = task.Deal != null ? task.Deal.Name : "---";
                model.MainDetails.DealId = task.Deal != null ? task.Deal.Id.ToString() : "";
                model.MainDetails.ProductionOrderName = task.ProductionOrder != null ? task.ProductionOrder.Name : "---";
                model.MainDetails.ProductionOrderId = task.ProductionOrder != null ? task.ProductionOrder.Id.ToString() : "";
                model.MainDetails.Description = task.Description;
                model.MainDetails.AllowToViewCreatedByDetails = userService.IsPossibilityToViewDetails(task.CreatedBy, user);
                model.MainDetails.AllowToViewExecitionByDetails = task.ExecutedBy != null ? userService.IsPossibilityToViewDetails(task.ExecutedBy, user) : false;
                model.MainDetails.AllowToViewContractorDetails = task.Contractor != null ? IsPossibilityToViewContractor(task.Contractor, user) : false;
                model.MainDetails.AllowToViewDealDetails = task.Deal != null ? dealService.IsPossibilityToViewDetails(task.Deal, user) : false;
                model.MainDetails.AllowToViewProductionOrderDetails = task.ProductionOrder != null ? productionOrderService.IsPossibilityToViewDetails(task.ProductionOrder, user) : false;

                // исполнения задачи
                model.TaskExecutions = GetTaskExecutionsLocal(task, user);

                return model;
            }
        }

        /// <summary>
        /// История исполнения задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public TaskExecutionsViewModel GetTaskExecutions(int taskId, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var task = taskService.CheckTaskExistence(taskId, user);

                return GetTaskExecutionsLocal(task, user);
            }
        }

        /// <summary>
        /// Исполнения задачи
        /// </summary>
        /// <param name="task">Задача, исполнение которой выводится</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        private TaskExecutionsViewModel GetTaskExecutionsLocal(Task task, User user)
        {
            var model = new TaskExecutionsViewModel();

            foreach (var item in task.ExecutionHistory.Where(x => x.IsCreatedByUser))
            {
                model.History.Add(GetTaskExecutionItemLocal(item, user));
            }

            return model;
        }

        /// <summary>
        /// Одно исполнение задачи
        /// </summary>
        /// <param name="item"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private TaskExecutionsItemViewModel GetTaskExecutionItemLocal(TaskExecutionItem item, User user)
        {
            return new TaskExecutionsItemViewModel()
                {
                    TaskExecutionId = item.Id.ToString(),
                    CreatedByName = item.CreatedBy.DisplayName,
                    CreatedById = item.CreatedBy.Id.ToString(),
                    Date = GetDateWithTextMonthFormatAndTime(item.Date),
                    ResultDescription = item.ResultDescription,
                    SpentTime = MinutesToTimeDisplay(item.SpentTime),
                    CompletionPercentage = item.CompletionPercentage.ForDisplay(),
                    ExecutionStateName = item.ExecutionState.Name,
                    AllowToViewCreatedBy = userService.IsPossibilityToViewDetails(item.CreatedBy, user),
                    AllowToEditTaskExecution = taskExecutionItemService.IsPossibilityToEditTaskExecution(item, user),
                    AllowToDeleteTaskExecution = taskExecutionItemService.IsPossibilityToDeleteTaskExecution(item, user)
                };
        }

        /// <summary>
        /// Одно исполнение задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskExecutionId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public TaskExecutionsViewModel GetTaskExecution(int taskId, int taskExecutionId, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var task = taskService.CheckTaskExistence(taskId, user);
                var taskExecution = task.ExecutionHistory.Where(x => x.Id == taskExecutionId).FirstOrDefault();

                ValidationUtils.NotNull(taskExecution, "Исполнение не найдено. Возможно, оно было удалено.");

                var model = new TaskExecutionsViewModel();
                model.History.Add(GetTaskExecutionItemLocal(taskExecution, user));
                model.NeedDelimFirstItem = task.ExecutionHistory.Any(x => x.CreationDate < taskExecution.CreationDate && x.IsCreatedByUser);

                return model;
            }
        }

        /// <summary>
        /// История задачи
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public TaskHistoryViewModel GetTaskHistory(int taskId, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var task = taskService.CheckTaskExistence(taskId, user);

                return GetTaskHistoryLocal(task, user);
            }
        }

        /// <summary>
        /// История задачи начиная с указанной даты
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="taskExecutionId">исполнение, для которого выводится история</param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public TaskHistoryViewModel GetTaskHistoryForTaskExecution(int taskId, int taskExecutionId, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var task = taskService.CheckTaskExistence(taskId, user);
                var taskExecution = task.ExecutionHistory.Where(x => x.Id == taskExecutionId).FirstOrDefault();

                ValidationUtils.NotNull(taskExecution, "Исполнение задачи не найдено. Возможно, оно было удалено.");

                return GetTaskHistoryLocal(task, user, taskExecution);
            }
        }

        /// <summary>
        /// Получение коллекции элементов истории для вывода
        /// </summary>
        /// <param name="task"></param>
        /// <param name="fromDate"></param>
        /// <returns></returns>
        private IEnumerable<BaseTaskHistoryItem> GetTaskHistoryList(Task task, TaskExecutionItem taskExecution = null)
        {
            var commonList = taskService.GetChangeHistory(task);

            if (taskExecution == null)
            {
                return commonList.OrderBy(x => x.CreationDate);
            }

            var allTaskHistory = commonList.Where(x => x.HistoryItemType == TaskHistoryItemType.TaskHistory && x.CreationDate <= taskExecution.Date).Select(x => x.As<TaskHistoryItem>());

            #region Изменения задачи

            var additionTaskHistory = new List<TaskHistoryItem>();
            var taskHistoryForOut = allTaskHistory.Where(x => x.CreationDate < taskExecution.CreationDate).OrderByDescending(x => x.CreationDate).ToList();

            if (taskHistoryForOut.Any(x => x.IsFactualCompletionDateChanged))
            {
                var item = allTaskHistory.Where(x => x.IsFactualCompletionDateChanged).FirstOrDefault();
                if (item != null) { additionTaskHistory.Add(item); }
            }
            if (taskHistoryForOut.Any(x => x.IsFactualSpentTimeChanged))
            {
                var item = allTaskHistory.Where(x => x.IsFactualSpentTimeChanged).FirstOrDefault();
                if (item != null) { additionTaskHistory.Add(item); }
            }
            if (taskHistoryForOut.Any(x => x.IsStartDateChanged))
            {
                var item = allTaskHistory.Where(x => x.IsStartDateChanged).FirstOrDefault();
                if (item != null) { additionTaskHistory.Add(item); }
            }
            if (taskHistoryForOut.Any(x => x.IsTaskExecutionStateChanged))
            {
                var item = allTaskHistory.Where(x => x.IsTaskExecutionStateChanged).FirstOrDefault();
                if (item != null) { additionTaskHistory.Add(item); }
            }


            #endregion

            var allTaskExecutionHistory = commonList.Where(x => x.HistoryItemType == TaskHistoryItemType.TaskExecutionHistory && x.CreationDate <= taskExecution.CreationDate).Select(x => x.As<TaskExecutionHistoryItem>());

            #region Изменения исполнения

            var additionTaskExecutionHistory = new List<TaskExecutionHistoryItem>();
            List<TaskExecutionHistoryItem> taskExecutionHistoryForOut = allTaskExecutionHistory.Where(x => x.CreationDate < taskExecution.CreationDate).OrderByDescending(x => x.CreationDate).ToList();

            if (taskExecutionHistoryForOut.Any(x => x.IsDateChanged))
            {
                var item = allTaskExecutionHistory.Where(x => x.IsDateChanged).FirstOrDefault();
                if (item != null) { additionTaskExecutionHistory.Add(item); }
            }
            if (taskExecutionHistoryForOut.Any(x => x.IsDeletionDateChanged))
            {
                var item = allTaskExecutionHistory.Where(x => x.IsDeletionDateChanged).FirstOrDefault();
                if (item != null) { additionTaskExecutionHistory.Add(item); }
            }
            if (taskExecutionHistoryForOut.Any(x => x.IsResultDescriptionChanged))
            {
                var item = allTaskExecutionHistory.Where(x => x.IsResultDescriptionChanged).FirstOrDefault();
                if (item != null) { additionTaskExecutionHistory.Add(item); }
            }
            if (taskExecutionHistoryForOut.Any(x => x.IsSpentTimeChanged))
            {
                var item = allTaskExecutionHistory.Where(x => x.IsSpentTimeChanged).FirstOrDefault();
                if (item != null) { additionTaskExecutionHistory.Add(item); }
            }
            if (taskExecutionHistoryForOut.Any(x => x.IsCompletionPercentageChanged))
            {
                var item = allTaskExecutionHistory.Where(x => x.IsCompletionPercentageChanged).FirstOrDefault();
                if (item != null) { additionTaskExecutionHistory.Add(item); }
            }
            if (taskExecutionHistoryForOut.Any(x => x.IsTaskExecutionStateChanged))
            {
                var item = allTaskExecutionHistory.Where(x => x.IsTaskExecutionStateChanged).FirstOrDefault();
                if (item != null) { additionTaskExecutionHistory.Add(item); }
            }

            additionTaskExecutionHistory.Add(allTaskExecutionHistory.Where(x => x.CreationDate == taskExecution.CreationDate).First());

            #endregion

            var result = new List<BaseTaskHistoryItem>();
            result.AddRange(additionTaskHistory);
            result.AddRange(additionTaskExecutionHistory);

            return result.Distinct().OrderBy(x => x.CreationDate);
        }

        /// <summary>
        /// Получение истории изменений задачи
        /// </summary>
        /// <param name="task"></param>
        /// <param name="user"></param>
        /// <param name="taskExecution">Исполнение, для которого выводится история. Если NULL, то история выводится полностью</param>
        /// <returns></returns>
        private TaskHistoryViewModel GetTaskHistoryLocal(Task task, User user, TaskExecutionItem taskExecution = null)
        {
            var history = GetTaskHistoryList(task, taskExecution);   // история по задаче
            var taskHistory = history.Where(x => x.HistoryItemType == TaskHistoryItemType.TaskHistory).Select(x => x.As<TaskHistoryItem>());
            var taskExecutionHistory = history.Where(x => x.HistoryItemType == TaskHistoryItemType.TaskExecutionHistory).Select(x => x.As<TaskExecutionHistoryItem>());

            var deals = dealService.GetList(taskHistory.Where(x => x.Deal != null).Select(x => x.Deal.Id).Distinct().ToList());
            var dealPermissons = GetPermissions(deals.Values.ToList(), x => x.Id, x => dealService.IsPossibilityToViewDetails(x, user));

            var userList = new List<User>();
            userList.AddRange(taskHistory.Where(x => x.ExecutedBy != null).Select(x => x.ExecutedBy));
            userList.AddRange(taskHistory.Select(x => x.CreatedBy));
            userList.AddRange(taskExecutionHistory.Select(x => x.CreatedBy).ToList());
            var users = userList.Distinct();
            
            var userPermissons = GetPermissions(users, x => x.Id, x => userService.IsPossibilityToViewDetails(x, user));

            var productionOrders = taskHistory.Where(x => x.ProductionOrder != null).Select(x => x.ProductionOrder).Distinct();
            var productionOrderPermissons = GetPermissions(productionOrders, x => x.Id, x => productionOrderService.IsPossibilityToViewDetails(x, user));

            var contractorPermissions = GetPermissions(taskHistory.Where(x => x.Contractor != null).Select(x => x.Contractor).Distinct(),
                x => x.Id, x => IsPossibilityToViewContractor(x, user));

            var _taskHistory = history.Where(x => x.HistoryItemType == TaskHistoryItemType.TaskHistory).Select(x => x.As<TaskHistoryItem>());
            // Элемент содержащий первое изменение не выводится, но используется для вывода последующих изменений
            // Массив сожержащий элементы истории, которые изменяли последними значение свойтсва.

            var previousTaskHistoryItems = new PreviousTaskHistoryItems();
            if (taskExecution == null)
            {
                previousTaskHistoryItems.Contractor = _taskHistory.FirstOrDefault(x => x.IsContractorChanged).Contractor;
                previousTaskHistoryItems.DeadLine = _taskHistory.FirstOrDefault(x => x.IsDeadLineChanged).Deadline;
                previousTaskHistoryItems.Deal = _taskHistory.FirstOrDefault(x => x.IsDealChanged).Deal;
                previousTaskHistoryItems.DeletionDate = _taskHistory.FirstOrDefault(x => x.IsDeletionDateChanged).DeletionDate;
                previousTaskHistoryItems.Description = _taskHistory.FirstOrDefault(x => x.IsDescriptionChanged).Description;
                previousTaskHistoryItems.ExecutedBy = _taskHistory.FirstOrDefault(x => x.IsExecutedByChanged).ExecutedBy;
                previousTaskHistoryItems.FactualCompletionDate = _taskHistory.FirstOrDefault(x => x.IsFactualCompletionDateChanged).FactualCompletionDate;
                previousTaskHistoryItems.FactualSpentTime = _taskHistory.FirstOrDefault(x => x.IsFactualSpentTimeChanged).FactualSpentTime;
                previousTaskHistoryItems.TaskPriority = _taskHistory.FirstOrDefault(x => x.IsTaskPriorityChanged).TaskPriority;
                previousTaskHistoryItems.ProductionOrder = _taskHistory.FirstOrDefault(x => x.IsProductionOrderChanged).ProductionOrder;
                previousTaskHistoryItems.StartDate = _taskHistory.FirstOrDefault(x => x.IsStartDateChanged).StartDate;
                previousTaskHistoryItems.TaskExecutionState = _taskHistory.FirstOrDefault(x => x.IsTaskExecutionStateChanged).TaskExecutionState;
                previousTaskHistoryItems.Topic = _taskHistory.FirstOrDefault(x => x.IsTopicChanged).Topic;
                previousTaskHistoryItems.TaskType = _taskHistory.FirstOrDefault(x => x.IsTaskTypeChanged).TaskType;
            }

            IDictionary<int, TaskExecutionHistoryItem[]> previousTaskExecutionHistoryItems = new Dictionary<int, TaskExecutionHistoryItem[]>();

            var model = new TaskHistoryViewModel();
            foreach (var baseItem in history)
            {
                TaskHistoryItemViewModel modelItem = null;

                // Пропускаем историю, дата создания которой совпадает с датой создания задачи, т.к. эта история отражает начальные значения задачи.
                if (baseItem.CreationDate == task.CreationDate) { continue; }

                if (baseItem.HistoryItemType == TaskHistoryItemType.TaskHistory)
                {
                    #region Заполнение модели для изменений в задаче

                    modelItem = new TaskHistoryItemViewModel();

                    var item = baseItem.As<TaskHistoryItem>();
                    if (item.IsContractorChanged)
                    {
                        modelItem.IsContractorChange = true;
                        modelItem.AllowToViewContractorNew = item.Contractor != null ? contractorPermissions[item.Contractor.Id] : false;
                        modelItem.AllowToViewContractorOld = previousTaskHistoryItems.Contractor != null ? contractorPermissions[previousTaskHistoryItems.Contractor.Id] : false;
                        modelItem.ContractorNameNew = item.Contractor != null ? item.Contractor.Name : "---";
                        modelItem.ContractorNameOld = previousTaskHistoryItems.Contractor != null ? previousTaskHistoryItems.Contractor.Name : "---";
                        modelItem.ContractorNewId = item.Contractor != null ? item.Contractor.Id.ToString() : "";
                        modelItem.ContractorOldId = previousTaskHistoryItems.Contractor != null ? previousTaskHistoryItems.Contractor.Id.ToString() : "";
                        modelItem.ContractorTypeNew = item.Contractor != null ? item.Contractor.ContractorType.ValueToString() : "";
                        modelItem.ContractorTypeOld = previousTaskHistoryItems.Contractor != null ? previousTaskHistoryItems.Contractor.ContractorType.ValueToString() : "";
                        previousTaskHistoryItems.Contractor = item.Contractor;
                    }
                    if (item.IsDeadLineChanged)
                    {
                        modelItem.IsDeadLineChange = true;
                        modelItem.DeadLineNew = item.Deadline.ForDisplay();
                        modelItem.DeadLineOld = previousTaskHistoryItems.DeadLine.ForDisplay();
                        previousTaskHistoryItems.DeadLine = item.Deadline;
                    }
                    if (item.IsDealChanged)
                    {
                        modelItem.IsDealChange = true;
                        modelItem.AllowToViewDealNew = item.Deal != null ? dealPermissons[item.Deal.Id] : false;
                        modelItem.AllowToViewDealOld = previousTaskHistoryItems.Deal != null ? dealPermissons[previousTaskHistoryItems.Deal.Id] : false;
                        modelItem.DealNameNew = item.Deal != null ? item.Deal.Name : "---";
                        modelItem.DealNameOld = previousTaskHistoryItems.Deal != null ? previousTaskHistoryItems.Deal.Name : "---";
                        modelItem.DealNewId = item.Deal != null ? item.Deal.Id.ToString() : "";
                        modelItem.DealOldId = previousTaskHistoryItems.Deal != null ? previousTaskHistoryItems.Deal.Id.ToString() : "";
                        previousTaskHistoryItems.Deal = item.Deal;
                    }
                    if (item.IsDeletionDateChanged)
                    {
                        modelItem.IsDeletionDateChange = true;
                        modelItem.DeletionDateNew = item.DeletionDate != null ? item.DeletionDate.ForDisplay() : "---";
                        modelItem.DeletionDateOld = previousTaskHistoryItems.DeletionDate != null ? previousTaskHistoryItems.DeletionDate.ForDisplay() : "---";
                        previousTaskHistoryItems.DeletionDate = item.DeletionDate;
                    }
                    if (item.IsDescriptionChanged)
                    {
                        modelItem.IsDescriptionChange = true;
                        previousTaskHistoryItems.Description = item.Description;
                    }
                    if (item.IsExecutedByChanged)
                    {
                        modelItem.IsExecutedByChange = true;
                        modelItem.AllowToViewExecutedByNew = item.ExecutedBy!= null ? userPermissons[item.ExecutedBy.Id] : false;
                        modelItem.AllowToViewExecutedByOld = previousTaskHistoryItems.ExecutedBy!= null ? userPermissons[previousTaskHistoryItems.ExecutedBy.Id] : false;
                        modelItem.ExecutedByNameNew = item.ExecutedBy!= null ? item.ExecutedBy.DisplayName : "---";
                        modelItem.ExecutedByNameOld = previousTaskHistoryItems.ExecutedBy!= null ? previousTaskHistoryItems.ExecutedBy.DisplayName : "---";
                        modelItem.ExecutedByNewId = item.ExecutedBy != null ? item.ExecutedBy.Id.ToString() : "";
                        modelItem.ExecutedByOldId = previousTaskHistoryItems.ExecutedBy != null ? previousTaskHistoryItems.ExecutedBy.Id.ToString() : "";
                        previousTaskHistoryItems.ExecutedBy = item.ExecutedBy;
                    }
                    if (item.IsFactualCompletionDateChanged)
                    {
                        modelItem.IsFactualCompletionDateChange = true;
                        modelItem.FactualCompletionDateNew = item.FactualCompletionDate != null ? item.FactualCompletionDate.ForDisplay() : "---";
                        modelItem.FactualCompletionDateOld = previousTaskHistoryItems.FactualCompletionDate != null ? previousTaskHistoryItems.FactualCompletionDate.ForDisplay() : "---";
                        previousTaskHistoryItems.FactualCompletionDate = item.FactualCompletionDate;
                    }
                    if (item.IsFactualSpentTimeChanged)
                    {
                        modelItem.IsFactualSpentTimeChange = true;
                        modelItem.FactualSpentTimeNew = item.FactualSpentTime != null ? MinutesToTimeDisplay(item.FactualSpentTime.Value) : "---";
                        modelItem.FactualSpentTimeOld = previousTaskHistoryItems.FactualSpentTime != null ? MinutesToTimeDisplay(previousTaskHistoryItems.FactualSpentTime.Value) : "---";
                        previousTaskHistoryItems.FactualSpentTime = item.FactualSpentTime;
                    }
                    if (item.IsTaskPriorityChanged)
                    {
                        modelItem.IsPriorityChange = true;
                        modelItem.PriorityNameNew = item.TaskPriority != null ? item.TaskPriority.Name : "---";
                        modelItem.PriorityNameOld = previousTaskHistoryItems.TaskPriority != null ? previousTaskHistoryItems.TaskPriority.Name : "---";
                        previousTaskHistoryItems.TaskPriority = item.TaskPriority;
                    }
                    if (item.IsProductionOrderChanged)
                    {
                        modelItem.IsProductionOrderChange = true;
                        modelItem.AllowToViewProductionOrderNew = item.ProductionOrder != null ? productionOrderPermissons[item.ProductionOrder.Id] : false;
                        modelItem.AllowToViewProductionOrderOld = previousTaskHistoryItems.ProductionOrder != null ? productionOrderPermissons[previousTaskHistoryItems.ProductionOrder.Id] : false;
                        modelItem.ProductionOrderNameNew = item.ProductionOrder!= null ? item.ProductionOrder.Name : "---";
                        modelItem.ProductionOrderNameOld = previousTaskHistoryItems.ProductionOrder!= null ? previousTaskHistoryItems.ProductionOrder.Name : "---";
                        modelItem.ProductionOrderNewId = item.ProductionOrder != null ? item.ProductionOrder.Id.ToString() : "";
                        modelItem.ProductionOrderOldId = previousTaskHistoryItems.ProductionOrder != null ? previousTaskHistoryItems.ProductionOrder.Id.ToString() : "";
                        previousTaskHistoryItems.ProductionOrder = item.ProductionOrder;
                    }
                    if (item.IsStartDateChanged)
                    {
                        modelItem.IsStartDateChange = true;
                        modelItem.StartDateNew = item.StartDate.ForDisplay();
                        modelItem.StartDateOld = previousTaskHistoryItems.StartDate.ForDisplay();
                        previousTaskHistoryItems.StartDate = item.StartDate;
                    }
                    if (item.IsTaskExecutionStateChanged)
                    {
                        modelItem.IsStateChange = true;
                        modelItem.StateNameNew = item.TaskExecutionState!= null ? item.TaskExecutionState.Name : "---";
                        modelItem.StateNameOld = previousTaskHistoryItems.TaskExecutionState != null ? previousTaskHistoryItems.TaskExecutionState.Name : "---";
                        previousTaskHistoryItems.TaskExecutionState = item.TaskExecutionState;
                    }
                    if (item.IsTopicChanged)
                    {
                        modelItem.IsTopicChange = true;
                        modelItem.TopicNew = item.Topic;
                        modelItem.TopicOld = previousTaskHistoryItems.Topic;
                        previousTaskHistoryItems.Topic = item.Topic;
                    }
                    if (item.IsTaskTypeChanged)
                    {
                        modelItem.IsTypeChange = true;
                        modelItem.TypeNameNew = item.TaskType != null ? item.TaskType.Name : "---";
                        modelItem.TypeNameOld = previousTaskHistoryItems.TaskType != null ? previousTaskHistoryItems.TaskType.Name : "---";
                        previousTaskHistoryItems.TaskType = item.TaskType;
                    }

                    modelItem.CreatedByName = item.CreatedBy.DisplayName;
                    modelItem.CreatedById = item.CreatedBy.Id.ToString();
                    modelItem.AllowToViewCreatedBy = userPermissons[item.CreatedBy.Id];
                    modelItem.CreationDate = GetDateWithTextMonthFormatAndTime(item.CreationDate);

                    #endregion
                }
                else
                {
                    #region Заполнение модели для изменений в исполнении задачи

                    var item = baseItem.As<TaskExecutionHistoryItem>();

                    if (item.TaskExecutionItem.IsCreatedByUser)
                    {
                        // исполнение создано пользователем

                        modelItem = new TaskHistoryItemViewModel();

                        if (!previousTaskExecutionHistoryItems.Keys.Contains(item.TaskExecutionItem.Id))
                        {
                            modelItem.IsTaskExecutionAdded = true;
                            previousTaskExecutionHistoryItems.Add(item.TaskExecutionItem.Id, new TaskExecutionHistoryItem[6] { null, null, null, null, null, null });
                        }

                        modelItem.TaskExecutionCurrentDate = GetDateWithTextMonthFormatAndTime(item.TaskExecutionItem.Date);
                        modelItem.TaskExecutionCreatedById = item.CreatedBy.Id.ToString();
                        modelItem.TaskExecutionCreatedByName = item.CreatedBy.DisplayName;
                        modelItem.AllowToViewTaskExecutionCreaetdBy = userPermissons[item.CreatedBy.Id];

                        if (item.IsDateChanged)
                        {
                            var previousItem = previousTaskExecutionHistoryItems[item.TaskExecutionItem.Id][0];
                            modelItem.IsTaskExecutionDateChange = true;
                            modelItem.TaskExecutionDateOld = previousItem != null ? previousItem.Date.ForDisplay() : "";
                            modelItem.TaskExecutionDateNew = item.Date.ForDisplay();
                            previousTaskExecutionHistoryItems[item.TaskExecutionItem.Id][0] = item;
                        }
                        if (item.IsDeletionDateChanged)
                        {
                            var previousItem = previousTaskExecutionHistoryItems[item.TaskExecutionItem.Id][1];
                            if (!modelItem.IsTaskExecutionAdded)  // Редактировалось ли исполнение
                            {
                                // Да, выводим изменение поля
                                modelItem.IsTaskExecutionDeletionDateChange = true;
                                modelItem.TaskExecutionDeletionDateOld = previousItem.DeletionDate.ForDisplay();
                                modelItem.TaskExecutionDeletionDateNew = item.DeletionDate.ForDisplay();
                            }
                            else
                            {
                                // Нет. Сравниваем значение со значением по умолчанию.
                                if (item.DeletionDate != null)    // Значение измелилось?
                                {
                                    // Да, выводим изменение поля
                                    modelItem.IsTaskExecutionDeletionDateChange = true;
                                    modelItem.TaskExecutionDeletionDateOld = "";
                                    modelItem.TaskExecutionDeletionDateNew = item.DeletionDate.ForDisplay();
                                }
                            }
                            previousTaskExecutionHistoryItems[item.TaskExecutionItem.Id][1] = item;
                        }
                        if (item.IsResultDescriptionChanged)
                        {
                            var previousItem = previousTaskExecutionHistoryItems[item.TaskExecutionItem.Id][2];
                            if (!modelItem.IsTaskExecutionAdded)  // Редактировалось ли исполнение
                            {
                                // Да, выводим изменение поля
                                modelItem.IsTaskExecutionResultDescriptionChange = true;
                            }
                            else
                            {
                                // Нет. Сравниваем значение со значением по умолчанию.
                                if (item.ResultDescription != "")    // Значение измелилось?
                                {
                                    // Да, выводим изменение поля
                                    modelItem.IsTaskExecutionResultDescriptionChange = true;
                                }
                            }
                            previousTaskExecutionHistoryItems[item.TaskExecutionItem.Id][2] = item;
                        }
                        if (item.IsSpentTimeChanged)
                        {
                            var previousItem = previousTaskExecutionHistoryItems[item.TaskExecutionItem.Id][3];
                            if (!modelItem.IsTaskExecutionAdded)  // Редактировалось ли исполнение
                            {
                                // Да, выводим изменение поля
                                modelItem.IsTaskExecutionSpentTimeChange = true;
                                modelItem.TaskExecutionSpentTimeOld = MinutesToTimeDisplay(previousItem.SpentTime.Value);
                                modelItem.TaskExecutionSpentTimeNew = MinutesToTimeDisplay(item.SpentTime.Value);
                            }
                            else
                            {
                                // Нет. Сравниваем значение со значением по умолчанию.
                                if (item.SpentTime != 0)    // Значение измелилось?
                                {
                                    // Да, выводим изменение поля
                                    modelItem.IsTaskExecutionSpentTimeChange = true;
                                    modelItem.TaskExecutionSpentTimeOld = "";
                                    modelItem.TaskExecutionSpentTimeNew = MinutesToTimeDisplay(item.SpentTime.Value);
                                }
                            }
                            previousTaskExecutionHistoryItems[item.TaskExecutionItem.Id][3] = item;
                        }
                        if (item.IsCompletionPercentageChanged)
                        {
                            var previousItem = previousTaskExecutionHistoryItems[item.TaskExecutionItem.Id][4];
                            if (!modelItem.IsTaskExecutionAdded)  // Редактировалось ли исполнение
                            {
                                // Да, выводим изменение поля
                                modelItem.IsCompletionPercentageChange = true;
                                modelItem.CompletionPercentageOld = previousItem.CompletionPercentage.Value.ForDisplay();
                                modelItem.CompletionPercentageNew = item.CompletionPercentage.ToString();
                            }
                            else
                            {
                                // Нет. Сравниваем значение со значением по умолчанию.
                                if (item.CompletionPercentage != 0)    // Значение измелилось?
                                {
                                    // Да, выводим изменение поля
                                    modelItem.IsCompletionPercentageChange = true;
                                    modelItem.CompletionPercentageOld = "";
                                    modelItem.CompletionPercentageNew = item.CompletionPercentage.ToString();
                                }
                            }
                            previousTaskExecutionHistoryItems[item.TaskExecutionItem.Id][4] = item;
                        }
                        if (item.IsTaskExecutionStateChanged)
                        {
                            var previousItem = previousTaskExecutionHistoryItems[item.TaskExecutionItem.Id][5];
                            if (!modelItem.IsTaskExecutionAdded)  // Редактировалось ли исполнение
                            {
                                // Да, выводим изменение поля
                                modelItem.IsTaskExecutionStateChange = true;
                                modelItem.TaskExecutionStateNameOld = previousItem.TaskExecutionState.Name;
                                modelItem.TaskExecutionStateNameNew = item.TaskExecutionState.Name;
                            }
                            else
                            {
                                // Нет. Сравниваем значение со значением по умолчанию.
                                if (item.TaskExecutionState.ExecutionStateType != TaskExecutionStateType.New)   // Значение измелилось?
                                {
                                    // Да, выводим изменение поля
                                    modelItem.IsTaskExecutionStateChange = true;
                                    modelItem.TaskExecutionStateNameOld = "";
                                    modelItem.TaskExecutionStateNameNew = item.TaskExecutionState.Name;
                                }
                            }
                            previousTaskExecutionHistoryItems[item.TaskExecutionItem.Id][5] = item;
                            previousTaskHistoryItems.TaskExecutionState = item.TaskExecutionState;
                        }
                    }

                    #endregion
                }

                // Пропускаем все элементы истории, ранее указанного исполнения
                if ((taskExecution == null || baseItem.CreationDate >= taskExecution.CreationDate) && modelItem != null)
                {
                    model.History.Add(modelItem);   //Добавляем элемент в коллекцию
                }
            }

            model.NeedDelimFirstItem = taskExecution != null ? task.ExecutionHistory.Count() > 2 : false;

            return model;
        }

        /// <summary>
        /// Получение изменяемых индикаторов главных деталей
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public object GetMainChangeableIndicators(int taskId, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var task = taskService.CheckTaskExistence(taskId, user);

                return new
                {
                    StartDate = task.StartDate != null ? task.StartDate.Value.ToFullDateTimeString() : "---",
                    FactualCompletionDate = task.FactualCompletionDate != null ? task.FactualCompletionDate.Value.ToFullDateTimeString() : "---",
                    FactualSpentTime = task.FactualSpentTime > 0 ? MinutesToTimeDisplay(task.FactualSpentTime) : "---",
                    ExecutionState = task.ExecutionState.Name,
                    CompletionPercentage = task.CompletionPercentage.ForDisplay()
                };
            }
        }

        #endregion

        #region Исполнение задачи

        /// <summary>
        /// Создание исполнения
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="isCompleteTask">Признак завершения задачи</param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public TaskExecutionEditViewModel TaskExecutionCreate(int taskId, bool isCompleteTask, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var task = taskService.CheckTaskExistence(taskId, user);

                taskExecutionItemService.CheckPossibilityToCreateTaskExecution(task, user);

                var currentDate = DateTimeUtils.GetCurrentDateTime();
                var model = new TaskExecutionEditViewModel();
                model.Title = isCompleteTask ? "Завершение задачи" : "Добавление исполнения";
                model.TaskId = task.Id.ToString();
                model.AllowToChangeDate = taskExecutionItemService.IsPossibilityToEditExecutionDate(null, user);
                model.Date = currentDate.ToShortDateString();
                model.Time = GetFullTime(currentDate);
                model.ExecutionId = 0;

                model.CompletionPercentage = task.CompletionPercentage;
                model.TaskExecutionStateId = task.ExecutionState.Id.ToString();

                var states = isCompleteTask ? task.Type.States.Where(x => x.ExecutionStateType == TaskExecutionStateType.Completed) : task.Type.States;

                model.TaskExecutionStateList = ComboBoxBuilder.GetComboBoxItemList(states.OrderBy(x => x.OrdinalNumber), x => x.Name, x => x.Id.ToString(), false, false);


                return model;
            }
        }

        /// <summary>
        /// Редактирование исполнения
        /// </summary>
        /// <param name="taskExecutionId">Исполнение</param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public TaskExecutionEditViewModel TaskExecutionEdit(int taskExecutionId, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var taskExecution = taskExecutionItemService.CheckTaskExecutionItemExistence(taskExecutionId);
                var taskType = taskTypeService.GetTaskTypeByExecutionState(taskExecution.ExecutionState);

                ValidationUtils.Assert(taskExecution.IsCreatedByUser, "Невозможно отредактировать системное исполнение.");

                taskExecutionItemService.CheckPossibilityToEditTaskExecution(taskExecution, user);

                var hours = taskExecution.SpentTime / 60;
                var minutes = taskExecution.SpentTime - hours * 60;

                var model = new TaskExecutionEditViewModel();
                model.AllowToChangeDate = taskExecutionItemService.IsPossibilityToEditExecutionDate(taskExecution, user);
                model.Date = taskExecution.Date.ToShortDateString();
                model.Time = GetFullTime(taskExecution.Date);
                model.SpentTime_Hours = hours > 0 ? hours : (int?)null;
                model.SpentTime_Minutes = minutes > 0 ? minutes : (int?)null;
                model.ResultDescription = taskExecution.ResultDescription;
                model.ExecutionId = taskExecution.Id;
                model.TaskId = taskExecution.Task.Id.ToString();

                model.CompletionPercentage = taskExecution.CompletionPercentage;
                model.TaskExecutionStateId = taskExecution.ExecutionState.Id.ToString();

                model.TaskExecutionStateList = ComboBoxBuilder.GetComboBoxItemList(taskType.States.OrderBy(x => x.OrdinalNumber), x => x.Name, x => x.Id.ToString(), false, false);

                return model;
            }
        }

        /// <summary>
        /// Сохранение исполнения
        /// </summary>
        /// <param name="model"></param>
        public object TaskExecutionSave(TaskExecutionEditViewModel model, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                ValidationUtils.NotNull(model, "Неверный входной параметр.");

                var user = userService.CheckUserExistence(currentUser.Id);
                var taskId = ValidationUtils.TryGetInt(model.TaskId, "Указан неверный идентификатор задачи.");
                var task = taskService.CheckTaskExistence(taskId, user);
                var currentDate = DateTimeUtils.GetCurrentDateTime();
                DateTime date;
                var spentTime = (model.SpentTime_Hours != null ? model.SpentTime_Hours.Value * 60 : 0) +
                        (model.SpentTime_Minutes != null ? model.SpentTime_Minutes.Value : 0);
                var resultDescription = StringUtils.ToHtml(model.ResultDescription);
                var stateId = ValidationUtils.TryGetInt(model.TaskExecutionStateId, "Указано неверное значение идентификатора статуса задачи.");

                ValidationUtils.Assert(0 <= model.CompletionPercentage && model.CompletionPercentage <= 100, "Процент выполнения задачи должен принадлежать отрезку [0;100].");

                TaskExecutionItem taskExecution;

                if (model.ExecutionId == 0)
                {
                    var state = task.Type.States.Where(x => x.Id == stateId).FirstOrDefault();
                    ValidationUtils.NotNull(state, "Статус задачи не найден. Возможно, он был удален.");

                    if (taskExecutionItemService.IsPossibilityToEditExecutionDate(null, user))
                    {
                        date = ValidationUtils.TryGetDate(model.Date + " " + model.Time, "Указано неверное значение даты исполнения.");
                        ValidationUtils.Assert(date <= currentDate, "Дата исполнения не может превышать текущую дату.");
                    }
                    else
                    {
                        date = currentDate;
                    }
                    taskExecution = new TaskExecutionItem(task, task.Type, state, model.CompletionPercentage, user, true, currentDate, date);
                    taskExecution.ResultDescription = resultDescription;
                    taskExecution.SpentTime = spentTime;
                }
                else
                {
                    taskExecution = taskExecutionItemService.CheckTaskExecutionItemExistence(model.ExecutionId);
                    ValidationUtils.Assert(taskExecution.IsCreatedByUser, "Невозможно отредактировать системное исполнение.");

                    var taskType = taskTypeService.GetTaskTypeByExecutionState(taskExecution.ExecutionState);
                    ValidationUtils.NotNull(taskType, "Тип задачи не найден. Возможно, он был удален.");

                    var state = taskType.States.Where(x => x.Id == stateId).FirstOrDefault();
                    ValidationUtils.NotNull(state, "Статус задачи не найден. Возможно, он был удален.");

                    if (taskExecutionItemService.IsPossibilityToEditExecutionDate(taskExecution, user))
                    {
                        date = ValidationUtils.TryGetDate(model.Date + " " + model.Time, "Указано неверное значение даты исполнения.");
                        ValidationUtils.Assert(date <= currentDate, "Дата исполнения не может превышать текущую дату.");
                    }
                    else
                    {
                        date = currentDate;
                    }
                    taskExecution.Date = date;
                    taskExecution.ExecutionState = state;
                    taskExecution.CompletionPercentage = model.CompletionPercentage;
                    taskExecution.ResultDescription = resultDescription;
                    taskExecution.SpentTime = spentTime;
                }

                taskExecutionItemService.Save(taskExecution, currentDate, user); //Сохраняем исполнение и историю изменений

                uow.Commit();

                return new { Message = "Исполнение сохранено.", TaskExecutionId = taskExecution.Id };
            }
        }

        /// <summary>
        /// Удаление исполнения
        /// </summary>
        /// <param name="taskExecutionId"></param>
        /// <param name="message">Сообщение</param>
        /// <param name="currentUser"></param>
        public void TaskExecutionDelete(int taskExecutionId, out string message, UserInfo currentUser)
        {
            using (var uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var taskExecution = taskExecutionItemService.CheckTaskExecutionItemExistence(taskExecutionId);
                var currentDate = DateTimeUtils.GetCurrentDateTime();
                var task = taskExecution.Task;

                taskExecutionItemService.CheckPossibilityToDeleteTaskExecution(taskExecution, user);

                taskExecutionItemService.Delete(taskExecution, currentDate, user);

                uow.Commit();

                message = "Исполнение удалено.";
            }
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>
        /// Формирование словаря разрешений
        /// </summary>
        /// <typeparam name="T">Тип сущности</typeparam>
        /// <typeparam name="TId">Тип идентификатора</typeparam>
        /// <param name="list">Список сущностей</param>
        /// <param name="getKey">Лямбда получения идентификатора (ключа словаря)</param>
        /// <param name="getValue">Лямбда получения разрешения (значения словаря))</param>
        /// <returns>Словарь разрешений (ключ - идентификатор; значение - разрешение)</returns>
        private IDictionary<TId, bool> GetPermissions<T, TId>(IEnumerable<T> list, Func<T, TId> getKey, Func<T, bool> getValue)
        {
            var result = new Dictionary<TId, bool>();
            foreach (var item in list)
            {
                result.Add(getKey.Invoke(item), getValue.Invoke(item));
            }

            return result;
        }

        /// <summary>
        /// Определение права на просмотр деталей контрагента
        /// </summary>
        /// <param name="contractor"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private bool IsPossibilityToViewContractor(Contractor contractor, User user)
        {
            bool result = false;

            switch (contractor.ContractorType)
            {
                case ContractorType.Client:
                    result = user.HasPermission(Permission.Client_List_Details);
                    break;
                case ContractorType.Producer:
                    result = user.HasPermission(Permission.Producer_List_Details);
                    break;
                case ContractorType.Provider:
                    result = user.HasPermission(Permission.Provider_List_Details);
                    break;
                default:
                    throw new Exception(String.Format("Неизвестный тип контрагента «{0}».", contractor.ContractorType.GetDisplayName()));
            }

            return result;
        }

        /// <summary>
        /// Приведенеи времени в минутах к формату вывода
        /// </summary>
        /// <param name="minutes">Время в минутах (должно быть >0)</param>
        /// <returns></returns>
        private string MinutesToTimeDisplay(int minutes)
        {
            var hours = minutes / 60;
            var mins = minutes - hours * 60;

            var strHours = hours > 0 ? hours.ToString() + " ч." : "";
            var strMins = mins > 0 ? mins.ToString() + " мин." : "";

            return minutes > 0 ? strHours + (!String.IsNullOrEmpty(strHours) && !String.IsNullOrEmpty(strMins) ? " " : "") + strMins : "---";
        }

        /// <summary>
        /// Приведение даты к формату "DD Month YYYY, HH:MM"
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private string GetDateWithTextMonthFormatAndTime(DateTime date)
        {
            return date.Day.ToString() + " " + DateTimeUtils.GetGenitiveMonthName(date.Month) + " " + date.Year.ToString() + ", " + date.ToShortTimeString();
        }

        /// <summary>
        /// Получения времени в формате "HH:MM:SS"
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private string GetFullTime(DateTime date)
        {
            return date.Hour.ToString("00") + ":" + date.Minute.ToString("00") + ":" + date.Second.ToString("00");
        }

        #endregion

        #endregion
    }
}