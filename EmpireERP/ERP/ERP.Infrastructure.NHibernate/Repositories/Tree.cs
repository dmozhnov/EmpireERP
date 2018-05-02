using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Utils;

namespace ERP.Infrastructure.NHibernate.Repositories
{
    public class Tree
    {
        #region Поля

        /// <summary>
        /// Корень дерева
        /// </summary>
        private TreeNode root = new TreeNode("");

        /// <summary>
        /// Корневые вершины
        /// </summary>
        public IEnumerable<TreeNode> Nodes
        {
            get
            {
                return root.Nodes;
            }
        }

        #endregion

        #region Конструктор
        #endregion

        #region Методы

        /// <summary>
        /// получение дочерней вершины по имени
        /// </summary>
        /// <param name="root">Родительский узел для искомой вершины</param>
        /// <param name="name">Имя искомой вершины</param>
        /// <returns></returns>
        private TreeNode FindNode(TreeNode root, string name)
        {
            TreeNode result = null;

            foreach (var node in root.Nodes)
            {
                if (node.Name == name)
                {
                    result = node;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Парсинг параметра в дерево
        /// </summary>
        /// <param name="item">Параметр</param>
        /// <param name="separator">Символ разделитель элементов строки</param>
        /// <returns>Узел дерева, соответствующий последнему элементу дерева</returns>
        private TreeNode ParseItem(ParameterStringItem item, char separator)
        {
            var vals = new List<string>(item.Key.Split(separator, '.'));
            var currentNode = root;

            currentNode = ParseItem(item, root, vals);

            return currentNode;
        }

        /// <summary>
        /// Парсинг параметра в дерево
        /// </summary>
        /// <param name="item">Параметр</param>
        /// <param name="separator">Символ разделитель элементов строки</param>
        /// <returns>Узел дерева, соответствующий последнему элементу дерева</returns>
        private TreeNode ParseItem(ParameterStringItem item, TreeNode root, IList<string> vals)
        {
            var val = vals[0];
            vals.RemoveAt(0);

            var currentNode = FindNode(root, val);
            if (currentNode == null)
            {
                if (vals.Count == 0)
                {
                    currentNode = root.Add(val, item);
                }
                else
                {
                    currentNode = root.Add(val, null);
                }
            }
            else
            {
                if (vals.Count == 0)
                {
                    currentNode.Value = item;
                }
            }

            if (vals.Count > 0)
            {
                currentNode = ParseItem(item, currentNode, vals);
            }

            return currentNode;
            
        }

        /// <summary>
        /// Парсинг параметров в дерево
        /// </summary>
        /// <param name="paramStr">парметры</param>
        /// <param name="separator">Символ разделитель элементов строки</param>
        public void Parse(ParameterString paramStr, char separator)
        {
            foreach (var key in paramStr.Keys)
            {
                ParseItem(paramStr[key], separator);
            }
        }

        #endregion
    }
}
