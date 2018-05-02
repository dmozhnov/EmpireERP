using System;
using System.Collections.Generic;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class WaybillRowArticleMovementRepository : BaseRepository, IWaybillRowArticleMovementRepository
    {
        public WaybillRowArticleMovementRepository() : base()
        {
        }

        public WaybillRowArticleMovement GetById(Guid id)
        {
            return CurrentSession.Get<WaybillRowArticleMovement>(id);
        }

        public void Save(WaybillRowArticleMovement value)
        {
            CurrentSession.SaveOrUpdate(value);
        }

        public void Delete(WaybillRowArticleMovement value)
        {            
            CurrentSession.Delete(value);
        }        

        /// <summary>
        /// Получение списка связей по коду позиции-приемника
        /// </summary>
        public IEnumerable<WaybillRowArticleMovement> GetByDestination(Guid destinationWaybillRowId)
        {
            return Query<WaybillRowArticleMovement>()
                .Where(x => x.DestinationWaybillRowId == destinationWaybillRowId)
                .ToList<WaybillRowArticleMovement>();
        }

        /// <summary>
        /// Получение списка связей по подкритерию для позиций-приемников
        /// </summary>
        public IEnumerable<WaybillRowArticleMovement> GetByDestinationSubQuery(ISubQuery destinationSubQuery)
        {
            return Query<WaybillRowArticleMovement>()
                .PropertyIn(x => x.DestinationWaybillRowId, destinationSubQuery)
                .ToList<WaybillRowArticleMovement>();
        }

        /// <summary>
        /// Получение списка связей по подкритерию для позиций-источников
        /// </summary>
        public IEnumerable<WaybillRowArticleMovement> GetBySourceSubQuery(ISubQuery sourceSubQuery)
        {
            return Query<WaybillRowArticleMovement>()
                .PropertyIn(x => x.SourceWaybillRowId, sourceSubQuery)
                .ToList<WaybillRowArticleMovement>();
        }

        /// <summary>
        /// Получение подкритерия для позиций исходящих накладных по подкритерию для позиций-источников
        /// </summary>
        /// <param name="sourceSubQuery">Подкритерий для позиций-источников</param>
        public ISubQuery GetDestinationsSubQueryBySourcesSubQuery(ISubQuery sourceSubQuery)
        {
            return SubQuery<WaybillRowArticleMovement>()
                .PropertyIn(x => x.SourceWaybillRowId, sourceSubQuery)
                .Select(x => x.DestinationWaybillRowId);
        }

        /// <summary>
        /// Получение подкритерия для позиций входящих накладных по подкритерию для позиций исходящих накладных
        /// </summary>
        /// <param name="destinationSubQuery">Подкритерий для позиций исходящих накладных</param>
        public ISubQuery GetSourcesSubQueryByDestinationsSubQuery(ISubQuery destinationSubQuery)
        {
            return SubQuery<WaybillRowArticleMovement>()
                .PropertyIn(x => x.DestinationWaybillRowId, destinationSubQuery)
                .Select(x => x.SourceWaybillRowId);
        }

        #region Получение подзапроса для позиций накладных, которые являются источниками для позиций-приемников из входящего подзапроса

        public ISubQuery GetReceiptWaybillSourceSubQuery(ISubQuery waybillRowsSubQuery)
        {
            return GetWaybillSourceSubQuery(waybillRowsSubQuery, WaybillType.ReceiptWaybill);
        }

        public ISubQuery GetMovementWaybillSourceSubQuery(ISubQuery waybillRowsSubQuery)
        {
            return GetWaybillSourceSubQuery(waybillRowsSubQuery, WaybillType.MovementWaybill);
        }

        public ISubQuery GetChangeOwnerWaybillSourceSubQuery(ISubQuery waybillRowsSubQuery)
        {
            return GetWaybillSourceSubQuery(waybillRowsSubQuery, WaybillType.ChangeOwnerWaybill);
        }

        public ISubQuery GetReturnFromClientWaybillSourceSubQuery(ISubQuery waybillRowsSubQuery)
        {
            return GetWaybillSourceSubQuery(waybillRowsSubQuery, WaybillType.ReturnFromClientWaybill);
        }

        private ISubQuery GetWaybillSourceSubQuery(ISubQuery waybillRowsSubQuery, WaybillType waybillType)
        {
            return SubQuery<WaybillRowArticleMovement>()
                .PropertyIn(x => x.DestinationWaybillRowId, waybillRowsSubQuery)
                .Where(x => x.SourceWaybillType == waybillType)
                .Select(x => x.SourceWaybillRowId);
        } 
        #endregion
    }
}
