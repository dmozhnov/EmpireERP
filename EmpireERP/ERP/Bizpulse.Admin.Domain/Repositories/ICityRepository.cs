using System.Collections.Generic;
using Bizpulse.Admin.Domain.Entities;
using ERP.Infrastructure.Repositories;

namespace Bizpulse.Admin.Domain.Repositories
{
    public interface ICityRepository : IRepository<City, short>
    {
        /// <summary>
        /// Получение списка городов по коду региона
        /// </summary>
        /// <param name="regionId">Код региона</param>
        IEnumerable<City> GetByRegionId(short regionId);
    }
}
