using System.Collections.Generic;
using System.Linq;
using ERP.Wholesale.Domain.AbstractServices;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.Services
{
    public class PermissionDistributionService : IPermissionDistributionService
    {
        #region Поля

        #endregion

        #region Конструктор

        public PermissionDistributionService()
        {
        }
        
        #endregion

        #region Методы

        /// <summary>
        /// Получить минимальное распространение для списка прав
        /// </summary>
        /// <param name="permissionList">Список прав</param>
        /// <param name="user">Пользователь</param>
        public PermissionDistributionType GetMinPermission(IEnumerable<Permission> permissionList, User user)
        {
            if (!permissionList.Any())
            {
                return PermissionDistributionType.None;
            }

            var permissionDistributionMin = PermissionDistributionType.All;
            foreach (var permission in permissionList)
            {
                var permissionDistribution = user.GetPermissionDistributionType(permission);
                permissionDistributionMin = permissionDistribution < permissionDistributionMin ? permissionDistribution : permissionDistributionMin;
            }

            return permissionDistributionMin;
        }

        /// <summary>
        /// Получить максимальное распространение для списка прав
        /// </summary>
        /// <param name="permissionList">Список прав</param>
        /// <param name="user">Пользователь</param>
        public PermissionDistributionType GetMaxPermission(IEnumerable<Permission> permissionList, User user)
        {
            if (!permissionList.Any())
            {
                return PermissionDistributionType.None;
            }

            var permissionDistributionMax = PermissionDistributionType.None;
            foreach (var permission in permissionList)
            {
                var permissionDistribution = user.GetPermissionDistributionType(permission);
                permissionDistributionMax = permissionDistribution > permissionDistributionMax ? permissionDistribution : permissionDistributionMax;
            }

            return permissionDistributionMax;
        }

        #endregion
    }
}
