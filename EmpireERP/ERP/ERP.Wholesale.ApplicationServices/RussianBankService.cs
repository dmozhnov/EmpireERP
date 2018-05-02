using System;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class RussianBankService : IRussianBankService
    {
        #region Поля

        private readonly IRussianBankRepository russianBankRepository;

        #endregion

        #region Конструкторы

        public RussianBankService(IRussianBankRepository russianBankRepository)
        {
            this.russianBankRepository = russianBankRepository;
        }

        #endregion

        #region Методы

        public void Save(RussianBank entity)
        {
            CheckBankUniqueness(entity);
            russianBankRepository.Save(entity);
        }

        public void Delete(RussianBank entity)
        {
            var count = russianBankRepository.Query<RussianBankAccount>().Where(x => x.Bank.Id == entity.Id).CountDistinct();
            if (count > 0)
            {
                throw new Exception("Невозможно удалить банк, так как с ним связаны расчетные счета организаций.");
            }

            entity.DeletionDate = DateTime.Now;
            russianBankRepository.Delete(entity);
        }

        public RussianBank GetByBIC(string BIC)
        {
            return russianBankRepository.Query<RussianBank>().Where(x => x.BIC == BIC).FirstOrDefault<RussianBank>();
        }

        public IList<RussianBank> GetFilteredList(object state, bool ignoreDeletedRows = true)
        {
            return russianBankRepository.GetFilteredList(state, ignoreDeletedRows);
        }

        public IList<RussianBank> GetFilteredList(object state, ParameterString parameterString, bool ignoreDeletedRows = true)
        {
            return russianBankRepository.GetFilteredList(state, parameterString, ignoreDeletedRows);
        }

        public RussianBank CheckBankExistence(int id)
        {
            var bank = russianBankRepository.GetById(id);
            ValidationUtils.NotNull(bank, "Банк не найден. Возможно, он был удален.");

            return bank;
        }

        public void CheckBankUniqueness(RussianBank bank)
        {
            // проверяем БИК
            if (russianBankRepository.Query<RussianBank>().Where(x => x.BIC == bank.BIC && x.Id != bank.Id).CountDistinct() > 0)
            {
                throw new Exception("Банк с данным БИК уже создан. Укажите другой БИК.");
            }

            // проверяем название банка
            if (russianBankRepository.Query<Bank>().Where(x => x.Name == bank.Name && x.Id != bank.Id).CountDistinct() > 0)
            {
                throw new Exception("Банк с данным именем уже создан. Укажите другое имя.");
            }
        }

        #endregion
    }
}
