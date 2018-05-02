using System.Collections.Generic;

namespace ERP.UI.ViewModels.TreeView
{
    /// <summary>
    /// Данные для элемента управления Tree
    /// </summary>
    public class TreeData
    {
        public IList<TreeNode> Nodes { get; protected set; }

        public string ValueToSelect { get; set; }

        public TreeData()
        {
            Nodes = new List<TreeNode>();
        }
    }
}
