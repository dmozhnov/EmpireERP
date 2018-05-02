using System;
using System.Web.UI.WebControls;
using ERP.UI.ViewModels.Grid;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters.Mediators;


namespace ERP.Wholesale.UI.LocalPresenters.Mediators
{
    public class TaskPresenterMediator : ITaskPresenterMediator
    {
        #region Поля

        private readonly ITaskService taskService;
        private readonly IUserService userService;

        #endregion

        #region Конструктор

        public TaskPresenterMediator(ITaskService taskService, IUserService userService)
        {
            this.taskService = taskService;
            this.userService = userService;
        }

        #endregion

        #region Методы

        #region Общие методы

        /// <summary>
        /// Определение статуса строки грида (цвет строки)
        /// </summary>
        /// <param name="processExecutingTask">Подсвечивать ли задачи, находящиеся в работе</param>
        private GridRowStyle GetRowStyle(Task task, DateTime currentDate, bool processExecutingTask)
        {
            GridRowStyle result = GridRowStyle.Normal;
            if (processExecutingTask && task.ExecutionState.ExecutionStateType == TaskExecutionStateType.Executing)
            {
                result = GridRowStyle.Success;  // Задача в работе
            }
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

        #endregion

        #region Связанные с задачей сущности

        /// <summary>
        /// Формирование грида задач
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private GridData GetTaskGridLocalForLinkedEntity(GridState state, User user)
        {
            if (state == null) state = new GridState();
            GridData model = new GridData();

            var currentDate = DateTimeUtils.GetCurrentDateTime();
            var ps = new ParameterString(state.Parameters);

            model.ButtonPermissions["AllowToCreateNewTask"] = taskService.IsPossibilityToCreateTask(user);
            model.Title = "Связанные задачи и мероприятия";

            model.AddColumn("Topic", "Тема", Unit.Percentage(75));
            model.AddColumn("Type", "Тип", Unit.Pixel(80));
            model.AddColumn("Priority", "Приоритет", Unit.Pixel(80));
            model.AddColumn("ExecutionState", "Состояние", Unit.Pixel(80));
            model.AddColumn("CompletionPercantage", "% выполнения", Unit.Pixel(75), align: GridColumnAlign.Right);
            model.AddColumn("StartDate", "Дата начала исполнения", Unit.Pixel(80));
            model.AddColumn("DeadLine", "Требуемое завершение", Unit.Pixel(80));
            model.AddColumn("ExecutedBy", "Ответственный", Unit.Percentage(25));
            model.AddColumn("Id", "", GridCellStyle.Hidden);
            model.AddColumn("ExecutedById", "", GridCellStyle.Hidden);

            var list = taskService.GetFilteredList(state, ps, user);

            foreach (var item in list)
            {
                var allowToViewExecutedBy = item.ExecutedBy != null ? userService.IsPossibilityToViewDetails(item.ExecutedBy, user) : false;

                model.AddRow(new GridRow(
                    new GridLinkCell("Topic") { Value = item.Topic },
                    new GridLabelCell("Type") { Value = item.Type.Name },
                    new GridLabelCell("Priority") { Value = item.Priority.Name },
                    new GridLabelCell("ExecutionState") { Value = item.ExecutionState.Name },
                    new GridLabelCell("CompletionPercantage") { Value = item.CompletionPercentage.ForDisplay() + "%" },
                    new GridLabelCell("StartDate") { Value = item.StartDate.ForDisplay() },
                    new GridLabelCell("DeadLine") { Value = item.DeadLine.ForDisplay() },
                    allowToViewExecutedBy ?
                        (GridCell)new GridLinkCell("ExecutedBy") { Value = item.ExecutedBy.DisplayName } :
                        (GridCell)new GridLabelCell("ExecutedBy") { Value = item.ExecutedBy != null ? item.ExecutedBy.DisplayName : "---" },
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "Id" },
                    new GridHiddenCell("ExecutedById") { Value = item.ExecutedBy != null ? item.ExecutedBy.Id.ToString() : "", Key = "ExecutedById" })
                {
                    Style = GetRowStyle(item, currentDate, true)
                });
            }
            model.State = state;

            return model;
        }

