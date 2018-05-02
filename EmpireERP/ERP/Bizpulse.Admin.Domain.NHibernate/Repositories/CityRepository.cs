using System.Collections.Generic;
using System.Linq;
using Bizpulse.Admin.Domain.Entities;
using Bizpulse.Admin.Domain.Repositories;
using NHibernate.Linq;

namespace Bizpulse.Admin.Domain.NHibernate.Repositories
{
    public class CityRepository : BaseAdminRepository<City, short>, ICityRepository
    {
        /// <summary>
        /// Получение списка городов по коду региона
        /// </summary>
        /// <param name="regionId">Код региона</param>
        public IEnumerable<City> GetByRegionId(short regionId)
        {
            return CurrentSession.Query<City>()
                .Where(x => x.Region.Id == regionId)
                .ToList();
        }
    }
}
