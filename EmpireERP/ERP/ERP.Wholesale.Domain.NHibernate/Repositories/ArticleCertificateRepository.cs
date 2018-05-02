using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using ERP.Wholesale.Domain.Entities;
using NHibernate.Criterion;
using ERP.Infrastructure.Repositories;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Wholesale.Domain.Repositories;
using ERP.Utils;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ArticleCertificateRepository : BaseRepository, IArticleCertificateRepository
    {
        public ArticleCertificateRepository() : base()
        {
        }

        public ArticleCertificate GetById(int Id)
        {
            return CurrentSession.Get<ArticleCertificate>(Id);
        }

        public void Save(ArticleCertificate Value)
        {
            CurrentSession.SaveOrUpdate(Value);
        }

        public void Delete(ArticleCertificate Value)
        {
            CurrentSession.Delete(Value);
        }

        /// <summary>
        /// Получение списка записей с учетом фильтра
        /// </summary>
        public IList<ArticleCertificate> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ArticleCertificate>(state, ignoreDeletedRows);
        }

        public IList<ArticleCertificate> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ArticleCertificate>(state, parameterString, ignoreDeletedRows);
        }
    }
}
