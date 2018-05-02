using ERP.Utils;

namespace ERP.Wholesale.UI.ViewModels.Report.Report0006
{
    /// <summary>
    /// Элемент таблицы 6 отчета, отражающий строку таблицы открытых договоров в период
    /// </summary>
    public class Report0006ContractItemViewModel
    {
        #region Свойства

        /// <summary>
        /// Название клиента
        /// </summary>
        public string ClientName { get; protected set; }

        /// <summary>
        /// Название договора "Название договора № ... от ..."
        /// </summary>
        public string ClientContractFullName { get; protected set; }

        /// <summary>
        /// Краткое название собственной организации
        /// </summary>
        public string AccountOrganizationShortName { get; protected set; }

        /// <summary>
        /// Краткое название организации клиента
        /// </summary>
        public string ClientOrganizationShortName { get; protected set; }

        #endregion

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="clientName">Название клиента</param>
        /// <param name="clientContractFullName">Название договора</param>
        /// <param name="accountOrganizationShortName">Краткое название собственной организации</param>
        /// <param name="clientOrganizationShortName">Краткое название организации клиента</param>
        public Report0006ContractItemViewModel(string clientName, string clientContractFullName, string accountOrganizationShortName,
            string clientOrganizationShortName)
        {
            ClientName = clientName;
            ClientContractFullName = clientContractFullName;
            AccountOrganizationShortName = accountOrganizationShortName;
            ClientOrganizationShortName = clientOrganizationShortName;
        }

        #endregion
    }
}
