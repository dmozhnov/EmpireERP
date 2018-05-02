namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Тип клиента
    /// </summary>
    public class ClientType : BaseDictionary
    {
        #region Свойства

        #endregion

        #region Конструкторы

        public ClientType()
            : base()
        {
        }

        public ClientType(string name) : base(name)
        {
        }

        #endregion
    }
}
