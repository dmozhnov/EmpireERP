using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels
{
    public class TaskHistoryItemViewModel
    {
        /// <summary>
        /// Право на просмотр деталей
        /// </summary>
        public bool AllowToViewContractorOld { get; set; }

        /// <summary>
        /// Право на просмотр деталей
        /// </summary>
        public bool AllowToViewContractorNew { get; set; }

        /// <summary>
        /// Признак изменения контрагента
        /// </summary>
        public bool IsContractorChange { get; set; }

        /// <summary>
        /// Идентификатор контрагента старый
        /// </summary>
        public string ContractorOldId { get; set; }

        /// <summary>
        ///  Идентификатор контрагента новый
        /// </summary>
        public string ContractorNewId { get; set; }

        /// <summary>
        /// Контрагент старый
        /// </summary>
        public string ContractorNameOld { get;  set; }

        /// <summary>
        /// Контрагент новый
        /// </summary>
        [DisplayName("Контрагент")]
        public string ContractorNameNew { get;  set; }

        /// <summary>
        /// Тип контрагента старый
        /// </summary>
        public string ContractorTypeOld { get; set; }

        /// <summary>
        /// Тип контрагента новый
        /// </summary>
        public string ContractorTypeNew { get; set; }

        /// <summary>
        /// Признак изменения требуемого срока исполнения задачи
        /// </summary>
        public bool IsDeadLineChange { get; set; }

        /// <summary>
        /// Требуемый срок завершения задачи старый
        /// </summary>
        public string DeadLineOld { get; set; }

        /// <summary>
        /// Требуемый срок завершения задачи новый
        /// </summary>
        [DisplayName("Требуемая дата завершения")]
        public string DeadLineNew { get; set; }

        /// <summary>
        /// Право на просмотр сделки старой
        /// </summary>
        public bool AllowToViewDealOld { get; set; }

        /// <summary>
        /// Право на просмотр сделки новой
        /// </summary>
        public bool AllowToViewDealNew { get; set; }

        /// <summary>
        /// Признак изменения сделки
        /// </summary>
        public bool IsDealChange { get; set; }

        /// <summary>
        /// Идентификаор сделки старый
        /// </summary>
        public string DealOldId { get;  set; }

        /// <summary>
        /// Идентификаор сделки новый
        /// </summary>
        public string DealNewId { get; set; }

        /// <summary>
        /// Сделка старая
        /// </summary>
        public string DealNameOld { get; set; }

        /// <summary>
        /// Сделка новая
        /// </summary>
        [DisplayName("Сделка")]
        public string DealNameNew { get; set; }

        /// <summary>
        /// Признак измения даты удаления
        /// </summary>
        public bool IsDeletionDateChange { get; set; }

        /// <summary>
        /// Дата удаления старая
        /// </summary>
        public string DeletionDateOld { get; set; }

        /// <summary>
        /// Дата удаления новая
        /// </summary>
        [DisplayName("Дата удаления")]
        public string DeletionDateNew { get; set; }

        /// <summary>
        /// Признак изменения описания
        /// </summary>
        [DisplayName("Описание")]
        public bool IsDescriptionChange { get; set; }

        /// <summary>
        /// Право на просмотр исполнителя старого
        /// </summary>
        public bool AllowToViewExecutedByOld { get; set; }

        /// <summary>
        /// Право на просмотр исполнителя нового
        /// </summary>
        public bool AllowToViewExecutedByNew { get; set; }

        /// <summary>
        /// Признак изменения ответсвенного лица
        /// </summary>
        public bool IsExecutedByChange { get; set; }

        /// <summary>
        /// Идентификатор исполнителя старый
        /// </summary>
        public string ExecutedByOldId { get; set; }

        /// <summary>
        /// Идентификатор исполнителя нвоый
        /// </summary>
        public string ExecutedByNewId { get; set; }

        /// <summary>
        /// Исполнитель старый
        /// </summary>
        public string ExecutedByNameOld { get; set; }

        /// <summary>
        /// Исполнитель новый
        /// </summary>
        [DisplayName("Ответственное лицо")]
        public string ExecutedByNameNew { get; set; }

        /// <summary>
        /// Признак изменения фактической даты завершения
        /// </summary>
        public bool IsFactualCompletionDateChange { get; set; }

        /// <summary>
        /// Фактическая дата завершения старая
        /// </summary>
        public string FactualCompletionDateOld { get; set; }

        /// <summary>
        /// Фактическая дата завершения новая
        /// </summary>
        [DisplayName("Фактическая дата завершения")]
        public string FactualCompletionDateNew { get; set; }

        /// <summary>
        /// Признак изменения фактического затраченного времени
        /// </summary>
        public bool IsFactualSpentTimeChange { get; set; }

        /// <summary>
        /// Фактически затраченное время старое
        /// </summary>
        public string FactualSpentTimeOld { get; set; }

        /// <summary>
        /// Фактически затраченное время новое
        /// </summary>
        [DisplayName("Фактически затраченное время")]
        public string FactualSpentTimeNew { get; set; }

        /// <summary>
        /// Признак изменения приоретета задачи
        /// </summary>
        public bool IsPriorityChange { get; set; }

        /// <summary>
        /// Приоритет старый
        /// </summary>
        public string PriorityNameOld { get; set; }

        /// <summary>
        /// Приоритет новый
        /// </summary>
        [DisplayName("Приоритет")]
        public string PriorityNameNew { get; set; }

        /// <summary>
        /// Право на просмотр заказ на производство старого
        /// </summary>
        public bool AllowToViewProductionOrderOld { get; set; }

        /// <summary>
        /// Право на просмотр заказ на производство нового
        /// </summary>
        public bool AllowToViewProductionOrderNew { get; set; }

        /// <summary>
        /// Призхнак изменения заказа на производство
        /// </summary>
        public bool IsProductionOrderChange { get; set; }

        /// <summary>
        /// Идентификатор заказа на производство старый
        /// </summary>
        public string ProductionOrderOldId { get; set; }

        /// <summary>
        /// Идентификатор заказа на производство новый
        /// </summary>
        public string ProductionOrderNewId { get; set; }
        
        /// <summary>
        /// Заказ на производство старый
        /// </summary>
        public string ProductionOrderNameOld { get; set; }
        
        /// <summary>
        /// Заказ на производство новый
        /// </summary>
        [DisplayName("Заказ на производство")]
        public string ProductionOrderNameNew { get; set; }

        /// <summary>
        /// Признак изменения даты начала исполнения
        /// </summary>
        public bool IsStartDateChange { get; set; }

        /// <summary>
        /// Начало исполнения задачи старое
        /// </summary>
        public string StartDateOld { get; set; }

        /// <summary>
        /// Начало исполнения задачи новое
        /// </summary>
        [DisplayName("Дата начала исполнения")]
        public string StartDateNew { get; set; }

        /// <summary>
        /// Признак изменения статуса задачи
        /// </summary>
        public bool IsStateChange { get; set; }

        /// <summary>
        /// Статус задачи старый
        /// </summary>
        public string StateNameOld { get; set; }

        /// <summary>
        /// Состояние задачи новый
        /// </summary>
        [DisplayName("Состояние")]
        public string StateNameNew { get; set; }

        /// <summary>
        /// признак изменения заголовка задачи
        /// </summary>
        public bool IsTopicChange { get; set; }

        /// <summary>
        /// Тема задачи старая
        /// </summary>
        public string TopicOld { get; set; }

        /// <summary>
        /// Тема задачи новая
        /// </summary>
        [DisplayName("Заголовок")]
        public string TopicNew { get; set; }

        /// <summary>
        /// Признак изменения типа задачи
        /// </summary>
        public bool IsTypeChange { get; set; }

        /// <summary>
        /// Тип задачи старый
        /// </summary>
        public string TypeNameOld { get;  set; }

        /// <summary>
        /// Тип задачи новый
        /// </summary>
        [DisplayName("Фактически затраченное время")]
        public string TypeNameNew { get; set; }

        //------------------------------------------------

        /// <summary>
        /// Автор зменений
        /// </summary>
        public string CreatedByName { get; set; }

        /// <summary>
        /// Идентификатор автора изменений
        /// </summary>
        public string CreatedById { get; set; }

        /// <summary>
        /// Право на просмотр деталей автора изменений
        /// </summary>
        public bool AllowToViewCreatedBy { get; set; }

        /// <summary>
        /// Дата изменений
        /// </summary>
        public string CreationDate { get; set; }


        //-------------------------------------------------
        //Изменения в исполнении
        //-------------------------------------------------

        /// <summary>
        /// Дата исполнения
        /// </summary>
        public string TaskExecutionCurrentDate { get; set; }

        /// <summary>
        /// Идентификатор пользователя, сделавшего изменение в исполнении
        /// </summary>
        public string TaskExecutionCreatedById { get; set; }

        /// <summary>
        /// Имя пользователя, сделавшего изменение в исполнении
        /// </summary>
        public string TaskExecutionCreatedByName { get; set; }

        /// <summary>
        /// Право на просмотр пользователя, сделавшего изменение в исполнении
        /// </summary>
        public bool AllowToViewTaskExecutionCreaetdBy { get; set; }

        /// <summary>
        /// Признак изменения даты исполнения
        /// </summary>
        public bool IsTaskExecutionDateChange { get; set; }

        /// <summary>
        /// Старая дата исполнения
        /// </summary>
        public string TaskExecutionDateOld { get; set; }

        /// <summary>
        /// Новая дата исполнения
        /// </summary>
        [DisplayName("Дата исполнения")]
        public string TaskExecutionDateNew { get; set; }

        /// <summary>
        /// Признак изменения даты удаления исполнения
        /// </summary>
        public bool IsTaskExecutionDeletionDateChange { get; set; }

        /// <summary>
        /// Старая дата удаления исполнения
        /// </summary>
        public string TaskExecutionDeletionDateOld { get; set; }

        /// <summary>
        /// Новая дата удаления исполнения
        /// </summary>
        [DisplayName("Дата удаления")]
        public string TaskExecutionDeletionDateNew { get; set; }

        /// <summary>
        /// Признак изменения достигнутых результатов
        /// </summary>
        [DisplayName("Достигнутые результаты")]
        public bool IsTaskExecutionResultDescriptionChange { get; set; }

        /// <summary>
        /// Признак изменения затраченного времени
        /// </summary>
        public bool IsTaskExecutionSpentTimeChange { get; set; }

        /// <summary>
        /// Старое значение затраченного времени
        /// </summary>
        public string TaskExecutionSpentTimeOld { get; set; }

        /// <summary>
        /// Новое значение затраченного времени
        /// </summary>
        [DisplayName("Затраченное время")]
        public string TaskExecutionSpentTimeNew { get; set; }

        /// <summary>
        /// Признак изменения статуса задачи
        /// </summary>
        public bool IsTaskExecutionStateChange { get; set; }

        /// <summary>
        /// Статус задачи старый
        /// </summary>
        public string TaskExecutionStateNameOld { get; set; }

        /// <summary>
        /// Состояние задачи новый
        /// </summary>
        [DisplayName("Состояние")]
        public string TaskExecutionStateNameNew { get; set; }

        /// <summary>
        /// Признак изменения процета выполенения
        /// </summary>
        public bool IsCompletionPercentageChange { get; set; }

        /// <summary>
        /// Процент выполнения старый
        /// </summary>
        public string CompletionPercentageOld { get; set; }

        /// <summary>
        /// Процент выполнения новый
        /// </summary>
        [DisplayName("Процент выполнения")]
        public string CompletionPercentageNew { get; set; }

        /// <summary>
        /// Признак добавления нового исполнения. Если false - исполнение редактировалось.
        /// </summary>
        public bool IsTaskExecutionAdded { get; set; }
    }
}