        #region Поставщик

        /// <summary>
        /// Формирование грида для поставщика
        /// </summary>
        /// <param name="provider">Поставщик, для которого выводится грид</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        public GridData GetTaskGridForProvider(Provider provider, User user)
        {
            var state = new GridState() { Parameters = "Contractor=" + provider.Id.ToString(), Sort = "DeadLine=Desc;" };

            return GetTaskGridLocalForLinkedEntity(state, user);
        }

        /// <summary>
        /// Формирование грида для поставщика
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        public GridData GetTaskGridForProvider(GridState state, User user)
        {
            return GetTaskGridLocalForLinkedEntity(state, user);
        }

        #endregion

        #region Производитель

        /// <summary>
        /// Формирование грида для производителя
        /// </summary>
        /// <param name="producer">Производитель, для которого выводится грид</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        public GridData GetTaskGridForProducer(Producer producer, User user)
        {
            var state = new GridState() { Parameters = "Contractor=" + producer.Id.ToString(), Sort = "DeadLine=Desc;" };

            return GetTaskGridLocalForLinkedEntity(state, user);
        }

        /// <summary>
        /// Формирование грида для производителя
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        public GridData GetTaskGridForProducer(GridState state, User user)
        {
            return GetTaskGridLocalForLinkedEntity(state, user);
        }

        #endregion

        #region Клиент

        /// <summary>
        /// Формирование грида для клиента
        /// </summary>
        /// <param name="client">Клиент, для которого выводится грид</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        public GridData GetTaskGridForClient(Client client, User user)
        {
            var state = new GridState() { Parameters = "Contractor=" + client.Id.ToString(), Sort = "DeadLine=Desc;" };

            return GetTaskGridLocalForLinkedEntity(state, user);
        }

        /// <summary>
        /// Формирование грида для клиента
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        public GridData GetTaskGridForClient(GridState state, User user)
        {
            return GetTaskGridLocalForLinkedEntity(state, user);
        }

        #endregion

        #region Сделка

        /// <summary>
        /// Формирование грида для сделок
        /// </summary>
        /// <param name="deal">Сделка, для которой выводится грид</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        public GridData GetTaskGridForDeal(Deal deal, User user)
        {
            var state = new GridState() { Parameters = "Deal.Id=" + deal.Id.ToString(), Sort = "DeadLine=Desc;" };

            return GetTaskGridLocalForLinkedEntity(state, user);
        }

        /// <summary>
        /// Формирование грида для клиента
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        public GridData GetTaskGridForDeal(GridState state, User user)
        {
            return GetTaskGridLocalForLinkedEntity(state, user);
        }

        #endregion

        #region Заказ

        /// <summary>
        /// Формирование грида для заказов
        /// </summary>
        /// <param name="productionOrder">Заказ, для которого выводится грид</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        public GridData GetTaskGridForProductionOrder(ProductionOrder productionOrder, User user)
        {
            var state = new GridState() { Parameters = "ProductionOrder.Id=" + productionOrder.Id.ToString(), Sort = "DeadLine=Desc;" };

            return GetTaskGridLocalForLinkedEntity(state, user);
        }

        /// <summary>
        /// Формирование грида для заказов
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        public GridData GetTaskGridForProductionOrder(GridState state, User user)
        {
            return GetTaskGridLocalForLinkedEntity(state, user);
        }

        #endregion

        #endregion

        #region Пользователи

        #region Детали пользователя

