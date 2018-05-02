using System;
using System.Linq;
using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class TaskExecutionItemRepository : BaseNHRepository, ITaskExecutionItemRepository
    {
        public TaskExecutionItem GetById(int id)
        {
            return CurrentSession.Get<TaskExecutionItem>(id);
        }

        public void Save(TaskExecutionItem entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(TaskExecutionItem entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }
    }
}
