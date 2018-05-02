namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип истории задачи
    /// </summary>
    public enum TaskHistoryItemType: byte
    {
        /// <summary>
        /// История задачи
        /// </summary>
        TaskHistory = 1,

        /// <summary>
        /// История исполнения задачи
        /// </summary>
        TaskExecutionHistory
    }
}
