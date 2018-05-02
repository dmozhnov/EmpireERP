using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface ITaskPriorityRepository : IRepository<TaskPriority, short>, IGetAllRepository<TaskPriority>
    {
    }
}
