using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface ITaskRepository : IRepository<Task, int>, IFilteredRepository<Task>
    {
        /// <summary>
        /// Получение истории изменений по задачи
        /// </summary>
        /// <param name="taskId">Идентификатор задачи</param>
        /// <returns></returns>
        IEnumerable<BaseTaskHistoryItem> GetChangeHistory(int taskId);

        /// <summary>
        /// Количество связанных с конрагентом задач
        /// </summary>
        /// <param name="contractorId">Код контрагента</param>
        /// <returns>Кол-во связанных задач</returns>
        int GetTaskCountForContractor(int contractorId);

        /// <summary>
        /// Количество открытых связанных со сделкой задач
        /// </summary>
        /// <param name="dealId">Код сделки</param>
        /// <returns>Кол-во связанных задач</returns>
        int GetOpenTaskCountForDeal(int dealId);

        /// <summary>
        /// Количество открытых связанных с заказаном на производство
        /// </summary>
        /// <param name="productionOrderId">Код заказа на производство</param>
        /// <returns>Кол-во связанных задач</returns>
        int GetOpenTaskCountForProductionOrder(Guid productionOrderId);
    }
}
