using System;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.Currency;
using ERP.Infrastructure.Security;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface ICurrencyPresenter
    {
        CurrencyListViewModel List(UserInfo currentUser);
        GridData GetCurrencyGrid(GridState state, UserInfo currentUser);

        CurrencyEditViewModel Create(UserInfo currentUser);
        CurrencyEditViewModel Edit(short currencyId, UserInfo currentUser);
        GridData GetCurrencyRateGrid(GridState state, UserInfo currentUser);
        object Save(CurrencyEditViewModel model, UserInfo currentUser);
        void Delete(short currencyId, UserInfo currentUser);
        CurrencyRateEditViewModel CreateRate(short currencyId, UserInfo currentUser);

        /// <summary>
        /// Редактирование курса валюты
        /// </summary>
        /// <param name="currencyRateId">Код курса валюты</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Модель данных формы редактирования</returns>
        CurrencyRateEditViewModel EditRate(int currencyRateId, UserInfo currentUser);

        void SaveRate(CurrencyRateEditViewModel model, UserInfo currentUser);
        
        /// <summary>
        /// Удаление курса валюты
        /// </summary>
        /// <param name="currencyRateId">Код курса валюты</param>
        /// <param name="currentUser">Пользователь</param>
        void DeleteRate(int currencyRateId, UserInfo currentUser);

        //object ImportCurrencyRate(short currencyId);
        SelectCurrencyRateViewModel SelectCurrencyRate(short currencyId, string selectFunctionName);
        GridData GetSelectCurrencyRateGrid(GridState state);

        object GetCurrentCurrencyRate(short currencyId);
        object GetCurrencyRate(int currencyRateId);
    }
}
