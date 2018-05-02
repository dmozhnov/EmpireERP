using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Settings;

namespace ERP.Wholesale.ApplicationServices
{
    public class UserService : IUserService
    {
        #region Поля

        private readonly IUserRepository userRepository;
        private readonly ISettingRepository settingRepository;

        #endregion

        #region Конструктор

        public UserService(IUserRepository userRepository, ISettingRepository settingRepository)
        {
            this.userRepository = userRepository;
            this.settingRepository = settingRepository;
        }
        #endregion

        #region Методы

        #region Получение по Id и проверка существования

        /// <summary>
        /// Получение пользователя по Id. Если пользователь заблокирован - генерится исключение
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private User GetById(int id)
        {
            var user = userRepository.GetById(id);

            if (user != null && user.IsBlocked)
            {
                throw new Exception(String.Format("Пользователь {0} заблокирован. Работа в системе невозможна.", user.DisplayName));
            }

            return user;
        }

        private User GetById(int id, User requestingUser, Permission permission, bool ignoreBlocking)
        {
            var type = requestingUser.GetPermissionDistributionType(permission);

            // если права нет - то сразу возвращаем null
            if (type == PermissionDistributionType.None)
            {
                return null;
            }
            else
            {
                var user = userRepository.GetById(id);

                bool contains = (requestingUser.Teams.SelectMany(x => x.Users).Contains(user));

                if ((type == PermissionDistributionType.Personal && requestingUser == user) || (type == PermissionDistributionType.Teams && contains) || type == PermissionDistributionType.All)
                {
                    if (!ignoreBlocking && user.IsBlocked)
                    {
                        throw new Exception(String.Format("Пользователь {0} заблокирован.", user.DisplayName));
                    }

                    return user;
                }

                return null;
            }
        }

        /// <summary>
        /// Получение пользователя по id с проверкой его существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User CheckUserExistence(int id, string message = "")
        {
            var user = GetById(id);
            ValidationUtils.NotNull(user, String.IsNullOrEmpty(message) ? "Пользователь не найден. Возможно, он был удален." : message);

            return user;
        }

        /// <summary>
        /// Получение пользователя по Id независимо от того, заблокирован он или нет
        /// </summary>
        /// <param name="id"></param>
        public User CheckUserExistenceIgnoreBlocking(int id)
        {
            var user = userRepository.GetById(id);
            ValidationUtils.NotNull(user, "Пользователь не найден. Возможно, он был удален.");                        

            return user;
        }

        public User CheckUserExistence(int id, User requestingUser, string message = "")
        {
            var user = GetById(id, requestingUser, Permission.User_List_Details, false);
            ValidationUtils.NotNull(user, String.IsNullOrEmpty(message) ? "Пользователь не найден. Возможно, он был удален." : message);

            return user;
        }

        public User CheckUserExistence(int id, User requestingUser, Permission permission, string message = "")
        {
            var user = GetById(id, requestingUser, permission, false);
            ValidationUtils.NotNull(user, String.IsNullOrEmpty(message) ? "Пользователь не найден. Возможно, он был удален." : message);

            return user;
        }

        public User CheckUserExistenceIgnoreBlocking(int id, User requestingUser, Permission permission, string message = "")
        {
            var user = GetById(id, requestingUser, permission, true);
            ValidationUtils.NotNull(user, String.IsNullOrEmpty(message) ? "Пользователь не найден. Возможно, он был удален." : message);

            return user;
        }

        #endregion

        public bool IsLoginUnique(string login, int userId)
        {
            return userRepository.IsLoginUnique(login, userId);
        }

        public IEnumerable<User> FilterByUser(IEnumerable<User> list, User user, Permission permission)
        {
            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<User>();

                case PermissionDistributionType.Personal:
                case PermissionDistributionType.Teams:
                    return user.Teams.SelectMany(x => x.Users).Intersect(list).Distinct();

                case PermissionDistributionType.All:
                    return list;

                default:
                    return null;
            }
        }

        public int Save(User user)
        {
            if (!IsLoginUnique(user.Login, user.Id))
            {
                throw new Exception("Пользователь с таким логином уже существует.");
            }

            // куча проверок


            userRepository.Save(user);

            return user.Id;
        }

        public IEnumerable<User> GetFilteredList(object state, User user)
        {
            return GetFilteredList(state, null, user, Permission.User_List_Details);
        }

        public IEnumerable<User> GetFilteredList(object state, ParameterString parameterString, User currentUser)
        {
            return GetFilteredList(state, parameterString, currentUser, Permission.User_List_Details);
        }

