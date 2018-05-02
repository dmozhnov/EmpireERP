using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IValueAddedTaxService : IBaseDictionaryService<ValueAddedTax>
    {
        /// <summary>
        /// Проверка ставки НДС на уникальность
        /// </summary>
        /// <param name="id">id сущности</param>
        /// <param name="name">Наименование</param>
        /// <param name="value">Ставка НДС</param>
        void CheckValueAddedTaxUniqueness(short id, string name, decimal value);
    }
}
