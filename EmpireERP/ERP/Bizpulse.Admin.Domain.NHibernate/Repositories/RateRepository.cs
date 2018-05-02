using System.Collections.Generic;
using System.Linq;
using Bizpulse.Admin.Domain.Entities;
using Bizpulse.Admin.Domain.Repositories;
using NHibernate.Linq;

namespace Bizpulse.Admin.Domain.NHibernate.Repositories
{
    public class RateRepository : BaseAdminRepository<Rate, short>, IRateRepository
    {
        /// <summary>
        /// Получение списка всех тарифов на услуги Bizpulse
        /// </summary>
        public IEnumerable<Rate> GetList()
        {
            return CurrentSession.Query<Rate>().ToList();
        }
    }
}
