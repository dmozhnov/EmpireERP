using System.Collections.Generic;
using ERP.Wholesale.Domain.Entities.Security;

namespace ERP.Wholesale.Domain.AbstractServices
{
    public interface IPermissionDistributionService
    {
        /// <summary>
        /// Получить минимальное распространение для списка прав
        /// </summary>
        /// <param name="permissionList">Список прав</param>
        /// <param name="user">Пользователь</param>
        PermissionDistributionType GetMinPermission(IEnumerable<Permission> permissionList, User user);

        /// <summary>
        /// Получить максимальное распространение для списка прав
        /// </summary>
        /// <param name="permissionList">Список прав</param>
        /// <param name="user">Пользователь</param>
        PermissionDistributionType GetMaxPermission(IEnumerable<Permission> permissionList, User user);
    }
}
