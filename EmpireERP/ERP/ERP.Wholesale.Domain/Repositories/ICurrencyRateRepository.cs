using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface ICurrencyRateRepository
    {
        /// <summary>
        /// Сохранение изменений
        /// </summary>
        void Flush();

        /// <summary>
        /// Получение курса валюты по коду
        /// </summary>
        /// <param name="id">Код курса валюты</param>
        /// <returns>Курс валюты. Если не найден, то null.</returns>
        CurrencyRate GetRateById(int id);

        /// <summary>
        /// Получение курсов валюты, действующих на указанную дату
        /// </summary>
        ///<param name="currencyId">Код валюты</param>
        /// <param name="date">Дата</param>
        /// <returns></returns>
        IEnumerable<CurrencyRate> GetRatesOnDate(short currencyId, DateTime date);

        /// <summary>
        /// Получение следующего курса валюты после указанной даты по дате действия
        /// </summary>
        /// <param name="currencyId">Код валюты</param>
        /// <param name="date">Дата</param>
        /// <param name="creationDate">Дата создания курса</param>
        /// <returns>Следующий курс валюты</returns>
        CurrencyRate GetNextRateByDate(short currencyId, DateTime date, DateTime creationDate);

        /// <summary>
        /// Получение предыдущего курса
        /// </summary>
        /// <param name="currencyRateId">Код курса валюты, для которого нужно получить предыдущий</param>
        /// <returns>Предыдущий курс</returns>
        CurrencyRate GetPreviouseRate(int currencyRateId);

        /// <summary>
        /// Получение следующего курса
        /// </summary>
        /// <param name="currencyRateId">Код курса валюты, для которого нужно получить следующий</param>
        /// <returns>Следующий курс</returns>
        CurrencyRate GetNextRate(int currencyRateId);

        /// <summary>
        /// Проверка используется ли курс валюты
        /// </summary>
        /// <param name="currencyRateId">Код курса валюты</param>
        /// <returns>Признак использования курса валюты. true - используется.</returns>
        bool CheckCurrencyRateUsing(int currencyRateId);
    }
}
