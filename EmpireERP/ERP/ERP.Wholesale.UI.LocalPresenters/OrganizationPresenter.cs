using System.Data;
using ERP.Infrastructure.UnitOfWork;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.UI.AbstractPresenters;

namespace ERP.Wholesale.UI.LocalPresenters
{
    public class OrganizationPresenter : IOrganizationPresenter
    {
        #region Поля

        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        private readonly IRussianBankService russianBankService;
        private readonly IForeignBankService foreignBankService;

        #endregion

        #region Конструкторы

        public OrganizationPresenter(IUnitOfWorkFactory unitOfWorkFactory, IRussianBankService russianBankService, IForeignBankService foreignBankService)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
            this.russianBankService = russianBankService;
            this.foreignBankService = foreignBankService;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Получение данных банка по БИК
        /// </summary>
        /// <param name="bic">БИК</param>
        /// <returns></returns>
        public object GetBankByBIC(string bic)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                ValidationUtils.Assert(bic != null && bic.Length == 9, "Неверное значение БИК.");

                var bank = russianBankService.GetByBIC(bic);

                var result = bank != null ? new { BankName = bank.Name, CorAccount = bank.CorAccount } : new { BankName = "", CorAccount = "" };

                return result;
            }
        }

        /// <summary>
        /// Получение данных банка по SWIFT
        /// </summary>
        /// <param name="bic">SWIFT банка</param>
        /// <returns></returns>
        public object GetBankBySWIFT(string swift)
        {
            using (IUnitOfWork uow = unitOfWorkFactory.Create(IsolationLevel.ReadCommitted))
            {
                ValidationUtils.Assert(swift != null && (swift.Length == 8 || swift.Length == 11), "Неверное значение SWIFT-кода.");

                object result;

                var bank = foreignBankService.GetBySWIFT(swift);

                if (bank != null)
                {
                    result = new
                    {
                        BankName = bank.Name,
                        Address = bank.Address,
                        ClearingCode = bank.ClearingCode,
                        ClearingCodeType = bank.ClearingCodeType != null ? bank.ClearingCodeType.Value.GetDisplayName() : ""
                    };
                }
                else
                {
                    result = new
                    {
                        BankName = "",
                        Address = "",
                        ClearingCode = "",
                        ClearingCodeType = ""
                    };
                }

                return result;
            }
        }

        #endregion
    }
}
