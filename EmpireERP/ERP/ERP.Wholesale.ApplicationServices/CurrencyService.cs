using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class CurrencyService : ICurrencyService
    {
        #region Поля

        private readonly ICurrencyRepository currencyRepository;

        #endregion

        #region Конструкторы

        public CurrencyService(ICurrencyRepository currencyRepository)
        {
            this.currencyRepository = currencyRepository;
        }

        #endregion

        #region Методы

        public Currency CheckCurrencyExistence(short id)
        {
            var currency = currencyRepository.GetById(id);
            ValidationUtils.NotNull(currency, "Валюта не найдена. Возможно, она была удалена.");

            return currency;
        }
        
        public IEnumerable<Currency> GetAll()
        {
            return currencyRepository.GetAll();
        }

        public IList<Currency> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return currencyRepository.GetFilteredList(state, ignoreDeletedRows);
        }

        public void Save(Currency entity)
        {
            CheckCurrencyNameUniqueness(entity);

            currencyRepository.Save(entity);
        }

        public void CheckCurrencyNameUniqueness(Currency entity)
        {
            var countName = currencyRepository.Query<Currency>().Where(x => x.Name == entity.Name && x.Id != entity.Id).Count();
            if (countName > 0)
            {
                throw new Exception("Валюта с таким названием уже добавлена. Укажите другое название.");
            }

            var countLiteralCode = currencyRepository.Query<Currency>().Where(x => x.LiteralCode == entity.LiteralCode && x.Id != entity.Id).Count();
            if (countLiteralCode > 0)
            {
                throw new Exception("Валюта с таким сокращением уже добавлена. Укажите другое сокращение.");
            }

            var countNumericCode = currencyRepository.Query<Currency>().Where(x => x.NumericCode == entity.NumericCode && x.Id != entity.Id).Count();
            if (countNumericCode > 0)
            {
                throw new Exception("Валюта с таким кодом уже добавлена. Укажите другой код.");
            }
        }

        public void Delete(Currency entity, User user)
        {
            CheckPossibilityToDelete(entity, user);
            entity.DeletionDate = DateTime.Now;
            currencyRepository.Delete(entity);
        }

        /// <summary>
        /// Возвращает текущую базовую валюту (заглушка всегда возвращает рубли)
        /// </summary>
        /// <returns></returns>
        public Currency GetCurrentBaseCurrency()
        {
            return currencyRepository.GetByLiteralCode("RUB");
        }

        /// <summary>
        /// Получить текущий курс валюты
        /// </summary>
        /// <param name="currency">Валюта</param>
        /// <returns>Текущий курс, или null, если ни одного курса нет</returns>
        /// <remarks>Не учитывает того, что последний действующий курс может иметь другую базовую валюту (не текущую базовую валюту).
        /// Также не учитывает того, что теоретически для базовой валюты может быть вбит курс, отличный от 1, и вернет его.</remarks>
        public CurrencyRate GetCurrentCurrencyRate(Currency currency)
        {
            return currency.Rates.Where(x => x.EndDate == null).FirstOrDefault();
        }

        /// <summary>
        /// По курсу валюты получить его значение.
        /// Курс должен быть окончательным, т.е. null означает не текущий курс, а то, что курса не существует (тогда возвращается null)
        /// </summary>
        /// <param name="currencyRate">Курс валюты (null означает не текущий курс, а то, что курса не существует)</param>
        /// <returns>Значение поля Rate курса (или null, если курса не существует)</returns>
        public decimal? GetCurrencyRateValue(CurrencyRate currencyRate)
        {
            return currencyRate != null ? currencyRate.Rate : (decimal?)null;
        }

        /// <summary>
        /// Перевести сумму из произвольной валюты в базовую
        /// </summary>
        /// <param name="currency">Исходная валюта</param>
        /// <param name="currencyRate">Исходный курс валюты (null - текущий)</param>
        /// <param name="sumInCurrency">Сумма в исходной валюте</param>
        /// <returns>Сумма в базовой валюте</returns>
        public decimal? CalculateSumInBaseCurrency(Currency currency, CurrencyRate currencyRate, decimal sumInCurrency)
        {
            if (currency == GetCurrentBaseCurrency())
            {
                return sumInCurrency;
            }

            if (currencyRate == null)
            {
                currencyRate = GetCurrentCurrencyRate(currency);
            }

            return currencyRate != null ? Math.Round(currencyRate.Rate * sumInCurrency, 6) : (decimal?)null;
        }

        /// <summary>
        /// Перевести сумму из исходной валюты в новую
        /// </summary>
        /// <param name="currencyFrom">Исходная валюта</param>
        /// <param name="currencyRateFrom">Курс исходной валюты (null - текущий)</param>
        /// <param name="sumInCurrency">Сумма в исходной валюте</param>
        /// <param name="currencyTo">Новая валюта</param>
        /// <param name="currencyRateTo">Курс новой валюты (null - текущий)</param>
        /// <returns>Сумма в новой валюте</returns>
        public decimal? CalculateSumInCurrency(Currency currencyFrom, CurrencyRate currencyRateFrom, decimal sumInCurrency, Currency currencyTo, CurrencyRate currencyRateTo)
        {
            if (currencyRateFrom == null)
            {
                currencyRateFrom = GetCurrentCurrencyRate(currencyFrom);
            }
            if (currencyRateTo == null)
            {
                currencyRateTo = GetCurrentCurrencyRate(currencyTo);
            }

            if (currencyRateFrom == null || currencyRateTo == null || currencyRateTo.Rate == 0M)
            {
                return null;
            }

            if (currencyFrom == currencyTo && currencyRateFrom == currencyRateTo)
            {
                return sumInCurrency;
            }

            decimal sumInBaseCurrency = currencyRateFrom.Rate * sumInCurrency;

            return Math.Round(sumInBaseCurrency / currencyRateTo.Rate, 6);
        }

        // TODO: перенести в сервис заказов или удалить
        public decimal? CalculateSumInBaseCurrency(ProductionOrder productionOrder, decimal sumInCurrency)
        {
            Currency currency;
            CurrencyRate currencyRate;
            GetCurrencyAndCurrencyRate(productionOrder, out currency, out currencyRate);

            return CalculateSumInBaseCurrency(currency, currencyRate, sumInCurrency);
        }

        // TODO: перенести все методы GetCurrencyAndCurrencyRate в сервис заказов
        public void GetCurrencyAndCurrencyRate(ProductionOrder productionOrder, out Currency currency, out CurrencyRate currencyRate)
        {
            currency = productionOrder.Currency;
            currencyRate = productionOrder.CurrencyRate;
        }

        public void GetCurrencyAndCurrencyRate(ProductionOrderTransportSheet transportSheet, out Currency currency, out CurrencyRate currencyRate)
        {
            switch (transportSheet.CurrencyDeterminationType)
            {
                case ProductionOrderCurrencyDeterminationType.ProductionOrderCurrency:
                    currency = transportSheet.ProductionOrder.Currency;
                    currencyRate = transportSheet.ProductionOrder.CurrencyRate;
                    break;
                case ProductionOrderCurrencyDeterminationType.BaseCurrency:
                    currency = GetCurrentBaseCurrency();
                    currencyRate = null;
                    break;
                case ProductionOrderCurrencyDeterminationType.SelectCurrency:
                    currency = transportSheet.Currency;
                    currencyRate = transportSheet.CurrencyRate;
                    break;
                default:
                    throw new Exception("Неизвестный тип поля «Способ выбора валюты».");
            };
        }

        public void GetCurrencyAndCurrencyRate(ProductionOrderExtraExpensesSheet extraExpensesSheet, out Currency currency, out CurrencyRate currencyRate)
        {
            switch (extraExpensesSheet.CurrencyDeterminationType)
            {
                case ProductionOrderCurrencyDeterminationType.ProductionOrderCurrency:
                    currency = extraExpensesSheet.ProductionOrder.Currency;
                    currencyRate = extraExpensesSheet.ProductionOrder.CurrencyRate;
                    break;
                case ProductionOrderCurrencyDeterminationType.BaseCurrency:
                    currency = GetCurrentBaseCurrency();
                    currencyRate = null;
                    break;
                case ProductionOrderCurrencyDeterminationType.SelectCurrency:
                    currency = extraExpensesSheet.Currency;
                    currencyRate = extraExpensesSheet.CurrencyRate;
                    break;
                default:
                    throw new Exception("Неизвестный тип поля «Способ выбора валюты».");
            };
        }

        public void GetCurrencyAndCurrencyRate(ProductionOrderPayment productionOrderPayment, out Currency currency, out CurrencyRate currencyRate)
        {
            switch (productionOrderPayment.Type)
            {
                case ProductionOrderPaymentType.ProductionOrderProductionPayment:
                    GetCurrencyAndCurrencyRate(productionOrderPayment.ProductionOrder, out currency, out currencyRate);
                    currencyRate = productionOrderPayment.CurrencyRate;

                    return;
                case ProductionOrderPaymentType.ProductionOrderTransportSheetPayment:
                    GetCurrencyAndCurrencyRate(productionOrderPayment.As<ProductionOrderTransportSheetPayment>().TransportSheet, out currency, out currencyRate);
                    currencyRate = productionOrderPayment.CurrencyRate;

                    return;
                case ProductionOrderPaymentType.ProductionOrderExtraExpensesSheetPayment:
                    GetCurrencyAndCurrencyRate(productionOrderPayment.As<ProductionOrderExtraExpensesSheetPayment>().ExtraExpensesSheet, out currency, out currencyRate);
                    currencyRate = productionOrderPayment.CurrencyRate;

                    return;
                case ProductionOrderPaymentType.ProductionOrderCustomsDeclarationPayment:
                    currency = GetCurrentBaseCurrency();
                    currencyRate = null;

                    return;
                default:
                    throw new Exception("Неизвестное назначение оплаты.");
            };
        }

        /* Метод пока не используется
        public decimal ImportCurrencyRate(Currency currency)
        {
            //var serv = new DailyInfoSoapClient();
            //var dataSet = serv.GetCursOnDate(DateTime.Now);
            decimal rate = 0;

            //foreach (DataRow row in dataSet.Tables[0].Rows)
            //{
            //    var a = row;
            //}

            return rate;
        }*/

        public IList<CurrencyRate> GetCurrencyRateFilteredList(short currencyId, object state, bool ignoreDeletedRows = true)
        {
            var ps = new ParameterString(state.GetType().GetProperty("Filter").GetValue(state, null) as string);
            ps.Add("Currency.Id", ParameterStringItem.OperationType.Eq, currencyId.ToString());

            return currencyRepository.GetCurrencyRateFilteredList(state, ps, ignoreDeletedRows);
        }

        public IList<CurrencyRate> GetCurrencyRateFilteredList(short currencyId, object state, Utils.ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            parameterString.Add("Currency.Id", ParameterStringItem.OperationType.Eq, currencyId.ToString());

            return currencyRepository.GetCurrencyRateFilteredList(state, parameterString, ignoreDeletedRows);
        }

        #region Проверка возможности выполнения операций

        #region Удаление

        public bool IsPossibilityToDelete(Currency currency, User user)
        {
            try
            {
                CheckPossibilityToDelete(currency, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDelete(Currency currency, User user)
        {
            // права
            user.CheckPermission(Permission.Currency_Delete);

            if (currencyRepository.Query<BankAccount>().Where(x => x.Currency.Id == currency.Id).Count() > 0)
            {
                throw new Exception("Невозможно удалить валюту, используемую в расчетных счетах.");
            }

            if (currencyRepository.Query<ProductionOrder>().Where(x => x.Currency.Id == currency.Id).Count() > 0)
            {
                throw new Exception("Невозможно удалить валюту, используемую в заказах на производство.");
            }

            if (currencyRepository.Query<ProductionOrderTransportSheet>().Where(x => x.Currency.Id == currency.Id).Count() > 0)
            {
                throw new Exception("Невозможно удалить валюту, используемую в транспортных листах.");
            }

            if (currencyRepository.Query<ProductionOrderTransportSheet>().Where(x => x.Currency.Id == currency.Id).Count() > 0)
            {
                throw new Exception("Невозможно удалить валюту, используемую в листах дополнительных расходов.");
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
