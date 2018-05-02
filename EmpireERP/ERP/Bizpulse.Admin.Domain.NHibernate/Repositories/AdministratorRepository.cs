using System.Linq;
using Bizpulse.Admin.Domain.Entities;
using Bizpulse.Admin.Domain.Repositories;
using NHibernate.Linq;

namespace Bizpulse.Admin.Domain.NHibernate.Repositories
{
    public class AdministratorRepository : BaseAdminRepository<Administrator, short>, IAdministratorRepository
    {
        /// <summary>
        /// Получение администратора по логину
        /// </summary>
        /// <param name="login">Логин</param>
        public Administrator GetByLogin(string login)
        {
            return CurrentSession.Query<Administrator>()
                .Where(x => x.Login == login)
                .FirstOrDefault();
        }
    }
}
