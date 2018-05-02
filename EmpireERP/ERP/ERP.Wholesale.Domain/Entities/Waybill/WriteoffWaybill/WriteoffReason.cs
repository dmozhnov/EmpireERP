namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Основание для списания товара
    /// </summary>
    public class WriteoffReason : BaseDictionary
    {
        #region Свойства

        #endregion

        #region Конструкторы

        public WriteoffReason()
            : base()
        {
        }

        public WriteoffReason(string name)
            : base(name)
        {
        }

        #endregion
    }
}