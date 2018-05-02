using System.Data;
using ERP.Infrastructure.Security;
using ERP.Infrastructure.UnitOfWork;
using ERP.UI.ViewModels.Grid;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.UI.AbstractPresenters;
using ERP.Wholesale.UI.ViewModels.BaseDictionary;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class ProviderTypePresenter : BaseDictionaryPresenter<ProviderType>, IProviderTypePresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        #endregion

        #region Конструктор

        public ProviderTypePresenter(IUnitOfWorkFactory unitOfWorkFactory, IProviderTypeService providerTypeService, IUserService userService)
            : base(providerTypeService, userService)
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

        public GridData GetProviderTypeGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = base.GetBaseDictionaryGrid(state, currentUser);

                return model;
            }
        }

        #endregion

        #region Создание / редактирование / удаление

        public BaseDictionaryEditViewModel Create(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = base.CreateBaseDictionary(currentUser);

                model.Title = "Добавление типа поставщика";

                return model;
            }
        }

        public BaseDictionaryEditViewModel Edit(short id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = base.EditBaseDictionary(id, currentUser);
                model.Title = model.AllowToEdit ? "Редактирование типа поставщика" : "Детали типа поставщика";

                return model;
            }
        }

        public object Save(BaseDictionaryEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var providerType = base.SaveBaseDictionary(model, currentUser);

                uow.Commit();

                return new
                {
                    Name = providerType.Name,
                    Id = providerType.Id
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

        #region Выбор

        public BaseDictionarySelectViewModel SelectProviderType(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = base.SelectBaseDictionary(currentUser);

                return model;
            }
        }

        public GridData GetProviderTypeSelectGrid(GridState state)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = base.GetBaseDictionarySelectGrid(state);

                return model;
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