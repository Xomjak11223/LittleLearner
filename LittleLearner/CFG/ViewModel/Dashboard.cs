using CfgCompLib.classes;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CfgCompLib;

namespace LittleLearner.CFG.ViewModel
{
    public partial class Dashboard : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<(Node, Node)> mcsNodes; // (NodesFromCode, NodesFromGraph)

        [ObservableProperty]
        public ObservableCollection<SimilarLabels> similarLabels;

        [ObservableProperty]
        public ObservableCollection<string> editSteps;

        [ObservableProperty]
        public ObservableCollection<NodeViewModel> codeGraphNodes;

        [ObservableProperty]
        public ObservableCollection<NodeViewModel> flowchartGraphNodes;

        [ObservableProperty]
        public double costNodeInsert;

        [ObservableProperty]
        public double costNodeDelete;

        [ObservableProperty]
        public double costNodeRelabel;

        [ObservableProperty]
        public double costEdgeInsert;

        [ObservableProperty]
        public double costEdgeDelete;

        [ObservableProperty]
        public double totalCost;

        [ObservableProperty]
        public double maxPoints;

        [ObservableProperty]
        public double receivedPoints;

        [ObservableProperty]
        public double similarityPercent;

        public Dashboard()
        {
            mcsNodes = new ObservableCollection<(Node, Node)>();
            editSteps = new ObservableCollection<string>();
            similarLabels = new ObservableCollection<SimilarLabels>();
            codeGraphNodes = new ObservableCollection<NodeViewModel>();
            flowchartGraphNodes = new ObservableCollection<NodeViewModel>();
        }

        public void UpdateViewModel(Graph codeGraph, Graph flowchartGraph, double equalThreshold)
        {
            if (equalThreshold <= 0 || equalThreshold > 1) { throw new ArgumentException("equalTreshold is not in range [0, 1]"); }
            if (codeGraph == null || flowchartGraph == null) { throw new ArgumentNullException("The input fields of DashboardViewModel.UpdateViewModel() should not be null"); }

            codeGraph = GraphUtils.ExpandToMaxGraph(codeGraph);
            flowchartGraph = GraphUtils.ExpandToMaxGraph(flowchartGraph);
            codeGraph.Description = "Control Flow Graph";
            flowchartGraph.Description = "Flow Chart";

            HashSet<(Node, Node)> mcs = GraphUtils.FindMCCS(codeGraph, flowchartGraph);
            HashSet<string> editSteps = [];
            var (totalCosts, splitCosts) = GraphUtils.CalculateGED(codeGraph, flowchartGraph, out editSteps);

            CodeGraphNodes.Clear();
            foreach (var nodePair in codeGraph.GetNodes())
            {
                NodeViewModel newNode = new NodeViewModel(nodePair.Value);
                newNode.OutgoingNodesString = "";
                newNode.IngoingNodesString = "";
                newNode.Title = "";

                foreach (Node succeessor in newNode.GetSuccessors()) newNode.OutgoingNodesString += $"{succeessor.Id}, ";
                if (newNode.OutDegree > 0) newNode.OutgoingNodesString = newNode.OutgoingNodesString.Remove(newNode.OutgoingNodesString.Length - 2);

                foreach (Node predecessors in newNode.GetPredecessors()) newNode.IngoingNodesString += $"{predecessors.Id}, ";
                if (newNode.InDegree > 0) newNode.IngoingNodesString = newNode.IngoingNodesString.Remove(newNode.IngoingNodesString.Length - 2);

                newNode.Title = newNode.LabelToString();

                CodeGraphNodes.Add(newNode);
            }

            FlowchartGraphNodes.Clear();
            foreach (var nodePair in flowchartGraph.GetNodes())
            {
                NodeViewModel newNode = new NodeViewModel(nodePair.Value);
                newNode.OutgoingNodesString = "";
                newNode.IngoingNodesString = "";
                newNode.Title = "";

                foreach (Node succeessor in newNode.GetSuccessors()) newNode.OutgoingNodesString += $"{succeessor.Id}, ";
                if (newNode.OutDegree > 0) newNode.OutgoingNodesString = newNode.OutgoingNodesString.Remove(newNode.OutgoingNodesString.Length - 2);

                foreach (Node predecessors in newNode.GetPredecessors()) newNode.IngoingNodesString += $"{predecessors.Id}, ";
                if (newNode.InDegree > 0) newNode.IngoingNodesString = newNode.IngoingNodesString.Remove(newNode.IngoingNodesString.Length - 2);

                newNode.Title = newNode.LabelToString();

                FlowchartGraphNodes.Add(newNode);
            }

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
                        if (TotalEQ >= equalThreshold * 100) SimilarLabels.Add(new SimilarLabels(nodeCfg.GetLabel()[0], nodeFc.GetLabel()[0], TotalEQ));
                    }
                }
            }

            foreach (var edit in editSteps) EditSteps.Add(edit);

            CostNodeInsert = splitCosts.CostsNodeInsert;
            CostNodeDelete = splitCosts.CostsNodeDelete;
            CostNodeRelabel = splitCosts.CostsNodeRelabel;
            CostEdgeInsert = splitCosts.CostsEdgeInsert;
            CostEdgeDelete = splitCosts.CostsEdgeDelete;
            TotalCost = totalCosts;

            MaxPoints = 2 * codeGraph.NodeCount + codeGraph.EdgeCount;
            ReceivedPoints = MaxPoints - totalCosts;
            SimilarityPercent = ReceivedPoints * 100 / MaxPoints;
        }
    }
}
