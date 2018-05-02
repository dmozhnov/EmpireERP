using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IArticleService
    {
        Article CheckArticleExistence(int id, string message = "");
        IList<Article> GetFilteredList(object state, bool ignoreDeletedRows = true);
        IList<Article> GetFilteredList(object state, ParameterString param, bool ignoreDeletedRows = true);
        
        /// <summary>
        /// Получение списка товаров по Id
        /// </summary>        
        IEnumerable<Article> GetList(IEnumerable<int> idList);

        /// <summary>
        /// Получение списка товаров по Id группы товаров
        /// </summary>        
        IEnumerable<Article> GetListByArticleGroup(IEnumerable<short> articleGroupIDs);

        /// <summary>
        /// Получение списка товаров по Id группы товаров
        /// </summary>        
        IEnumerable<Article> GetListByArticleGroup(short articleGroupID);

        /// <summary>
        /// Получить отфильтрованный список товаров, которые есть в расширенном наличии для указанных склада и организации. 
        /// В список попадут только те товары, у которых [количество_доступное_для_резерва] > 0.
        /// </summary>
        /// <param name="storage">Место хранения.</param>
        /// <param name="organization">Собственная организация.</param>
        /// <param name="date">Дата.</param>
        /// <param name="state">Параметры фильтра и грида, в который будут выводится товары. После работы сюда будет записано новое состояние грида для возвращаемого списка товаров.</param>
        /// <returns>Отфильтрованный список товаров, имеющихся в расширенном наличии для указанных склада и организации.</returns>
        IList<Article> GetExtendedAvailableArticles(Storage storage, AccountOrganization organization, DateTime date, object state);

        void Save(Article article);
        void Delete(Article article, User user);

        void CheckPossibilityToDelete(Article article, User user);
    }
}
