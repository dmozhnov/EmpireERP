namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Должность пользователя
    /// </summary>
    public class EmployeePost : BaseDictionary
    {
        #region Свойства

        #endregion

        #region Конструкторы

        public EmployeePost()
            : base()
        {
        }
        
        public EmployeePost(string name)
            : base(name)
        {
        }

        #endregion
    }
}
