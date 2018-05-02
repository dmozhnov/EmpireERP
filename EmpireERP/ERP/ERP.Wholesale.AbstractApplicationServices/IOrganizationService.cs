using ERP.Wholesale.Domain.Entities;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface IOrganizationService
    {
        /// <summary>
        /// Проверка возможности удаления расчетного счета
        /// </summary>
        /// <param name="bankAccount">Расчетный счет</param>
        void CheckBankAccountDeletionPossibility(RussianBankAccount bankAccount);
        void CheckBankAccountDeletionPossibility(ForeignBankAccount bankAccount);

        void CheckBankAccountUniqueness(ForeignBankAccount bankAccount);
        
        /// <summary>
        /// Проверка расчетного счета на уникальность
        /// </summary>
        /// <param name="bankAccount">Расчетный счет</param>
        void CheckBankAccountUniqueness(RussianBankAccount bankAccount);

        /// <summary>
        /// Проверка организации на уникальность
        /// </summary>
        /// <param name="organization"></param>
        void CheckOrganizationUniqueness<T>(T organization) where T : Organization;
    }
}
