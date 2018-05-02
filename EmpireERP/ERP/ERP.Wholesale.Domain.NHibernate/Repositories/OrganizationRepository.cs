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
    public class OrganizationRepository : BaseRepository, IOrganizationRepository
    {
        public OrganizationRepository()
            : base()
        {
        }

        /// <summary>
        /// Получение организации по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns></returns>
        public Organization GetById(int id)
        {
            return Query<Organization>().Where(x => x.Id == id).FirstOrDefault<Organization>();
        }

        /// <summary>
        /// Сохранение
        /// </summary>
        /// <param name="entity">Организация</param>
        public void Save(Organization entity)
        {
            CurrentSession.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Удаление организации
        /// </summary>
        /// <param name="entity">Организация</param>
        public void Delete(Organization entity)
        {
            entity.DeletionDate = DateTime.Now;
            CurrentSession.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Получение отфильтрованного списка организаций (только нужную страницу)
        /// </summary>
        /// <param name="state">Состояние грида</param>
        /// <returns></returns>
        public IEnumerable<Organization> GetFilteredList(object state)
        {           
            return GetBaseFilteredList<Organization>(state);
        }

        /// <summary>
        /// Определяем собственный метод фильтрации организаций
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="criteria"></param>
        protected override void CreateFilter<T>(Infrastructure.Repositories.Criteria.ICriteria<T> criteria)
        {
            Infrastructure.Repositories.Criteria.ICriteria<Organization> query = (Infrastructure.Repositories.Criteria.ICriteria<Organization>)criteria;

            ParameterString df = new ParameterString(filter);

            var jQuery = SubQuery<JuridicalPerson>().Select(x => x.Id);
            var pQuery = SubQuery<PhysicalPerson>().Select(x => x.Id);

            if (!String.IsNullOrEmpty(df["Name"].Value as string))
            {
                query.Like(x => x.ShortName, df["Name"].Value as string);
            }

            if (!String.IsNullOrEmpty(df["INN"].Value as string))
            {
                jQuery.Where(x => x.INN == df["INN"].Value as string);
                pQuery.Where(x => x.INN == df["INN"].Value as string);
            }

            if (!String.IsNullOrEmpty(df["Address"].Value as string))
            {
                query.Like(x => x.Address, df["Address"].Value as string);
            }

            if (!String.IsNullOrEmpty(df["OGRN"].Value as string))
            {
                jQuery.Where(x => x.OGRN == df["OGRN"].Value as string);
                pQuery.Where(x => x.OGRNIP == df["OGRN"].Value as string);
            }

            if (!String.IsNullOrEmpty(df["OKPO"].Value as string))
            {
                jQuery.Where(x => x.OKPO == df["OKPO"].Value as string);
                pQuery = null;
            }

            if (!String.IsNullOrEmpty(df["KPP"].Value as string))
            {
                jQuery.Where(x => x.KPP == df["KPP"].Value as string);
                pQuery = null;
            }

            //Выбираем все организации, удовлетворяющие фильтру
            if (pQuery != null)
                query.Or(x => x.PropertyIn(y => y.Id, jQuery), x => x.PropertyIn(y => y.Id, pQuery));
            else
                query.PropertyIn(y => y.Id, jQuery);
        }
    }
}
