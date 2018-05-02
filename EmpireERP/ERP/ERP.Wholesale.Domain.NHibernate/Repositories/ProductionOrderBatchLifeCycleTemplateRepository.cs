using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Utils;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ProductionOrderBatchLifeCycleTemplateRepository : BaseRepository, IProductionOrderBatchLifeCycleTemplateRepository
    {
        public ProductionOrderBatchLifeCycleTemplateRepository() : base()
        {
        }

        /// <summary>
        /// Получение сущности по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        public ProductionOrderBatchLifeCycleTemplate GetById(short id)
        {
            return CurrentSession.Get<ProductionOrderBatchLifeCycleTemplate>(id);
        }

        /// <summary>
        /// Сохранение
        /// </summary>
        /// <param name="entity">Сущность</param>
        public void Save(ProductionOrderBatchLifeCycleTemplate entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Удаление
        /// </summary>
        /// <param name="entity">Сущность</param>
        public void Delete(ProductionOrderBatchLifeCycleTemplate entity)
        {
            CurrentSession.Delete(entity);
        }
        
        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        /// <param name="state">Состояние грида</param>
        public IList<ProductionOrderBatchLifeCycleTemplate> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ProductionOrderBatchLifeCycleTemplate>(state, ignoreDeletedRows);
        }

        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        public IList<ProductionOrderBatchLifeCycleTemplate> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ProductionOrderBatchLifeCycleTemplate>(state, parameterString, ignoreDeletedRows);
        }
    }
}
