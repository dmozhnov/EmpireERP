using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface ITaskExecutionItemRepository:  IRepository<TaskExecutionItem, int>
    {
    }
}
