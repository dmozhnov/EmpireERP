using System;
using System.Linq;
using System.Collections.Generic;
using ERP.Utils;
using ERP.Wholesale.AbstractApplicationServices;
using ERP.Wholesale.Domain.Entities;
using ERP.Wholesale.Domain.Entities.Security;
using ERP.Wholesale.Domain.Repositories;

namespace ERP.Wholesale.ApplicationServices
{
    public class DealQuotaService : IDealQuotaService
    {
        #region Поля

        private readonly IDealQuotaRepository dealQuotaRepository;

        #endregion

        #region Конструкторы

        public DealQuotaService(IDealQuotaRepository dealQuotaRepository)
        {
            this.dealQuotaRepository = dealQuotaRepository;
        }

        #endregion

        #region Методы

        #region Получение одной квоты

        private DealQuota GetById(int id, User user)
        {
            var type = user.GetPermissionDistributionType(Permission.DealQuota_List_Details);

            //// если права нет - то сразу возвращаем null
            if (type == PermissionDistributionType.None)
            {
                return null;
            }
            else
            {
                return dealQuotaRepository.GetById(id);
            }
        }


        /// <summary>
        /// Получение сделки по id с проверкой ее существования
        /// </summary>
        /// <param name="id">Код сделки</param>
        /// <returns>Сделка</returns>
        public DealQuota CheckDealQuotaExistence(int id, User user, string message = "")
        {
            var dealQuota = GetById(id, user);

            ValidationUtils.NotNull(dealQuota, String.IsNullOrEmpty(message) ? "Квота не найдена. Возможно, она была удалена." : message);

            return dealQuota;
        }

        #endregion

        #region Список

        public IEnumerable<DealQuota> GetFilteredList(object state, ParameterString param, User user)
        {
            if (param == null)
            {
                param = new ParameterString("");
            }

            switch (user.GetPermissionDistributionType(Permission.DealQuota_List_Details))
            {
                case PermissionDistributionType.None:
                    return new List<DealQuota>();                

                case PermissionDistributionType.All:
                    break;
            }

            return dealQuotaRepository.GetFilteredList(state, param);
        }

        public IEnumerable<DealQuota> FilterByUser(IEnumerable<DealQuota> list, User user, Permission permission)
        {
            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    return new List<DealQuota>();
                
                case PermissionDistributionType.All:
                    return list;

                default:
                    return null;
            }
        }

        public IEnumerable<DealQuota> GetActiveDealQuotasList(User user)
        {
            CheckPermissionToPerformOperation(user, Permission.DealQuota_List_Details);

            return dealQuotaRepository.Query<DealQuota>().Where(x => x.EndDate == null).ToList<DealQuota>();
        }

        #endregion

        public void Save(DealQuota dealQuota, User user)
        {
            CheckDealQuotaNameUniqueness(dealQuota);

            dealQuotaRepository.Save(dealQuota);
        }

        /// <summary>
        /// Проверка имени квоты на уникальность
        /// </summary>
        /// <param name="deal">Квота</param>
        public void CheckDealQuotaNameUniqueness(DealQuota dealQuota)
        {
            var isUnique = dealQuotaRepository.Query<DealQuota>().Where(x => x.Name == dealQuota.Name && x.Id != dealQuota.Id).Count() == 0;
            if (!isUnique)
            {
                throw new Exception(String.Format("Квота с названием «{0}» уже существует.", dealQuota.Name));
            }
        }

        #region Удаление квоты

        public void Delete(DealQuota quota, User user)
        {
            CheckPossibilityToDelete(quota, user);

            RemoveDealQuotaFromAllDeals(quota);

            dealQuotaRepository.Delete(quota);
        }

        #endregion

        #region Удаление квоты из всех сделок

        public void RemoveDealQuotaFromAllDeals(DealQuota quota)
        {
            var dealsQuery = dealQuotaRepository.Query<Deal>();
            dealsQuery.Restriction<DealQuota>(x => x.Quotas).Where(x => x.Id == quota.Id);
            var deals = dealsQuery.ToList<Deal>();

            foreach (var deal in deals)
            {
                deal.RemoveQuota(quota, false);
            }
        }

        #endregion        

        #region Вспомогательные методы

        private bool IsPermissionToPerformOperation(User user, Permission permission)
        {
            bool result = false;

            switch (user.GetPermissionDistributionType(permission))
            {
                case PermissionDistributionType.None:
                    result = false;
                    break;

                case PermissionDistributionType.All:
                    result = true;
                    break;
            }

            return result;
        }

        private void CheckPermissionToPerformOperation(User user, Permission permission)
        {
            if (!IsPermissionToPerformOperation(user, permission))
            {
                throw new Exception(String.Format("Недостаточно прав для выполнения операции «{0}».", permission.GetDisplayName()));
            }
        }

        /// <summary>
        /// Проверка наличия накладных реализации по квоте
        /// </summary>
        /// <param name="quota">Квота</param>
        /// <returns><value>true</value>, если существует хотя бы одна накладная реализации; <value>false</value> иначе</returns>
        private bool AreDealQuotaSales(DealQuota quota)
        {            
            return dealQuotaRepository.Query<SaleWaybill>().Where(x => x.Quota.Id == quota.Id).Count() > 0;
        }


        public bool IsPossibilityToCreate(User user)
        {
            try
            {
                CheckPossibilityToCreate(user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsPossibilityToEdit(DealQuota dealQuota, User user, bool checkLogic = true)
        {
            try
            {
                CheckPossibilityToEdit(dealQuota, user, checkLogic);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsPossibilityToDelete(DealQuota dealQuota, User user)
        {
            try
            {
                CheckPossibilityToDelete(dealQuota, user);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CheckPossibilityToCreate(User user)
        {
            CheckPermissionToPerformOperation(user, Permission.DealQuota_Create);
        }

        public void CheckPossibilityToEdit(DealQuota dealQuota, User user, bool checkLogic = true)
        {
            CheckPermissionToPerformOperation(user, Permission.DealQuota_Edit);

            if (checkLogic)
            {
                ValidationUtils.Assert(!AreDealQuotaSales(dealQuota), "Невозможно изменить квоту, по которой имеются накладные реализации.");
            }
        }

        public void CheckPossibilityToDelete(DealQuota dealQuota, User user)
        {
            CheckPermissionToPerformOperation(user, Permission.DealQuota_Delete);

            ValidationUtils.Assert(!AreDealQuotaSales(dealQuota), "Невозможно удалить квоту, по которой имеются накладные реализации.");
        }

        #endregion

        #endregion
    }
}