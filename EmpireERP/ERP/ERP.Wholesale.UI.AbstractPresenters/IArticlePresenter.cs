using System;
using System.Collections.Generic;
using ERP.Infrastructure.Security;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.UI.ViewModels.Article;

namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IArticlePresenter
    {
        ArticleListViewModel List(UserInfo currentUser);
        GridData GetActualArticleGrid(GridState state, UserInfo currentUser);
        GridData GetObsoleteArticleGrid(GridState state, UserInfo currentUser);
        GridData GetArticleSelectGrid(GridState state, UserInfo currentUser);
        GridData GetArticleBatchSelectGrid(GridState state, UserInfo currentUser);

        ArticleBatchSelectViewModel SelectArticleBatch(int articleId, short senderStorageId, short recipientStorageId, int senderId, UserInfo currentUser, string date = "", Guid? articleBatchToExcludeId = null);
        ArticleBatchSelectViewModel SelectArticleBatchByStorage(int articleId, short storageId, int senderId, UserInfo currentUser, string date = "", Guid? articleBatchToExcludeId = null);

        ArticleEditViewModel Create(UserInfo currentUser);
        ArticleEditViewModel Edit(int id, UserInfo currentUser);
        ArticleEditViewModel Copy(int id, UserInfo currentUser);

        /// <summary>
        /// Получить словарь товаров по идентификатору группы товара
        /// </summary>
        /// <param name="articleGroupId">Идентификатор группы товара</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns>Dictionary[id,название товара]</returns>
        Dictionary<string, string> GetArticleFromArticleGroup(short articleGroupId, UserInfo currentUser);

        object Save(ArticleEditViewModel model, UserInfo currentUser);
        void Delete(int id, UserInfo currentUser);

        /// <summary>
        /// Выбор товара из полного перечня товаров
        /// </summary>
        /// <returns></returns>
        ArticleSelectViewModel SelectArticle(UserInfo currentUser);

        /// <summary>
        /// Выбор товара из перечня товаров на складе storageId организации senderId
        /// </summary>
        /// <returns></returns>
        ArticleSelectViewModel SelectArticleFromStorage(short storageId, int senderId, UserInfo currentUser);
        
        /// <summary>
        /// Получить список товаров для возврата
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <param name="teamId">Код команды</param>
        /// <param name="recipientId">Код организации</param>
        /// <param name="currentUser">Пользователь</param>
        /// <returns></returns>
        ArticleSelectViewModel SelectArticleToReturn(int dealId, int teamId, int recipientId, UserInfo currentUser);

        /// <summary>
        /// Получить грид партий для возврата товара
        /// </summary>
        /// <param name="AvailableToReturnTotalCount">Общее количество товара для возврата</param>
        /// <param name="state">Состояние грида</param>
        /// <returns></returns>
        GridData GetArticleSaleSelectGrid(GridState state, UserInfo currentUser);

        ArticleSaleSelectViewModel SelectArticleSale(int articleId, int dealId, short teamId, int recipientId, short storageId, UserInfo currentUser, Guid? articleSaleToExcludeId = null);
    }
}
