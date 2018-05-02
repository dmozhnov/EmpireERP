namespace ERP.Utils
{
    /// <summary>
    /// Тип операции сравнения
    /// </summary>
    public enum CompareOperationType : byte
    {
        /// <summary>
        /// Равно
        /// </summary>
        Eq = 1,

        /// <summary>
        /// Меньше чем
        /// </summary>
        Lt,

        /// <summary>
        /// Меньше или равно
        /// </summary>
        Le,

        /// <summary>
        /// Больше или равно
        /// </summary>
        Ge,

        /// <summary>
        /// Больше чем
        /// </summary>
        Gt
    }
}
