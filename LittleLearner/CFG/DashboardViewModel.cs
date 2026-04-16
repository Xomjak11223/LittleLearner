using CfgCompLib.classes;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CfgCompLib;

namespace LittleLearner.CFG
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<(Node, Node)> mcsNodes; // (NodesFromCode, NodesFromGraph)

        [ObservableProperty]
        public ObservableCollection<(double, string, string)> similarLabels; // (similarity in %, codeLabel, graphLabel)

        [ObservableProperty]
        public ObservableCollection<string> editSteps;

        [ObservableProperty]
        public Graph codeGraph;

        [ObservableProperty]
        public Graph flowchartGraph;

        [ObservableProperty]
        public double costsNodeInsert;

        [ObservableProperty]
        public double costsNodeDelete;

        [ObservableProperty]
        public double costsNodeRelabel;

        [ObservableProperty]
        public double costsEdgeInsert;

        [ObservableProperty]
        public double costsEdgeDelete;

        [ObservableProperty]
        public double totalCost;

        [ObservableProperty]
        public double maxPoints;

        [ObservableProperty]
        public double receivedPoints;

        [ObservableProperty]
        public double similarityPercent;

        public DashboardViewModel()
        {
            mcsNodes = new ObservableCollection<(Node, Node)>();
            editSteps = new ObservableCollection<string>();
            similarLabels = new ObservableCollection<(double, string, string)>();
            codeGraph = new Graph();
            flowchartGraph = new Graph();
        }

        public void UpdateViewModel(Graph codeGraph, Graph flowchartGraph, double equalThreshold)
        {
            SimilarityPercent = 100;
            return;
            if (equalThreshold < 0 || equalThreshold > 1) { throw new ArgumentException("equalTreshold is not in range [0, 1]"); }
            if (codeGraph == null || flowchartGraph == null) { throw new ArgumentNullException("The input fields of DashboardViewModel.UpdateViewModel() should not be null"); }

            codeGraph = GraphUtils.ExpandToMaxGraph(codeGraph);
            flowchartGraph = GraphUtils.ExpandToMaxGraph(flowchartGraph);
            codeGraph.Description = "Control Flow Graph";
            flowchartGraph.Description = "Flow Chart";

            HashSet<(Node, Node)> mcs = GraphUtils.FindMCCS(codeGraph, flowchartGraph);
            HashSet<string> editSteps = [];
            var (totalCosts, splitCosts) = GraphUtils.CalculateGED(codeGraph, flowchartGraph, out editSteps);

            CodeGraph = codeGraph;
            FlowchartGraph = flowchartGraph;

            McsNodes.Clear();
            foreach ((Node, Node) nodePair in mcs){ McsNodes.Add(nodePair); }

            SimilarLabels.Clear();
            if(flowchartGraph.NodeCount != 0)
            {
                foreach (Node? nodeCfg in codeGraph.GetNodes().Values)
                {
                    foreach (Node? nodeFc in flowchartGraph.GetNodes().Values)
                    {
                        var (TotalEQ, _, _, _) = GraphUtils.CalculateLabelEquality(nodeFc.GetLabel()[0], nodeCfg.GetLabel()[0]);
                        if (TotalEQ >= equalThreshold * 100) SimilarLabels.Add((TotalEQ, nodeCfg.GetLabel()[0], nodeFc.GetLabel()[0]));
                    }
                }
            }

            EditSteps.Clear();
            foreach (var edit in editSteps) EditSteps.Add(edit);

            CostsNodeInsert = splitCosts.CostsNodeInsert;
            CostsNodeDelete = splitCosts.CostsNodeDelete;
            CostsNodeRelabel = splitCosts.CostsNodeRelabel;
            CostsEdgeInsert = splitCosts.CostsEdgeInsert;
            CostsEdgeDelete = splitCosts.CostsEdgeDelete;
            TotalCost = totalCosts;

            MaxPoints = 2 * codeGraph.NodeCount + codeGraph.EdgeCount;
            ReceivedPoints = MaxPoints - totalCosts;
            SimilarityPercent = ReceivedPoints * 100 / MaxPoints;
        }
    }
}
