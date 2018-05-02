using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Settings;

namespace ERP.Wholesale.ApplicationServices
{
    public class TeamService : ITeamService
    {
        #region Поля

        private readonly ITeamRepository teamRepository;
        private readonly ISettingRepository settingRepository;

        #endregion

        #region Конструкторы

        public TeamService(ITeamRepository teamRepository, ISettingRepository settingRepository)
        {
            this.teamRepository = teamRepository;
            this.settingRepository = settingRepository;
        }

        #endregion

        #region Методы

        private Team GetById(short id, User user)
        {
            var type = user.GetPermissionDistributionType(Permission.Team_List_Details);

            // если права нет - то сразу возвращаем null
            if (type == PermissionDistributionType.None)
            {
                return null;
            }
            else
            {
                var team = teamRepository.GetById(id);

                if (type == PermissionDistributionType.All)
                {
                    return team;
                }

                bool contains = (user.Teams.Contains(team));

                if (type == PermissionDistributionType.Teams && contains)
                {
                    return team;
                }

                return null;
            }
        }

        /// <summary>
        /// Получение команды по id с проверкой ее существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Team CheckTeamExistence(short id, User user, string message = "")
        {
            var team = GetById(id, user);
            ValidationUtils.NotNull(team, String.IsNullOrEmpty(message) ? "Команда не найдена. Возможно, она была удалена." : message);

            return team;
        }

        /// <summary>
        /// Получение команды по id с проверкой ее существования (без проверки прав)
        /// </summary>
        /// <param name="id">Код команды</param>
        /// <returns></returns>
        public Team CheckTeamExistence(short id, string message = "")
        {
            var team = teamRepository.GetById(id);
            ValidationUtils.NotNull(team, String.IsNullOrEmpty(message) ? "Команда не найдена. Возможно, она была удалена." : message);

            return team;
        }

        public IEnumerable<Team> GetList(User user, Permission permission)
        {
            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<Team>();

                case PermissionDistributionType.Personal:
                case PermissionDistributionType.Teams:
                    return user.Teams;

                case PermissionDistributionType.All:
                    return teamRepository.GetAll();

                default: return null;
            }
        }

        public IEnumerable<Team> GetFilteredList(object state)
        {
            return teamRepository.GetFilteredList(state);
        }

        public IEnumerable<Team> GetFilteredList(object state, ParameterString parameterString, User user)
        {
            return GetFilteredList(state, parameterString, user, Permission.Team_List_Details);
        }

        public IEnumerable<Team> GetFilteredList(object state, ParameterString parameterString, User user, Permission permission)
        {
            if (parameterString == null)
            {
                parameterString = new ParameterString("");
            }

            ParameterString param = new ParameterString("");

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<Team>();

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

                    param.Add("Id", ParameterStringItem.OperationType.OneOf, list);
                    break;

                case PermissionDistributionType.All:
                    break;
            }

