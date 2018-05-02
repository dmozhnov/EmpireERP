using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace ERP.Wholesale.UI.ViewModels
{
    /// <summary>
    /// Детали задачи
    /// </summary>
    public class TaskMainDetailsViewModel
    {
        /// <summary>
        /// Идентификатор задачи
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Право на просмотр деталей контрагента
        /// </summary>
        public bool AllowToViewContractorDetails { get; set; }

        /// <summary>
        /// Контрагент, с которым связана задача
        /// </summary>
        public string ContractorId { get; set; }

        /// <summary>
        /// Тип контрагента (enum ContractorType)
        /// </summary>
        public string ContractorType { get; set; }

        /// <summary>
        /// Название контрагента
        /// </summary>
        [DisplayName("Контрагент")]
        public string ContractorName { get; set; }

        /// <summary>
        /// Автор задачи
        /// </summary>
        [DisplayName("Автор задачи")]
        public string CreatedByName { get; set; }

        /// <summary>
        /// Идентификатор автора задачи
        /// </summary>
        public string CreatedById { get; set; }

        /// <summary>
        /// Разрешение на просмотр автора задачи
        /// </summary>
        public bool AllowToViewCreatedByDetails { get; set; }

        /// <summary>
        /// Дата создания задачи
        /// </summary>
        [DisplayName("Дата создания")]
        public string CreationDate { get; set; }

        /// <summary>
        /// Требуемая дата завершения задачи
        /// </summary>
        [DisplayName("Требуемая дата завершения")]
        public string DeadLine { get; set; }

        /// <summary>
        /// Право на просмотр сделки
        /// </summary>
        public bool AllowToViewDealDetails { get; set; }

        /// <summary>
        /// Сделка, с которой связана задача
        /// </summary>
        public string DealId { get; set; }

        /// <summary>
        /// Название сделки
        /// </summary>
        [DisplayName("Сделка")]
        public string DealName { get; set; }

        /// <summary>
        /// Описание задачи
        /// </summary>
        [DisplayName("Описание задачи")]
        public string Description { get; set; }

        /// <summary>
        /// Право на просмотр заказа на производство
        /// </summary>
        public bool AllowToViewProductionOrderDetails { get; set; }

        /// <summary>
        /// Заказ на производство, с которым связана задача
        /// </summary>
        public string ProductionOrderId { get; set; }

        /// <summary>
        /// Название заказа на производство
        /// </summary>
        [DisplayName("Заказ на производство")]
        public string ProductionOrderName { get; set; }

        /// <summary>
        /// Ответственный за исполнение задачи пользователь
        /// </summary>
        public string ExecutedById { get; set; }

        /// <summary>
        /// Имя ответственного пользователя
        /// </summary>
        [DisplayName("Ответственное лицо")]
        public string ExecutedByName { get; set; }

        /// <summary>
        /// Право на просмотр ответственного лица
        /// </summary>
        public bool AllowToViewExecitionByDetails { get; set; }

        /// <summary>
        /// Приоритет задачи
        /// </summary>
        [DisplayName("Приоритет")]
        public string TaskPriorityName { get; set; }

        /// <summary>
        /// Дата начала исполенения задачи
        /// </summary>
        [DisplayName("Начало")]
        public string StartDate { get; set; }

        /// <summary>
        /// Состояние исполнения задачи
        /// </summary>
        [DisplayName("Состояние")]
        public string TaskExecutionStateName { get; set; }

        /// <summary>
        /// Загловок задачи
        /// </summary>
        [DisplayName("Заголовок")]
        public string Topic { get; set; }

        /// <summary>
        /// Тип задачи
        /// </summary>
        [DisplayName("Тип")]
        public string TaskTypeName { get; set; }

        /// <summary>
        /// Процент выполнения
        /// </summary>
        [DisplayName("% выполнения")]
        public string CompletionPercentage { get; set; }

        /// <summary>
        /// Фактическое завершение
        /// </summary>
        [DisplayName("Фактическое завершение")]
        public string FactualCompletionDate { get; set; }

        /// <summary>
        /// Общее затраченное время
        /// </summary>
        [DisplayName("Общее затраченное время")]
        public string FactualSpentTime { get; set; }
    }
}
