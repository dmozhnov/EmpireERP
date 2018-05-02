namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Программа обслуживания клиента
    /// </summary>
    public class ClientServiceProgram : BaseDictionary
    {
        #region Свойства

        #endregion

        #region Конструкторы

        public ClientServiceProgram()
            : base()
        {
        }

        public ClientServiceProgram(string name)
            : base(name)
        {
        }

        #endregion
    }
}