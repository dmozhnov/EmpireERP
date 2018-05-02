using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class LogItemRepository : BaseRepository, ILogItemRepository
    {
        public LogItemRepository()
        {
        }

        public LogItem GetById(long id)
        {
            return CurrentSession.Get<LogItem>(id);
        }

        public void Save(LogItem entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(LogItem entity)
        {
            CurrentSession.Delete(entity);
        }
    }
}
