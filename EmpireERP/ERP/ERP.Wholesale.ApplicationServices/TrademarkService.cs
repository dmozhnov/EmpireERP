using System;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    /// <summary>
    /// Сервис для сущностей «Торговая марка»
    /// </summary>
    public class TrademarkService : BaseDictionaryService<Trademark>, ITrademarkService
    {
        #region Свойства

        protected override string UniquenessErrorString
        {
            get { return "Торговая марка с таким наименованием уже существует."; }
        }

        protected override string CheckExistenceErrorString
        {
            get { return "Торговая марка не найдена. Возможно, она была удалена."; }
        }

        #region Права на операции

        public override Permission CreationPermission
        {
            get { return Permission.Trademark_Create; }
        }

        public override Permission EditingPermission
        {
            get { return Permission.Trademark_Edit; }
        }

        public override Permission DeletionPermission
        {
            get { return Permission.Trademark_Delete; }
        }

        public override Permission ListViewingPermission
        {
            get { return Permission.Article_List_Details; }
        }

        #endregion

        #endregion

        #region Конструктор

        public TrademarkService(IBaseDictionaryRepository<Trademark> baseDictionaryRepository)
            : base(baseDictionaryRepository)
        {
        }

        #endregion

        #region Методы

        protected override void CheckPossibilityToDelete(Trademark trademark, User user, bool checkLogic = true)
        {
            CheckPermissionToPerformOperation(user, DeletionPermission);

            if (checkLogic)
            {
                if (baseDictionaryRepository.Query<Article>().Where(x => x.Trademark.Id == trademark.Id).Count() > 0)
                {
                    throw new Exception("Невозможно удалить торговую марку, т.к. существуют товары с такой маркой.");
                }
            }
        }

        #endregion
    }
}