        /// <summary>
        /// Формирование грида задач
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private GridData GetTaskGridLocalForUser(GridState state, User user)
        {
            if (state == null) state = new GridState();
            GridData model = new GridData();

            var currentDate = DateTimeUtils.GetCurrentDateTime();
            var psIn = new ParameterString(state.Parameters);
            var psOut = new ParameterString(state.Filter);

            // Добавляем условие вывода задач для конкретного пользователя
            var forUserId = psIn["UserId"].Value as string;
            psOut.Add(ParameterStringItem.OperationType.Or,
                new ParameterStringItem("CreatedBy", ParameterStringItem.OperationType.Eq, forUserId),
                new ParameterStringItem("ExecutedBy", ParameterStringItem.OperationType.Eq, forUserId));

            var type = (TaskExecutionStateType)Convert.ToInt16(psIn["ExecutionState.ExecutionStateType"].Value);
            psOut.Add("ExecutionState.ExecutionStateType", ParameterStringItem.OperationType.Eq, type.ValueToString());

            switch (type)
            {
                case TaskExecutionStateType.New:
                    var forUser = userService.CheckUserExistenceIgnoreBlocking(ValidationUtils.TryGetInt(forUserId));
                    model.ButtonPermissions["AllowToCreateNewTask"] = taskService.IsPossibilityToSetByExecutedBy(forUser, user);
                    model.Title = "Новые задачи и мероприятия";
                    break;
                case TaskExecutionStateType.Executing:
                    model.Title = "Задачи и мероприятия в работе";
                    break;
                case TaskExecutionStateType.Completed:
                    model.Title = "Завершенные задачи и мероприятия";
                    break;
            }

            model.AddColumn("Topic", "Тема", Unit.Percentage(50));
            model.AddColumn("Type", "Тип", Unit.Pixel(80));
            model.AddColumn("Priority", "Приоритет", Unit.Pixel(80));
            if (type == TaskExecutionStateType.Executing)
            {
                model.AddColumn("CompletionPercentage", "% выполнения", Unit.Pixel(75), align: GridColumnAlign.Right);
            }
            model.AddColumn("StartDate", "Дата начала исполнения", Unit.Pixel(80));
            model.AddColumn("DeadLine", "Требуемое завершение", Unit.Pixel(80));
            model.AddColumn("CreatedBy", "Автор", Unit.Percentage(25));
            model.AddColumn("ExecutedBy", "Ответственный", Unit.Percentage(25));
            model.AddColumn("Id", "", GridCellStyle.Hidden);
            model.AddColumn("CreatedById", "", GridCellStyle.Hidden);
            model.AddColumn("ExecutedById", "", GridCellStyle.Hidden);

            var list = taskService.GetFilteredList(state, psOut, user);

            foreach (var item in list)
            {
                var allowToViewCreatedBy = userService.IsPossibilityToViewDetails(item.CreatedBy, user);
                var allowToViewExecutedBy = item.ExecutedBy != null ? userService.IsPossibilityToViewDetails(item.ExecutedBy, user) : false;

                var row = new GridRow() { Style = GetRowStyle(item, currentDate, false) };

                row.AddCell(new GridLinkCell("Topic") { Value = item.Topic });
                row.AddCell(new GridLabelCell("Type") { Value = item.Type.Name });
                row.AddCell(new GridLabelCell("Priority") { Value = item.Priority.Name });
                if (type == TaskExecutionStateType.Executing)
                {
                    row.AddCell(new GridLabelCell("CompletionPercentage") { Value = item.CompletionPercentage.ForDisplay() + "%" });
                }
                row.AddCell(new GridLabelCell("StartDate") { Value = item.StartDate.ForDisplay() });
                row.AddCell(new GridLabelCell("DeadLine") { Value = item.DeadLine.ForDisplay() });
                row.AddCell(allowToViewCreatedBy ?
                    (GridCell)new GridLinkCell("CreatedBy") { Value = item.CreatedBy.DisplayName } :
                     (GridCell)new GridLabelCell("CreatedBy") { Value = item.CreatedBy.DisplayName });
                row.AddCell(allowToViewExecutedBy ?
                    (GridCell)new GridLinkCell("ExecutedBy") { Value = item.ExecutedBy.DisplayName } :
                    (GridCell)new GridLabelCell("ExecutedBy") { Value = item.ExecutedBy != null ? item.ExecutedBy.DisplayName : "---" });
                row.AddCell(new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "Id" });
                row.AddCell(new GridHiddenCell("CreatedById") { Value = item.CreatedBy.Id.ToString(), Key = "CreatedById" });
                row.AddCell(new GridHiddenCell("ExecutedById") { Value = item.ExecutedBy != null ? item.ExecutedBy.Id.ToString() : "", Key = "ExecutedById" });

                model.AddRow(row);
            }
            model.State = state;

