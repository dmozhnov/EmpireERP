using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.IoC;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using ERP.Utils;

namespace ERP.Wholesale.ApplicationServices
{
    public class ArticleGroupService : IArticleGroupService
    {
        #region Поля

        private readonly IArticleGroupRepository articleGroupRepository;

        #endregion

        #region Конструкторы

        public ArticleGroupService(IArticleGroupRepository articleGroupRepository)
        {
            this.articleGroupRepository = articleGroupRepository;
        }

        #endregion

        #region Методы
        
        public IEnumerable<ArticleGroup> GetList()
        {
            return articleGroupRepository.GetAll();
        }

        /// <summary>
        /// Получение группы товаров по id с проверкой ее существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ArticleGroup CheckArticleGroupExistence(short id, string message = "")
        {
            var articleGroup = articleGroupRepository.GetById(id);
            ValidationUtils.NotNull(articleGroup, String.IsNullOrEmpty(message) ? "Группа товаров не найдена. Возможно, она была удалена." : message);

            return articleGroup;
        }

        public void Save(ArticleGroup articleGroup)
        {
            CheckArticleGroupNameUniqueness(articleGroup);
            articleGroupRepository.Save(articleGroup);
        }

        public void Delete(ArticleGroup articleGroup, User user)
        {
            CheckPossibilityToDelete(articleGroup, user);

            articleGroupRepository.Delete(articleGroup);
        }

        #region Права на удаление

        private void CheckPossibilityToDelete(ArticleGroup articleGroupToDelete, User user)
        {
            user.CheckPermission(Permission.ArticleGroup_Delete);

            if (articleGroupRepository.Query<Article>().Where(x => x.ArticleGroup.Id == articleGroupToDelete.Id).ToList<Article>().Any())
            {
                throw new Exception("Невозможно удалить группу, в которой имеются товары.");
            }
        }

        #endregion

        #region Вспомогательные методы

        private bool IsNameUnique(ArticleGroup articleGroup)
        {
            //var parent = articleGroupRepository.GetById(parentArticleGroupId);
            int articleGroupsCount;

            if (articleGroup.Parent != null)
            {
                articleGroupsCount = articleGroupRepository.Query<ArticleGroup>()
                .Where(x => (x.Parent.Id == articleGroup.Parent.Id && x.Id != articleGroup.Id) && (x.Name == articleGroup.Name)).Count();
            }
            else
            {
                articleGroupsCount = articleGroupRepository.Query<ArticleGroup>()
                .Where(x => (x.Parent.Id == null && x.Id != articleGroup.Id) && (x.Name == articleGroup.Name)).Count();
            }

            if (articleGroupsCount > 0)
                return false;

            return true;
        }

        /// <summary>
        /// Проверка названия группы товара на уникальность
        /// </summary>
        /// <param name="model"></param>
        private void CheckArticleGroupNameUniqueness(ArticleGroup articleGroup)
        {
            if (!IsNameUnique(articleGroup))
            {
                throw new Exception("Группа товаров с таким названием в этой же родительской группе уже существует.");
            }
        }

        #endregion

        #endregion
    }
}
