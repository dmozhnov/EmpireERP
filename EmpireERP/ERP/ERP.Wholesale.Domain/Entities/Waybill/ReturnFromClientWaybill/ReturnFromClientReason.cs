namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Основание для возврата товара от клиента
    /// </summary>
    public class ReturnFromClientReason : BaseDictionary
    {
        #region Свойства

        #endregion

        #region Конструкторы

        public ReturnFromClientReason()
            : base()
        {
        }

        public ReturnFromClientReason(string name)
            : base(name)
        {
        }

        #endregion
    }
}
