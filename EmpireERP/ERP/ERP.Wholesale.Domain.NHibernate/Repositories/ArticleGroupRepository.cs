using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Domain.Entities;
using NHibernate;
using NHibernate.Criterion;
using ERP.Infrastructure.NHibernate.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ArticleGroupRepository : BaseRepository, IArticleGroupRepository
    {
        public ArticleGroupRepository() : base()
        {
        }

        public ArticleGroup GetById(short id)
        {
            return CurrentSession.Get<ArticleGroup>(id);            
        }

        public void Save(ArticleGroup Value)
        {
            CurrentSession.SaveOrUpdate(Value);            
        }

        public void Delete(ArticleGroup Value)
        {
            CurrentSession.Delete(Value);                
        }

        public IEnumerable<ArticleGroup> GetAll()
        {
            return CurrentSession.CreateCriteria(typeof(ArticleGroup)).List<ArticleGroup>();
        }
    }
}
