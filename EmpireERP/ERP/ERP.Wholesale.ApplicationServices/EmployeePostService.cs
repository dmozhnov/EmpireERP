using System;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    /// <summary>
    /// Сервис для сущностей «Должность пользователя»
    /// </summary>
    public class EmployeePostService : BaseDictionaryService<EmployeePost>, IEmployeePostService
    {
        #region Свойства

        protected override string UniquenessErrorString
        {
            get { return "Такая должность пользователя уже существует."; }
        }

        protected override string CheckExistenceErrorString
        {
            get { return "Должность пользователя не найдена. Возможно, она была удалена."; }
        }

        #region Права на операции

        public override Permission CreationPermission
        {
            get { return Permission.EmployeePost_Create; }
        }

        public override Permission EditingPermission
        {
            get { return Permission.EmployeePost_Edit; }
        }

        public override Permission DeletionPermission
        {
            get { return Permission.EmployeePost_Delete; }
        }

        public override Permission ListViewingPermission
        {
            get { return Permission.User_List_Details; }
        }

        #endregion

        #endregion

        #region Конструктор

        public EmployeePostService(IBaseDictionaryRepository<EmployeePost> baseDictionaryRepository)
            : base(baseDictionaryRepository)
        {
        }

        #endregion

        #region Методы

        protected override void CheckPossibilityToDelete(EmployeePost employeePost, User user, bool checkLogic = true)
        {
            CheckPermissionToPerformOperation(user, DeletionPermission);

            if (checkLogic)
            {
                if (baseDictionaryRepository.Query<Employee>().Where(x => x.Post.Id == employeePost.Id).Count() > 0)
                {
                    throw new Exception("Невозможно удалить должность пользователя, т.к. существуют пользователи с такой должностью.");
                }
            }
        }

        #endregion
    }
}
