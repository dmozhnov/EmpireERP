using System;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    /// <summary>
    /// Сервис для сущностей «Ставка НДС»
    /// </summary>
    public class ValueAddedTaxService : BaseDictionaryService<ValueAddedTax>, IValueAddedTaxService
    {
        #region Свойства

        protected override string UniquenessErrorString
        {
            get { return "Ставка НДС с таким наименованием или процентом уже существует."; }
        }

        protected override string CheckExistenceErrorString
        {
            get { return "Ставка НДС не найдена. Возможно, она была удалена."; }
        }

        #region Права на операции

        public override Permission CreationPermission
        {
            get { return Permission.ValueAddedTax_Create; }
        }

        public override Permission EditingPermission
        {
            get { return Permission.ValueAddedTax_Edit; }
        }

        public override Permission DeletionPermission
        {
            get { return Permission.ValueAddedTax_Delete; }
        }

        public override Permission ListViewingPermission
        {
            get { return Permission.ReceiptWaybill_List_Details; }
        }

        #endregion

        #endregion

        #region Конструктор

        public ValueAddedTaxService(IBaseDictionaryRepository<ValueAddedTax> baseDictionaryRepository)
            : base(baseDictionaryRepository)
        {
        }

        #endregion

        #region Методы

        /// <summary>
        /// Сохранить ставку НДС
        /// </summary>
        /// <param name="valueAddedTax"></param>
        /// <returns></returns>
        public override short Save(ValueAddedTax valueAddedTax)
        {
            CheckValueAddedTaxUniqueness(valueAddedTax.Id, valueAddedTax.Name, valueAddedTax.Value);

            if (valueAddedTax.IsDefault)
            {
                //Отменим предыдущую дефолтную, если есть
                var oldDefault =
                    baseDictionaryRepository.Query<ValueAddedTax>().Where(x => x.IsDefault == true && x.Id != valueAddedTax.Id).FirstOrDefault<ValueAddedTax>();

                if (oldDefault != null)
                {
                    oldDefault.IsDefault = false;
                    baseDictionaryRepository.Save(oldDefault);
                }
            }
            else
            {
                //проверим, осталась ли дефолтная НДС в справочнике
                var oldDefault =
                    baseDictionaryRepository.Query<ValueAddedTax>().Where(x => x.IsDefault == true).FirstOrDefault<ValueAddedTax>();
                ValidationUtils.NotNull(oldDefault, "Невозможно отменить значение по умолчанию для ставки НДС. Установите значение по умолчанию для нужной ставки НДС.");
            }

            baseDictionaryRepository.Save(valueAddedTax);

            return valueAddedTax.Id;
        }

        /// <summary>
        /// Проверка ставки НДС на уникальность
        /// </summary>
        /// <param name="id">id сущности</param>
        /// <param name="name">Наименование</param>
        /// <param name="value">Ставка НДС</param>
        public void CheckValueAddedTaxUniqueness(short id, string name, decimal value)
        {
            int count = baseDictionaryRepository.Query<ValueAddedTax>().Where(
                x => (x.Name == name || x.Value == value) && x.Id != id).Count();

            if (count > 0)
            {
                throw new Exception(UniquenessErrorString);
            }
        }

        protected override void CheckPossibilityToDelete(ValueAddedTax valueAddedTax, User user, bool checkLogic = true)
        {
            CheckPermissionToPerformOperation(user, DeletionPermission);

            if (checkLogic)
            {
                if(valueAddedTax.IsDefault)
                {
                    throw new Exception("Невозможно удалить используемую по умолчанию ставку НДС");
                }

                CheckPossibilityToModify(valueAddedTax, "удалить");
            }
        }

        private void CheckPossibilityToModify(ValueAddedTax valueAddedTax, string modifyName)
        {
            if (baseDictionaryRepository.Query<ReceiptWaybill>().Where(x => x.PendingValueAddedTax.Id == valueAddedTax.Id).Count() > 0)
            {
                throw new Exception(String.Format("Невозможно {0} ставку НДС, т.к. существуют приходные накладные с такой ставкой НДС.", modifyName));
            }

            if (baseDictionaryRepository.Query<ExpenditureWaybill>().Where(x => x.ValueAddedTax.Id == valueAddedTax.Id).Count() > 0)
            {
                throw new Exception(String.Format("Невозможно {0} ставку НДС, т.к. существуют накладные реализации с такой ставкой НДС.", modifyName));
            }

            if (baseDictionaryRepository.Query<MovementWaybill>().Where(x => x.ValueAddedTax.Id == valueAddedTax.Id).Count() > 0)
            {
                throw new Exception(String.Format("Невозможно {0} ставку НДС, т.к. существуют накладные перемещения с такой ставкой НДС.", modifyName));
            }

            if (baseDictionaryRepository.Query<ChangeOwnerWaybill>().Where(x => x.ValueAddedTax.Id == valueAddedTax.Id).Count() > 0)
            {
                throw new Exception(String.Format("Невозможно {0} ставку НДС, т.к. существуют накладные смены собственника с такой ставкой НДС.", modifyName));
            }

            if (baseDictionaryRepository.Query<ReceiptWaybillRow>().Where(x => x.ApprovedValueAddedTax.Id == valueAddedTax.Id || x.PendingValueAddedTax.Id == valueAddedTax.Id).Count() > 0)
            {
                throw new Exception(String.Format("Невозможно {0} ставку НДС, т.к. существуют позиции приходных накладных с такой ставкой НДС.", modifyName));
            }

            if (baseDictionaryRepository.Query<ExpenditureWaybillRow>().Where(x => x.ValueAddedTax.Id == valueAddedTax.Id).Count() > 0)
            {
                throw new Exception(String.Format("Невозможно {0} ставку НДС, т.к. существуют позиции накладных реализации с такой ставкой НДС.", modifyName));
            }

            if (baseDictionaryRepository.Query<MovementWaybillRow>().Where(x => x.ValueAddedTax.Id == valueAddedTax.Id).Count() > 0)
            {
                throw new Exception(String.Format("Невозможно {0} ставку НДС, т.к. существуют позиции накладных перемещения с такой ставкой НДС.", modifyName));
            }

            if (baseDictionaryRepository.Query<ChangeOwnerWaybillRow>().Where(x => x.ValueAddedTax.Id == valueAddedTax.Id).Count() > 0)
            {
                throw new Exception(String.Format("Невозможно {0} ставку НДС, т.к. существуют позиции накладных смены собственника с такой ставкой НДС.", modifyName));
            }
        }

        #endregion
    }
}
