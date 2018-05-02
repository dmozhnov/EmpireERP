using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;
using System.Text;

namespace ERP.Wholesale.ApplicationServices
{
    public class ContractorService : IContractorService
    {
        #region Поля

        private readonly IContractorRepository contractorRepository;

        #endregion

        #region Конструктор

        public ContractorService(IContractorRepository contractorRepository)
        {
            this.contractorRepository = contractorRepository;
        }

        #endregion

        #region Методы

        #region Получение по Id

        /// <summary>
        /// Получение контрагента по id с проверкой его существования
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Contractor CheckContractorExistence(int id, string message = "")
        {
            var contractor = contractorRepository.GetById(id);
            ValidationUtils.NotNull(contractor, String.IsNullOrEmpty(message) ? "Контрагент не найден. Возможно, он был удален." : message);

            return contractor;
        }

        /// <summary>
        /// Получение отфильтрованного списка контрагентов
        /// </summary>
        /// <param name="state"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public IEnumerable<Contractor> GetContractorByUser(object state, User user)
        { 
            List<string> types = new List<string>();
            if (user.HasPermission(Permission.Provider_List_Details))
            {
                types.Add(EnumUtils.ValueToString(ContractorType.Provider));
            }
            if (user.HasPermission(Permission.Producer_List_Details))
            {
                types.Add(EnumUtils.ValueToString(ContractorType.Producer));
            }
            if (user.HasPermission(Permission.Client_List_Details))
            {
                types.Add(EnumUtils.ValueToString(ContractorType.Client));
            }

            if (types.Count == 0)   //Прав нет ни на один тип
            {
                return new List<Contractor>();
            }

            var ps = new ParameterString("");
            ps.Add("ContractorType", ParameterStringItem.OperationType.OneOf, types);

            return contractorRepository.GetFilteredList(state, ps);
        }

        #endregion

        #endregion
    }
}
