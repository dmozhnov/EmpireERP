using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IRoleService
    {
        //Role GetById(short id);
        Role CheckRoleExistence(short id, User user, string message = "");
        void Delete(Role role, User user);
        short Save(Role role);
        IEnumerable<Role> GetFilteredList(object state);
        IEnumerable<Role> GetFilteredList(object state, ParameterString parameterString, User user);
        IEnumerable<Role> GetFilteredList(object state, ParameterString parameterString, User user, Permission permission);

        void AddUser(Role role, User user, User currentUser);
        void RemoveUser(Role role, User user, User currentUser);

        bool IsPossibilityToDelete(Role role, User user);
        void CheckPossibilityToDelete(Role role, User user);
        bool IsPossibilityToEdit(Role role, User user);
        void CheckPossibilityToEdit(Role role, User user);
        bool IsPossibilityToEditUserPermissions(Role role, User user);
        void CheckPossibilityToEditUserPermissions(Role role, User user);
        bool IsPossibilityToViewDetails(Role role, User user);
        void CheckPossibilityToViewDetails(Role role, User user);
    }
}
