using System.Linq;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class SettingRepository : BaseRepository, ISettingRepository
    {
        public SettingRepository()
        {
        }

        public Setting Get()
        {
            return CurrentSession.Get<Setting>((byte)1); // в данной таблице только одна запись
        }
    }
}
