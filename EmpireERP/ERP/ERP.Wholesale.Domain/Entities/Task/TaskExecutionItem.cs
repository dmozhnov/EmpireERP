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
    /// Исполнение задачи
    /// </summary>
    public class TaskExecutionItem: Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Пользователь, создавший исполнение
        /// </summary>
        public virtual User CreatedBy { get; protected set; }

        /// <summary>
        /// Дата, от которой создано исполнение
        /// </summary>
        public virtual DateTime Date
        {
            get { return date; }
            set
            {
                if (date != value)
                {
                    date = value;
                    isDateChanged = true;

                    Task.UpdateByLastExecution();
                }
            }
        }
        private DateTime date;

        /// <summary>
        /// Признак изменения даты
        /// </summary>
        private bool isDateChanged;

        /// <summary>
        /// Дата создания исполнения
        /// </summary>
        public virtual DateTime CreationDate { get; protected set; }

        /// <summary>
        /// Дата удаления исполнения
        /// </summary>
        public virtual DateTime? DeletionDate
        {
            get { return deletionDate; }
            set
            {
                // Запрещаем повторное удаление
                if (deletionDate == null && value != null)
                {
                    deletionDate = value;
                    isDeletionDateChanged = true;

                    Task.RemoveExecutionItem(this);
                    Task.UpdateByLastExecution();
                    Task.FactualSpentTime -= spentTime;
                }
            }
        }

        private DateTime? deletionDate;

        /// <summary>
        /// Признак изменения даты удаления
        /// </summary>
        private bool isDeletionDateChanged;

        /// <summary>
        /// История изменений исполнения
        /// </summary>
        public virtual IEnumerable<TaskExecutionHistoryItem> History
        {
            get { return history; }
        }
        private Iesi.Collections.Generic.ISet<TaskExecutionHistoryItem> history;

        /// <summary>
        /// Описание результата
        /// </summary>
        public virtual string ResultDescription
        {
            get { return resultDescription; }
            set
            {
                if (resultDescription != value)
                {
                    resultDescription = value;
                    isResultDescriptionChanged = true;
                }
            }
        }
        private string resultDescription;

        /// <summary>
        /// Признак изменения описания результата
        /// </summary>
        private bool isResultDescriptionChanged;

        /// <summary>
        /// Затраченное время (в минутах)
        /// </summary>
        public virtual int SpentTime
        {
            get { return spentTime; }
            set
            {
                if (spentTime != value)
                {
                    Task.FactualSpentTime += value - spentTime;

                    spentTime = value;
                    isSpentTimeChanged = true;
                }
            }
        }
        private int spentTime;

        /// <summary>
        /// Признак изменения затраченного времени
        /// </summary>
        private bool isSpentTimeChanged;

        /// <summary>
        /// Статус исполнения задачи
        /// </summary>
        public virtual TaskExecutionState ExecutionState
        { 
            get { return executionState; }
            set
            {
                ValidationUtils.NotNull(value, "Статус исполнения указан неверно.");

                if (executionState != value)
                {
                    executionState = value;
                    isExecutionStateChanged = true;

                    Task.UpdateTaskExecutionState(this);
                }
            }
        }
        private TaskExecutionState executionState;

        /// <summary>
        /// Признак изменения статуса исполнения задачи
        /// </summary>
        private bool isExecutionStateChanged;

        /// <summary>
        /// Процент выполнения задачи
        /// </summary>
        public virtual byte CompletionPercentage 
        {
            get { return completionPercentage; }
            set
            {
                ValidationUtils.Assert(0 <= value && value <= 100, "Процент выполнения задачи указан не верно.");
                if (completionPercentage != value)
                {
                    completionPercentage = value;
                    isCompletionPercentageChanged = true;

                    Task.UpdateCompletionPercentage(this);
                }
            }
        }
        private byte completionPercentage; 

        /// <summary>
        /// Признак изменения процента выполнения задачи
        /// </summary>
        private bool isCompletionPercentageChanged;

        /// <summary>
        /// Задача, к которой относится исполнение
        /// </summary>
        public virtual Task Task { get; protected set; }

        /// <summary>
        /// Тип задачи
        /// </summary>
        public virtual TaskType TaskType
        {
            get { return taskType; }
            private set
            {
                ValidationUtils.NotNull(value, "Тип задачи указан не верно.");

                if (taskType != value)
                {
                    taskType = value;
                    isTaskTypeChanged = true;
                }
            }
        }
        private TaskType taskType;

        /// <summary>
        /// Признак изменения типа задачи
        /// </summary>
        private bool isTaskTypeChanged;

        /// <summary>
        /// Признак того, что исполнение создано пользователем, а не автоматически при редактировании задачи
        /// </summary>
        public virtual bool IsCreatedByUser { get; private set; }

        #endregion

        #region Конструкторы
        
        protected TaskExecutionItem() {}

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="task">Задача</param>
        /// <param name="executionState">Статус задачи</param>
        /// <param name="completionPercentage">% выполнения</param>
        /// <param name="createdBy">Пользователь, создавший исполение</param>
        /// <param name="creationDate">Дата создания</param>
        /// <param name="date">Дата исполнения</param>
        public TaskExecutionItem(Task task, TaskType taskType, TaskExecutionState executionState, byte completionPercentage, User createdBy, bool isCreatedByUser, DateTime creationDate, DateTime date)
        {
            ValidationUtils.NotNull(task, "Необходимо указать задачу, к которой относится исполнение.");
            ValidationUtils.NotNull(createdBy, "Необходимо указать пользователя, создавшего исполнение.");
            ValidationUtils.Assert(taskType.States.Contains(executionState), String.Format("Статус исполнения «{0}» не допустим для типа задачи «{1}».", executionState.Name, taskType.Name));

            history = new HashedSet<TaskExecutionHistoryItem>();
            resultDescription = "";
            spentTime = 0;

            task.AddExecutionItem(this);    //Добавляем исполнение в задачу

            Task = task;
            
            this.taskType = taskType;
            this.executionState = executionState;

            CreatedBy = createdBy;
            CreationDate = creationDate;
            
            this.completionPercentage = completionPercentage;
            
            IsCreatedByUser = isCreatedByUser;
            this.date = date;

            isDateChanged = isDeletionDateChanged = isResultDescriptionChanged = isSpentTimeChanged = isCompletionPercentageChanged = isExecutionStateChanged = 
                isTaskTypeChanged = true;

            Task.UpdateByLastExecution();
            Task.UpdateTaskExecutionState(this);
            Task.UpdateCompletionPercentage(this);
            Task.FactualSpentTime += spentTime;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Сохранение истории изменений
        /// </summary>
        /// <param name="user">Пользователь, внесший изменения</param>
        public virtual void SaveHistory(DateTime currentDate, User user)
        {
            if (isDateChanged || isDeletionDateChanged || isResultDescriptionChanged || isSpentTimeChanged || isCompletionPercentageChanged || isExecutionStateChanged)
            {
                history.Add(new TaskExecutionHistoryItem(this, currentDate, user,
                   isDateChanged,
                   isDateChanged ? Date : (DateTime?)null,
                   isDeletionDateChanged,
                   isDeletionDateChanged ? DeletionDate : null,
                   isResultDescriptionChanged,
                   isResultDescriptionChanged ? ResultDescription : "",
                   isSpentTimeChanged,
                   isSpentTimeChanged ? SpentTime : (int?)null,
                   isExecutionStateChanged,
                   isExecutionStateChanged ? executionState : null,
                   isTaskTypeChanged,
                   isTaskTypeChanged ? taskType : null,
                   isCompletionPercentageChanged,
                   isCompletionPercentageChanged ? completionPercentage : (byte?)null));

                // Сбрасываем флаги изменений
                isDateChanged = isDeletionDateChanged = isResultDescriptionChanged = isSpentTimeChanged = isCompletionPercentageChanged = isExecutionStateChanged = false;
            }
        }

        #endregion
    }
}
