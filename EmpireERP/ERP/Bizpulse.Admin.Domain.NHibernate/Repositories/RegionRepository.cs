using System.Collections.Generic;
using System.Linq;
using Bizpulse.Admin.Domain.Entities;
using Bizpulse.Admin.Domain.Repositories;
using NHibernate.Linq;

namespace Bizpulse.Admin.Domain.NHibernate.Repositories
{
    public class RegionRepository : BaseAdminRepository<Region, short>, IRegionRepository
    {
        /// <summary>
        /// Получение списка всех регионов
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Region> GetList()
        {
            return CurrentSession.Query<Region>().ToList();
        }
    }
}
