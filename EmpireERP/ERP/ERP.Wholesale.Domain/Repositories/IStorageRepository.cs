using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IStorageRepository : IRepository<Storage, short>, IFilteredRepository<Storage>, IGetAllRepository<Storage>
    {
        IList<Storage> GetStoragesByType(StorageType type);
        bool IsNameUnique(string name, short storageId);
        bool IsSectionNameUnique(string sectionName, short sectionId, short storageId);
        
        IEnumerable<short> GetAllIds();

        /// <summary>
        /// Преобразование коллекции идентификаторов МХ в подзапрос, который ищет МХ по этим идентификаторам
        /// </summary>
        /// <param name="storageIdList">Список идентификаторов мест хранения</param>
        ISubQuery GetListSubQuery(IEnumerable<short> storageIdList);

        /// <summary>
        /// Получение подзапроса для МХ
        /// </summary>
        /// <param name="id">Код МХ</param>
        ISubQuery GetStorageSubQuery(short id);

        ISubCriteria<Storage> GetStorageSubQueryByAllPermission();        
        ISubCriteria<Storage> GetStorageSubQueryByTeamPermission(int userId);
    }
}
