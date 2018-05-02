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
    public class ReturnFromClientReasonPresenter : BaseDictionaryPresenter<ReturnFromClientReason>, IReturnFromClientReasonPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        #endregion

        #region Конструктор

        public ReturnFromClientReasonPresenter(IUnitOfWorkFactory unitOfWorkFactory, IReturnFromClientReasonService returnFromClientReasonService, IUserService userService)
            : base(returnFromClientReasonService, userService)
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

        public GridData GetReturnFromClientReasonGrid(GridState state, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = base.GetBaseDictionaryGrid(state, currentUser);

                return model;
            }
        }

        public object GetReturnFromClientReasons()
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                return base.GetBaseDictionarySelectList();
            }
        }

        #endregion

        #region Создание / редактирование / удаление

        public BaseDictionaryEditViewModel Create(UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = base.CreateBaseDictionary(currentUser);

                model.Title = "Добавление основания для возврата товара от клиента";

                return model;
            }
        }

        public BaseDictionaryEditViewModel Edit(short id, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                var model = base.EditBaseDictionary(id, currentUser);
                model.Title = model.AllowToEdit ? "Редактирование основания для возврата товара от клиента" : "Детали основания для возврата товара от клиента";

                return model;
            }
        }

        public object Save(BaseDictionaryEditViewModel model, UserInfo currentUser)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.Serializable))
            {
                var returnFromClientReason = base.SaveBaseDictionary(model, currentUser);

                uow.Commit();

                return new
                {
                    Name = returnFromClientReason.Name,
                    Id = returnFromClientReason.Id
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
