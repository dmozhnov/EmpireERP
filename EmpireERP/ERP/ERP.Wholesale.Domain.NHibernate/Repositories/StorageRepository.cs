using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Criterion;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class StorageRepository : BaseRepository, IStorageRepository
    {
        public StorageRepository() : base()
        {
        }

        public Storage GetById(short id)
        {
            return CurrentSession.Get<Storage>(id);
        }

        public void Save(Storage value)
        {            
            CurrentSession.SaveOrUpdate(value);            
        }

        public void Delete(Storage value)
        {                            
            CurrentSession.SaveOrUpdate(value);            
        }

        public IEnumerable<Storage> GetAll()
        {
            return CurrentSession.CreateCriteria(typeof(Storage)).List<Storage>();
        }

        public IEnumerable<short> GetAllIds()
        {
            return Query<Storage>().Select(x => x.Id).ToList<short>();
        }

        /// <summary>
        /// Преобразование коллекции идентификаторов МХ в подзапрос, который ищет МХ по этим идентификаторам
        /// </summary>
        /// <param name="storageIdList">Список идентификаторов мест хранения</param>
        public ISubQuery GetListSubQuery(IEnumerable<short> storageIdList)
        {
            return SubQuery<Storage>().OneOf(x => x.Id, storageIdList).Select(x => x.Id);
        }

        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>        
        public IList<Storage> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Storage>(state, ignoreDeletedRows);
        }
        public IList<Storage> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Storage>(state, parameterString, ignoreDeletedRows);
        }

        public IList<Storage> GetStoragesByType(StorageType type)
        {
            return CurrentSession.
                CreateCriteria(typeof(Storage)).
                Add(Expression.Eq("Type", type)).
                List<Storage>();
        }

        public bool IsNameUnique(string name, short storageId)
        {
            return Query<Storage>().Where(x => x.Name == name && x.Id != storageId).CountDistinct() == 0;
        }

        public bool IsSectionNameUnique(string sectionName, short sectionId, short storageId)
        {
            return Query<StorageSection>().Where(x => x.Storage.Id == storageId && x.Name == sectionName && x.Id != sectionId)
                .CountDistinct() == 0;
        }

        /// <summary>
        /// Получение подзапроса для МХ
        /// </summary>
        /// <param name="id">Код МХ</param>
        public ISubQuery GetStorageSubQuery(short id)
        {
            return SubQuery<Storage>()
                .Where(x => x.Id == id)
                .Select(x => x.Id);
        }

        public ISubCriteria<Storage> GetStorageSubQueryByAllPermission()
        {
            return SubQuery<Storage>().Select(x => x.Id);
        }

        public ISubCriteria<Storage> GetStorageSubQueryByTeamPermission(int userId)
        {
            var sq = SubQuery<Team>();
            sq.Restriction<User>(x => x.Users).Where(x => x.Id == userId);
            sq.Restriction<Storage>(x => x.Storages).Select(x => x.Id);

            return SubQuery<Storage>().PropertyIn(x => x.Id, sq).Select(x => x.Id);
        }
    }
}
