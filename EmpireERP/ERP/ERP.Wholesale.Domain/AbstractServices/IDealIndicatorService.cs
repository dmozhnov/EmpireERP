using System;
using System.Collections.Generic;
using ERP.Infrastructure.Repositories.Criteria;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Misc;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IDealIndicatorService
    {
        /// <summary>
        /// Расчет остатка кредитного лимита по всем постоплатным накладным реализации для данной сделки.
        /// Накладная реализации должна иметь квоту с постоплатой (т.е. с отсрочкой платежа).
        /// Если же накладная имеет квоту с предоплатой, currentCreditLimitSum будет равно null.
        /// Если квота по реализации имеет безлимитный кредит, currentCreditLimitSum будет равно 0.
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="currentCreditLimitSum">Текущее значение кредитного лимита (0 - бесконечный кредит)</param>
        /// <returns>Остаток кредитного лимита</returns>
        decimal CalculateCreditLimitRemainder(Deal deal, out decimal? currentCreditLimitSum, SaleWaybill saleWaybillToExclude/* = null*/);

        /// <summary>
        /// Расчет остатка кредитного лимита по отгруженным постоплатным накладным реализации для данной сделки.
        /// Накладная реализации должна иметь квоту с постоплатой (т.е. с отсрочкой платежа).
        /// Если же накладная имеет квоту с предоплатой, currentCreditLimitSum будет равно null.
        /// Если накладная реализации имеет квоту с безлимитным кредитом, currentCreditLimitSum будет равно 0.
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="currentCreditLimitSum">Текущее значение кредитного лимита (0 - бесконечный кредит)</param>
        /// <returns>Остаток кредитного лимита</returns>        
        List<KeyValuePair<SaleWaybill, decimal>> CalculatePostPaymentShippedCreditLimitRemainder(Deal deal, out List<KeyValuePair<SaleWaybill, decimal?>> currentCreditLimitSumList);

        /// <summary>
        /// Расчет основных показателей
        /// </summary>
        /// <param name="deal">Сделка</param>
        /// <param name="team">Команда, по которой рассчитать показатели. null - рассчитать по всем командам.</param>
        /// <param name="calculateSaleSum">Рассчитывать сумму реализации</param>
        /// <param name="calculateShippingPendingSaleSum">Рассчитывать сумму реализации по неотгруженным накладным</param>
        /// <param name="calculateBalance">Рассчитывать сальдо</param>
        /// <param name="calculatePaymentDelayPeriod">Рассчитывать период просрочки</param>
        /// <param name="calculatePaymentDelaySum">Рассчитывать сумму просрочки</param>
        /// <param name="calculateReturnedFromClientSum">Рассчитывать сумму принятых возвратов</param>
        /// <param name="calculateReservedByReturnFromClientSum">Рассчитывать сумму оформленных возвратов</param>
        /// <param name="calculateInitialBalance">Рассчитывать сумму корректировок сальдо</param>        
        DealMainIndicators CalculateMainIndicators(Deal deal, Team team = null, bool calculateSaleSum = false, bool calculateShippingPendingSaleSum = false,
            bool calculateBalance = false, bool calculatePaymentDelayPeriod = false,
            bool calculatePaymentDelaySum = false, bool calculateReturnedFromClientSum = false, bool calculateReservedByReturnFromClientSum = false,
            bool calculateInitialBalance = false);

        /// <summary>
        /// Расчет суммы реализации
        /// </summary>
        /// <param name="deal">Сделка</param>
        decimal CalculateSaleSum(Deal deal);

        /// <summary>
        /// Расчет текущего периода просрочки
        /// </summary>
        /// <param name="deal">Сделка</param>
        int CalculatePaymentDelayPeriod(Deal deal);

        /// <summary>
        /// Получение подкритерия по сделке, ограничивающего ее видимостью данного юзера для данного права
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="permission">Право, для которого берутся сделки</param>
        /// <returns>null, если пользователь не имеет права; иначе подкритерий возможных сделок</returns>
        ISubCriteria<Deal> RestrictByUserPermissions(User user, Permission permission);

        /// <summary>
        /// Проверка, есть ли невидимые для пользователя сделки по клиенту
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        bool AreAnyRestrictedDeals(Client client, IEnumerable<Permission> permissionList, User user);

        /// <summary>
        /// Проверка, есть ли невидимые для пользователя сделки по организации клиента
        /// </summary>
        /// <param name="clientOrganization"></param>
        /// <param name="user"></param>
        bool AreAnyRestrictedDeals(ClientOrganization clientOrganization, IEnumerable<Permission> permissionList, User user);

        /// <summary>
        /// Рассчитать список балансов по сделкам, принадлежащим данному клиенту, сгруппированный по сделкам, на дату
        /// (для печатной формы - все сделки, все виды документов)
        /// </summary>
        IList<InitialBalanceInfo> CalculateBalanceOnDateByClient(Client client, DateTime date, User user);

        /// <summary>
        /// Рассчитать список балансов по сделкам, принадлежащим данным клиентам и командам, сгруппированный по сделкам, на дату
        /// (для отчета - видимые по подкритерию сделки)
        /// </summary>
        /// <param name="clientIdList">Список кодов клиентов. Null - все клиенты</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        IList<InitialBalanceInfo> CalculateBalanceOnDateByClientAndTeamList(IList<int> clientIdList, IEnumerable<short> teamIdList, DateTime date,
            User user, bool includeExpenditureWaybillsAndReturnFromClientWaybills, bool includeDealPayments, bool includeDealInitialBalanceCorrections);

        /// <summary>
        /// Рассчитать список балансов по сделкам, принадлежащим данной организации клиента, сгруппированный по сделкам, на дату
        /// (для печатной формы - все сделки, все виды документов)
        /// </summary>
        IList<InitialBalanceInfo> CalculateBalanceOnDateByClientOrganization(ClientOrganization clientOrganization, DateTime date, User user);

        /// <summary>
        /// Рассчитать список балансов по сделкам, принадлежащим данным организациям клиента и командам, сгруппированный по сделкам, на дату
        /// (для отчета - видимые по подкритерию сделки)
        /// </summary>
        /// <param name="clientOrganizationIdList">Список кодов организаций клиента. Null - все организации клиента</param>
        /// <param name="teamIdList">Список кодов команд. Null - все команды</param>
        /// <returns>Список {AccountOrganizationId, ClientId, ClientOrganizationId, ContractId, TeamId, Сумма реализаций}</returns>
        IList<InitialBalanceInfo> CalculateBalanceOnDateByClientOrganizationAndTeamList(IList<int> clientOrganizationIdList, IEnumerable<short> teamIdList, DateTime date,
            User user, bool includeExpenditureWaybillsAndReturnFromClientWaybills, bool includeDealPayments, bool includeDealInitialBalanceCorrections);

        /// <summary>
        /// Получить нужный подзапрос в зависимости от распространения права (null - права нет)
        /// </summary>
        /// <param name="allDealSubQuery">Подкритерий, ограничивающий множество сделок (без учета видимости)</param>
        /// <param name="teamDealSubQuery">Подкритерий, ограничивающий множество сделок (командная видимость)</param>
        /// <param name="personalDealSubQuery">Подкритерий, ограничивающий множество сделок (персональная видимость)</param>
        /// <param name="permission">Право</param>
        /// <param name="user">Пользователь, видимостью которого ограничивать множества документов. Null - не ограничивать видимостью</param>
        ISubQuery GetDealSubQueryByPermissionDistribution(ISubQuery allDealSubQuery, ISubQuery teamDealSubQuery, ISubQuery personalDealSubQuery,
            Permission permission, User user);
    }
}
