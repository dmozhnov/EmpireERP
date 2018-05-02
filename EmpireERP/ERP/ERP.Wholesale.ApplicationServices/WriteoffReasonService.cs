using System;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    /// <summary>
    /// Сервис для сущностей «Основание для списания»
    /// </summary>
    public class WriteoffReasonService : BaseDictionaryService<WriteoffReason>, IWriteoffReasonService
    {
        #region Свойства

        protected override string UniquenessErrorString
        {
            get { return "Основание для списания с таким наименованием уже существует."; }
        }

        protected override string CheckExistenceErrorString
        {
            get { return "Основание для списания не найдено. Возможно, оно было удалено."; }
        }

        #region Права на операции

        public override Permission CreationPermission
        {
            get { return Permission.WriteoffReason_Create; }
        }

        public override Permission EditingPermission
        {
            get { return Permission.WriteoffReason_Edit; }
        }

        public override Permission DeletionPermission
        {
            get { return Permission.WriteoffReason_Delete; }
        }

        public override Permission ListViewingPermission
        {
            get { return Permission.WriteoffWaybill_List_Details; }
        }

        #endregion

        #endregion

        #region Конструктор

        public WriteoffReasonService(IBaseDictionaryRepository<WriteoffReason> baseDictionaryRepository)
            : base(baseDictionaryRepository)
        {
        }

        #endregion

        #region Методы

        protected override void CheckPossibilityToDelete(WriteoffReason writeoffReason, User user, bool checkLogic = true)
        {
            CheckPermissionToPerformOperation(user, DeletionPermission);

            if (checkLogic)
            {
                if (baseDictionaryRepository.Query<WriteoffWaybill>().Where(x => x.WriteoffReason.Id == writeoffReason.Id).Count() > 0)
                {
                    throw new Exception("Невозможно удалить основание для списания, т.к. существуют списания товаров с таким основанием.");
                }
            }
        }

        #endregion
    }
}