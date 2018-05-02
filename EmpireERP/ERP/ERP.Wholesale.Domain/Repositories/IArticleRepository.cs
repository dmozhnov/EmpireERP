using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IArticleRepository : IRepository<Article, int>, IFilteredRepository<Article>
    {
        /// <summary>
        /// Получить фильтрованную коллекцию из произвольной коллекции
        /// </summary>
        /// <param name="state">Объект класса GridState</param>
        /// <param name="list">Список товаров для фильтра</param>
        /// <returns>Список товаров, фильтрованный</returns>
        IList<Article> GetFilteredListByCollection(object state, IEnumerable<Article> list);

        /// <summary>
        /// Преобразование идентификатора товара в подзапрос, который ищет товар по этому идентификатору
        /// </summary>
        /// <param name="articleId">Код товара</param>
        ISubQuery GetArticleSubQuery(int articleId);

        /// <summary>
        /// Преобразование коллекции идентификаторов товара в подзапрос, который ищет товары по этим идентификаторам
        /// </summary>
        /// <param name="articleId">Список идентификаторов товаров</param>
        ISubQuery GetListSubQuery(IEnumerable<int> articleIdList);

        /// <summary>
        /// Получение подкритерия для товаров по списку кодов групп товаров
        /// </summary>
        /// <param name="articleGroupIDs">Список кодов групп товаров</param>
        ISubQuery GetArticleSubQueryByArticleGroupList(IEnumerable<short> articleGroupIDs);

        /// <summary>
        /// Получение списка товаров по Id
        /// </summary>        
        IEnumerable<Article> GetList(IEnumerable<int> idList);

        /// <summary>
        /// Получение полного списка товаров
        /// </summary>        
        IEnumerable<Article> GetAll();
    }
}
