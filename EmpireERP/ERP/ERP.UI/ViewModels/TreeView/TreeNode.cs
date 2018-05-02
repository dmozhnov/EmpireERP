using System.Collections.Generic;

namespace ERP.UI.ViewModels.TreeView
{
    /// <summary>
    /// Узел дерева
    /// </summary>
    public class TreeNode
    {
        /// <summary>
        /// Значение элемента узла
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Текст узла
        /// </summary>
        public string Text { get; protected set; }

        /// <summary>
        /// Адрес узла
        /// Если равен "", то элемент дерева будет не ссылкой, а простым текстом
        /// </summary>
        //public string Url { get; protected set; }

        /// <summary>
        /// Родительский узел
        /// </summary>
        public TreeNode ParentNode { get; protected set; }

        /// <summary>
        /// Перечень дочерних узлов
        /// </summary>
        public IList<TreeNode> ChildNodes { get; protected set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="text">Текст узла</param>
        /// <param name="url">Адрес узла</param>
        public TreeNode(string text,/* string url,*/ string value, TreeNode parentNode)
        {
            Value = value;
            Text = text;
            //Url = url;
            ChildNodes = new List<TreeNode>();
            ParentNode = parentNode;
        }
    }
}