            return teamRepository.GetFilteredList(state, param);
        }

        public short Save(Team team)
        {
            var repeatTeam = teamRepository.Query<Team>().Where(x => x.Name == team.Name && x.Id != team.Id).FirstOrDefault<Team>();
            if (repeatTeam != null)
            {
                throw new Exception("Команда с таким названием уже существует.");
            }

            teamRepository.Save(team);

            return team.Id;
        }

        #region Добавление / удаление пользователя

        public void AddUser(Team team, User user, User currentUser)
        {
            ValidationUtils.Assert(user != currentUser, "Невозможно добавить себя в команду.");
            CheckPossibilityToAddUser(team, currentUser);

            team.AddUser(user);

            teamRepository.Save(team);

        }

        public void RemoveUser(Team team, User user, User currentUser)
        {
            ValidationUtils.Assert(user != currentUser, "Невозможно удалить себя из команды.");

            CheckPossibilityToRemoveUser(team, user, currentUser);

            team.RemoveUser(user);

            teamRepository.Save(team);
        }

        #endregion

        #region Добавление / удаление сделки

        public void AddDeal(Team team, Deal deal, User user)
        {
            CheckPossibilityToAddDeal(team, user);

            team.AddDeal(deal);

            teamRepository.Save(team);
        }

        public void RemoveDeal(Team team, Deal deal, User user)
        {
            CheckPossibilityToRemoveDeal(team, user);

            team.RemoveDeal(deal);

            teamRepository.Save(team);
        }

        #endregion

        #region Добавление / удаление места хранения

        public void AddStorage(Team team, Storage storage, User user)
        {
            CheckPossibilityToAddStorage(team, user);

            team.AddStorage(storage);

            teamRepository.Save(team);
        }

        public void RemoveStorage(Team team, Storage storage, User user)
        {
            CheckPossibilityToRemoveStorage(team, user);

            team.RemoveStorage(storage);

            teamRepository.Save(team);
        }

        #endregion

        #region Добавление / удаление заказа на производство

        public void AddProductionOrder(Team team, ProductionOrder order, User user)
        {
            CheckPossibilityToAddProductionOrder(team, user);

            team.AddProductionOrder(order);

            teamRepository.Save(team);
        }

        public void RemoveProductionOrder(Team team, ProductionOrder order, User user)
        {
            CheckPossibilityToRemoveProductionOrder(team, user);

            team.RemoveProductionOrder(order);

            teamRepository.Save(team);
        }

        #endregion

        #region Удаление

        public void Delete(Team team, User user)
        {
            CheckPossibilityToDelete(team, user);

            team.RemoveAllUsers();
            team.RemoveAllStorages();
            team.RemoveAllDeals();
            team.RemoveAllProductionOrders();
            
            team.DeletionDate = DateTime.Now;

            teamRepository.Save(team);
        }

        #endregion

        #region Права на совершение операций

        #region Дополнительные методы

        private bool IsPermissionToPerformOperation(Team team, User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.Teams:
                    if (team.Users.Contains(user))
                    {
                        result = true;
                        break;
                    }

                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperation(Team team, User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(team, user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        #endregion

        #region Создание

        public bool IsPossibilityToCreate(User user)
        {
            try
            {
                CheckPossibilityToCreate(user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreate(User user)
        {
            user.CheckPermission(Permission.Team_Create);

            // для SaaS-версии проверяем максимальное кол-во команд
            if (AppSettings.IsSaaSVersion)
            {
                var teamCountLimit = settingRepository.Get().TeamCountLimit;
                ValidationUtils.Assert(teamRepository.GetAll().Count() < teamCountLimit, 
                    String.Format("Невозможно создать команду, т.к. их количество для данного аккаунта ограничено {0} шт.", teamCountLimit));
            }
        }

        #endregion

        #region Редактирование

        public bool IsPossibilityToEdit(Team team, User user)
        {
            try
            {
                CheckPossibilityToEdit(team, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToEdit(Team team, User user)
        {
            // права
            CheckPermissionToPerformOperation(team, user, Permission.Team_Edit);
        }

        #endregion

        #region Удаление

        public bool IsPossibilityToDelete(Team team, User user)
        {
            try
            {
                CheckPossibilityToDelete(team, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToDelete(Team team, User user)
        {
            // права
            CheckPermissionToPerformOperation(team, user, Permission.Team_Delete);
        }

        #endregion

        #region Добавление пользователя в команду

        public bool IsPossibilityToAddUser(Team team, User user)
        {
            try
            {
                CheckPossibilityToAddUser(team, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToAddUser(Team team, User user)
        {
            // права
            CheckPermissionToPerformOperation(team, user, Permission.Team_User_Add);

            // сущность
            //team.CheckPossibilityToAddUser();
        }

        #endregion

        #region Исключение пользователя из команды

        public bool IsPossibilityToRemoveUser(Team team, User user, User currentUser)
        {
            try
            {
                CheckPossibilityToRemoveUser(team, user, currentUser);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToRemoveUser(Team team, User user, User currentUser)
        {
            // права
            CheckPermissionToPerformOperation(team, currentUser, Permission.Team_User_Remove);

            ValidationUtils.Assert(user != currentUser, "Невозможно удалить себя из команды.");

            // сущность
            //team.CheckPossibilityToRemoveUser();
        }

        #endregion

        #region Добавление места хранения в команду

        public bool IsPossibilityToAddStorage(Team team, User currentUser)
        {
            try
            {
                CheckPossibilityToAddStorage(team, currentUser);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToAddStorage(Team team, User currentUser)
        {
            // права
            CheckPermissionToPerformOperation(team, currentUser, Permission.Team_Storage_Add);
        }

        #endregion

        #region Исключение места хранения из команды

        public bool IsPossibilityToRemoveStorage(Team team, User currentUser)
        {
            try
            {
                CheckPossibilityToRemoveStorage(team, currentUser);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToRemoveStorage(Team team, User currentUser)
        {
            // права
            CheckPermissionToPerformOperation(team, currentUser, Permission.Team_Storage_Remove);
        }

        #endregion

        #region Добавление сделки в команду

        public bool IsPossibilityToAddDeal(Team team, User currentUser)
        {
            try
            {
                CheckPossibilityToAddDeal(team, currentUser);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToAddDeal(Team team, User currentUser)
        {
            // права
            CheckPermissionToPerformOperation(team, currentUser, Permission.Team_Deal_Add);
        }

        #endregion

        #region Исключение сделки из команды

        public bool IsPossibilityToRemoveDeal(Team team, User currentUser)
        {
            try
            {
                CheckPossibilityToRemoveDeal(team, currentUser);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToRemoveDeal(Team team, User currentUser)
        {
            // права
            CheckPermissionToPerformOperation(team, currentUser, Permission.Team_Deal_Remove);
        }

        #endregion

        #region Добавление заказа на производство в команду

        public bool IsPossibilityToAddProductionOrder(Team team, User currentUser)
        {
            try
            {
                CheckPossibilityToAddProductionOrder(team, currentUser);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToAddProductionOrder(Team team, User currentUser)
        {
            // права
            CheckPermissionToPerformOperation(team, currentUser, Permission.Team_ProductionOrder_Add);
        }

        #endregion

        #region Исключение заказа на производство из команды

        public bool IsPossibilityToRemoveProductionOrder(Team team, User currentUser)
        {
            try
            {
                CheckPossibilityToRemoveProductionOrder(team, currentUser);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToRemoveProductionOrder(Team team, User currentUser)
        {
            // права
            CheckPermissionToPerformOperation(team, currentUser, Permission.Team_ProductionOrder_Remove);
        }

        #endregion

        #region Просмотр деталей

        public bool IsPossibilityToViewDetails(Team team, User user)
        {
            try
            {
                CheckPossibilityToViewDetails(team, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToViewDetails(Team team, User user)
        {
            // права
            CheckPermissionToPerformOperation(team, user, Permission.Team_List_Details);
        }

        #endregion


        #endregion

        #endregion
    }
}
