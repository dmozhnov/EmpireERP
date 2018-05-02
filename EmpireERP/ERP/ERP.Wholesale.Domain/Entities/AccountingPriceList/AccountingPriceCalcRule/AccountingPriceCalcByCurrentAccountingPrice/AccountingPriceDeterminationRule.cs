using System;
using System.Collections.Generic;

namespace ERP.Wholesale.Domain.Entities
{
    /// <summary>
    /// Правило определения учетной цены
    /// </summary>
    public class AccountingPriceDeterminationRule
    {
        /// <summary>
        /// Тип правила определения учетной цены
        /// </summary>
        public virtual AccountingPriceDeterminationRuleType Type
        {
            get
            {
                return type;
            }
            /*protected internal*/ set //будет паблик, пока GetTipsForArticle не будет пересен из контроллера в службу
            {
                if (Type!= 0 && value == AccountingPriceDeterminationRuleType.ByAccountingPriceOnStorage)
                {
                    throw new Exception("Невозможно установить данный тип правила расчета учетной цены.");
                }
                type = value;

            }
        }
        private AccountingPriceDeterminationRuleType type;

        /// <summary>
        /// Тип места хранения для правила определения учетной цены
        /// </summary>
        public virtual AccountingPriceListStorageTypeGroup StorageType
        {
            get
            {
                return storageType;
            }
            protected internal set
            {
                if (value != 0 && Type == AccountingPriceDeterminationRuleType.ByAccountingPriceOnStorage)
                {
                    throw new Exception("Невозможно установить тип места хранения для указанного типа правила.");
                }

                storageType = value;
            }
        }
        private AccountingPriceListStorageTypeGroup storageType;

        /// <summary>
        /// Список мест хранения для правила определения учетной цены
        /// </summary>
        protected internal virtual IEnumerable<Storage> StorageList {get;set;}

        public virtual Storage Storage
        {
            get
            {
                return storage;
            }
            protected internal set
            {
                if (Type != AccountingPriceDeterminationRuleType.ByAccountingPriceOnStorage && value != null)
                {
                    throw new Exception("Невозможно установить конкретное место хранения для указанного типа правила.");
                }
                storage = value;
            }
        }
        private Storage storage;

        #region Конструкторы
        protected AccountingPriceDeterminationRule()
        {
        }

        public AccountingPriceDeterminationRule(AccountingPriceDeterminationRuleType _type,  AccountingPriceListStorageTypeGroup _storageType,
            IEnumerable<Storage> storageList)
        {
            storageType = _storageType;
            StorageList = storageList;
            type = _type;

        }

        public AccountingPriceDeterminationRule(Storage _storage)
        {
            storage = _storage;            
            type = AccountingPriceDeterminationRuleType.ByAccountingPriceOnStorage;
        }

        #endregion


    }
}
