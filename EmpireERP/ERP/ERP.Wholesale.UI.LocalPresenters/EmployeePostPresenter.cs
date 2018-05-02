using System.Data;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;
using ERP.Wholesale.UI.ViewModels.EmployeePost;


namespace ERP.Wholesale.UI.LocalPresenters
{
    public class EmployeePostPresenter : BaseDictionaryPresenter<EmployeePost>, IEmployeePostPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        #endregion

        #region Конструктор

        public EmployeePostPresenter(IUnitOfWorkFactory unitOfWorkFactory, IEmployeePostService employeePostService, IUserService userService)
            : base(employeePostService, userService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        #endregion

        #region Методы

        #region Список

        public BaseDictionaryListViewModel List(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = base.ListBaseDictionary(currentUser);

                return model;
            }
        }

        public GridData GetEmployeePostGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = base.GetBaseDictionaryGrid(state, currentUser);

                return model;
            }
        }

        public object GetEmployeePosts()
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                return base.GetBaseDictionarySelectList();
            }
        }

        #endregion

        #region Создание / редактирование / удаление

        public EmployeePostEditViewModel Create(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = base.CreateBaseDictionary(currentUser);

                model.Title = "Добавление должности пользователя";

                return new EmployeePostEditViewModel(model);
            }
        }

        public EmployeePostEditViewModel Edit(short id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = base.EditBaseDictionary(id, currentUser);
                model.Title = model.AllowToEdit ? "Редактирование должности пользователя" : "Детали должности пользователя";

                return new EmployeePostEditViewModel(model);
            }
        }

        public object Save(EmployeePostEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var employeePost = base.SaveBaseDictionary(model, currentUser);

                uow.Commit();

                return new
                {
                    Name = employeePost.Name,
                    Id = employeePost.Id
                };
            }
        }

        public void Delete(short id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                base.DeleteBaseDictionary(id, currentUser);

                uow.Commit();
            }
        }

        #endregion


        #region Вспомогательные методы

        /// <summary>
        /// Проверка наименования на уникальность
        /// </summary>
        /// <param name="name">Наименование</param>
        /// <returns></returns>
        public void CheckNameUniqueness(string name, short id)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                base.CheckBaseDictionaryNameUniqueness(name, id);
            }
        }

        #endregion

        #endregion
    }
}
