using System.Collections.Generic;
using Bizpulse.Admin.Domain.Entities;
using ERP.Infrastructure.Repositories;

namespace Bizpulse.Admin.Domain.Repositories
{
    public interface IRegionRepository : IRepository<Region, short>
    {
        /// <summary>
        /// Получение списка всех регионов
        /// </summary>
        IEnumerable<Region> GetList();
    }
}
