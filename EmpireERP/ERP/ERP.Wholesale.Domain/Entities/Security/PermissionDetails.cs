using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Utils;

namespace ERP.Wholesale.Domain.Entities.Security
{
    /// <summary>
    /// Детали права роли (без сохранения в БД)
    /// </summary>
    public class PermissionDetails
    {
        #region Свойства

        /// <summary>
        /// Право
        /// </summary>
        public Permission Permission { get; protected set; }

        /// <summary>
        /// Коллекция доминирующих прав с прямой зависимостью
        /// </summary>
        public IEnumerable<PermissionDetails> ParentDirectRelations
        { 
            get { return parentDirectRelations; } 
        }
        public List<PermissionDetails> parentDirectRelations;

        /// <summary>
        /// Коллекция доминирующих прав с обратной зависимостью
        /// </summary>
        public IEnumerable<PermissionDetails> ParentInverseRelations { get; protected set; }

        /// <summary>
        /// Коллекция зависимых прав с прямой зависимостью
        /// </summary>
        public IEnumerable<PermissionDetails> ChildDirectRelations 
        {
            get { return childDirectRelations; } 
        }
        private List<PermissionDetails> childDirectRelations;

        /// <summary>
        /// Коллекция зависимых прав с обратной зависимостью
        /// </summary>
        public IEnumerable<PermissionDetails> ChildInverseRelations { get; protected set; }
        
        /// <summary>
        /// Доступные типы распространения права
        /// </summary>
        public IEnumerable<PermissionDistributionType> AvailableDistributionTypes { get; protected set; }

        #endregion

        #region Конструкторы

        public PermissionDetails(Permission permission, IEnumerable<PermissionDistributionType> availableDistributionTypes)
        {
            Permission = permission;
            AvailableDistributionTypes = availableDistributionTypes;
            parentDirectRelations = new List<PermissionDetails>();
            ParentInverseRelations = new List<PermissionDetails>();
            childDirectRelations = new List<PermissionDetails>();
            ChildInverseRelations = new List<PermissionDetails>();
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавление зависимого права с прямой зависимостью
        /// </summary>
        /// <param name="permissionDetails"></param>
        public void AddChildDirectRelation(PermissionDetails permissionDetails)
        {
            if (childDirectRelations.Contains(permissionDetails))
            {
                throw new Exception(string.Format("Право «{0}» уже имеет зависимое право «{1}» с прямой зависимостью.", this.Permission.GetDisplayName(), permissionDetails.Permission.GetDisplayName()));
            }
            
            childDirectRelations.Add(permissionDetails);
            permissionDetails.parentDirectRelations.Add(this);
        }

        /// <summary>
        /// Добавление доминирующего права с прямой зависимостью
        /// </summary>
        /// <param name="permissionDetails"></param>
        public void AddParentDirectRelation(PermissionDetails permissionDetails)
        {
            if (parentDirectRelations.Contains(permissionDetails))
            {
                throw new Exception(string.Format("Право «{0}» уже имеет доминирующее право «{1}» с прямой зависимостью.", this.Permission.GetDisplayName(), permissionDetails.Permission.GetDisplayName()));
            }
            
            parentDirectRelations.Add(permissionDetails);
            permissionDetails.childDirectRelations.Add(this);
        }

        /// <summary>
        /// Получение коллекции зависимых права с прямой зависимостью в виде строки
        /// </summary>
        /// <returns></returns>
        public string GetChildDirectRelationString()
        {
            string s = "";

            foreach (var item in childDirectRelations)
            {
                if(s != "")
                {
                    s += ",";
                }

                s += item.Permission.ToString();
            }

            return s;
        }

        /// <summary>
        /// Получение значений доступных типов распространения в виде строки
        /// </summary>
        /// <returns></returns>
        public string GetPossibleValuesString()
        {
            string s = "";

            s += (AvailableDistributionTypes.Contains(PermissionDistributionType.None) ? "1" : "0");
            s += (AvailableDistributionTypes.Contains(PermissionDistributionType.Personal) ? "1" : "0");
            s += (AvailableDistributionTypes.Contains(PermissionDistributionType.Teams) ? "1" : "0");
            s += (AvailableDistributionTypes.Contains(PermissionDistributionType.All) ? "1" : "0");

            return s;
        }

        #endregion

        
        

        
    }
}
