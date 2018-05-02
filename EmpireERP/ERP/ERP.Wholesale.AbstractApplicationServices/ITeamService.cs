using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface ITeamService
    {
        Team CheckTeamExistence(short id, User user, string message = "");
        Team CheckTeamExistence(short id, string message = "");

        short Save(Team team);
        void Delete(Team team, User user);
        IEnumerable<Team> GetFilteredList(object state);
        IEnumerable<Team> GetFilteredList(object state, ParameterString parameterString, User user);
        IEnumerable<Team> GetFilteredList(object state, ParameterString parameterString, User user, Permission permission);

        IEnumerable<Team> GetList(User user, Permission permission);

        void AddUser(Team team, User user, User currentUser);
        void RemoveUser(Team team, User user, User currentUser);

        void AddDeal(Team team, Deal deal, User user);
        void RemoveDeal(Team team, Deal deal, User user);

        void RemoveStorage(Team team, Storage storage, User user);
        void AddStorage(Team team, Storage storage, User user);

        void AddProductionOrder(Team team, ProductionOrder order, User user);
        void RemoveProductionOrder(Team team, ProductionOrder order, User user);
                
        bool IsPossibilityToAddUser(Team team, User user);
        void CheckPossibilityToAddUser(Team team, User user);

        bool IsPossibilityToRemoveUser(Team team, User user, User currentUser);
        void CheckPossibilityToRemoveUser(Team team, User user, User currentUser);

        bool IsPossibilityToAddStorage(Team team, User currentUser);
        void CheckPossibilityToAddStorage(Team team, User currentUser);

        bool IsPossibilityToRemoveStorage(Team team, User currentUser);
        void CheckPossibilityToRemoveStorage(Team team, User currentUser);

        bool IsPossibilityToAddProductionOrder(Team team, User currentUser);
        void CheckPossibilityToAddProductionOrder(Team team, User currentUser);

        bool IsPossibilityToRemoveProductionOrder(Team team, User currentUser);
        void CheckPossibilityToRemoveProductionOrder(Team team, User currentUser);

        bool IsPossibilityToAddDeal(Team team, User currentUser);
        void CheckPossibilityToAddDeal(Team team, User currentUser);

        bool IsPossibilityToRemoveDeal(Team team, User currentUser);
        void CheckPossibilityToRemoveDeal(Team team, User currentUser);

        bool IsPossibilityToCreate(User user);
        void CheckPossibilityToCreate(User user);

        bool IsPossibilityToEdit(Team team, User user);
        void CheckPossibilityToEdit(Team team, User user);

        bool IsPossibilityToDelete(Team team, User user);
        void CheckPossibilityToDelete(Team team, User user);

        bool IsPossibilityToViewDetails(Team team, User user);
        void CheckPossibilityToViewDetails(Team team, User user);
    }
}
