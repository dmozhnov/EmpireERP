using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using System;

namespace ERP.Wholesale.Domain.NHibernate.Repositories.Security
{
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        public RoleRepository()
            : base()
        {
        }

        public Role GetById(short id)
        {
            return CurrentSession.Get<Role>(id);
        }

        public void Save(Role value)
        {
            CurrentSession.SaveOrUpdate(value);
        }

        public void Delete(Role value)
        {
            value.DeletionDate = DateTime.Now;
            CurrentSession.SaveOrUpdate(value);
        }

        public IEnumerable<Role> GetAll()
        {
            return CurrentSession.CreateCriteria(typeof(Role)).List<Role>();
        }

        public IList<Role> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Role>(state, ignoreDeletedRows);
        }
        public IList<Role> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Role>(state, parameterString, ignoreDeletedRows);
        }
    }
}
