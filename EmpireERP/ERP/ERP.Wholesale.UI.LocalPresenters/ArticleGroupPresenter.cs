using System.Collections.Generic;
using System.Data;
using System.Linq;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.ViewModels.TreeView;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.ArticleGroup;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ArticleGroupPresenter : IArticleGroupPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;
        private readonly IArticleGroupService articleGroupService;
        private readonly IUserService userService;

        #endregion

        #region Конструкторы

        public ArticleGroupPresenter(IUnitOfWorkFactory unitOfWorkFactory, IArticleGroupService articleGroupService, IUserService userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;

            this.articleGroupService = articleGroupService;
            this.userService = userService;
        }

        #endregion 

        #region Методы

        #region Список

        public ArticleGroupListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var model = new ArticleGroupListViewModel();

                model.ArticleGroupTree = GetArticleGroupTree("");
                model.AllowToCreate = user.HasPermission(Permission.ArticleGroup_Create);

                return model;
            }
        }

        public TreeData GetArticleGroupTree(string valueToSelect)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                TreeData tree = new TreeData();
                tree.ValueToSelect = valueToSelect;

                var articleGroups = articleGroupService.GetList();

                foreach (var articleGroup in articleGroups.Where(x => x.Parent == null).OrderBy(x => x.Name))
                {
                    TreeNode node = new TreeNode(articleGroup.Name, articleGroup.Id.ToString(), null);
                    tree.Nodes.Add(node);
                    FillChildArticleGroups(node, articleGroup.Childs);
                }

                return tree;
            }
        }

        #endregion

        #region Детали

        public ArticleGroupDetailsViewModel Details(short id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var articleGroup = articleGroupService.CheckArticleGroupExistence(id);

                var model = new ArticleGroupDetailsViewModel();
                model.Id = articleGroup.Id;
                model.ParentArticleGroupId = (articleGroup.Parent == null ? (short?)null : articleGroup.Parent.Id);
                model.Name = articleGroup.Name;
                model.MarkupPercent = articleGroup.MarkupPercent.ForDisplay(ValueDisplayType.Percent);
                model.SalaryPercent = articleGroup.SalaryPercent.ForDisplay(ValueDisplayType.Percent);
                model.Comment = articleGroup.Comment;
                model.NameFor1C = articleGroup.NameFor1C;

                model.AllowToCreate = user.HasPermission(Permission.ArticleGroup_Create);
                model.AllowToEdit = user.HasPermission(Permission.ArticleGroup_Edit);
                model.AllowToDelete = user.HasPermission(Permission.ArticleGroup_Delete);

                return model;
            }
        }

        private void FillChildArticleGroups(TreeNode parentNode, IEnumerable<ArticleGroup> childGroups)
        {
            foreach (var articleGroup in childGroups.OrderBy(x => x.Name))
            {
                TreeNode node = new TreeNode(articleGroup.Name, articleGroup.Id.ToString(), parentNode);
                parentNode.ChildNodes.Add(node);
                FillChildArticleGroups(node, articleGroup.Childs);
            }
        }

        #endregion

        #region Создание, редактирование, удаление

        public ArticleGroupEditViewModel Create(short? parentGroupId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                user.CheckPermission(Permission.ArticleGroup_Create);

                var model = new ArticleGroupEditViewModel();
                model.Title = "Добавление группы товаров";
                model.ParentArticleGroupId = parentGroupId;

                if (parentGroupId != null)
                {
                    var parentArticleGroup = articleGroupService.CheckArticleGroupExistence(parentGroupId.Value, "Родительская группа товаров не найдена. Возможно, она была удалена.");

                    // процент наценки берем из родителя
                    model.MarkupPercent = parentArticleGroup.MarkupPercent.ForEdit();
                    model.SalaryPercent = parentArticleGroup.SalaryPercent.ForEdit();
                }
                else
                {
                    model.MarkupPercent = "0";
                    model.SalaryPercent = "0";
                }
                model.AllowToEdit = true;

                return model;
            }
        }

        public ArticleGroupEditViewModel Edit(short id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                var articleGroup = articleGroupService.CheckArticleGroupExistence(id);

                var model = new ArticleGroupEditViewModel
                {
                    Id = articleGroup.Id,
                    ParentArticleGroupId = (articleGroup.Parent == null ? (short?)null : articleGroup.Parent.Id),
                    Name = articleGroup.Name,
                    SalaryPercent = articleGroup.SalaryPercent.ForEdit(),
                    Comment = articleGroup.Comment,
                    MarkupPercent = articleGroup.MarkupPercent.ForEdit(),
                    Title = "Редактирование группы товаров",
                    NameFor1C = articleGroup.NameFor1C,

                    AllowToEdit = user.HasPermission(Permission.ArticleGroup_Edit)
                };

                return model;
            }
        }

        public object Save(ArticleGroupEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);

                ArticleGroup parentArticleGroup = null;

                if (model.ParentArticleGroupId != null)
                {
                    parentArticleGroup = articleGroupService.CheckArticleGroupExistence(model.ParentArticleGroupId.Value,
                        "Родительская группа товаров не найдена. Возможно, она была удалена.");
                }

                ArticleGroup articleGroup = null;
                var isNewArticleGroup = false;

                // добавление группы товаров
                if (model.Id == 0)
                {
                    user.CheckPermission(Permission.ArticleGroup_Create);

                    articleGroup = new ArticleGroup(model.Name, model.NameFor1C);

                    isNewArticleGroup = true;

                    if (parentArticleGroup != null)
                    {
                        parentArticleGroup.AddChildGroup(articleGroup);
                    }
                }
                // редактирование группы товаров
                else
                {
                    user.CheckPermission(Permission.ArticleGroup_Edit);

                    articleGroup = articleGroupService.CheckArticleGroupExistence(model.Id);
                    articleGroup.Name = model.Name;
                    articleGroup.NameFor1C = model.NameFor1C;
                }
                articleGroup.Comment = StringUtils.ToHtml(model.Comment);
                articleGroup.SalaryPercent = ValidationUtils.TryGetDecimal(model.SalaryPercent);
                articleGroup.MarkupPercent = ValidationUtils.TryGetDecimal(model.MarkupPercent);
                

                articleGroupService.Save(articleGroup);

                uow.Commit();

                var j = new
                {
                    Name = articleGroup.Name,
                    Id = articleGroup.Id,
                    ParentId = (articleGroup.Parent == null ? (short?)null : articleGroup.Parent.Id),
                    IsNewArticleGroup = isNewArticleGroup,
                    NameFor1C = articleGroup.NameFor1C
                };

                return j;
            }
        }

        public void Delete(short id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var articleGroup = articleGroupService.CheckArticleGroupExistence(id);

                articleGroupService.Delete(articleGroup, user);

                uow.Commit();
            }
        }

        #endregion  

        #region Получение информации о группе товара

        public object GetArticleGroupInfo(short articleGroupId, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var user = userService.CheckUserExistence(currentUser.Id);
                var articleGroup = articleGroupService.CheckArticleGroupExistence(articleGroupId);

                var result = new
                {
                    MarkupPercent = articleGroup.MarkupPercent.ForEdit(),
                    SalaryPercent = articleGroup.SalaryPercent.ForEdit()
                };

                return result;
            }
        }

        #endregion

        #endregion
    }
}
