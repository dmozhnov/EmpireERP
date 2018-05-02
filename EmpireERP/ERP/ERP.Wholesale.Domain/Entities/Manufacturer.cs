namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Фабрика-изготовитель
    /// </summary>
    public class Manufacturer : BaseDictionary
    {
        #region Свойства

        #endregion

        #region Конструкторы

        public Manufacturer()
            : base()
        {
        }

        public Manufacturer(string name)
            : base(name)
        {
        }

        #endregion
    }
}