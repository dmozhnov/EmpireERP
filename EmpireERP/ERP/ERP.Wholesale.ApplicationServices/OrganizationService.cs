using System;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class OrganizationService : IOrganizationService
    {
        #region Поля

        private readonly IOrganizationRepository organizationRepository;

        #endregion

        #region Конструктор

        public OrganizationService(IOrganizationRepository organizationRepository)
        {
            this.organizationRepository = organizationRepository;
        }

        #endregion

        #region Методы

        #region Проверка на уникальность

        #region Проверка расчетных счетов на уникальность

        /// <summary>
        /// Проверка расчетного счета на уникальность
        /// </summary>
        /// <param name="bankAccount">Расчетный счет</param>
        public void CheckBankAccountUniqueness(RussianBankAccount bankAccount)
        {
            var crit = organizationRepository.Query<RussianBankAccount>()
                .Where(x => x.Number == bankAccount.Number)
                .Where(x => x.Bank.Id == bankAccount.Bank.Id)
                .Where(x => x.Id != bankAccount.Id);

            var bankAccountCount = crit.Count();

            if (bankAccountCount != 0)
            {
                throw new Exception(String.Format("Расчетный счет с номером «{0}» уже существует.", bankAccount.Number));
            }
        }

        /// <summary>
        /// Проверка расчетного счета на уникальность
        /// </summary>
        /// <param name="bankAccount">Расчетный счет</param>
        public void CheckBankAccountUniqueness(ForeignBankAccount bankAccount)
        {
            var crit = organizationRepository.Query<ForeignBankAccount>()
                .Where(x => x.Number == bankAccount.Number || (x.IBAN == bankAccount.IBAN && x.IBAN != ""))
                .Where(x => x.Bank.Id == bankAccount.Bank.Id)
                .Where(x => x.Id != bankAccount.Id);

            var bankAccounts = crit.ToList<ForeignBankAccount>();

            if (bankAccounts.Count != 0)
            {
                if (bankAccounts.Where(x => x.Number == bankAccount.Number).Count() > 0)
                {
                    throw new Exception(String.Format("Расчетный счет с номером «{0}» уже существует.", bankAccount.Number));
                }
                if (bankAccount.IBAN.Length > 0 && bankAccounts.Where(x => x.IBAN == bankAccount.IBAN).Count() > 0)
                {
                    throw new Exception(String.Format("Расчетный счет с IBAN «{0}» уже существует.", bankAccount.IBAN));
                }
            }
        } 
        #endregion

        #region Проверка организации на уникальность

        /// <summary>
        /// Проверка организации на уникальность
        /// </summary>
        /// <param name="organization"></param>
        public void CheckOrganizationUniqueness<T>(T organization) where T : Organization
        {
            string organizationTypeName = GetOrganizationTypeName(organization);

            CheckOrganizationShortNameUniqueness(organization, organizationTypeName);
            CheckOrganizationFullNameUniqueness(organization, organizationTypeName);
            CheckOrganizationEconomicAgentUniqueness(organization, organizationTypeName);
        }

        /// <summary>
        /// Проверка уникальности полного наименования организации клиента
        /// </summary>
        /// <param name="organization"></param>
        private void CheckOrganizationFullNameUniqueness<T>(T organization, string organizationTypeName) where T : Organization
        {
            ValidationUtils.Assert(organizationRepository.Query<T>()
                .Where(x => x.FullName == organization.FullName && x.Id != organization.Id).Count() == 0,
                organizationTypeName + " с таким полным наименованием уже существует.");
        }

        /// <summary>
        /// Проверка уникальности краткого наименования организации клиента
        /// </summary>
        /// <param name="organization"></param>
        private void CheckOrganizationShortNameUniqueness<T>(T organization, string organizationTypeName) where T:Organization
        {
            ValidationUtils.Assert(organizationRepository.Query<T>()
                .Where(x => x.ShortName == organization.ShortName && x.Id != organization.Id).Count() == 0,
                organizationTypeName + " с таким кратким наименованием уже существует.");
        }

        /// <summary>
        /// Проверка уникальности кодов хозяйствующего субъекта(экономического агента)
        /// </summary>
        /// <param name="organization"></param>
        private void CheckOrganizationEconomicAgentUniqueness<T>(T organization, string organizationTypeName) where T:Organization
        {
            var economicAgentsSubQ = organizationRepository.SubQuery<T>().Select(x => x.EconomicAgent.Id);

            if (organization.EconomicAgent.Is<JuridicalPerson>())
            {
                var juridicalPerson = organization.EconomicAgent.As<JuridicalPerson>();
                var INN_KPPCount = organizationRepository.Query<JuridicalPerson>()
                    .PropertyIn(x => x.Id, economicAgentsSubQ)
                    .Where(x => x.INN != "")
                    .Where(x => x.INN == juridicalPerson.INN && x.KPP == juridicalPerson.KPP && x.Id != juridicalPerson.Id)
                    .Count();
                ValidationUtils.Assert(INN_KPPCount == 0, organizationTypeName + " с таким сочетанием ИНН и КПП уже существует.");


                var OGRNCount = organizationRepository.Query<JuridicalPerson>()
                    .PropertyIn(x => x.Id, economicAgentsSubQ)
                    .Where(x => x.OGRN != "")
                    .Where(x => x.OGRN == juridicalPerson.OGRN && x.Id != juridicalPerson.Id)
                    .Count();
                ValidationUtils.Assert(OGRNCount == 0, organizationTypeName + " с таким ОГРН уже существует.");
            }

            if (organization.EconomicAgent.Is<PhysicalPerson>())
            {
                var physicalPerson = organization.EconomicAgent.As<PhysicalPerson>();
                var INNCount = organizationRepository.Query<PhysicalPerson>()
                    .PropertyIn(x => x.Id, economicAgentsSubQ)
                    .Where(x => x.INN != "")
                    .Where(x => x.INN == physicalPerson.INN && x.Id != physicalPerson.Id)
                    .Count();
                ValidationUtils.Assert(INNCount == 0, organizationTypeName + " с таким ИНН уже существует.");

                var OGRNIPCount = organizationRepository.Query<PhysicalPerson>()
                    .PropertyIn(x => x.Id, economicAgentsSubQ)
                    .Where(x => x.OGRNIP != "")
                    .Where(x => x.OGRNIP == physicalPerson.OGRNIP && x.Id != physicalPerson.Id)
                    .Count();
                ValidationUtils.Assert(OGRNIPCount == 0, organizationTypeName + " с таким ОГРНИП уже существует.");
            }
        }

        #endregion

        #endregion

        #region Проверка расчетных счетов на возможность удаления

        /// <summary>
        /// Проверка возможности удаления расчетного счета
        /// </summary>
        /// <param name="bankAccount">Расчетный счет</param>
        public void CheckBankAccountDeletionPossibility(RussianBankAccount bankAccount)
        {
            // Пока никаких проверок нет.
        }


        /// <summary>
        /// Проверка возможности удаления расчетного счета
        /// </summary>
        /// <param name="bankAccount">Расчетный счет</param>
        public void CheckBankAccountDeletionPossibility(ForeignBankAccount bankAccount)
        {
            // Пока никаких проверок нет.
        } 
        #endregion

        #region Вспомогательные методы

        private string GetOrganizationTypeName(Organization organization)
        {
            if (organization.Is<ProviderOrganization>())
            {
                return "Организация поставщика";
            }

            if (organization.Is<ClientOrganization>())
            {
                return "Организация клиента";
            }

            if (organization.Is<AccountOrganization>())
            {
                return "Собственная организация";
            }

            throw new Exception("Неизвестный тип организации");

        }

        #endregion

        #endregion

    }
}
