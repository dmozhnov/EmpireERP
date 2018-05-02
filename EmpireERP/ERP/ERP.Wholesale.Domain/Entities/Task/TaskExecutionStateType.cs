using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Логическое состояние, к которому относится статус исполнения задачи
    /// </summary>
    public enum TaskExecutionStateType: byte
    {
        /// <summary>
        /// Новая задача
        /// </summary>
        [EnumDisplayName("Новая")]
        New = 1,

        /// <summary>
        /// Задача выполняется
        /// </summary>
        [EnumDisplayName("В исполнении")]
        Executing,

        /// <summary>
        /// Задача выполнена
        /// </summary>
        [EnumDisplayName("Завершена")]
        Completed
    }
}
