using System;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    /// <summary>
    /// Сервис для сущностей «Тип поставщика»
    /// </summary>
    public class ProviderTypeService : BaseDictionaryService<ProviderType>, IProviderTypeService
    {
        #region Свойства

        protected override string UniquenessErrorString
        {
            get { return "Тип поставщика с таким наименованием уже существует."; }
        }

        protected override string CheckExistenceErrorString
        {
            get { return "Тип поставщика не найден. Возможно, он был удален."; }
        }

        #region Права на операции

        public override Permission CreationPermission
        {
            get { return Permission.ProviderType_Create; }
        }

        public override Permission EditingPermission
        {
            get { return Permission.ProviderType_Edit; }
        }

        public override Permission DeletionPermission
        {
            get { return Permission.ProviderType_Delete; }
        }

        public override Permission ListViewingPermission
        {
            get { return Permission.Provider_List_Details; }
        }

        #endregion

        #endregion

        #region Конструктор

        public ProviderTypeService(IBaseDictionaryRepository<ProviderType> baseDictionaryRepository)
            : base(baseDictionaryRepository)
        {
        }

        #endregion

        #region Методы

        protected override void CheckPossibilityToDelete(ProviderType providerType, User user, bool checkLogic = true)
        {
            CheckPermissionToPerformOperation(user, DeletionPermission);

            if (checkLogic)
            {
                if (baseDictionaryRepository.Query<Provider>().Where(x => x.Type == providerType).Count() > 0)
                {
                    throw new Exception("Невозможно удалить тип поставщика, т.к. существуют поставщики такого типа.");
                }
            }
        }

        #endregion
    }
}