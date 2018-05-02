using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class CurrencyRepository : BaseRepository, ICurrencyRepository
    {
        public CurrencyRepository() : base()
        {
        }

        public Currency GetById(short id)
        {
            return Query<Currency>().Where(x => x.Id == id).FirstOrDefault<Currency>();
        }

        /// <summary>
        /// Получение валюты по буквенному коду
        /// </summary>
        /// <param name="literalCode">Буквенный код валюты</param>
        /// <returns>Валюта</returns>
        public Currency GetByLiteralCode(string literalCode)
        {
            return CurrentSession.Query<Currency>().Where(x => x.LiteralCode == literalCode).Cacheable<Currency>().FirstOrDefault<Currency>();
        }

        public void Save(Currency value)
        {
            CurrentSession.SaveOrUpdate(value);
        }

        public void Delete(Currency value)
        {
            CurrentSession.Delete(value);
        }

        public IEnumerable<Currency> GetAll()
        {
            return CurrentSession.CreateCriteria(typeof(Currency)).List<Currency>();
        }

        public IList<Currency> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Currency>(state, ignoreDeletedRows);
        }

        public IList<Currency> GetFilteredList(object state, Utils.ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Currency>(state, parameterString, ignoreDeletedRows);
        }

        public IList<CurrencyRate> GetCurrencyRateFilteredList(object state, Utils.ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            ReadState(state);

            var crit = Query<CurrencyRate>(); //Создаем критерий

            var ps = new ParameterString(filter);
            filter = "";

            if (ps["Date"] != null)
            {
                var dates = (ps["Date"].Value as string).Split('-');
                DateTime? date1 = null;
                if (dates[0].Length > 0)
                {
                    date1 = ValidationUtils.TryGetDate(dates[0]);
                    date1 = date1.Value.SetHoursMinutesAndSeconds(0, 0, 0);
                }
                DateTime? date2 = null;
                if (dates[1].Length > 0)
                {
                    date2 = ValidationUtils.TryGetDate(dates[1]);
                    date2 = date2.Value.SetHoursMinutesAndSeconds(23, 59, 59);
                }

                // Фильтруем записи
                if (date1 != null && date2 != null)
                {
                    crit.Where(x => (x.StartDate >= date1 && x.StartDate <= date2) || (x.EndDate >= date1 && x.EndDate <= date2));
                }
                else if (date1 != null && date2 == null)
                {
                    crit.Where(x => x.StartDate >= date1 || x.EndDate >= date1);
                }
                else if (date1 == null && date2 != null)
                {
                    crit.Where(x => x.StartDate <= date2 || (x.EndDate <= date2 && x.EndDate != null));
                }
            }

            parameterString.Delete("Date");
            this.parameterString = parameterString;
            CreateFilter(crit);

            int totalRowCount = crit.CountDistinct();
            WriteTotalRowCount(state, totalRowCount);   //Записываем общее кол-во строк

            SortByCriteria(crit, sort); //Сортируем выборку

            state.GetType().GetMethod("CheckAndCorrectCurrentPage").Invoke(state, null);  //приводит текущую страницу к корректному значению
            currentPage = (int)state.GetType().GetProperty("CurrentPage").GetValue(state, null);

            crit.SetFirstResult((currentPage - 1) * pageSize).SetMaxResults(pageSize);

            this.parameterString = null;

            return crit.ToList<CurrencyRate>();
        }
    }
}
