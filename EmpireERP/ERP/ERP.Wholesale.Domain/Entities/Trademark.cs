namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Торговая марка
    /// </summary>
    public class Trademark : BaseDictionary
    {
        #region Свойства

        #endregion

        #region Конструкторы

        public Trademark()
            : base()
        {
        }

        public Trademark(string name)
            : base(name)
        {
        }

        #endregion
    }
}