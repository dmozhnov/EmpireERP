using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IStorageService
    {
        Storage GetById(short id);  // TODO: убрать после реализации прав
        Storage CheckStorageExistence(short id, string message = "");
        Storage CheckStorageExistence(short id, User user, string message = "");
        Storage CheckStorageExistence(short id, User user, Permission permission, string message = "");

        IEnumerable<Storage> GetList(); // TODO: убрать после реализации прав
        IEnumerable<Storage> GetList(User user, Permission permission);

        IEnumerable<Storage> FilterByUser(IEnumerable<Storage> list, User user, Permission permission);        
        IEnumerable<Storage> GetStoragesByType(StorageType storageType, User user, Permission permission);
        IEnumerable<Storage> GetFilteredList(object state, User user, ParameterString param = null);

        bool IsNameUnique(string name, short storageId);
        void Save(Storage storage, User user);
        void Delete(Storage storage, User user);

        void AddSection(Storage storage, StorageSection section, User user);
        void DeleteSection(Storage storage, StorageSection section, User user);
        
        void AddAccountOrganization(Storage storage, AccountOrganization accountOrganization, User user);
        void RemoveAccountOrganization(Storage storage, AccountOrganization accountOrganization, User user);

        bool IsPossibilityToViewDetails(Storage storage, User user);
        bool IsPossibilityToCreate(User user);
        bool IsPossibilityToEdit(Storage storage, User user);
        bool IsPossibilityToDelete(Storage storage, User user, bool checkLogic = true);
        bool IsPossibilityToAddAccountOrganization(Storage storage, User user);
        bool IsPossibilityToRemoveAccountOrganization(Storage storage, AccountOrganization accountOrganization, User user, bool checkLogic = true);
        bool IsPossibilityToCreateAndEditSection(Storage storage, User user);
        bool IsPossibilityToDeleteSection(Storage storage, User user);

        void CheckPossibilityToViewDetails(Storage storage, User user);
        void CheckPossibilityToCreate(User user);
        void CheckPossibilityToEdit(Storage storage, User user);
        void CheckPossibilityToDelete(Storage storage, User user, bool checkLogic = true);
        void CheckPossibilityToAddAccountOrganization(Storage storage, User user);
        void CheckPossibilityToRemoveAccountOrganization(Storage storage, AccountOrganization accountOrganization, User user, bool checkLogic = true);
        void CheckPossibilityToCreateAndEditSection(Storage storage, User user);
        void CheckPossibilityToDeleteSection(Storage storage, User user);
        
        /// <summary>
        /// Получение списка мест хранения по Id
        /// </summary>        
        Dictionary<short, Storage> GetList(IEnumerable<short> idList);

        /// <summary>
        /// Получение списка мест хранения по Id с проверкой существования и прав
        /// </summary>        
        Dictionary<short, Storage> CheckStorageListExistence(IEnumerable<short> idList, User user, Permission permission, string message = "");
    }
}
