using System;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Изменения задачи для ее истории
    /// </summary>
    public class TaskHistoryItem : BaseTaskHistoryItem
    {
        #region Свойства

        /// <summary>
        /// Задача, к которой относится история
        /// </summary>
        public virtual Task Task { get; set; }

        /// <summary>
        /// Тип истории
        /// </summary>
        public override TaskHistoryItemType HistoryItemType
        {
            get { return TaskHistoryItemType.TaskHistory; }
        }

        #region Изменения в задаче

        /// <summary>
        /// Контрагент
        /// </summary>
        public virtual Contractor Contractor { get; private set; }
        public virtual bool IsContractorChanged { get; private set; }

        /// <summary>
        /// Требуемый срок завершения задачи
        /// </summary>
        public virtual DateTime? Deadline { get; private set; }
        public virtual bool IsDeadLineChanged { get; private set; }

        /// <summary>
        /// Сделка
        /// </summary>
        public virtual Deal Deal { get; private set; }
        public virtual bool IsDealChanged { get; private set; }

        /// <summary>
        /// Дата удаления
        /// </summary>
        public virtual DateTime? DeletionDate { get; private set; }
        public virtual bool IsDeletionDateChanged { get; private set; }

        /// <summary>
        /// Описание
        /// </summary>
        public virtual string Description { get; private set; }
        public virtual bool IsDescriptionChanged { get; private set; }

        /// <summary>
        /// Исполнитель
        /// </summary>
        public virtual User ExecutedBy { get; private set; }
        public virtual bool IsExecutedByChanged { get; private set; }

        /// <summary>
        /// Фактическая дата завершения
        /// </summary>
        public virtual DateTime? FactualCompletionDate { get; private set; }
        public virtual bool IsFactualCompletionDateChanged { get; private set; }

        /// <summary>
        /// Фактически затраченное время
        /// </summary>
        public virtual int? FactualSpentTime { get; private set; }
        public virtual bool IsFactualSpentTimeChanged { get; private set; }

        /// <summary>
        /// Приоритет
        /// </summary>
        public virtual TaskPriority TaskPriority { get; private set; }
        public virtual bool IsTaskPriorityChanged { get; private set; }

        /// <summary>
        /// Заказ
        /// </summary>
        public virtual ProductionOrder ProductionOrder { get; private set; }
        public virtual bool IsProductionOrderChanged { get; private set; }

        /// <summary>
        /// Начало исполнения задачи
        /// </summary>
        public virtual DateTime? StartDate { get; private set; }
        public virtual bool IsStartDateChanged { get; private set; }

        /// <summary>
        /// Тема задачи
        /// </summary>
        public virtual string Topic { get; private set; }
        public virtual bool IsTopicChanged { get; private set; }

        /// <summary>
        /// Тип задачи
        /// </summary>
        public virtual TaskType TaskType { get; private set; }
        public virtual bool IsTaskTypeChanged { get; private set; }

        /// <summary>
        /// Статус исполнения задачи
        /// </summary>
        public virtual TaskExecutionState TaskExecutionState { get; set; }
        public virtual bool IsTaskExecutionStateChanged { get; set; }

        /// <summary>
        /// Исполнение
        /// </summary>
        public virtual TaskExecutionItem TaskExecutionItem { get; set; }

        #endregion

        #endregion

        #region Конструкторы

        protected TaskHistoryItem() { }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="creationDate">Дата совершения изменений</param>
        /// <param name="createdBy">Пользователь, внесший изменения в исполнение задачи</param>
        internal TaskHistoryItem(Task task, DateTime creationDate, User createdBy,
            bool isContractorChanged, Contractor contractor, bool isDeadLineChanged, DateTime? deadLine, bool isDealChanged, Deal deal,
            bool isDeletionDateChanged, DateTime? deletionDate, bool isDescriptionChanged, string description, bool isExecutedByChanged, User executedBy,
            bool isFactualCompletionDateChanged, DateTime? factualCompletionDate, bool isFactualSpentTimeChanged, int? factualSpentTime, bool isTaskPriorityChanged,
            TaskPriority taskPriority, bool isProductionOrderChanged, ProductionOrder productionOrder, bool isStartDateChanged, DateTime? startDate, bool isTopicChanged,
            string topic, bool isTaskTypeChanged, TaskType taskType, bool isTaskExecutionStateChanged, TaskExecutionState taskExecutionState, TaskExecutionItem taskExecutionItem)
            : base(creationDate, createdBy)
        {
            ValidationUtils.NotNull(task, "Необходимо указать задачу, к которой относится история.");
            ValidationUtils.NotNull(description, "Описание не может быть null.");

            Task = task;

            IsContractorChanged = isContractorChanged;
            Contractor = contractor;
            IsDeadLineChanged = isDeadLineChanged;
            Deadline = deadLine;
            IsDealChanged = isDealChanged;
            Deal = deal;
            IsDeletionDateChanged = isDeletionDateChanged;
            DeletionDate = deletionDate;
            IsDescriptionChanged = isDescriptionChanged;
            Description = description;
            IsExecutedByChanged = isExecutedByChanged;
            ExecutedBy = executedBy;
            IsFactualCompletionDateChanged = isFactualCompletionDateChanged;
            FactualCompletionDate = factualCompletionDate;
            IsFactualSpentTimeChanged = isFactualSpentTimeChanged;
            FactualSpentTime = factualSpentTime;
            IsTaskPriorityChanged = isTaskPriorityChanged;
            TaskPriority = taskPriority;
            IsProductionOrderChanged = isProductionOrderChanged;
            ProductionOrder = productionOrder;
            IsStartDateChanged = isStartDateChanged;
            StartDate = startDate;
            IsTopicChanged = isTopicChanged;
            Topic = topic;
            IsTaskTypeChanged = isTaskTypeChanged;
            TaskType = taskType;
            IsTaskExecutionStateChanged = isTaskExecutionStateChanged;
            TaskExecutionState = taskExecutionState;
            TaskExecutionItem = taskExecutionItem;
        }

        #endregion

        #region Методы
        #endregion
    }
}
