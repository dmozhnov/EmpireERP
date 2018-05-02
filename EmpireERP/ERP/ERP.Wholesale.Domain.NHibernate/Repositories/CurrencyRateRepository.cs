using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    /// <summary>
    /// Репозиторий курса валюты
    /// </summary>
    public class CurrencyRateRepository : BaseRepository, ICurrencyRateRepository
    {
        /// <summary>
        /// Сохранение изменений
        /// </summary>
        public void Flush()
        {
            CurrentSession.Flush();
        }

        /// <summary>
        /// Получение курса валюты по коду
        /// </summary>
        /// <param name="id">Код курса валюты</param>
        /// <returns>Курс валюты. Если не найден, то null.</returns>
        public CurrencyRate GetRateById(int id)
        {
            return CurrentSession.Query<CurrencyRate>().Where(x => x.Id == id).FirstOrDefault<CurrencyRate>();
        }

        /// <summary>
        /// Получение курсов валюты, действующих на указанную дату
        /// </summary>
        /// <param name="currencyId">Код валюты</param>
        /// <param name="date">Дата</param>
        /// <returns></returns>
        public IEnumerable<CurrencyRate> GetRatesOnDate(short currencyId, DateTime date)
        {
            return CurrentSession.Query<CurrencyRate>()
                .Where(x => x.Currency.Id == currencyId && x.StartDate <= date && (x.EndDate >= date || x.EndDate == null))
                .ToList();
        }

        /// <summary>
        /// Получение следующего курса валюты после указанной даты по дате действия
        /// </summary>
        /// <param name="currencyId">Код валюты</param>
        /// <param name="date">Дата</param>
        /// <param name="creationDate">Дата создания курса</param>
        /// <returns>Следующий курс валюты</returns>
        public CurrencyRate GetNextRateByDate(short currencyId, DateTime date, DateTime creationDate)
        {
            return CurrentSession.Query<CurrencyRate>()
                .Where(x => x.Currency.Id == currencyId && (x.StartDate > date || (x.StartDate == date && x.CreationDate > creationDate)))
                .OrderBy(x => x.StartDate)  // Сортируем по дате начала действия
                .ThenBy(x => x.CreationDate)    //Если несколько курсов на дату, то берем первый по дате создания
                .Take(1)    // берем первый
                .FirstOrDefault();
        }

        /// <summary>
        /// Получение предыдущего курса
        /// </summary>
        /// <param name="currencyRateId">Код курса валюты, для которого нужно получить предыдущий</param>
        /// <returns>Предыдущий курс</returns>
        public CurrencyRate GetPreviouseRate(int currencyRateId)
        {
            return CurrentSession.Query<CurrencyRate>()
                .Where(x => x.Id == currencyRateId)
                .Select(x => x.PreviousCurrencyRate)
                .FirstOrDefault();
        }

        /// <summary>
        /// Получение следующего курса
        /// </summary>
        /// <param name="currencyRateId">Код курса валюты, для которого нужно получить следующий</param>
        /// <returns>Следующий курс</returns>
        public CurrencyRate GetNextRate(int currencyRateId)
        {
           return CurrentSession.Query<CurrencyRate>()
                .Where(x => x.PreviousCurrencyRate.Id == currencyRateId)
                .FirstOrDefault();
        }

        /// <summary>
        /// Проверка используется ли курс валюты
        /// </summary>
        /// <param name="currencyRateId">Код курса валюты</param>
        /// <returns>Признак использования курса валюты. true - используется.</returns>
        public bool CheckCurrencyRateUsing(int currencyRateId)
        {
            var cond1 = CurrentSession.Query<ProductionOrder>()
                .Where(x=>x.CurrencyRate.Id == currencyRateId)
                .Count();
            if (cond1 > 0) { return true; }

            var cond2 = CurrentSession.Query<ProductionOrderTransportSheet>()
                .Where(x=>x.CurrencyRate.Id == currencyRateId)
                .Count();
            if (cond2 > 0) { return true; }

            var cond3 = CurrentSession.Query<ProductionOrderPayment>()
                .Where(x => x.CurrencyRate.Id == currencyRateId)
                .Count();
            if (cond3 > 0) { return true; }

            var cond4 = CurrentSession.Query<ProductionOrderExtraExpensesSheet>()
                .Where(x => x.CurrencyRate.Id == currencyRateId)
                .Count();
            if (cond4 > 0) { return true; }

            return false;
        }
    }
}
