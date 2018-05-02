
namespace ERP.Wholesale.UI.AbstractPresenters
{
    public interface IOrganizationPresenter
    {
        /// <summary>
        /// Получение данных банка по БИК
        /// </summary>
        /// <param name="bic">БИК</param>
        /// <returns></returns>
        object GetBankByBIC(string bic);

        /// <summary>
        /// Получение данных банка по SWIFT
        /// </summary>
        /// <param name="bic">SWIFT банка</param>
        /// <returns></returns>
        object GetBankBySWIFT(string swift);
    }
}
