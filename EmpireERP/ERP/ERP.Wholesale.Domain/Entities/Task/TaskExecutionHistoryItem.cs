using System;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// История исполнения задачи
    /// </summary>
    public class TaskExecutionHistoryItem : BaseTaskHistoryItem
    {
        #region Свойства

        /// <summary>
        /// Задача, к которой относится история
        /// </summary>
        public virtual Task Task { get; protected set; }

        /// <summary>
        /// Исполнение, к которому принадлежит история
        /// </summary>
        public virtual TaskExecutionItem TaskExecutionItem { get; set; }

        /// <summary>
        /// Тип истории
        /// </summary>
        public override TaskHistoryItemType HistoryItemType
        {
            get { return TaskHistoryItemType.TaskExecutionHistory; }
        }

        #region Изменения в исполнении

        /// <summary>
        /// Дата выполнения
        /// </summary>
        public virtual DateTime? Date { get; private set; }

        /// <summary>
        /// Признак изменения даты исполнения
        /// </summary>
        public virtual bool IsDateChanged { get; private set; }

        /// <summary>
        /// Дата удаления
        /// </summary>
        public virtual DateTime? DeletionDate { get; private set; }

        /// <summary>
        /// Признак изменения даты удаления
        /// </summary>
        public virtual bool IsDeletionDateChanged { get; private set; }

        /// <summary>
        /// Описание результата
        /// </summary>
        public virtual string ResultDescription { get; private set; }

        /// <summary>
        /// Признак изменения описания достигнутых результатов
        /// </summary>
        public virtual bool IsResultDescriptionChanged { get; private set; }

        /// <summary>
        /// Затраченное время
        /// </summary>
        public virtual int? SpentTime { get; private set; }

        /// <summary>
        /// Признак изменения затраченного времени
        /// </summary>
        public virtual bool IsSpentTimeChanged { get; private set; }

        /// <summary>
        /// Статус исполнения задачи
        /// </summary>
        public virtual TaskExecutionState TaskExecutionState { get; set; }

        /// <summary>
        /// Признак изменения статуса исполнения задачи
        /// </summary>
        public virtual bool IsTaskExecutionStateChanged { get; set; }

        /// <summary>
        /// Тип задачи
        /// </summary>
        public virtual TaskType TaskType { get; set; }

        /// <summary>
        /// Признак изменения типа задачи
        /// </summary>
        public virtual bool IsTaskTypeChanged { get; set; }

        /// <summary>
        /// Процент выполнения задачи
        /// </summary>
        public virtual byte? CompletionPercentage { get; set; }

        /// <summary>
        /// Признак изменения процента выполнения задачи
        /// </summary>
        public virtual bool IsCompletionPercentageChanged { get; set; }

        #endregion

        #endregion

        #region Конструкторы

        protected TaskExecutionHistoryItem() { }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="creationDate">Дата совершения изменений</param>
        /// <param name="createdBy">Пользователь, внесший изменения в исполнение задачи</param>
        internal TaskExecutionHistoryItem(TaskExecutionItem taskExecutionItem, DateTime creationDate, User createdBy,
            bool isDateChanged, DateTime? date, bool isDeletionDateChanged, DateTime? deletionDate, bool isResultDescriptionChanged,
            string resultDescription, bool isSpentTimeChanged, int? spentTime, bool isTaskExecutionStateChanged, TaskExecutionState taskExecutionState,
            bool isTaskTypeChanged, TaskType taskType, bool isCompletionPercentageChanged, byte? completionPercentage)
            : base(creationDate, createdBy)
        {
            ValidationUtils.NotNull(taskExecutionItem, "Необходимо указать исполнение.");
            ValidationUtils.NotNull(resultDescription, "Описание достигнутых результатов не может быть null.");

            Task = taskExecutionItem.Task;
            TaskExecutionItem = taskExecutionItem;

            IsDateChanged = isDateChanged;
            IsDeletionDateChanged = isDeletionDateChanged;
            IsResultDescriptionChanged = isResultDescriptionChanged;
            IsSpentTimeChanged = isSpentTimeChanged;
            IsCompletionPercentageChanged = isCompletionPercentageChanged;
            IsTaskExecutionStateChanged = isTaskExecutionStateChanged;
            IsTaskTypeChanged = isTaskTypeChanged;

            Date = date;
            DeletionDate = deletionDate;
            ResultDescription = resultDescription;
            SpentTime = spentTime;
            CompletionPercentage = completionPercentage;
            TaskExecutionState = taskExecutionState;
            TaskType = taskType;
        }

        #endregion
    }
}
