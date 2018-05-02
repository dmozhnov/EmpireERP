using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Domain.Entities;
using NHibernate;
using LinFu.IoC.Configuration;
using NHibernate.Criterion;
using ERP.Infrastructure.NHibernate.Repositories;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ProviderOrganizationRepository : BaseRepository, IProviderOrganizationRepository
    {
        public ProviderOrganizationRepository() : base()
        {
        }

        public ProviderOrganization GetById(int id)
        {
            return Query<ProviderOrganization>().Where(x => x.Id == id).FirstOrDefault<ProviderOrganization>();
        }

        public void Save(ProviderOrganization Value)
        {
            CurrentSession.SaveOrUpdate(Value);            
        }

        public void Delete(ProviderOrganization Value)
        {
            Value.DeletionDate = DateTime.Now;
            CurrentSession.SaveOrUpdate(Value);            
        }

        public IList<ProviderOrganization> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ProviderOrganization>(state, true);
        }

        public IList<ProviderOrganization> GetFilteredList(object state, Utils.ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ProviderOrganization>(state, parameterString, ignoreDeletedRows);
        }

        /// <summary>
        /// Определяем собственный метод фильтрации организаций контрагента
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="criteria"></param>
        protected override void CreateFilter<T>(Infrastructure.Repositories.Criteria.ICriteria<T> criteria)
        {
            Infrastructure.Repositories.Criteria.ICriteria<ProviderOrganization> query = (Infrastructure.Repositories.Criteria.ICriteria<ProviderOrganization>)criteria;

            parameterString.MergeWith(new Utils.ParameterString(filter));

            var jQuery = SubQuery<JuridicalPerson>().Select(x => x.Id);
            var pQuery = SubQuery<PhysicalPerson>().Select(x => x.Id);

            if (parameterString["INN"] != null && parameterString["INN"].Value != null)
            {
                var str = parameterString["INN"].Value is IList<string> ? (parameterString["INN"].Value as IList<string>)[0] : parameterString["INN"].Value as string;

                jQuery.Where(x => x.INN == str);
                pQuery.Where(x => x.INN == str);
            }
            parameterString.Delete("INN");

            if (parameterString["Address"] != null && parameterString["Address"].Value != null)
            {
                var str = parameterString["Address"].Value is IList<string> ? (parameterString["Address"].Value as IList<string>)[0] : parameterString["Address"].Value as string;

                query.Like(x => x.Address, str);
            }
            parameterString.Delete("Address");

            if (parameterString["OGRN"] != null && parameterString["OGRN"].Value != null)
            {
                var str = parameterString["OGRN"].Value is IList<string> ? (parameterString["OGRN"].Value as IList<string>)[0] : parameterString["OGRN"].Value as string;

                jQuery.Where(x => x.OGRN == str);
                pQuery.Where(x => x.OGRNIP == str);
            }
            parameterString.Delete("OGRN");

            if (parameterString["OKPO"] != null && parameterString["OKPO"].Value != null)
            {
                var str = parameterString["OKPO"].Value is IList<string> ? (parameterString["OKPO"].Value as IList<string>)[0] : parameterString["OKPO"].Value as string;

                jQuery.Where(x => x.OKPO == str);
                pQuery = null;
            }
            parameterString.Delete("OKPO");

            if (parameterString["KPP"] != null && parameterString["KPP"].Value != null)
            {
                var str = parameterString["KPP"].Value is IList<string> ? (parameterString["KPP"].Value as IList<string>)[0] : parameterString["KPP"].Value as string;

                jQuery.Where(x => x.KPP == str);
                pQuery = null;
            }
            parameterString.Delete("KPP");

            //Выбираем все организации, удовлетворяющие фильтру
            if (pQuery != null)
                query.Or(x => x.PropertyIn(y => y.EconomicAgent.Id, jQuery), x => x.PropertyIn(y => y.EconomicAgent.Id, pQuery));
            else
                query.PropertyIn(y => y.EconomicAgent.Id, jQuery);

            filter = "";

            base.CreateFilter(query);
        }
    }
}
