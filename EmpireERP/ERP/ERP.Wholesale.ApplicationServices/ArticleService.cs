using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.IoC;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class ArticleService : IArticleService
    {
        #region Поля

        private readonly IArticleRepository articleRepository;
        private readonly IReceiptWaybillRepository receiptWaybillRepository;
        private readonly IArticleAvailabilityService articleAvailabilityService;

        #endregion

        #region Конструкторы

        public ArticleService(IArticleRepository articleRepository, IReceiptWaybillRepository receiptWaybillRepository)
        {
            this.articleRepository = articleRepository;
            this.receiptWaybillRepository = receiptWaybillRepository;

            articleAvailabilityService = IoCContainer.Resolve<IArticleAvailabilityService>();
        }

        #endregion

        #region Методы

        #region Получение по Id

        /// <summary>
        /// Получение товара по id с проверкой его существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Article CheckArticleExistence(int id, string message = "")
        {
            var article = articleRepository.GetById(id);
            ValidationUtils.NotNull(article, String.IsNullOrEmpty(message) ? "Товар не найден. Возможно, он был удален." : message);

            return article;
        }
        #endregion

        #region Списки

        public IList<Article> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return articleRepository.GetFilteredList(state, ignoreDeletedRows);
        }

        public IList<Article> GetFilteredList(object state, ParameterString param, bool ignoreDeletedRows = true)
        {
            if (param == null)
            {
                param = new ParameterString("");
            }

            return articleRepository.GetFilteredList(state, param, ignoreDeletedRows);
        }

        /// <summary>
        /// Получить отфильтрованный список товаров, которые есть в расширенном наличии для указанных склада и организации. 
        /// В список попадут только те товары, у которых [количество_доступное_для_резерва] > 0.
        /// </summary>
        /// <param name="storage">Место хранения.</param>
        /// <param name="organization">Собственная организация.</param>
        /// <param name="date">Дата.</param>
        /// <param name="state">Параметры фильтра и грида, в который будут выводится товары. После работы сюда будет записано новое состояние грида для возвращаемого списка товаров.</param>
        /// <returns>Отфильтрованный список товаров, имеющихся в расширенном наличии для указанных склада и организации.</returns>
        public IList<Article> GetExtendedAvailableArticles(Storage storage, AccountOrganization organization, DateTime date, object state)
        {
           var articleIdList = articleAvailabilityService.GetExtendedArticleAvailability(storage.Id, organization.Id, date).Where(x => x.Count > 0).Select(x => x.ArticleId).Distinct().ToList();

            var articleList = articleRepository.GetList(articleIdList);

            return articleRepository.GetFilteredListByCollection(state, articleList);
        }

        // TODO: Оставить только метод репозитория
        /// <summary>
        /// Получение списка товаров по Id
        /// </summary>        
        public IEnumerable<Article> GetList(IEnumerable<int> idList)
        {
            return articleRepository.GetList(idList);                
        }
        
        /// <summary>
        /// Получение списка товаров по Id группы товаров
        /// </summary>        
        public IEnumerable<Article> GetListByArticleGroup(IEnumerable<short> articleGroupIDs)
        {
            var articleIDsSubQuery = articleRepository.GetArticleSubQueryByArticleGroupList(articleGroupIDs);
            var list = articleRepository.Query<Article>()
                .PropertyIn(x => x.Id, articleIDsSubQuery)
                .ToList<Article>();
            return list;
        }

        /// <summary>
        /// Получение списка товаров по Id группы товаров
        /// </summary>        
        public IEnumerable<Article> GetListByArticleGroup(short articleGroupID)
        {
            return GetListByArticleGroup(new List<short> { articleGroupID });
        }
        #endregion

        public void Save(Article article)
        {
            CheckArticleNumberUniqueness(article);
            articleRepository.Save(article);
        }

        public void Delete(Article article, User user)
        {
            CheckPossibilityToDelete(article, user);

            articleRepository.Delete(article);
        }

        #region Вспомогательные методы

        /// <summary>
        /// Проверка артикула товара на уникальность
        /// </summary>
        /// <param name="model"></param>
        private void CheckArticleNumberUniqueness(Article article)
        {
            int count = articleRepository.Query<Article>().Where(x => x.Number == article.Number && x.Id != article.Id).Count();
            if (count > 0)
            {
                throw new Exception("Товар с таким артикулом уже существует.");
            }
        }

        #endregion

        #region Права на удаление

        public void CheckPossibilityToDelete(Article article, User user)
        {
            user.CheckPermission(Permission.Article_Delete);

            int receiptWaybillTotalCount;
            List<ReceiptWaybill> receiptWaybillList = receiptWaybillRepository.GetListByArticleId(article.Id, 3, out receiptWaybillTotalCount).ToList();
            
            var productionOrderBatchRowList = articleRepository.Query<ProductionOrderBatchRow>().Where(x => x.Article.Id == article.Id)
                .ToList<ProductionOrderBatchRow>();
            var productionOrderBatchList = productionOrderBatchRowList.Select(x => x.Batch).Distinct();
            var productionOrderList = productionOrderBatchList.Select(x => x.ProductionOrder).Distinct().OrderByDescending(x => x.CreationDate);
            var productionOrderTakeList = productionOrderList.Take(3);

            if (receiptWaybillList.Count > 0 || productionOrderList.Count() > 0)
            {
                string exceptionString = "Невозможно удалить товар, так как он используется в";
                bool useComma = false;

                if (receiptWaybillList.Count > 0)
                {
                    foreach (ReceiptWaybill waybill in receiptWaybillList)
                    {
                        if (useComma)
                            exceptionString += ",";
                        exceptionString += String.Format(" " + "приходной накладной №{0}", waybill.Number);
                        useComma = true;
                    }

                    if (receiptWaybillTotalCount > 3)
                        exceptionString += " и еще в " + (receiptWaybillTotalCount - 3) + " приходных накладных";
                }

                if (productionOrderList.Count() > 0)
                {
                    foreach (ProductionOrder productionOrder in productionOrderTakeList)
                    {
                        if (useComma)
                            exceptionString += ",";
                        exceptionString += String.Format(" " + "заказе на производство товаров «{0}»", productionOrder.Name);
                        useComma = true;
                    }

                    if (productionOrderList.Count() > 3)
                        exceptionString += " и еще в " + (productionOrderList.Count() - 3) + " заказах на производство товаров";
                }

                exceptionString += ".";

                throw new Exception(exceptionString);
            }
        }
        #endregion

        #endregion
    }
}
