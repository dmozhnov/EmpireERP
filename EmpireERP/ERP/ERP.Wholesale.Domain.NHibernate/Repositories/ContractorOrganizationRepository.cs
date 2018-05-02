using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Infrastructure.NHibernate.Repositories;
using ERP.Wholesale.Domain.Repositories;
using ERP.Wholesale.Domain.Entities;
using ERP.Infrastructure.Repositories;
using ERP.Utils;

namespace ERP.Wholesale.Domain.NHibernate.Repositories
{
    public class ContractorOrganizationRepository: BaseRepository, IContractorOrganizationRepository
    {
        public ContractorOrganizationRepository()
        {
        }

        public IList<ContractorOrganization> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ContractorOrganization>(state, ignoreDeletedRows);
        }
        public IList<ContractorOrganization> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return GetBaseFilteredList<ContractorOrganization>(state, parameterString, ignoreDeletedRows);
        }

        /// <summary>
        /// Определяем собственный метод фильтрации организаций контрагента
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="criteria"></param>
        protected override void CreateFilter<T>(Infrastructure.Repositories.Criteria.ICriteria<T> criteria)
        {
            Infrastructure.Repositories.Criteria.ICriteria<ContractorOrganization> query = (Infrastructure.Repositories.Criteria.ICriteria<ContractorOrganization>)criteria;

            ParameterString df = parameterString;

            var jQuery = SubQuery<JuridicalPerson>().Select(x => x.Id);
            var pQuery = SubQuery<PhysicalPerson>().Select(x => x.Id);

            if (df["Name"] != null && df["Name"].Value != null)
            {
                string str = df["Name"].Value is IList<string> ? (df["Name"].Value as IList<string>)[0] : df["Name"].Value as string;
                if (str.Length > 0)
                {
                    query.Like(x => x.ShortName, str);
                }
            }

            if (df["INN"] != null && df["INN"].Value != null)
            {
                string str = df["INN"].Value is IList<string> ? (df["INN"].Value as IList<string>)[0] : df["INN"].Value as string;
                if (str.Length > 0)
                {
                    jQuery.Where(x => x.INN == str);
                    pQuery.Where(x => x.INN == str);
                }
            }

            if (df["Address"] != null && df["Address"].Value != null)
            {
                string str = df["Address"].Value is IList<string> ? (df["Address"].Value as IList<string>)[0] : df["Address"].Value as string;
                if (str.Length > 0)
                {
                    query.Like(x => x.Address, str);                    
                }
            }

            if (df["OGRN"] != null && df["OGRN"].Value != null)
            {
                string str = df["OGRN"].Value is IList<string> ? (df["OGRN"].Value as IList<string>)[0] : df["OGRN"].Value as string;
                if (str.Length > 0)
                {
                    jQuery.Where(x => x.OGRN == str);
                    pQuery.Where(x => x.OGRNIP == str);
                }
            }

            if (df["OKPO"] != null && df["OKPO"].Value != null)
            {
                string str = df["OKPO"].Value is IList<string> ? (df["OKPO"].Value as IList<string>)[0] : df["OKPO"].Value as string;
                if (str.Length > 0)
                {
                    jQuery.Where(x => x.OKPO == str);
                    pQuery = null;
                }
            }

            if (df["KPP"] != null && df["KPP"].Value != null)
            {
                string str = df["KPP"].Value is IList<string> ? (df["KPP"].Value as IList<string>)[0] : df["KPP"].Value as string;
                if (str.Length > 0)
                {
                    jQuery.Where(x => x.KPP == str);
                    pQuery = null;
                }
            }

            //Выбираем все организации, удовлетворяющие фильтру
            if (pQuery != null)
                query.Restriction<EconomicAgent>(x=>x.EconomicAgent).Or(x => x.PropertyIn(y => y.Id, jQuery), x => x.PropertyIn(y => y.Id, pQuery));
            else
                query.Restriction<EconomicAgent>(x=>x.EconomicAgent).PropertyIn(y => y.Id, jQuery);

            if (df["Type"] != null)
            {
                if (df["Type"].Value is IList<string>)
                {
                    var list = df["Type"].Value as IList<string>;
                    if (list.Count > 0)
                    {
                        query.OneOf(x => x.Type, list);
                    }
                }
            }
        }
    }
}
