
namespace ERP.Infrastructure.Repositories.Criteria
{
    /// <summary>
    /// Условие сравнения
    /// </summary>
    public enum CriteriaCond
    {
        /// <summary>
        /// Равно
        /// </summary>
        Eq,

        /// <summary>
        /// Не равно
        /// </summary>
        NotEq,

        /// <summary>
        /// Больше
        /// </summary>
        Gt,

        /// <summary>
        /// Больше или равно
        /// </summary>
        Ge,

        /// <summary>
        /// Меньше
        /// </summary>
        Lt,

        /// <summary>
        /// Меньше или равно
        /// </summary>
        Le
    };
}
