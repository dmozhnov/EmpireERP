using ERP.Infrastructure.Entities;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Операция
    /// </summary>
    public abstract class Operation : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Тип операции
        /// </summary>
        public OperationType Type { get; protected set; }

        #endregion
    }
}
