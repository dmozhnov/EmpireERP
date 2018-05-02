using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ArticleRepository : BaseNHRepository, IArticleRepository
    {
        public ArticleRepository()
            : base()
        {
        }

        public Article GetById(int id)
        {
            return Query<Article>().Where(x => x.Id == id).FirstOrDefault<Article>();
        }

        public void Save(Article Value)
        {
            CurrentSession.SaveOrUpdate(Value);
        }

        public void Delete(Article Value)
        {
            CurrentSession.Delete(Value);
        }

        /// <summary>
        /// Получение списка товаров по Id
        /// </summary>        
        public IEnumerable<Article> GetList(IEnumerable<int> idList)
        {
            return base.GetList<int, Article>(idList).Values.ToList<Article>();
        }

        /// <summary>
        /// Получение полного списка товаров
        /// </summary>        
        public IEnumerable<Article> GetAll()
        {
            return CurrentSession.CreateCriteria<Article>().List<Article>();
        }

        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        /// <param name="state">Объект класса GridState</param>
        /// <returns>Список объектов</returns>
        public IList<Article> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Article>(state, ignoreDeletedRows);
        }
        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        public IList<Article> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Article>(state, parameterString, ignoreDeletedRows);
        }

        /// <summary>
        /// Определяем собственную реализацию фильтра для товаров
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="criteria"></param>
        protected override void CreateFilter<T>(Infrastructure.Repositories.Criteria.ICriteria<T> criteria)
        {
            Infrastructure.Repositories.Criteria.ICriteria<Article> crit = (Infrastructure.Repositories.Criteria.ICriteria<Article>)criteria;

            Regex regexp = new Regex(@"ArticleGroup=\d+;");
            var match = regexp.Match(filter);
            IEnumerable<ArticleGroup> childGroups = null;
            if (match.Length > 0)
            {
                var value = match.Value;
                filter = filter.Replace(value, "");
                var articleGroupId = Convert.ToInt16(value.Replace("ArticleGroup=", "").Replace(";", ""));

                var articleGroupRepository = new ArticleGroupRepository();
                var articleGroup = articleGroupRepository.GetById(articleGroupId);

                childGroups = (new List<ArticleGroup> { articleGroup }).Concat(articleGroup.Childs.Concat(articleGroup.Childs.SelectMany(x => x.Childs)));
            }

            if (childGroups != null)
            {
                crit.OneOf("ArticleGroup", childGroups);
            }

            base.CreateFilter<T>(criteria);
        }

        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        /// <param name="state">Объект класса GridState</param>
        /// <param name="storageId">Идентификатор склада</param>
        /// <returns>Список объектов</returns>
        public IList<Article> GetFilteredListByCollection(object state, IEnumerable<Article> list)
        {
            //----------------------------------------------------------------------------------------------------------------
            //   Не реализовано через базовый метод, т.к. метод не тянет данные из БД, а использует переданную ему коллекцию
            //----------------------------------------------------------------------------------------------------------------

            ReadState(state);   //Читаем данные из объекта state

            ParameterString df = new ParameterString(filter);

            #region Фильтр

            if (df["ArticleGroup"] != null && !String.IsNullOrEmpty((string)df["ArticleGroup"].Value))
                list = list.Where(x => x.ArticleGroup.Id.ToString() == (string)(df["ArticleGroup"].Value));

            if (df["Id"] != null && !String.IsNullOrEmpty(df["Id"].Value as string))
                list = list.Where(x => x.Id.ToString() == df["Id"].Value as string);

            if (df["Number"] != null && !String.IsNullOrEmpty(df["Number"].Value as string))
                list = list.Where(x => x.Number.ToLower().Contains((df["Number"].Value as string).ToLower()));

            if (df["FullName"] != null && !String.IsNullOrEmpty(df["FullName"].Value as string))
                list = list.Where(x => x.FullName.ToLower().Contains((df["FullName"].Value as string).ToLower()));

            if (df["Manufacturer"] != null && !String.IsNullOrEmpty(df["Manufacturer"].Value as string))
                list = list.Where(x => x.Manufacturer != null && x.Manufacturer.Id.ToString() == df["Manufacturer"].Value.ToString());

            if (df["Trademark"] != null && !String.IsNullOrEmpty(df["Trademark"].Value as string))
                list = list.Where(x => x.Trademark != null && x.Trademark.Id.ToString() == df["Trademark"].Value.ToString());

            var crit = Query<Article>();

            CreateFilter(crit);

            #endregion

            int totalRowCount = list.Count();
            WriteTotalRowCount(state, totalRowCount);   //Записываем общее кол-во строк

            //Вычисляем начальную и конечную строку страницы
            int maxPage = (totalRowCount + pageSize - 1) / pageSize;
            currentPage = currentPage > maxPage ? maxPage : currentPage;

            IEnumerable<Article> orderedList;

            if (sort != null && sort.Replace(" ", "") == "Number=Asc")
            {
                orderedList = list.OrderBy(x => x.Number);
            }
            else
            {
                orderedList = list.OrderBy(x => x.Id);
            }

            return orderedList.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// Преобразование идентификатора товара в подзапрос, который ищет товар по этому идентификатору
        /// </summary>
        /// <param name="articleId">Код товара</param>
        public ISubQuery GetArticleSubQuery(int articleId)
        {
            return SubQuery<Article>().Where(x => x.Id == articleId).Select(x => x.Id);
        }

        /// <summary>
        /// Преобразование коллекции идентификаторов товара в подзапрос, который ищет товары по этим идентификаторам
        /// </summary>
        /// <param name="articleId">Список идентификаторов товаров</param>
        public ISubQuery GetListSubQuery(IEnumerable<int> articleIdList)
        {
            return SubQuery<Article>().OneOf(x => x.Id, articleIdList).Select(x => x.Id);
        }

        /// <summary>
        /// Получение подкритерия для товаров по списку кодов групп товаров
        /// </summary>
        /// <param name="articleGroupIDs">Список кодов групп товаров</param>
        public ISubQuery GetArticleSubQueryByArticleGroupList(IEnumerable<short> articleGroupIDs)
        {
            return SubQuery<Article>()
                .OneOf(x => x.ArticleGroup.Id, articleGroupIDs)
                .Select(x => x.Id);
        }
    }
}
