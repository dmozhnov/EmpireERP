using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.AbstractApplicationServices
{
    public interface ILegalFormService : IBaseDictionaryService<LegalForm>
    {
        /// <summary>
        /// Возможно ли редактировать организационно-правовую форму
        /// </summary>
        /// <param name="legalForm">организационно-правовая форма</param>
        /// <param name="user">Пользователь</param>
        /// <param name="checkLogic">Проверять ли логику</param>
        /// <returns></returns>
        bool IsPossibilityToEdit(LegalForm legalForm, User user, bool checkLogic = true);

        /// <summary>
        /// Получить список ОПФ с типом - юридическое лицо
        /// </summary>
        /// <returns></returns>
        IEnumerable<LegalForm> GetJuridicalLegalForms();

        /// <summary>
        /// Получить список ОПФ с типом - физическое лицо
        /// </summary>
        /// <returns></returns>
        IEnumerable<LegalForm> GetPhysicalLegalForms();
    }
}
