using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;
using NHibernate.Linq;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class TaskRepository : BaseNHRepository, ITaskRepository
    {
        public Task GetById(int id)
        {
            return CurrentSession.Get<Entities.Task>(id);
        }

        public void Save(Task entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(Task entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public IList<Task> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Entities.Task>(state, true);
        }

        public IList<Task> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Entities.Task>(state, parameterString, ignoreDeletedRows);
        }

        /// <summary>
        /// Получение истории изменений по задачи
        /// </summary>
        /// <param name="taskId">Идентификатор задачи</param>
        /// <returns></returns>
        public IEnumerable<BaseTaskHistoryItem> GetChangeHistory(int taskId)
        {
            var list1 = CurrentSession.Query<TaskHistoryItem>()
               .Where(x => x.Task.Id == taskId);

            var list2 = CurrentSession.Query<TaskExecutionHistoryItem>()
                .Where(x => x.Task.Id == taskId);

            return CurrentSession.Query<BaseTaskHistoryItem>()
                .Where(y => list1.Any(z => z == y) || list2.Any(z => z == y))
                .ToList();            
        }

        /// <summary>
        /// Количество связанных (в прошлом и настоящем) с конрагентом задач
        /// </summary>
        /// <param name="contractorId">Код контрагента</param>
        /// <returns>Кол-во связанных задач</returns>
        public int GetTaskCountForContractor(int contractorId)
        {
            return CurrentSession.Query<Task>() // DeletionDate не проверяется, т.к. c удаленными задача связана быть не может
                .Where(x => x.Contractor.Id == contractorId || x.History.Where(y => y.Contractor.Id == contractorId).Any())
                .Count();
        }

        /// <summary>
        /// Количество открытых связанных со сделкой задач
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <returns>Кол-во связанных задач</returns>
        public int GetOpenTaskCountForDeal(int dealId)
        {
            return CurrentSession.Query<Task>() // DeletionDate не проверяется, т.к. c удаленными задача связана быть не может
                .Where(x => x.Deal.Id == dealId && x.ExecutionState.ExecutionStateType != TaskExecutionStateType.Completed)
                .Count();
        }

        /// <summary>
        /// Количество открытых связанных с заказаном на производство
        /// </summary>
        /// <param name="productionOrderId">Код заказа на производство</param>
        /// <returns>Кол-во связанных задач</returns>
        public int GetOpenTaskCountForProductionOrder(Guid productionOrderId)
        {
            return CurrentSession.Query<Task>() // DeletionDate не проверяется, т.к. c удаленными задача связана быть не может
                .Where(x => x.ProductionOrder.Id == productionOrderId && x.ExecutionState.ExecutionStateType != TaskExecutionStateType.Completed)
                .Count();
        }
    }
}
