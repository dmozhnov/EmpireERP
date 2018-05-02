using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;
using Iesi.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Задача
    /// </summary>
    public class Task : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Процент выполения задачи
        /// </summary>
        public virtual byte CompletionPercentage { get; private set; }

        /// <summary>
        /// Пользователь создавший задачу
        /// </summary>
        public virtual User CreatedBy { get; protected set; }

        /// <summary>
        /// Дата создания
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Требуемый срок исполнения
        /// </summary>
        public virtual DateTime? DeadLine 
        {
            get { return deadLine; }
            set
            {
                if (deadLine != value)
                {
                    deadLine = value;
                    isDeadLineChanged = true;
                }
            }
        }
        private DateTime? deadLine;

        /// <summary>
        /// Признак изменения требуемого срока выполнения
        /// </summary>
        private bool isDeadLineChanged;

        /// <summary>
        /// Дата удаления 
        /// </summary>
        public virtual DateTime? DeletionDate
        {
            get { return DeletionDate; }
            set
            {
                // Запрещаем повторное удаление
                if (deletionDate == null && value != null)
                {
                    deletionDate = value;
                    isDeletionDateChanged = true;
                }
            }
        }
        private DateTime? deletionDate;

        /// <summary>
        /// Признак изменения даты удаления
        /// </summary>
        private bool isDeletionDateChanged;

        /// <summary>
        /// Описание задачи
        /// </summary>
        public virtual string Description 
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    isDescriptionChanged = true;
                }
            }
        }
        private string description;

        /// <summary>
        /// Признак изменения описания
        /// </summary>
        private bool isDescriptionChanged;

        /// <summary>
        /// Исполнитель
        /// </summary>
        public virtual User ExecutedBy
        {
            get { return executedBy; }
            set
            {
                if (executedBy != value)
                {
                    executedBy = value;
                    isExecutedByChanged = true;
                }
            }
        }
        private User executedBy;

        /// <summary>
        /// Признак изменения исполнителя
        /// </summary>
        private bool isExecutedByChanged;

        /// <summary>
        /// История исполнения
        /// </summary>
        public virtual IEnumerable<TaskExecutionItem> ExecutionHistory 
        { 
            get 
            { 
                return executionHistory.OrderBy(x => x.CreationDate); 
            } 
        }
        private Iesi.Collections.Generic.ISet<TaskExecutionItem> executionHistory;

        /// <summary>
        /// Фактическая дата выполнения
        /// </summary>
        public virtual DateTime? FactualCompletionDate
        {
            get { return factualCompletionDate; }
            private set
            {
                if (factualCompletionDate != value)
                {
                    factualCompletionDate = value;
                    isFactualCompletionDateChanged = true;
                }
            }
        }
        private DateTime? factualCompletionDate;

        /// <summary>
        /// Признак изменения фактической даты выполнения
        /// </summary>
        private bool isFactualCompletionDateChanged;

        /// <summary>
        /// Фактически затраченное время
        /// </summary>
        public virtual int FactualSpentTime
        {
            get { return factualSpentTime; }
            protected internal set
            {
                if (factualSpentTime != value)
                {
                    factualSpentTime = value;
                    isFactualSpentTimeChanged = true;
                }
            }
        }
        private int factualSpentTime;

        /// <summary>
        /// Признак изменения фактически затраченного времени
        /// </summary>
        private bool isFactualSpentTimeChanged;

        /// <summary>
        /// Признак изменения фактической даты начала исполнения задачи
        /// </summary>
        private bool isFactualStartDateChanged;

        /// <summary>
        /// История задачи
        /// </summary>
        public virtual IEnumerable<TaskHistoryItem> History { get { return history; } }
        private Iesi.Collections.Generic.ISet<TaskHistoryItem> history;

        /// <summary>
        /// Приоритет задачи
        /// </summary>
        public virtual TaskPriority Priority
        {
            get { return priority; }
            set
            {
                ValidationUtils.NotNull(value, "Приоритет задачи указан неверно.");
                if (priority != value)
                {
                    priority = value;
                    isPriorityChanged = true;
                }
            }
        }
        private TaskPriority priority;

        /// <summary>
        /// Признак изменения приоритета
        /// </summary>
        private bool isPriorityChanged;

        /// <summary>
        /// Дата начала исполнения 
        /// </summary>
        public virtual DateTime? StartDate
        {
            get { return startDate; }
            private set
            {
                if (startDate != value)
                {
                    startDate = value;
                    isStartDateChanged = true;
                }
            }
        }
        private DateTime? startDate;

        /// <summary>
        /// Признак изменения даты начала исполнения
        /// </summary>
        private bool isStartDateChanged;

        /// <summary>
        /// Статус задачи
        /// </summary>
        public virtual TaskExecutionState ExecutionState 
        {
            get { return executionState; }
            protected internal set
            {
                ValidationUtils.NotNull(value, "Статус исполнения задачи указан неверно.");
                if (executionState != value)
                {
                    executionState = value;
                    isExecutionStateChanged = true;
                }
            }
        }
        private TaskExecutionState executionState;
        private bool isExecutionStateChanged;

        /// <summary>
        /// Тема задачи
        /// </summary>
        public virtual string Topic
        {
            get { return topic; }
            set
            {
                ValidationUtils.NotNull(value, "Тема задачи указана неверно.");
                if (topic != value)
                {
                    topic = value;
                    isTopicChanged = true;
                }
            }
        }
        private string topic;

        /// <summary>
        /// Признак изменения заголовка задачи
        /// </summary>
        private bool isTopicChanged;

        /// <summary>
        /// Тип задачи
        /// </summary>
        public virtual TaskType Type {
            get { return type; }
            protected internal set
            {
                ValidationUtils.NotNull(value, "Тип задачи указан неверно.");
                if (type != value)
                {
                    type = value;
                    isTypeChanged = true;

                    // Сбрасываем статус задачи
                    var state = type.States.Where(x => x.ExecutionStateType == TaskExecutionStateType.New).OrderBy(x => x.OrdinalNumber).FirstOrDefault();
                    ValidationUtils.NotNull(state, String.Format("Тип задачи «{0}» не содержит допустимых статусов с типом «{1}».", type.Name, TaskExecutionStateType.New.GetDisplayName()));
                    ExecutionState = state;
                    CompletionPercentage = 0;
                }
            }
        }
        private TaskType type;

        /// <summary>
        /// Признак изменения типа задачи
        /// </summary>
        private bool isTypeChanged;

        /// <summary>
        /// Идентифкатор исполнения, котрое изменило задачу. Нужно для истории, в БД не хранится.
        /// </summary>
        public virtual TaskExecutionItem TaskExecutionItem { get; protected internal set; }

        #region Связь задачи с сущностями

        /// <summary>
        /// Контрагент
        /// </summary>
        public virtual Contractor Contractor
        {
            get { return contractor; }
            set
            {
                if (contractor != value)
                {
                    contractor = value; //Устанавливаем нового контрагента, и
                    Deal = null;    // сбрасываем сделку
                    ProductionOrder = null; // и заказ на производство
                    isContractorChanged = true;
                }
            }
        }
        private Contractor contractor;

        /// <summary>
        /// Признак изменения контрагента
        /// </summary>
        private bool isContractorChanged;

        /// <summary>
        /// Сделка
        /// </summary>
        public virtual Deal Deal
        {
            get { return deal; }
            set
            {
                // Проверяем связь контрагента и сделки
                if (contractor != null && value != null)
                {
                    ValidationUtils.Assert(value.Client == contractor, String.Format("Сделка «{0}» не связана с контрагентом «{1}».",value.Name, contractor.Name));
                }

                if (deal != value)
                {
                    if (value != null)
                    {
                        Contractor = value.Client;    // Устанавливаем контрагента
                    }
                    deal = value;   // Устанваливаем сделку
                    isDealChanged = true;
                }
            }
        }
        private Deal deal;

        /// <summary>
        /// Признак изменения сделки
        /// </summary>
        private bool isDealChanged;

        /// <summary>
        /// Заказ на производство
        /// </summary>
        public virtual ProductionOrder ProductionOrder
        {
            get { return productionOrder; }
            set
            {
                // Проверяем связь контрагента и заказа на производство
                if (contractor != null && value != null)
                {
                    ValidationUtils.Assert(value.Producer == contractor, String.Format("Заказ на производство «{0}» не связан с контрагентом «{1}».", value.Name, contractor.Name));
                }

                if (productionOrder != value)
                {
                    if (value != null)    
                    {
                        Contractor = value.Producer; // Устанавливаем контрагента
                    }
                    productionOrder = value;
                    isProductionOrderChanged = true;
                }
            }
        }
        private ProductionOrder productionOrder;

        /// <summary>
        /// Признак изменения заказа на производство
        /// </summary>
        private bool isProductionOrderChanged;

        #endregion

        #endregion

        #region Конструкторы
        
        protected Task() {}

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="topic">Тема задачи</param>
        /// <param name="type">Тип задачи</param>
        /// <param name="priority">Приоритет задачи</param>
        /// <param name="state">Стастус задачи</param>
        /// <param name="createdBy">Пользователь, создавший задачу</param>
        public Task(string topic, TaskType type, TaskPriority priority, TaskExecutionState state, DateTime creationDate, User createdBy)            
        {
            ValidationUtils.Assert(!String.IsNullOrEmpty(topic),"Необходимо указать тему задачи.");
            ValidationUtils.NotNull(type, "Необходимо указать тип задачи.");
            ValidationUtils.NotNull(priority, "Необходимо указать приоритет задачи.");
            ValidationUtils.NotNull(state, "Необходимо указать статус задачи.");
            ValidationUtils.NotNull(createdBy, "Необходимо указать пользователя, создавшего задачу.");
            ValidationUtils.Assert(type.States.Contains(state), String.Format("Статус исполнения задачи «{0}» не допустим для типа задачи «{1}».", state.Name, type.Name));

            Topic = topic;
            Type = type;
            ExecutionState = state;
            Priority = priority;
            CreatedBy = createdBy;
            CreationDate = creationDate;
            Description = "";

            history = new HashedSet<TaskHistoryItem>();
            executionHistory = new HashedSet<TaskExecutionItem>();

            // Устанавливаем признак изменения полей (чтобы они все попали как первое изменение задачи в историю)
            isContractorChanged = isDeadLineChanged = isDealChanged = isDeletionDateChanged = isDescriptionChanged = isExecutedByChanged =
            isFactualCompletionDateChanged = isFactualSpentTimeChanged = isFactualStartDateChanged = isPriorityChanged = isProductionOrderChanged =
            isStartDateChanged = isTopicChanged = isTypeChanged = true;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавление исполнения
        /// </summary>
        /// <param name="item">Исполнение</param>
        protected internal virtual void AddExecutionItem(TaskExecutionItem item)
        {
            ValidationUtils.NotNull(item, "Необходимо указать исполнение задачи.");

            executionHistory.Add(item);
        }

        protected internal virtual void RemoveExecutionItem(TaskExecutionItem item)
        {
            ValidationUtils.NotNull(item, "Необходимо указать исполнение задачи.");

            executionHistory.Remove(item);
        }

        /// <summary>
        /// Сохранение истории
        /// </summary>
        /// <param name="user">Пользователь, внесший изменения</param>
        public virtual void SaveHistory(DateTime currentDate, User user)
        {
            // Сохраянем историю изменений
            if (isContractorChanged || isDeadLineChanged || isDealChanged || isDeletionDateChanged || isDescriptionChanged || isExecutedByChanged ||
                isFactualCompletionDateChanged || isFactualSpentTimeChanged || isFactualStartDateChanged || isPriorityChanged || isProductionOrderChanged ||
                isStartDateChanged || isTopicChanged || isTypeChanged || isExecutionStateChanged)
            {
                history.Add(new TaskHistoryItem(this, currentDate, user,
                    isContractorChanged,
                    isContractorChanged ? contractor : null,
                    isDeadLineChanged,
                    isDeadLineChanged ? deadLine : (DateTime?)null,
                    isDealChanged,
                    isDealChanged ? deal : null,
                    isDeletionDateChanged,
                    isDeletionDateChanged ? deletionDate : (DateTime?)null,
                    isDescriptionChanged,
                    isDescriptionChanged ? description : "",
                    isExecutedByChanged,
                    isExecutedByChanged ? executedBy : null,
                    isFactualCompletionDateChanged,
                    isFactualCompletionDateChanged ? factualCompletionDate : (DateTime?)null,
                    isFactualSpentTimeChanged,
                    isFactualSpentTimeChanged ? factualSpentTime : (int?)null,
                    isPriorityChanged,
                    isPriorityChanged ? priority : null,
                    isProductionOrderChanged,
                    isProductionOrderChanged ? productionOrder : null,
                    isStartDateChanged,
                    isStartDateChanged ? startDate : (DateTime?)null,
                    isTopicChanged,
                    isTopicChanged ? topic : "",
                    isTypeChanged,
                    isTypeChanged ? type : null,
                    isExecutionStateChanged,
                    isExecutionStateChanged ? ExecutionState : null,
                    TaskExecutionItem));

                // Сбрасываем флаги изменения свойств
                isContractorChanged = isDeadLineChanged = isDealChanged = isDeletionDateChanged = isDescriptionChanged = isExecutedByChanged =
                isFactualCompletionDateChanged = isFactualSpentTimeChanged = isFactualStartDateChanged = isPriorityChanged = isProductionOrderChanged =
                isStartDateChanged = isTopicChanged = isTypeChanged = isExecutionStateChanged = false;
            }
        }

        /// <summary>
        /// Обновление даты начала исполнения и завершения задачи
        /// </summary>
        private void UpdateStartAndCompletionDates()
        {
            #region StartDate
            {
                var newList = ExecutionHistory.Where(x => x.ExecutionState.ExecutionStateType == TaskExecutionStateType.New).Select(x => x.Date);
                var lastNewStateDate = DateTimeUtils.GetMaxDate(newList);

                var execList = ExecutionHistory.Where(x => x.ExecutionState.ExecutionStateType != TaskExecutionStateType.New).Select(x => x.Date);
                // Если статуса "новая" задача не имела, то берем все даты исполнений со статусом "в работе"
                var execStateDate = DateTimeUtils.GetMinDate(execList.Where(x => lastNewStateDate != null ? x > lastNewStateDate : true));    

                if (StartDate != execStateDate)
                {
                    StartDate = execStateDate;  // выставляем новую дату начала исполнения
                }
            }
            #endregion

            #region ComletionDate
            {
                var openList = ExecutionHistory.Where(x => x.ExecutionState.ExecutionStateType != TaskExecutionStateType.Completed).Select(x => x.Date);
                var lastOpenStateDate = DateTimeUtils.GetMaxDate(openList);

                var comletedList = ExecutionHistory.Where(x => x.ExecutionState.ExecutionStateType == TaskExecutionStateType.Completed).Select(x => x.Date);
                // Если "открытых" статусов задача не имела, то берем все даты исполнений с "закрытым" статусом
                var comletionStateDate = DateTimeUtils.GetMinDate(comletedList.Where(x => lastOpenStateDate != null ? x > lastOpenStateDate : true));

                if (FactualCompletionDate != comletionStateDate)
                {
                    FactualCompletionDate = comletionStateDate;
                }
            }
            #endregion
        }

        /// <summary>
        /// Определение, является ли исполнение последним
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsLastExecution(TaskExecutionItem item)
        {
            var maxDate = DateTimeUtils.GetMaxDate(ExecutionHistory.Select(x => x.Date));

            return item.Date == maxDate;
        }

        /// <summary>
        /// Обновление статуса задачи
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void UpdateTaskExecutionState(TaskExecutionItem item)
        {
            if (IsLastExecution(item))
            {
                Type = item.TaskType;
                ExecutionState = item.ExecutionState;
            }
            UpdateStartAndCompletionDates();
        }

        /// <summary>
        /// Обновление процента выполнения задачи
        /// </summary>
        /// <param name="item"></param>
        protected internal virtual void UpdateCompletionPercentage(TaskExecutionItem item)
        {
            if (IsLastExecution(item))
            {
                CompletionPercentage = item.CompletionPercentage;
            }
        }

        /// <summary>
        /// Обновить задачу по последнему исполнению
        /// </summary>
        protected internal virtual void UpdateByLastExecution()
        {
            var maxDate = DateTimeUtils.GetMaxDate(ExecutionHistory.Select(x => x.Date));
            var lastExecution = ExecutionHistory.Where(x => x.Date == maxDate).First();

            UpdateTaskExecutionState(lastExecution);
            CompletionPercentage = lastExecution.CompletionPercentage;
            UpdateStartAndCompletionDates();
        }

        #endregion
    }
}