        public IEnumerable<User> GetFilteredList(object state, ParameterString parameterString, User currentUser, Permission permission)
        {
            if (parameterString == null) parameterString = new ParameterString("");

            var type = currentUser.GetPermissionDistributionType(permission);

            switch (type)
            {
                case PermissionDistributionType.None:
                    return new List<User>();

                case PermissionDistributionType.Teams:
                    var list = GetList(currentUser, permission).Select(x => x.Id.ToString()).ToList();

                    // если список пуст - то добавляем несуществующее значение

                    if (parameterString.Keys.Contains("Id"))
                    {
                        if (parameterString["Id"].Operation == ParameterStringItem.OperationType.NotOneOf)
                        {
                            foreach (var excludeId in parameterString["Id"].Value as List<string>)
                            {
                                list.Remove(excludeId);
                            }
                            parameterString.Delete("Id");
                        }
                    }

                    if (!list.Any()) { list.Add("0"); }

                    parameterString.Add("Id", ParameterStringItem.OperationType.OneOf);
                    parameterString["Id"].Value = list;

                    break;

                case PermissionDistributionType.Personal:
                    parameterString.Add("Id", ParameterStringItem.OperationType.Eq, currentUser.Id.ToString());
                    break;

                case PermissionDistributionType.All:
                    break;
            }

            Func<ISubCriteria<User>, ISubCriteria<User>> cond = null;
            if (parameterString.Keys.Contains("StorageIds") || parameterString.Keys.Contains("DealId"))
            {
                IEnumerable<string> storageIds = null;
                string dealId = null;

                var waybillTypeId = (parameterString["WaybillTypeId"].Value as IEnumerable<string>).ElementAt(0);
                parameterString.Delete("WaybillTypeId");

                if (parameterString.Keys.Contains("StorageIds"))
                {
                    storageIds = parameterString["StorageIds"].Value as IEnumerable<string>;    
                    parameterString.Delete("StorageIds");
                }
                if (parameterString.Keys.Contains("DealId"))
                {
                    dealId = (parameterString["DealId"].Value as IEnumerable<string>).ElementAt(0);
                    parameterString.Delete("DealId");
                }

                cond = GetCondForUserByWaybillTypeStorageAndDeal(waybillTypeId, storageIds, dealId, currentUser);  // формируем лямбду

            }

            return userRepository.GetFilteredList(state, parameterString, cond: cond);

        }

        #region Получение лябды для выборки кураторов

        /// <summary>
        /// Получение права на просмотр накладных указанного типа
        /// </summary>
        /// <param name="waybillTypeId">Код типа накладных</param>
        /// <returns>Право</returns>
        private Permission GetPermissionOnViewListDetailsByWaybillType(string waybillTypeId)
        {
            var waybillType = ValidationUtils.TryGetEnum<WaybillType>(waybillTypeId);
            Permission permission;

            switch (waybillType)
            {
                case WaybillType.ReceiptWaybill:
                    permission = Permission.ReceiptWaybill_List_Details;
                    break;
                case WaybillType.MovementWaybill:
                    permission = Permission.MovementWaybill_List_Details;
                    break;
                case WaybillType.ChangeOwnerWaybill:
                    permission = Permission.ChangeOwnerWaybill_List_Details;
                    break;
                case WaybillType.ExpenditureWaybill:
                    permission = Permission.ExpenditureWaybill_List_Details;
                    break;
                case WaybillType.ReturnFromClientWaybill:
                    permission = Permission.ReturnFromClientWaybill_List_Details;
                    break;
                case WaybillType.WriteoffWaybill:
                    permission = Permission.WriteoffWaybill_List_Details;
                    break;
                default:
                    throw new Exception("Неизвестный тип накладной.");
            }

            return permission;
        }

