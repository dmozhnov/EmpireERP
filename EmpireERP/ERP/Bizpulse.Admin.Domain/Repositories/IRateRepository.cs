using Bizpulse.Admin.Domain.Entities;
using ERP.Infrastructure.Repositories;
using System.Collections.Generic;

namespace Bizpulse.Admin.Domain.Repositories
{
    public interface IRateRepository : IRepository<Rate, short>
    {
        /// <summary>
        /// Получение списка всех тарифов на услуги Bizpulse
        /// </summary>
        IEnumerable<Rate> GetList();
    }
}
