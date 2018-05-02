using System;
using System.Linq;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Поставщик
    /// </summary>
    public class Provider : Contractor
    {
        #region Свойства

        /// <summary>
        /// Тип поставщика
        /// </summary>
        public virtual ProviderType Type { get; set; }

        /// <summary>
        /// Надежность
        /// </summary>
        public virtual ProviderReliability Reliability { get; set; }

        #endregion

        #region Конструкторы

        protected Provider()
        {
        }

        public Provider(string name, ProviderType type, ProviderReliability reliability, byte rating) : base(name)
        {
            ContractorType = ContractorType.Provider;

            Type = type;
            Reliability = reliability;
            Rating = rating;
        }

        #endregion

        #region Методы

        #region Договоры поставщика

        /// <summary>
        /// Связать договор с поставщиком
        /// </summary>
        /// <param name="contract">Договор с поставщиком</param>
        public virtual void AddProviderContract(ProviderContract contract)
        {
            if (contracts.Contains(contract))
            {
                throw new Exception("Данный договор уже содержится в списке договоров поставщика.");
            }

            contracts.Add(contract);
            if (!contract.Contractors.Contains(this))
            {
                contract.AddContractor(this);
            }
        }

        /// <summary>
        /// Убрать связь договора с поставщиком
        /// </summary>
        /// <param name="contract">Договор с поставщиком</param>
        public virtual void RemoveProviderContract(ProviderContract contract)
        {
            if (!contracts.Contains(contract))
            {
                throw new Exception("Данный договор не содержится в списке договоров поставщика. Возможно, он был удален.");
            }

            contracts.Remove(contract);
            if (contract.Contractors.Contains(this))
            {
                contract.RemoveContractor(this);
            }
        }

        #endregion

        #endregion
    }
}