        /// <summary>
        /// Получение лямбды для выборки пользователей, которые видят накладную
        /// </summary>
        /// <param name="waybillTypeId">Код типа накладной</param>
        /// <param name="storageIds">Коды МХ</param>
        /// <param name="user">Пользователь</param>
        /// <returns>Лямбда выражение на выборку кодов пользователей, которые могут видеть накладную</returns>
        private Func<ISubCriteria<User>, ISubCriteria<User>> GetCondForUserByWaybillTypeStorageAndDeal(string waybillTypeId, 
			IEnumerable<string> storageIds, string dealId, User user)
        {
            Permission permission = GetPermissionOnViewListDetailsByWaybillType(waybillTypeId);

            ISubQuery personalSubQueryByStorage = null, teamSubQueryByStorage = null, allSubQueryByStorage = null;
            ISubQuery personalSubQueryByDeal = null, teamSubQueryByDeal = null, allSubQueryByDeal = null;

            if (storageIds != null)
            {
                personalSubQueryByStorage = userRepository.GetUserSubQueryByPersonalWaybillListPermissionAndStorage(permission, storageIds, user.Id);
                teamSubQueryByStorage = userRepository.GetUserSubQueryByTeamWaybillListPermissionAndStorage(permission, storageIds);
                allSubQueryByStorage = userRepository.GetUserSubQueryByAllWaybillListPermissionAndStorage(permission);
            }

            if (dealId != null)
            {
                var dealIdValue = ValidationUtils.TryGetInt(dealId);

                personalSubQueryByDeal = userRepository.GetUserSubQueryByPersonalWaybillListPermissionAndDeal(permission, dealIdValue, user.Id);
                teamSubQueryByDeal = userRepository.GetUserSubQueryByTeamWaybillListPermissionAndDeal(permission, dealIdValue);
                allSubQueryByDeal = userRepository.GetUserSubQueryByAllWaybillListPermissionAndDeal(permission);
            }

            Func<ISubCriteria<User>, ISubCriteria<User>> cond = null;

            if (storageIds != null && dealId == null)
            {
                cond = y => y.Or(
                    z => z.Or(
                        t => t.PropertyIn(x => x.Id, personalSubQueryByStorage),
                        t => t.PropertyIn(x => x.Id, teamSubQueryByStorage)),
                    z => z.PropertyIn(x => x.Id, allSubQueryByStorage))
                    .Select(x => x.Id);
            }
            else if (storageIds == null && dealId != null)
            {
                cond = y => y.Or(
                    z => z.Or(
                        t => t.PropertyIn(x => x.Id, personalSubQueryByDeal),
                        t => t.PropertyIn(x => x.Id, teamSubQueryByDeal)),
                    z => z.PropertyIn(x => x.Id, allSubQueryByDeal))
                    .Select(x => x.Id);
            }
            else if (storageIds != null && dealId != null)
            {
                cond = i => i.And(y=>
                    y.Or(
                        z => z.Or(
                            t => t.PropertyIn(x => x.Id, personalSubQueryByStorage),
                            t => t.PropertyIn(x => x.Id, teamSubQueryByStorage)),
                        z => z.PropertyIn(x => x.Id, allSubQueryByStorage)),
                    y => y.Or(
                        z => z.Or(
                            t => t.PropertyIn(x => x.Id, personalSubQueryByDeal),
                            t => t.PropertyIn(x => x.Id, teamSubQueryByDeal)),
                        z => z.PropertyIn(x => x.Id, allSubQueryByDeal)))
                    .Select(x => x.Id);
            }

            return cond;
        }

        #endregion

        public IEnumerable<User> GetList(User currentUser)
        {
            return GetList(currentUser, Permission.User_List_Details);
        }

        public IDictionary<int, User> GetList(IList<int> listId)
        {
            return userRepository.GetList(listId);
        }

        public IEnumerable<User> GetList(User currentUser, Permission permission)
        {
            switch (currentUser.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<User>();

                case PermissionDistributionType.Personal:
                    return new List<User>() { currentUser };

                case PermissionDistributionType.Teams:
                    return currentUser.Teams.SelectMany(x => x.Users).Distinct();

                case PermissionDistributionType.All:
                    return userRepository.GetAll();

                default:
                    return null;
            }
        }

        /// <summary>
        /// Получение списка пользователей, входящих в команду
        /// </summary>
        /// <param name="teamId">Код команды</param>
        /// <param name="includeBlockedUsers">Включать ли в выборку заблокированных пользователей</param>
        public IEnumerable<User> GetListByTeam(short teamId, bool includeBlockedUsers)
        {
            return userRepository.GetListByTeam(teamId, includeBlockedUsers);
        }

        #region Блокировка / разблокировка

        /// <summary>
        /// Блокировка пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <param name="blocker"></param>
        public void BlockUser(User user, User blocker)
        {
            CheckPossibilityToEdit(user, blocker);

            user.Block(blocker);

            userRepository.Save(user);
        }

