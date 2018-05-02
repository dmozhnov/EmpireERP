using System;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    /// <summary>
    /// Сервис для сущностей «Основание для возврата товара от клиента»
    /// </summary>
    public class ReturnFromClientReasonService : BaseDictionaryService<ReturnFromClientReason>, IReturnFromClientReasonService
    {
        #region Свойства

        protected override string UniquenessErrorString
        {
            get { return "Основание для возврата товара от клиента с таким наименованием уже существует."; }
        }

        protected override string CheckExistenceErrorString
        {
            get { return "Основание для возврата товара от клиента не найдено. Возможно, оно было удалено."; }
        }

        #region Права на операции

        public override Permission CreationPermission
        {
            get { return Permission.ReturnFromClientReason_Create; }
        }

        public override Permission EditingPermission
        {
            get { return Permission.ReturnFromClientReason_Edit; }
        }

        public override Permission DeletionPermission
        {
            get { return Permission.ReturnFromClientReason_Delete; }
        }

        public override Permission ListViewingPermission
        {
            get { return Permission.Client_List_Details; }
        }

        #endregion

        #endregion

        #region Конструктор

        public ReturnFromClientReasonService(IBaseDictionaryRepository<ReturnFromClientReason> baseDictionaryRepository)
            : base(baseDictionaryRepository)
        {
        }

        #endregion

        #region Методы

        protected override void CheckPossibilityToDelete(ReturnFromClientReason returnFromClientReason, User user, bool checkLogic = true)
        {
            CheckPermissionToPerformOperation(user, DeletionPermission);

            if (checkLogic)
            {
                if (baseDictionaryRepository.Query<ReturnFromClientWaybill>().Where(x => x.ReturnFromClientReason.Id == returnFromClientReason.Id).Count() > 0)
                {
                    throw new Exception("Невозможно удалить основание для возврата товара от клиента, т.к. существуют возвраты товаров с таким основанием.");
                }
            }
        }

        #endregion
    }
}