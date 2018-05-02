using ERP.Infrastructure.Entities;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Infrastructure.NHibernate.SessionManager;
using NHibernate;

namespace Bizpulse.Admin.Domain.NHibernate.Repositories
{
    public abstract class BaseAdminRepository<TEntity, TEntityId> : BaseRepository where TEntity : Entity<TEntityId>
    {
        /// <summary>
        /// Текущая сессия NHibernate
        /// </summary>
        protected new ISession CurrentSession
        {
            get { return NHibernateSingleDBSessionManager.CurrentSession; }
        }

        public TEntity GetById(TEntityId id)
        {
            return CurrentSession.Get<TEntity>(id);
        }

        public void Save(TEntity entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(TEntity entity)
        {
            CurrentSession.Delete(entity);
        }
    }
}