        /// <summary>
        /// Разблокировка пользователя
        /// </summary>
        /// <param name="user"></param>
        public void UnBlockUser(User user, User currentUser)
        {
            CheckPossibilityToEdit(user, currentUser);

            // для SaaS-версии проверяем максимальное кол-во активных пользователей, ко
            if (AppSettings.IsSaaSVersion)
            {
                var activeUserCountLimit = settingRepository.Get().ActiveUserCountLimit;
                ValidationUtils.Assert(userRepository.GetAll().Count(x => !x.IsBlocked) < activeUserCountLimit, 
                    String.Format("Невозможно разблокировать пользователя, т.к. количество активных пользователей для данного аккаунта ограничено {0}.", activeUserCountLimit));
            }
            
            user.UnBlock();

            userRepository.Save(user);
        }
        #endregion

        #region Добавление / удаление роли

        public void AddRole(User user, Role role, User currentUser)
        {
            CheckPossibilityToAddRole(user, currentUser);

            user.AddRole(role);

            userRepository.Save(user);
        }

        public void RemoveRole(User user, Role role, User currentUser)
        {
            ValidationUtils.Assert(!(role.IsSystemAdmin && role.Users.Count() == 1), String.Format("Невозможно лишить роли «{0}» последнего пользователя с этой ролью.", role.Name));

            CheckPossibilityToRemoveRole(user, currentUser);

            user.RemoveRole(role);

            userRepository.Save(user);
        }

        #endregion

        #region Добавление / удаление команды

        public void AddTeam(User user, Team team, User currentUser)
        {
            ValidationUtils.Assert(user != currentUser, "Невозможно добавить себя в команду.");

            user.AddTeam(team);

            userRepository.Save(user);
        }

        public void RemoveTeam(User user, Team team, User currentUser)
        {
            ValidationUtils.Assert(user != currentUser, "Невозможно удалить себя из команды.");

            user.RemoveTeam(team);

            userRepository.Save(user);
        }

        #endregion

        #region Аутентификация

        public User TryLogin(string login, string password)
        {
            // вычисление хэша пароля            
            string passwordHash = CryptographyUtils.ComputeHash(password);

            // поиск пользователя по логину
            var user = userRepository.Query<User>().Where(x => x.Login == login).FirstOrDefault<User>();
            ValidationUtils.NotNull(user, "Неверный логин или пароль.");

            // если пароль не совпадает, то выдаем такое же сообщение (чтобы не выдать наличие или отсутствие пользователя по логину)
            ValidationUtils.Assert(user.PasswordHash == passwordHash, "Неверный логин или пароль.");

            // если пользователь аутентифицирован, но заблокирован
            ValidationUtils.Assert(user.BlockingDate == null, String.Format("Пользователь {0} заблокирован и не имеет доступа к системе.", user.DisplayName));

            return user;
        }

        public User TryLoginByHash(string login, string passwordHash)
        {
            // поиск пользователя по логину
            var user = userRepository.Query<User>().Where(x => x.Login == login).FirstOrDefault<User>();
            ValidationUtils.NotNull(user, "Неверный логин или пароль.");

            // если пароль не совпадает, то выдаем такое же сообщение (чтобы не выдать наличие или отсутствие пользователя по логину)
            ValidationUtils.Assert(user.PasswordHash == passwordHash, "Неверный логин или пароль.");

            // если пользователь аутентифицирован, но заблокирован
            ValidationUtils.Assert(user.BlockingDate == null, "Пользователь заблокирован и не имеет доступа к системе.");

            return user;
        }

        #endregion

        #region Смена пароля

        public void ChangePassword(User user, string currentPassword, string newPassword, string newPasswordConfirmation)
        {
            ValidationUtils.Assert(newPassword == newPasswordConfirmation, "Пароли не совпадают.");

            CheckPossibilityToChangePassword(user);

            user.ChangePassword(currentPassword, newPassword);

            userRepository.Save(user);
        }

        #endregion

        #region Сброс пароля

        public void ResetPassword(User user, string newPassword, string newPasswordConfirmation, User currentUser)
        {
            ValidationUtils.Assert(newPassword == newPasswordConfirmation, "Пароли не совпадают.");

            CheckPossibilityToResetPassword(user, currentUser);

            user.ResetPassword(newPassword);

            userRepository.Save(user);
        }

        #endregion

        #region Права на совершение операций

        #region Вспомогательные методы

