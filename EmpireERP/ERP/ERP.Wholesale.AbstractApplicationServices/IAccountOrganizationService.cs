using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IAccountOrganizationService
    {
        AccountOrganization CheckAccountOrganizationExistence(int id, string message = "");

        IEnumerable<AccountOrganization> GetList();

        IList<AccountOrganization> GetFilteredList(object state, ParameterString param = null);

        int Save(AccountOrganization organization);
        void Delete(AccountOrganization organization, User user);

        void AddStorage(AccountOrganization organization, Storage storage, User user);
        void RemoveStorage(AccountOrganization organization, Storage storage, User user);

        void AddRussianBankAccount(AccountOrganization organization, RussianBankAccount bankAccount);
        void AddForeignBankAccount(AccountOrganization organization, ForeignBankAccount bankAccount);

        void DeleteRussianBankAccount(AccountOrganization organization, RussianBankAccount bankAccount);
        void DeleteForeignBankAccount(AccountOrganization organization, ForeignBankAccount bankAccount);
        Dictionary<int, AccountOrganization> GetList(IEnumerable<int> idList);
        bool IsPossibilityToCreate(User user);
        void CheckPossibilityToCreate(User user);
    }
}
