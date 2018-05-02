using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Infrastructure.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Infrastructure.Repositories.Criteria;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IProductionOrderPaymentRepository : IFilteredRepository<ProductionOrderPayment>
    {
        /// <summary>
        /// Получение сущности по Id
        /// </summary>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>Сущность</returns>
        ProductionOrderPayment GetById(Guid id);

        /// <summary>
        /// Получение списка сущностей
        /// </summary>
        /// <returns>Список сущностей</returns>
        IEnumerable<ProductionOrderPayment> GetAll();

        /// <summary>
        /// Выполнение запроса по критериям
        /// </summary>
        /// <returns></returns>
        ICriteria<TResult> Query<TResult>(bool ignoreDeletedRows = true, string alias = "") where TResult : class;

        /// <summary>
        /// Подзапрос
        /// </summary>
        ISubCriteria<TResult> SubQuery<TResult>(bool ignoreDeletedRows = true) where TResult : class;
    }
}
