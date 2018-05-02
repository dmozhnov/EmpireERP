using System;
using System.Linq;
using ERP.Infrastructure.Entities;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities.Security
{
    /// <summary>
    /// Распространение права роли
    /// </summary>
    public class PermissionDistribution : Entity<int>
    {
        #region Свойства

        /// <summary>
        /// Роль пользователя
        /// </summary>
        public virtual Role Role { get; protected internal set; }

        /// <summary>
        /// Право
        /// </summary>
        public virtual Permission Permission { get; protected set; }

        /// <summary>
        /// Тип распространения права
        /// </summary>
        public virtual PermissionDistributionType Type { get; set; }

        #endregion

        #region Конструкторы

        protected PermissionDistribution()
        {
        }

        public PermissionDistribution(Permission permission, PermissionDistributionType type)
        {
            if (!PermissionDetailsSet.PermissionDetails.Where(x => x.Permission == permission).First().AvailableDistributionTypes.Contains(type))
            {
                throw new Exception(String.Format("Право «{0}» не может иметь распространение «{1}».",
                    permission.GetDisplayName(), type.GetDisplayName()));
            }
            
            Permission = permission;
            Type = type;
        }

        #endregion

        #region Методы

        #endregion
    }
}
