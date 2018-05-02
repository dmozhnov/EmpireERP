using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Utils;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ProducerRepository : BaseRepository, IProducerRepository
    {
        public ProducerRepository() : base()
        {
        }

        public Producer GetById(int id)
        {
            return Query<Producer>().Where(x => x.Id == id).FirstOrDefault<Producer>();
        }

        public void Save(Producer value)
        {
            CurrentSession.SaveOrUpdate(value);
        }

        public void Delete(Producer value)
        {
            CurrentSession.SaveOrUpdate(value);
        }
        
        public IList<Producer> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Producer>(state, ignoreDeletedRows);
        }

        public IList<Producer> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<Producer>(state, parameterString, ignoreDeletedRows);
        }

        public IList<ProductionOrderPayment> GetPaymentsFilteredList(object state, ParameterString parameterString)
        {
            ReadState(state); // Читаем данные из объекта state

            ParameterString df = new ParameterString(filter);
            ParameterString dp = new ParameterString(parameters);

            var producerId = ValidationUtils.TryGetInt(dp["ProducerId"].Value as string);
            var productionOrderPaymentTypeId = ValidationUtils.TryGetInt(dp["ProductionOrderPaymentTypeId"].Value as string);

            #region Фильтр

            var crit = Query<ProductionOrderPayment>();

            crit.Where(x => x.Type == (ProductionOrderPaymentType)productionOrderPaymentTypeId);
            crit.Restriction<ProductionOrder>(x => x.ProductionOrder).Where(x => x.Producer.Id == producerId);

            CreateFilter(crit);

            #endregion

            int totalRowCount = crit.Count();
            WriteTotalRowCount(state, totalRowCount); // Записываем общее кол-во строк

            SortByCriteria(crit, sort);

            // Вычисляем начальную и конечную строку страницы
            int maxPage = (totalRowCount + pageSize - 1) / pageSize;
            currentPage = currentPage > maxPage ? maxPage : currentPage;

            return crit.SetFirstResult((currentPage - 1) * pageSize).SetMaxResults(pageSize).ToList<ProductionOrderPayment>();
        }

        public IList<ProductionOrder> GetProductionOrders(Producer producer)
        {
            return Query<ProductionOrder>().Where(x => x.Producer.Id == producer.Id).ToList<ProductionOrder>();
        }
    }
}