        private bool IsPermissionToPerformOperation(User user, User currentUser, Permission permission)
        {
            bool result = false;

            switch (currentUser.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.Teams:
                    foreach (var item in currentUser.Teams)
                    {
                        if (user.Teams.Contains(item))
                        {
                            result = true;
                            break;
                        }
                    }
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperation(User user, User currentUser, Permission permission)
        {
            if (!IsPermissionToPerformOperation(user, currentUser, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        #endregion

        #region Просмотр деталей

        public bool IsPossibilityToViewDetails(User _user, User user)
        {
            try
            {
                CheckPossibilityToViewDetails(_user, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewDetails(User _user, User user)
        {
            if (user != _user)
            {
                CheckPermissionToPerformOperation(_user, user, Permission.User_List_Details);
            }
        }
        #endregion

        #region Создание

        public bool IsPossibilityToCreate(User currentUser)
        {
            try
            {
                CheckPossibilityToCreate(currentUser);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreate(User currentUser)
        {
            currentUser.CheckPermission(Permission.User_Create);

            // для SaaS-версии проверяем максимальное кол-во активных пользователей
            if (AppSettings.IsSaaSVersion)
            {
                var activeUserCountLimit = settingRepository.Get().ActiveUserCountLimit;
                ValidationUtils.Assert(userRepository.GetAll().Count(x => !x.IsBlocked) < activeUserCountLimit, 
                    String.Format("Невозможно создать нового пользователя, т.к. количество активных пользователей для данного аккаунта ограничено {0}.", activeUserCountLimit));
            }
        }

        #endregion

        #region Редактирование пользователя

        public bool IsPossibilityToEdit(User user, User currentUser)
        {
            try
            {
                CheckPossibilityToEdit(user, currentUser);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEdit(User user, User currentUser)
        {
            // права
            CheckPermissionToPerformOperation(user, currentUser, Permission.User_Edit);
        }

        #endregion

        #region Удаление пользователя

        public bool IsPossibilityToDelete(User user, User currentUser)
        {
            try
            {
                CheckPossibilityToDelete(user, currentUser);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDelete(User user, User currentUser)
        {
            // права
            CheckPermissionToPerformOperation(user, currentUser, Permission.User_Delete);

            // сущность
            //user.CheckPossibilityToDelete();
        }

        #endregion

        #region Добавление роли

        public bool IsPossibilityToAddRole(User user, User currentUser)
        {
            try
            {
                CheckPossibilityToAddRole(user, currentUser);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToAddRole(User user, User currentUser)
        {
            // права
            CheckPermissionToPerformOperation(user, currentUser, Permission.User_Role_Add);

            ValidationUtils.Assert(user != currentUser, "Невозможно назначить роль себе.");

            // сущность
            //user.CheckPossibilityToAddRole();
        }

        #endregion

        #region Удаление роли

        public bool IsPossibilityToRemoveRole(User user, User currentUser)
        {
            try
            {
                CheckPossibilityToRemoveRole(user, currentUser);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToRemoveRole(User user, User currentUser)
        {
            // права
            CheckPermissionToPerformOperation(user, currentUser, Permission.User_Role_Remove);

            ValidationUtils.Assert(user != currentUser, "Невозможно лишить роли самого себя.");

            // сущность
            //user.CheckPossibilityToAddRole();
        }

        #endregion

        #region Смена пароля

        public bool IsPossibilityToChangePassword(User user, User currentUser)
        {
            try
            {
                ValidationUtils.Assert(user == currentUser);
                CheckPossibilityToChangePassword(user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToChangePassword(User user)
        {
            ValidationUtils.NotNull(user);
        }

        #endregion

        #region Сброс пароля

        public bool IsPossibilityToResetPassword(User user, User currentUser)
        {
            try
            {
                CheckPossibilityToResetPassword(user, currentUser);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToResetPassword(User user, User currentUser)
        {
            ValidationUtils.Assert(currentUser.IsSystemAdmin, "Сбросить пароль может только системный администратор.");
            ValidationUtils.Assert(user.Id != currentUser.Id, "Невозможно сбросить собственный пароль.");
        }

        #endregion

        #endregion

        /// <summary>
        /// Проверка наличия у пользователя определенного права
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="permission">Право</param>
        public bool HasPermission(User user, Permission permission)
        {
            return userRepository.Query<PermissionDistribution>()
                .Where(x => x.Permission == permission && x.Type != PermissionDistributionType.None)
                .OneOf(x => x.Role, user.Roles)
                .FirstOrDefault<PermissionDistribution>() != null;
        }

        /// <summary>
        /// Проверка наличия у пользователя определенного права с генерацией исключения при отсутствии права
        /// </summary>
        public void CheckPermission(User user, Permission permission)
        {
            ValidationUtils.Assert(HasPermission(user, permission), String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
        }

        #endregion
    }
}
