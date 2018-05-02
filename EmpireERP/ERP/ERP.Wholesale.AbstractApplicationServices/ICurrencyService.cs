using System;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface ICurrencyService
    {
        Currency CheckCurrencyExistence(short id);
        
        IEnumerable<Currency> GetAll();
        
        IList<Currency> GetFilteredList(object state, bool ignoreDeletedRows = true);

        void Save(Currency entity);
        void CheckCurrencyNameUniqueness(Currency entity);
        void Delete(Currency entity, User user);

        /// <summary>
        /// Возвращает текущую базовую валюту (заглушка всегда возвращает рубли)
        /// </summary>
        /// <returns></returns>
        Currency GetCurrentBaseCurrency();
        CurrencyRate GetCurrentCurrencyRate(Currency currency);
        decimal? GetCurrencyRateValue(CurrencyRate currencyRate);
        decimal? CalculateSumInBaseCurrency(Currency currency, CurrencyRate currencyRate, decimal sumInCurrency);
        decimal? CalculateSumInCurrency(Currency currencyFrom, CurrencyRate currencyRateFrom, decimal sumInCurrency, Currency currencyTo, CurrencyRate currencyRateTo);

        decimal? CalculateSumInBaseCurrency(ProductionOrder productionOrder, decimal sumInCurrency);
        void GetCurrencyAndCurrencyRate(ProductionOrder productionOrder, out Currency currency, out CurrencyRate currencyRate);
        void GetCurrencyAndCurrencyRate(ProductionOrderTransportSheet transportSheet, out Currency currency, out CurrencyRate currencyRate);
        void GetCurrencyAndCurrencyRate(ProductionOrderExtraExpensesSheet extraExpensesSheet, out Currency currency, out CurrencyRate currencyRate);
        void GetCurrencyAndCurrencyRate(ProductionOrderPayment productionOrderPayment, out Currency currency, out CurrencyRate currencyRate);

        //decimal ImportCurrencyRate(Currency currency);
        //IEnumerable<CurrencyRate> GetCurrencyRateOnDate(Currency currency, DateTime date);

        IList<CurrencyRate> GetCurrencyRateFilteredList(short currencyId, object state, bool ignoreDeletedRows = true);
        IList<CurrencyRate> GetCurrencyRateFilteredList(short currencyId, object state, Utils.ParameterString parameterString, bool ignoreDeletedRows = true);
    }
}
