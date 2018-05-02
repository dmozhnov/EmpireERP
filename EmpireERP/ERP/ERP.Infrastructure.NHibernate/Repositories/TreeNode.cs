using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Utils;

namespace ERP.Infrastructure.NHibernate.Repositories
{
    public class TreeNode
    {
        #region Поля

        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; private set; }
       

        /// <summary>
        /// Дочерние вершины
        /// </summary>
        public IEnumerable<TreeNode> Nodes
        {
            get
            {
                return childNodes;
            }
        }
	private IList<TreeNode> childNodes = new List<TreeNode>();

        /// <summary>
        /// Значение узла
        /// </summary>
        public ParameterStringItem Value { get; set; }

        #endregion

        #region Конструктор

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">имя узла</param>
        /// <param name="value">значение узла</param>
        public TreeNode(string name, ParameterStringItem value = null)
        {
            Name = name;
            Value = value;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Добавление дочернего узла дерева
        /// </summary>
        /// <param name="name">имя узла</param>
        /// <param name="value">значение узла</param>
        /// <returns>Добавленный узел</returns>
        public TreeNode Add(string name, ParameterStringItem value = null)
        {
            var node = new TreeNode(name, value);
            childNodes.Add(node);

            return node;
        }

        #endregion
    }
}
