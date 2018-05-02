using System;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    /// <summary>
    /// Сервис для сущностей «Регион клиента»
    /// </summary>
    public class ClientRegionService : BaseDictionaryService<ClientRegion>, IClientRegionService
    {
        #region Свойства

        protected override string UniquenessErrorString
        {
            get { return "Регион клиента с таким наименованием уже существует."; }
        }

        protected override string CheckExistenceErrorString
        {
            get { return "Регион клиента не найден. Возможно, он был удален."; }
        }

        #region Права на операции

        public override Permission CreationPermission
        {
            get { return Permission.ClientRegion_Create; }
        }

        public override Permission EditingPermission
        {
            get { return Permission.ClientRegion_Edit; }
        }

        public override Permission DeletionPermission
        {
            get { return Permission.ClientRegion_Delete; }
        }

        public override Permission ListViewingPermission
        {
            get { return Permission.Client_List_Details; }
        }

        #endregion

        #endregion

        #region Конструктор

        public ClientRegionService(IBaseDictionaryRepository<ClientRegion> baseDictionaryRepository)
            : base(baseDictionaryRepository)
        {
        }

        #endregion

        #region Методы

        protected override void CheckPossibilityToDelete(ClientRegion clientRegion, User user, bool checkLogic = true)
        {
            CheckPermissionToPerformOperation(user, DeletionPermission);

            if (checkLogic)
            {
                if (baseDictionaryRepository.Query<Client>().Where(x => x.Region.Id == clientRegion.Id).Count() > 0)
                {
                    throw new Exception("Невозможно удалить регион клиента, т.к. существуют клиенты с таким регионом.");
                }
            }
        }

        #endregion
    }
}
