using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Repositories;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Utils;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class DefaultProductionOrderStageRepository : BaseRepository, IDefaultProductionOrderStageRepository
    {
        public DefaultProductionOrderStageRepository()
        {
        }

        public DefaultProductionOrderBatchStage GetById(int id)
        {
            return CurrentSession.Get<DefaultProductionOrderBatchStage>(id);
        }

        public void Save(DefaultProductionOrderBatchStage entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        public void Delete(DefaultProductionOrderBatchStage entity)
        {
            CurrentSession.Delete(entity);
        }
    }
}
