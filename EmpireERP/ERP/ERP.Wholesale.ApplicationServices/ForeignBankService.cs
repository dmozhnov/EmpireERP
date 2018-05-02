using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class ForeignBankService : IForeignBankService
    {
        #region Поля

        private readonly IForeignBankRepository foreignBankRepository;

        #endregion

        #region Конструкторы

        public ForeignBankService(IForeignBankRepository foreignBankRepository)
        {
            this.foreignBankRepository = foreignBankRepository;
        }

        #endregion

        #region Методы

        public void Save(ForeignBank entity)
        {
            CheckBankUniqueness(entity);
            foreignBankRepository.Save(entity);
        }

        public void Delete(ForeignBank entity)
        {
            var count = foreignBankRepository.Query<ForeignBankAccount>().Where(x => x.Bank.Id == entity.Id).CountDistinct();
            if (count > 0)
            {
                throw new Exception("Невозможно удалить банк, так как с ним связаны расчетные счета организаций.");
            }

            entity.DeletionDate = DateTime.Now;
            foreignBankRepository.Delete(entity);
        }

        public ForeignBank GetBySWIFT(string swift)
        {
            return foreignBankRepository.GetBySWIFT(swift);
        }

        public IList<ForeignBank> GetFilteredList(object state, Utils.ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return foreignBankRepository.GetFilteredList(state, parameterString, ignoreDeletedRows);
        }

        public ForeignBank CheckBankExistence(int id)
        {
            var bank = foreignBankRepository.GetById(id);
            ValidationUtils.NotNull(bank, "Банк не найден. Возможно, он был удален.");

            return bank;
        }

        public void CheckBankUniqueness(ForeignBank bank)
        {
            // проверяем SWIFT-код
            if (foreignBankRepository.Query<ForeignBank>().Where(x => x.SWIFT == bank.SWIFT && x.Id != bank.Id).CountDistinct() > 0)
            {
                throw new Exception("Банк с таким SWIFT-кодом уже создан. Укажите другой SWIFT-код.");
            }

            // проверяем название банка
            if (foreignBankRepository.Query<Bank>().Where(x => x.Name == bank.Name && x.Id != bank.Id).CountDistinct() > 0)
            {
                throw new Exception("Банк с таким именем уже создан. Укажите другое имя.");
            }
        }

        #endregion
    }
}
