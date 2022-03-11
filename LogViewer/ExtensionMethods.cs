using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LogViewer
{
    public static class ExtensionMethods
    {
        internal static IEnumerable<TreeNode> Descendants(this TreeNodeCollection c)
        {
            foreach (var node in c.OfType<TreeNode>())
            {
                yield return node;

                foreach (var child in node.Nodes.Descendants())
                {
                    yield return child;
                }
            }
        }
    }
}
