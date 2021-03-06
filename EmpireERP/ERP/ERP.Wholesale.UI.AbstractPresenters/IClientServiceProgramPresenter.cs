﻿using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IClientServiceProgramPresenter
    {
        BaseDictionaryListViewModel List(UserInfo currentUser);
        GridData GetClientServiceProgramGrid(GridState state, UserInfo currentUser);
        BaseDictionaryEditViewModel Create(UserInfo currentUser);
        BaseDictionaryEditViewModel Edit(short id, UserInfo currentUser);
        object Save(BaseDictionaryEditViewModel model, UserInfo currentUser);
        void Delete(short id, UserInfo currentUser);

        object GetClientServicePrograms();

        /// <summary>
        /// Проверка наименования на уникальность
        /// </summary>
        /// <param name="name">Наименование</param>
        /// <returns></returns>
        void CheckNameUniqueness(string name, short id);
    }
}
