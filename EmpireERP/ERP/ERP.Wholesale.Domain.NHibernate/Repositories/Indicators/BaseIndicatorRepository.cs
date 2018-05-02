using System;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Wholesale.Domain.Indicators;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    /// <summary>
    /// Базовый репозиторий для показателей
    /// </summary>
    public abstract class BaseIndicatorRepository<T> : BaseRepository where T : BaseIndicator 
    {
        protected BaseIndicatorRepository() : base()
        {
        }

        public T GetById(Guid id)
        {
            return CurrentSession.Get<T>(id);
        }

        public void Save(T value)
        {
            CurrentSession.SaveOrUpdate(value);
        }

        public void Delete(T value)
        {
            CurrentSession.Delete(value);
        }
    }
}