            return model;
        }

        /// <summary>
        /// Получение грида задач для пользователя
        /// </summary>
        /// <param name="forUser">Пользователь, для которого ыводятся задачи</param>
        /// <param name="stateType">Статус задач для вывода</param>
        /// <param name="user">Текущий пользователь</param>
        /// <returns></returns>
        public GridData GetTaskGridForUser(User forUser, TaskExecutionStateType stateType, User user)
        {
            var paramTask = "";
            var sortTask = "";

            switch (stateType)
            {
                case TaskExecutionStateType.New:
                    paramTask = "ExecutionState.ExecutionStateType=" + EnumUtils.ValueToString(TaskExecutionStateType.New) + ";UserId=" + forUser.Id.ToString();
                    sortTask = "DeadLine=Asc;CreationDate=Asc;";
                    break;
                case TaskExecutionStateType.Executing:
                    paramTask = "ExecutionState.ExecutionStateType=" + EnumUtils.ValueToString(TaskExecutionStateType.Executing) + ";UserId=" + forUser.Id.ToString();
                    sortTask = "DeadLine=Asc;StartDate=Asc;";
                    break;
                case TaskExecutionStateType.Completed:
                    paramTask = "ExecutionState.ExecutionStateType=" + EnumUtils.ValueToString(TaskExecutionStateType.Completed) + ";UserId=" + forUser.Id.ToString();
                    sortTask = "FactualCompletionDate=Asc;StartDate=Asc;";
                    break;
            }
            var state = new GridState() { Parameters = paramTask, Sort = sortTask };

            return GetTaskGridLocalForUser(state, user);
        }

        /// <summary>
        /// Получение грида задач для пользователя
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="user">Текущий пользователь</param>
        /// <returns></returns>
        public GridData GetTaskGridForUser(GridState state, User user)
        {
            return GetTaskGridLocalForUser(state, user);
        }

        #endregion

        #region Домашняя старница пользователя

