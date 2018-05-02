using System;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    /// <summary>
    /// Сервис для сущностей «Программа обслуживания клиента»
    /// </summary>
    public class ClientServiceProgramService : BaseDictionaryService<ClientServiceProgram>, IClientServiceProgramService
    {
        #region Свойства

        protected override string UniquenessErrorString
        {
            get { return "Программа обслуживания клиента с таким наименованием уже существует."; }
        }

        protected override string CheckExistenceErrorString
        {
            get { return "Программа обслуживания клиента не найдена. Возможно, она была удалена."; }
        }

        #region Права на операции

        public override Permission CreationPermission
        {
            get { return Permission.ClientServiceProgram_Create; }
        }

        public override Permission EditingPermission
        {
            get { return Permission.ClientServiceProgram_Edit; }
        }

        public override Permission DeletionPermission
        {
            get { return Permission.ClientServiceProgram_Delete; }
        }

        public override Permission ListViewingPermission
        {
            get { return Permission.Client_List_Details; }
        }

        #endregion

        #endregion

        #region Конструктор

        public ClientServiceProgramService(IBaseDictionaryRepository<ClientServiceProgram> baseDictionaryRepository)
            : base(baseDictionaryRepository)
        {
        }

        #endregion

        #region Методы

        protected override void CheckPossibilityToDelete(ClientServiceProgram clientServiceProgram, User user, bool checkLogic = true)
        {
            CheckPermissionToPerformOperation(user, DeletionPermission);

            if (checkLogic)
            {
                if (baseDictionaryRepository.Query<Client>().Where(x => x.ServiceProgram.Id == clientServiceProgram.Id).Count() > 0)
                {
                    throw new Exception("Невозможно удалить программу обслуживания клиента, т.к. существуют клиенты с такой программой обслуживания.");
                }
            }
        }

        #endregion
    }
}