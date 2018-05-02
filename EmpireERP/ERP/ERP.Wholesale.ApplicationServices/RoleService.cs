using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class RoleService : IRoleService
    {
        #region Поля

        private readonly IRoleRepository roleRepository;

        #endregion

        #region Конструкторы

        public RoleService(IRoleRepository roleRepository)
        {
            this.roleRepository = roleRepository;
        }

        #endregion

        #region Методы

        private Role GetById(short id)
        {
            return roleRepository.GetById(id);
        }

        private Role GetById(short id, User user)
        {
            var type = user.GetPermissionDistributionType(Permission.Role_List_Details);

            // если права нет - то сразу возвращаем null
            if (type == PermissionDistributionType.None)
            {
                return null;
            }
            else
            {
                var role = roleRepository.GetById(id);

                if (type == PermissionDistributionType.All)
                {
                    return role;
                }

                return null;
            }
        }

        /// <summary>
        /// Получение роли по id с проверкой ее существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Role CheckRoleExistence(short id, User user, string message = "")
        {
            var role = GetById(id, user);
            ValidationUtils.NotNull(role, String.IsNullOrEmpty(message) ? "Роль не найдена. Возможно, она была удалена." : message);

            return role;
        }

        public IEnumerable<Role> GetList(User user, Permission permission)
        {
            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<Role>();

                case PermissionDistributionType.Personal:
                case PermissionDistributionType.Teams:
                    return user.Roles;

                case PermissionDistributionType.All:
                    return roleRepository.GetAll();

                default: return null;
            }
        }

        public IEnumerable<Role> GetFilteredList(object state)
        {
            return roleRepository.GetFilteredList(state);
        }

        public IEnumerable<Role> GetFilteredList(object state, ParameterString parameterString, User user)
        {
            return GetFilteredList(state, parameterString, user, Permission.Role_List_Details);
        }

        public IEnumerable<Role> GetFilteredList(object state, ParameterString parameterString, User user, Permission permission)
        {
            if (parameterString == null) parameterString = new ParameterString("");

            var type = user.GetPermissionDistributionType(permission);

            switch (type)
            {
                case PermissionDistributionType.None:
                    return new List<Role>();

                case PermissionDistributionType.Teams:
                    var list = GetList(user, permission).Select(x => x.Id.ToString()).ToList();

                    // если список пуст - то добавляем несуществующее значение
                    if (!list.Any()) { list.Add("0"); }

                    if (parameterString.Keys.Contains("Id"))
                    {
                        if (parameterString["Id"].Operation == ParameterStringItem.OperationType.NotOneOf)
                        {
                            foreach (var excludeId in parameterString["Id"].Value as List<String>)
                            {
                                list.Remove(excludeId);
                            }
                            parameterString.Delete("Id");
                        }
                    }

                    parameterString.Add("Id", ParameterStringItem.OperationType.OneOf);
                    parameterString["Id"].Value = list;
                    break;

                case PermissionDistributionType.All:
                    break;
            }

            return roleRepository.GetFilteredList(state, parameterString);

        }

        public short Save(Role role)
        {
            // проверка допустимости значений распространения прав роли
            CheckPermissionDistributionValues(role);

            // проверка уникальности названия роли 
            var repeatRole = roleRepository.Query<Role>().Where(x => x.Name == role.Name && x.Id != role.Id).FirstOrDefault<Role>();
            if (repeatRole != null)
            {
                throw new Exception("Роль с таким названием уже существует.");
            }

            roleRepository.Save(role);

            return role.Id;
        }

        /// <summary>
        /// Проверка прав роли на корректность значений распространения прав
        /// </summary>
        /// <param name="role"></param>
        private void CheckPermissionDistributionValues(Role role)
        {
            foreach (var rolePermissionDistribution in role.PermissionDistributions)
            {
                var permissionDetails = PermissionDetailsSet.PermissionDetails.First(x => x.Permission == rolePermissionDistribution.Permission);

                foreach (var parentPermissionDetails in permissionDetails.ParentDirectRelations)
                {
                    var pd = role.PermissionDistributions.FirstOrDefault(x => x.Permission == parentPermissionDetails.Permission);

                    if ((pd == null && rolePermissionDistribution.Type != PermissionDistributionType.None) ||
                        (pd != null && pd.Type < rolePermissionDistribution.Type && permissionDetails.AvailableDistributionTypes.Contains(pd.Type)))
                    {
                        throw new Exception(String.Format("Право «{0}» не может иметь распространение, большее, чем право «{1}».",
                            rolePermissionDistribution.Permission.GetDisplayName(), parentPermissionDetails.Permission.GetDisplayName()));
                    }
                }
            }
        }

        #region Добавление / удаление пользователя

        public void AddUser(Role role, User user, User currentUser)
        {
            ValidationUtils.Assert(user != currentUser, "Невозможно назначить самому себе роль.");

            role.AddUser(user);

            roleRepository.Save(role);
        }

        public void RemoveUser(Role role, User user, User currentUser)
        {
            ValidationUtils.Assert(user != currentUser, "Невозможно лишить самого себя роли.");

            ValidationUtils.Assert(!(role.IsSystemAdmin && role.Users.Count() == 1), String.Format("Невозможно лишить роли «{0}» последнего пользователя с этой ролью.", role.Name));

            role.RemoveUser(user);

            roleRepository.Save(role);
        }

        #endregion

        #region Удаление

        public void Delete(Role role, User user)
        {
            CheckPossibilityToDelete(role, user);

            roleRepository.Delete(role);
        }

        #endregion

        #region Проверки прав на совершения операций

        #region Удаление

        public bool IsPossibilityToDelete(Role role, User user)
        {
            try
            {
                CheckPossibilityToDelete(role, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDelete(Role role, User user)
        {
            // права
            user.CheckPermission(Permission.Role_Delete);

            // сущность
            role.CheckPossibilityToDelete();
        }

        #endregion

        #region Редактирование

        public bool IsPossibilityToEdit(Role role, User user)
        {
            try
            {
                CheckPossibilityToEdit(role, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEdit(Role role, User user)
        {
            // права
            user.CheckPermission(Permission.Role_Edit);

            ValidationUtils.Assert(user.Roles.Any(x => x.IsSystemAdmin), "Редактировать роли может только системный администратор.");

            // сущность
            role.CheckPossibilityToEdit();
        }

        #endregion

        #region Редактирование прав на пользователей

        public bool IsPossibilityToEditUserPermissions(Role role, User user)
        {
            try
            {
                CheckPossibilityToEditUserPermissions(role, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEditUserPermissions(Role role, User user)
        {
            ValidationUtils.Assert(!user.Roles.Contains(role), "Невозможно редактировать права пользователей для своих ролей.");
        }

        #endregion

        #region Просмотр деталей

        public bool IsPossibilityToViewDetails(Role role, User user)
        {
            try
            {
                CheckPossibilityToViewDetails(role, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewDetails(Role role, User user)
        {
            // права
            user.CheckPermission(Permission.Role_List_Details);
        }

        #endregion


        #endregion

        #endregion
    }
}
