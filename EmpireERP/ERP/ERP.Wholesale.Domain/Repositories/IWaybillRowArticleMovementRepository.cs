using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.Domain.Repositories
{
    public interface IWaybillRowArticleMovementRepository : IRepository<WaybillRowArticleMovement, Guid>
    {
        /// <summary>
        /// Получение списка связей по коду позиции-приемника
        /// </summary>
        IEnumerable<WaybillRowArticleMovement> GetByDestination(Guid destinationWaybillRowId);

        /// <summary>
        /// Получение списка связей по подкритерию для позиций-приемников
        /// </summary>
        IEnumerable<WaybillRowArticleMovement> GetByDestinationSubQuery(ISubQuery destinationSubquery);

        /// <summary>
        /// Получение списка связей по подкритерию для позиций-источников
        /// </summary>
        IEnumerable<WaybillRowArticleMovement> GetBySourceSubQuery(ISubQuery sourceSubQuery);

        /// <summary>
        /// Получение подкритерия для позиций исходящих накладных по подкритерию для позиций-источников
        /// </summary>
        /// <param name="sourceSubQuery">Подкритерий для позиций-источников</param>
        ISubQuery GetDestinationsSubQueryBySourcesSubQuery(ISubQuery sourceSubQuery);

        /// <summary>
        /// Получение подкритерия для позиций входящих накладных по подкритерию для позиций исходящих накладных
        /// </summary>
        /// <param name="destinationSubQuery">Подкритерий для позиций исходящих накладных</param>
        ISubQuery GetSourcesSubQueryByDestinationsSubQuery(ISubQuery destinationSubQuery);
        

        #region Получение подзапроса для позиций накладных, которые являются источниками для позиций-приемников из входящего подзапроса

        ISubQuery GetReceiptWaybillSourceSubQuery(ISubQuery waybillRowsSubQuery);
        ISubQuery GetMovementWaybillSourceSubQuery(ISubQuery waybillRowsSubQuery);
        ISubQuery GetChangeOwnerWaybillSourceSubQuery(ISubQuery waybillRowsSubQuery);
        ISubQuery GetReturnFromClientWaybillSourceSubQuery(ISubQuery waybillRowsSubQuery);

        #endregion
    }
}
