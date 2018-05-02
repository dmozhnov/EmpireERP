using System;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    /// <summary>
    /// Сервис для сущностей «Тип клиента»
    /// </summary>
    public class ClientTypeService : BaseDictionaryService<ClientType>, IClientTypeService
    {
        #region Свойства

        protected override string UniquenessErrorString
        {
            get { return "Тип клиента с таким наименованием уже существует."; }
        }

        protected override string CheckExistenceErrorString
        {
            get { return "Тип клиента не найден. Возможно, он был удален."; }
        }

        #region Права на операции

        public override Permission CreationPermission
        {
            get { return Permission.ClientType_Create; }
        }

        public override Permission EditingPermission
        {
            get { return Permission.ClientType_Edit; }
        }

        public override Permission DeletionPermission
        {
            get { return Permission.ClientType_Delete; }
        }

        public override Permission ListViewingPermission
        {
            get { return Permission.Client_List_Details; }
        }

        #endregion

        #endregion

        #region Конструктор

        public ClientTypeService(IBaseDictionaryRepository<ClientType> baseDictionaryRepository)
            : base(baseDictionaryRepository)
        {
        }

        #endregion

        #region Методы

        protected override void CheckPossibilityToDelete(ClientType clientType, User user, bool checkLogic = true)
        {
            CheckPermissionToPerformOperation(user, DeletionPermission);

            if (checkLogic)
            {
                if (baseDictionaryRepository.Query<Client>().Where(x => x.Type.Id == clientType.Id).Count() > 0)
                {
                    throw new Exception("Невозможно удалить тип клиента, т.к. существуют клиенты с таким типом.");
                }
            }
        }

        #endregion
    }
}