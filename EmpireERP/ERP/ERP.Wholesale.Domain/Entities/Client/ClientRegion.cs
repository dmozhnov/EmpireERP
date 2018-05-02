namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Регион клиента
    /// </summary>
    public class ClientRegion : BaseDictionary
    {
        #region Свойства

        #endregion

        #region Конструкторы

        public ClientRegion()
            : base()
        {
        }

        public ClientRegion(string name)
            : base(name)
        {
        }

        #endregion
    }
}