using System;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    /// <summary>
    /// Сервис для сущностей «Страна»
    /// </summary>
    public class CountryService : BaseDictionaryService<Country>, ICountryService
    {
        #region Свойства

        protected override string UniquenessErrorString
        {
            get { return "Страна с таким наименованием уже существует."; }
        }

        protected override string CheckExistenceErrorString
        {
            get { return "Страна не найдена. Возможно, она была удалена."; }
        }

        #region Права на операции

        public override Permission CreationPermission
        {
            get { return Permission.Country_Create; }
        }

        public override Permission EditingPermission
        {
            get { return Permission.Country_Edit; }
        }

        public override Permission DeletionPermission
        {
            get { return Permission.Country_Delete; }
        }

        public override Permission ListViewingPermission
        {
            get { return Permission.Article_List_Details; }
        }

        #endregion

        #endregion

        #region Конструктор

        public CountryService(IBaseDictionaryRepository<Country> baseDictionaryRepository)
            : base(baseDictionaryRepository)
        {
        }

        #endregion

        #region Методы

        protected override void CheckPossibilityToDelete(Country country, User user, bool checkLogic = true)
        {
            CheckPermissionToPerformOperation(user, DeletionPermission);

            if (checkLogic)
            {
                if (baseDictionaryRepository.Query<Article>().Where(x => x.ProductionCountry.Id == country.Id).Count() > 0)
                {
                    throw new Exception("Невозможно удалить страну, т.к. существуют товары с такой страной производства.");
                }
            }
        }

        public override void CheckUniqueness(Country country)
        {
            base.CheckUniqueness(country);

            int count = baseDictionaryRepository.Query<Country>().Where(x => x.NumericCode == country.NumericCode && x.Id != country.Id).Count();

            if (count > 0)
            {
                throw new Exception("Страна с таким цифровым кодом уже существует");
            }
        }       

        #endregion
    }
}