        /// <summary>
        /// Формирование грида задач для домашней страницы пользователя
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private GridData GetTaskGridLocalForUserHomePage(GridState state, User user)
        {
            GridData model = new GridData();

            var psIn = new ParameterString(state.Parameters);
            var psOut = new ParameterString(state.Filter);

            // Добавляем условие вывода задач
            var isUserAsExecutor = psIn["UserAsExecutor"].Value as string;
            if (isUserAsExecutor == "1")
            {
                psOut.Add("ExecutedBy", ParameterStringItem.OperationType.Eq, user.Id.ToString());
                model.Title = "Задачи и мероприятия, назначенные мне";
            }
            else
            {
                psOut.Add("CreatedBy", ParameterStringItem.OperationType.Eq, user.Id.ToString());
                model.Title = "Задачи и мероприятия, созданные мной";
                model.ButtonPermissions["AllowToCreateNewTask"] = taskService.IsPossibilityToCreateTask(user);
            }


            model.AddColumn("Topic", "Тема", Unit.Percentage(50));
            model.AddColumn("Type", "Тип", Unit.Pixel(80));
            model.AddColumn("Priority", "Приоритет", Unit.Pixel(80));
            model.AddColumn("ExecutionState", "Состояние", Unit.Pixel(80));
            model.AddColumn("CompletionPercantage", "% выполнения", Unit.Pixel(75), align: GridColumnAlign.Right);
            model.AddColumn("StartDate", "Дата начала исполнения", Unit.Pixel(80));
            model.AddColumn("DeadLine", "Требуемое завершение", Unit.Pixel(80));
            if (isUserAsExecutor == "1")
            {
                model.AddColumn("CreatedBy", "Автор", Unit.Percentage(25));
            }
            else
            {
                model.AddColumn("ExecutedBy", "Ответственный", Unit.Percentage(25));
            }
            model.AddColumn("Id", "", GridCellStyle.Hidden);
            model.AddColumn("CreatedById", "", GridCellStyle.Hidden);
            model.AddColumn("ExecutedById", "", GridCellStyle.Hidden);

            var list = taskService.GetFilteredList(state, psOut, user);
            var currentDate = DateTimeUtils.GetCurrentDateTime();

            foreach (var item in list)
            {
                var allowToViewCreatedBy = userService.IsPossibilityToViewDetails(item.CreatedBy, user);
                var allowToViewExecutedBy = item.ExecutedBy != null ? userService.IsPossibilityToViewDetails(item.ExecutedBy, user) : false;

                model.AddRow(new GridRow(
                    new GridLinkCell("Topic") { Value = item.Topic },
                    new GridLabelCell("Type") { Value = item.Type.Name },
                    new GridLabelCell("Priority") { Value = item.Priority.Name },
                    new GridLabelCell("ExecutionState") { Value = item.ExecutionState.Name },
                    new GridLabelCell("CompletionPercantage") { Value = item.CompletionPercentage.ForDisplay() + "%" },
                    new GridLabelCell("StartDate") { Value = item.StartDate.ForDisplay() },
                    new GridLabelCell("DeadLine") { Value = item.DeadLine.ForDisplay() },
                    isUserAsExecutor == "1" ?
                        (allowToViewCreatedBy ?
                                (GridCell)new GridLinkCell("CreatedBy") { Value = item.CreatedBy.DisplayName } :
                                (GridCell)new GridLabelCell("CreatedBy") { Value = item.CreatedBy.DisplayName })
                        :
                        (allowToViewExecutedBy ?
                                    (GridCell)new GridLinkCell("ExecutedBy") { Value = item.ExecutedBy.DisplayName } :
                                    (GridCell)new GridLabelCell("ExecutedBy") { Value = item.ExecutedBy != null ? item.ExecutedBy.DisplayName : "---" }),
                    new GridHiddenCell("Id") { Value = item.Id.ToString(), Key = "Id" },
                    isUserAsExecutor == "1" ?
                        new GridHiddenCell("CreatedById") { Value = item.CreatedBy.Id.ToString(), Key = "CreatedById" } :
                        new GridHiddenCell("ExecutedById") { Value = item.ExecutedBy != null ? item.ExecutedBy.Id.ToString() : "", Key = "ExecutedById" })
                    {
                        Style = GetRowStyle(item, currentDate, true)
                    });
            }
            model.State = state;

            return model;
        }

        /// <summary>
        /// Получение грида задач для домашней страницы пользователя
        /// </summary>
        /// <param name="userAsExecutor">Признак вывода грида где пользователь исполнитель. False - пользователь автор задачи</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        public GridData GetTaskGridForUserHomePage(bool userAsExecutor, User user)
        {
            var state = userAsExecutor ?
                new GridState() { Parameters = "UserAsExecutor=1", Sort = "ExecutionState.ExecutionStateType=Asc;DeadLine=Desc;" } :
                new GridState() { Parameters = "UserAsExecutor=0", Sort = "ExecutionState.ExecutionStateType=Asc;DeadLine=Desc;" };

            return GetTaskGridLocalForUserHomePage(state, user);
        }

        /// <summary>
        /// Получение грида задач для домашней страницы пользователя
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <param name="user">Пользователь</param>
        /// <returns></returns>
        public GridData GetTaskGridForUserHomePage(GridState state, User user)
        {
            return GetTaskGridLocalForUserHomePage(state, user);
        }

        #endregion

        #endregion

        #endregion
    }
}
