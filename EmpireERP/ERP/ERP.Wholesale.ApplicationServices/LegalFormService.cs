using System;
using System.Collections.Generic;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    /// <summary>
    /// Сервис для сущностей «Организационно-правовая форма»
    /// </summary>
    public class LegalFormService : BaseDictionaryService<LegalForm>, ILegalFormService
    {
        #region Свойства

        protected override string UniquenessErrorString
        {
            get { return "Организационно-правовая форма с таким наименованием уже существует."; }
        }

        protected override string CheckExistenceErrorString
        {
            get { return "Организационно-правовая форма не найдена. Возможно, она была удалена."; }
        }

        #region Права на операции

        public override Permission CreationPermission
        {
            get { return Permission.LegalForm_Create; }
        }

        public override Permission EditingPermission
        {
            get { return Permission.LegalForm_Edit; }
        }

        public override Permission DeletionPermission
        {
            get { return Permission.LegalForm_Delete; }
        }

        public override Permission ListViewingPermission
        {
            get { return Permission.Client_List_Details; }
        }

        #endregion

        #endregion

        #region Конструктор

        public LegalFormService(IBaseDictionaryRepository<LegalForm> baseDictionaryRepository)
            : base(baseDictionaryRepository)
        {
        }

        #endregion

        #region Методы

        #region Права

        protected override void CheckPossibilityToDelete(LegalForm legalForm, User user, bool checkLogic = true)
        {
            CheckPermissionToPerformOperation(user, DeletionPermission);

            if (checkLogic)
            {
                CheckPossibilityToModify(legalForm, "удалить");
            }
        }

        private void CheckPossibilityToEdit(LegalForm legalForm, User user, bool checkLogic = true)
        {
            CheckPermissionToPerformOperation(user, EditingPermission);

            if (checkLogic)
            {
                CheckPossibilityToModify(legalForm, "изменить");
            }
        }

        /// <summary>
        /// Возможно ли редактировать организационно-правовую форму
        /// </summary>
        /// <param name="legalForm">организационно-правовая форма</param>
        /// <param name="user">Пользователь</param>
        /// <param name="checkLogic">Проверять ли логику</param>
        /// <returns></returns>
        public bool IsPossibilityToEdit(LegalForm legalForm, User user, bool checkLogic = true)
        {
            try
            {
                CheckPossibilityToEdit(legalForm, user, checkLogic);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void CheckPossibilityToModify(LegalForm legalForm, string modifyName)
        {
            if (baseDictionaryRepository.Query<EconomicAgent>().Where(x => x.LegalForm.Id == legalForm.Id).Count() > 0)
            {
                throw new Exception(String.Format("Невозможно {0} организационно-правовую форму, т.к. существуют экономические агенты такой формы.", modifyName));
            }
        }

        #endregion

        /// <summary>
        /// Получить список ОПФ с типом - юридическое лицо
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LegalForm> GetJuridicalLegalForms()
        {
            return baseDictionaryRepository.Query<LegalForm>().Where(x => x.EconomicAgentType == EconomicAgentType.JuridicalPerson)
                .ToList<LegalForm>();
        }

        /// <summary>
        /// Получить список ОПФ с типом - физическое лицо
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LegalForm> GetPhysicalLegalForms()
        {
            return baseDictionaryRepository.Query<LegalForm>().Where(x => x.EconomicAgentType == EconomicAgentType.PhysicalPerson)
                .ToList<LegalForm>();
        }

        #endregion

    }
}
