using ScottPlot.Plottable;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LogViewer
{
    public partial class Main : Form
    {
        ILogHelper logHelper;
        private Dictionary<ParameterNode, SignalPlotXY> selectedPlots = new();
        public Main()
        {
            InitializeComponent();
            logHelper = new LogHelper();
            //logHelper.LoadLog(@"C:\Projects\RC\APM\LOGS\00000005.BIN");
            logHelper.LoadLog(@"C:\Projects\RC\gragon\LOGS\00000019.BIN");

            IEnumerable<ParameterNode> parameters = logHelper.GetParametersTree();
            IEnumerable<ControllerMessage> messages = logHelper.GetMessages();
            FillMessages(messages);
            FillParametersTree(parameters);

            mainPlot.Plot.XAxis.TickLabelFormat("HH:mm:ss", dateTimeFormat: true);
        }

        private void FillMessages(IEnumerable<ControllerMessage> messages)
        {
            foreach (ControllerMessage item in messages)
            {
                messagesListView.Items.Add(new ListViewItem(new string[] { item.Message, item.MessageTime.ToString("HH:mm:ss") }));
            }
        }

        private void FillParametersTree(IEnumerable<ParameterNode> parameters)
        {
            foreach (var item in parameters)
            {
                parametersTreeView.Nodes.Add(CreateTreeNodesRecursively(item));
            }
        }

        private TreeNode CreateTreeNodesRecursively(ParameterNode parameter)
        {
            TreeNode node = new TreeNode(parameter.Name)
            {
                Tag = parameter
            };
            foreach (var item in parameter.Nodes)
            {
                node.Nodes.Add(CreateTreeNodesRecursively(item));
            }
            return node;
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            var selectedNodes = parametersTreeView.Nodes.Descendants()
                    .Where(n => n.Checked)
                    .Select(n => n.Text)
                    .ToList();

            var parameter = e.Node.Tag as ParameterNode;

            if (parameter.Type != null)
            {
                if (!selectedPlots.ContainsKey(parameter))
                {
                    var curr = logHelper.GetData(parameter.Type, parameter.Name, parameter.Instance);
                    SignalPlotXY result = mainPlot.Plot.AddSignalXY(curr.GetTimes(), curr.GetValues());
                    selectedPlots.Add(parameter, result);
                    
                    if (selectedPlots.Count() > 1)
                    {
                        int axisIndex = selectedPlots.Count() + 2;
                        var yAxis3 = mainPlot.Plot.AddAxis(ScottPlot.Renderable.Edge.Left, axisIndex: axisIndex);
                        result.YAxisIndex = axisIndex;
                        yAxis3.Color(result.Color);
                        yAxis3.Label(parameter.Name);
                    }
                }
                else
                {
                    SignalPlotXY plottable = selectedPlots.GetValueOrDefault(parameter);
                    if (plottable != null)
                    {
                        
                        var axis = mainPlot.Plot.GetSettings().Axes.Where(a => a.AxisIndex == plottable.YAxisIndex).FirstOrDefault();
                        if (axis != null) mainPlot.Plot.GetSettings().Axes.Remove(axis);
                    }
                    mainPlot.Plot.Remove(plottable);
                    selectedPlots.Remove(parameter);
                }
            }
        }
    }
}
