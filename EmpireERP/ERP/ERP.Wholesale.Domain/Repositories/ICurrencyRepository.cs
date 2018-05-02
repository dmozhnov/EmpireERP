using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface ICurrencyRepository : IRepository<Currency, short>, IFilteredRepository<Currency>, IGetAllRepository<Currency>
    {
        /// <summary>
        /// Получение валюты по буквенному коду
        /// </summary>
        /// <param name="literalCode">Буквенный код валюты</param>
        /// <returns>Валюта</returns>
        Currency GetByLiteralCode(string literalCode);

        IList<CurrencyRate> GetCurrencyRateFilteredList(object state, Utils.ParameterString parameterString, bool ignoreDeletedRows = true);
    }
}
