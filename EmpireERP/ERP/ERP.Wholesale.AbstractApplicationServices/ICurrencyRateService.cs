using System;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface ICurrencyRateService
    {
        /// <summary>
        /// Проверка существования курса валюты
        /// </summary>
        /// <param name="id">Код курса валюты</param>
        /// <returns>Курс валюты</returns>
        CurrencyRate CheckCurrencyRateExistence(int id);

        /// <summary>
        /// Добавление курса в валюту
        /// </summary>
        /// <param name="currency">Валюта</param>
        /// <param name="rate">Курс</param>
        /// <param name="user">Пользователь</param>
        void AddRate(Currency currency, CurrencyRate rate, User user);

        /// <summary>
        /// Удаление курса валюты
        /// </summary>
        /// <param name="rate">Курс</param>
        /// <param name="user">Пользователь</param>
        void DeleteRate(CurrencyRate rate, User user);

        /// <summary>
        /// Редактирование курса валюты
        /// </summary>
        /// <param name="currencyRate">Курс валюты</param>
        /// <param name="startDate">Новая начальная дата</param>
        /// <param name="rateValue">Курс валюты</param>
        /// <param name="user">Пользователь</param>
        void EditRate(CurrencyRate currencyRate, DateTime startDate, decimal rateValue, User user);

        /// <summary>
        /// Признак возможности добавить курс валюты
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>true - можно добавить</returns>
        bool IsPossibilityToAddCurrencyRate(User user);
        
        /// <summary>
        /// Проверка разрешения добавить курс валюты
        /// </summary>
        /// <param name="user">Пользователь</param>
        void CheckPossibilityToAddCurrencyRate(User user);

        /// <summary>
        /// Признак возможности изменить курс валюты
        /// </summary>
        /// <param name="currencyRate">Курс валюты</param>
        /// <param name="user">Пользователь</param>
        /// <param name="checkPermissionOnly">Проверять только права</param>
        /// <returns>true - можно изменить</returns>
        bool IsPossibilityToEditCurrencyRate(CurrencyRate currencyRate, User user, bool checkPermissionOnly = false);
        
        /// <summary>
        /// Проверка разрешения изменять курс валюты
        /// </summary>
        /// <param name="currencyRate">Курс валюты</param>
        /// <param name="user">Пользователь</param>
        /// <param name="checkPermissionOnly">Проверять только права</param>
        void CheckPossibilityToEditCurrencyRate(CurrencyRate currencyRate, User user, bool checkPermissionOnly = false);

        /// <summary>
        /// Признак возможности удалить курс валюты
        /// </summary>
        /// <param name="currencyRate">Курс валюты</param>
        /// <param name="user">Пользователь</param>
        /// <param name="checkPermissionOnly">Проверять только права</param>
        /// <returns>true - можно удалить</returns>
        bool IsPossibilityToDeleteCurrencyRate(CurrencyRate currencyRate, User user, bool checkPermissionOnly = false);

        /// <summary>
        /// Проверка разрешения удалить курс валюты
        /// </summary>
        /// <param name="currencyRate">Курс валюты</param>
        /// <param name="user">Пользователь</param>
        /// <param name="checkPermissionOnly">Проверять только права</param>
        void CheckPossibilityToDeleteCurrencyRate(CurrencyRate currencyRate, User user, bool checkPermissionOnly = false);
    }
}
