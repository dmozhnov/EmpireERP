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
    /// <summary>
    /// Сервис курса валюты
    /// </summary>
    public class CurrencyRateService : ICurrencyRateService
    {
        #region Поля

        private readonly ICurrencyRateRepository currencyRateRepository;

        #endregion

        #region Конструктор

        public CurrencyRateService(ICurrencyRateRepository currencyRateRepository)
        {
            this.currencyRateRepository = currencyRateRepository;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Проверка существования курса валюты
        /// </summary>
        /// <param name="id">Код курса валюты</param>
        /// <returns>Курс валюты</returns>
        public CurrencyRate CheckCurrencyRateExistence(int id)
        {
            var currencyRate = currencyRateRepository.GetRateById(id);
            ValidationUtils.NotNull(currencyRate, "Курс валюты не найден. Возможно, он был удален.");

            return currencyRate;
        }

        /// <summary>
        /// Добавление курса в валюту
        /// </summary>
        /// <param name="currency">Валюта</param>
        /// <param name="rate">Курс</param>
        /// <param name="user">Пользователь</param>
        public void AddRate(Currency currency, CurrencyRate rate, User user)
        {
            // Проверяем права
            CheckPossibilityToAddCurrencyRate(user);

            // Получаем курсы на момент начала действия нового курса
            var rateList = currencyRateRepository.GetRatesOnDate(currency.Id, rate.StartDate);

            // Вставка курса в список курсов валюты
            InsertRateToCurrency(currency, rate, rateList);

            currency.AddRate(rate); //Добавляем курс в валюту
        }

        /// <summary>
        /// Вставка курса в список курсов валюты
        /// </summary>
        /// <param name="currency">Валюта</param>
        /// <param name="rate">Курс валюты</param>
        /// <param name="rateList">Список действующих курсов на момент вступления в действие нового курса</param>
        private void InsertRateToCurrency(Currency currency, CurrencyRate rate, IEnumerable<CurrencyRate> rateList)
        {
            DateTime? endDate = null;
            rate.PreviousCurrencyRate = null;

            // Если на указанную дату имеются курсы, то
            if (rateList.Count() > 0)
            {
                // берем их дату завершения как дату завершения нового курса
                var pList = rateList.Where(x =>(x.EndDate >= rate.StartDate || x.EndDate == null) &&
                    (x.StartDate < rate.StartDate || (x.StartDate == rate.StartDate && x.CreationDate <= rate.CreationDate)));
                var p = pList.Where(x => !pList.Any(y => y.PreviousCurrencyRate == x)).FirstOrDefault();

                if (p != null)
                {
                    var nextRate = currencyRateRepository.GetNextRate(p.Id);

                    endDate = p.EndDate;
                    p.EndDate = rate.StartDate;
                    rate.PreviousCurrencyRate = p;

                    if (nextRate != null)
                    {
                        nextRate.PreviousCurrencyRate = rate;
                    }
                }
            }
            else
            {
                // иначе ищем первый курс после даты начала действия нового курса
                var firstRate = currencyRateRepository.GetNextRateByDate(currency.Id, rate.StartDate, rate.CreationDate);
                // Если такой курс найден, то берем его дату начала. Иначе дата завершения нового курса null
                if (firstRate != null)
                {
                    endDate = firstRate.StartDate;
                    firstRate.PreviousCurrencyRate = rate;
                }
            }
            rate.EndDate = endDate; // выставляем дату завершения действия нового курса
        }

        /// <summary>
        /// Удаление курса валюты
        /// </summary>
        /// <param name="rate">Курс</param>
        /// <param name="user">Пользователь</param>
        public void DeleteRate(CurrencyRate rate, User user)
        {
            // Проверка на возможность удаления курса
            CheckPossibilityToDeleteCurrencyRate(rate, user);
            // Удаляем курс из хронологии
            RemoveRateFromCurrency(rate);
            // Удаляем курс из валюты
            rate.Currency.RemoveRate(rate);
        }

        /// <summary>
        /// Удаление курса из хронологии курсов валюты
        /// </summary>
        /// <param name="rate">Курс</param>
        private void RemoveRateFromCurrency(CurrencyRate rate)
        {
            var previousRate = currencyRateRepository.GetPreviouseRate(rate.Id);    // Получаем предыдущий курс
            var nextRate = currencyRateRepository.GetNextRate(rate.Id); // Получаем следующий курс
            // Если предыдущий найден, то
            if (previousRate != null)
            {
                // продлеваем его действие до конца удаляемого курса
                previousRate.EndDate = rate.EndDate;
            }
            // Если найден следующий курс, то...
            if (nextRate != null)
            {
                // ... перенаправляем его связь с предыдущим курсом
                nextRate.PreviousCurrencyRate = previousRate != null ? previousRate : null;
            }
        }

        /// <summary>
        /// Редактирование курса валюты
        /// </summary>
        /// <param name="currencyRate">Курс валюты</param>
        /// <param name="startDate">Новая начальная дата</param>
        /// <param name="rateValue">Курс валюты</param>
        /// <param name="user">Пользователь</param>
        public void EditRate(CurrencyRate currencyRate, DateTime startDate, decimal rateValue, User user)
        {
            // проверяем возможность редактирования курса
            CheckPossibilityToEditCurrencyRate(currencyRate, user, currencyRate.Rate == rateValue);

            // Удаляем курс из хронологии
            RemoveRateFromCurrency(currencyRate);

            // Обновляем значения курса
            currencyRate.StartDate = startDate;
            currencyRate.Rate = rateValue;
            currencyRate.PreviousCurrencyRate = null;

            currencyRateRepository.Flush();

            // Получаем курсы на момент начала действия нового курса
            var rateList = currencyRateRepository.GetRatesOnDate(currencyRate.Currency.Id, currencyRate.StartDate)
                .Where(x => x != currencyRate);
            // Вставка курса в хронолгию курсов валюты
            InsertRateToCurrency(currencyRate.Currency, currencyRate, rateList);
        }

        /// <summary>
        /// Признак возможности добавить курс валюты
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>true - можно добавить</returns>
        public bool IsPossibilityToAddCurrencyRate(User user)
        {
            return user.HasPermission(Permission.Currency_AddRate);
        }

        /// <summary>
        /// Проверка разрешения добавить курс валюты
        /// </summary>
        /// <param name="user">Пользователь</param>
        public void CheckPossibilityToAddCurrencyRate(User user)
        {
            ValidationUtils.Assert(IsPossibilityToAddCurrencyRate( user),
                "Недостаточно прав для добавления курса валюты.");
        }

        /// <summary>
        /// Признак возможности изменить курс валюты
        /// </summary>
        /// <param name="currencyRate">Курс валюты</param>
        /// <param name="user">Пользователь</param>
        /// <param name="checkPermissionOnly">Проверять только права</param>
        /// <returns>true - можно изменить</returns>
        public bool IsPossibilityToEditCurrencyRate(CurrencyRate currencyRate, User user, bool checkPermissionOnly = false)
        {
            return user.HasPermission(Permission.Currency_AddRate) && 
                (checkPermissionOnly || !currencyRateRepository.CheckCurrencyRateUsing(currencyRate.Id));
        }

        /// <summary>
        /// Проверка разрешения изменять курс валюты
        /// </summary>
        /// <param name="currencyRate">Курс валюты</param>
        /// <param name="user">Пользователь</param>
        /// <param name="checkPermissionOnly">Проверять только права</param>
        public void CheckPossibilityToEditCurrencyRate(CurrencyRate currencyRate, User user, bool checkPermissionOnly = false)
        {
            ValidationUtils.Assert(IsPossibilityToEditCurrencyRate(currencyRate, user, checkPermissionOnly),
                "Невозможно изменить курс валюты, т.к. он используется.");
        }

        /// <summary>
        /// Признак возможности удалить курс валюты
        /// </summary>
        /// <param name="currencyRate">Курс валюты</param>
        /// <param name="user">Пользователь</param>
        /// <param name="checkPermissionOnly">Проверять только права</param>
        /// <returns>true - можно удалить</returns>
        public bool IsPossibilityToDeleteCurrencyRate(CurrencyRate currencyRate, User user, bool checkPermissionOnly = false)
        {
            return user.HasPermission(Permission.Currency_AddRate) &&
                (checkPermissionOnly || !currencyRateRepository.CheckCurrencyRateUsing(currencyRate.Id));
        }

        /// <summary>
        /// Проверка разрешения удалить курс валюты
        /// </summary>
        /// <param name="currencyRate">Курс валюты</param>
        /// <param name="user">Пользователь</param>
        /// <param name="checkPermissionOnly">Проверять только права</param>
        public void CheckPossibilityToDeleteCurrencyRate(CurrencyRate currencyRate, User user, bool checkPermissionOnly = false)
        {
            ValidationUtils.Assert(IsPossibilityToDeleteCurrencyRate(currencyRate, user, checkPermissionOnly),
                "Невозможно удалить курс валюты, т.к. он используется.");
        }

        #endregion
    }
}
