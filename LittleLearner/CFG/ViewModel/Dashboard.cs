using CfgCompLib;
using CfgCompLib.classes;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace LittleLearner.CFG.ViewModel
{
    public partial class Dashboard : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<EqualNodes> mccsEqualNodes; // (NodesFromCode, NodesFromGraph)

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

        [ObservableProperty]
        public int nodesInCodeGraph;

        [ObservableProperty]
        public int edgesInCodeGraph;

        [ObservableProperty]
        public int nodesInFlowchart;

        [ObservableProperty]
        public int edgesInFlowchart;

        [ObservableProperty]
        public bool canSwapFlowchart;

        public Graph optimalGraph = null;
        public static float widthOptimalNode = 100;
        public static float heightOptimalNode = 50;

        public Dashboard()
        {
            MccsEqualNodes = new ObservableCollection<EqualNodes>();
            editSteps = new ObservableCollection<string>();
            similarLabels = new ObservableCollection<SimilarLabels>();
            codeGraphNodes = new ObservableCollection<NodeViewModel>();
            flowchartGraphNodes = new ObservableCollection<NodeViewModel>();
            CanSwapFlowchart = false;
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

            // creates a Copy of the flowchart Graph
            Graph solutionGraph = new Graph();
            solutionGraph.Description = "Optimal Flow Graph";
            // Creates every Node
            foreach (var nodeIndexPair in flowchartGraph.GetNodes())
            {
                Node node = nodeIndexPair.Value;

                List<string> labels = new List<string>();
                foreach(string label in node.GetLabel()) labels.Add(label);

                solutionGraph.AddNode(new(nodeIndexPair.Key, labels, null, null, new(0, 0, widthOptimalNode, heightOptimalNode, Shape.Action, false)));
            }

            // Connects every Node
            foreach (var nodeIndexPair in flowchartGraph.GetNodes())
            {
                Node node = nodeIndexPair.Value;
                foreach(Node successor in node.GetSuccessors())
                {
                    solutionGraph.GetNode(nodeIndexPair.Key).AddSuccessor(solutionGraph.GetNode(successor.Id));
                }
            }

            // Alters Graph
            foreach (string edit in editSteps)
            {
                if(edit.StartsWith("Insert Edge")) 
                {
                    Match match = Regex.Match(edit, "^Insert Edge from \\[\"(.*)\"\\] to \\[\"(.*)\"\\] in flow chart$");

                    string labelStart = match.Groups[1].Value;
                    string labelEnd = match.Groups[2].Value;

                    foreach (var nodeIndexPare in solutionGraph.GetNodes())
                    {
                        Node node = nodeIndexPare.Value;
                        if (node.GetLabel()[0].Equals(labelStart))
                        {
                            foreach (Node potentialEnd in node.GetSuccessors())
                            {
                                if (potentialEnd.GetLabel()[0].Equals(labelEnd))
                                {
                                    solutionGraph.AddEdge(node, potentialEnd);
                                }
                            }
                        }
                    }
                }
                else if(edit.StartsWith("Insert Node")) 
                {
                    Match specialInsert = Regex.Match(edit, "^Insert Node\\[\"(.*)\"\\]_ID_(\\d+) into flow chart$");
                    if (specialInsert.Success)
                    {
                        string specialLabel = specialInsert.Groups[1].Value;
                        int id = int.Parse(specialInsert.Groups[2].Value);
                        solutionGraph.AddNode(new(id, [specialLabel], null, null, new(0, 0, widthOptimalNode, heightOptimalNode, Shape.Action, false)));
                        continue;
                    }

                    Match match = Regex.Match(edit, "^Insert Node \\[\"(.*)\"\\] into flow chart$");

                    string label = match.Groups[1].Value;
                    solutionGraph.AddNode(new(solutionGraph.GetNewID(), [label], null, null, new(0, 0, widthOptimalNode, heightOptimalNode, Shape.Action, false)));
                }
                else if (edit.StartsWith("Relabel Node")) 
                {
                    Match match = Regex.Match(edit, "Relabel Node \\[\"(.*)\"\\] to \\[\"(.*)\"\\] in flow chart");
                    string labelFrom = match.Groups[1].Value;
                    string labelTo = match.Groups[2].Value;

                    Node[] relabeledNodes = [];
                    foreach (var nodeIndexPare in solutionGraph.GetNodes())
                    {
                        Node node = nodeIndexPare.Value;
                        if (node.GetLabel()[0].Equals(labelFrom))
                        {
                            relabeledNodes = relabeledNodes.Append(new(node.Id, [labelTo], node.GetPredecessors(), node.GetSuccessors(), node.Shape)).ToArray();
                            solutionGraph.RemoveNode(node);
                        }
                    }

                    foreach (Node node in relabeledNodes) solutionGraph.AddNode(node);
                }
                else if(edit.StartsWith("Delete Edge")) 
                {
                    Match match = Regex.Match(edit, "^Delete Edge from \\[\"(.*)\"\\] to \\[\"(.*)\"\\] from flow chart$");

                    string labelStart = match.Groups[1].Value;
                    string labelEnd = match.Groups[2].Value;

                    foreach (var nodeIndexPare in solutionGraph.GetNodes())
                    {
                        Node node = nodeIndexPare.Value;
                        if (node.GetLabel()[0].Equals(labelStart))
                        {
                            foreach (Node potentialEnd in node.GetSuccessors())
                            {
                                if (potentialEnd.GetLabel()[0].Equals(labelEnd))
                                {
                                    solutionGraph.RemoveEdge(node, potentialEnd);
                                }
                            }
                        }
                    }
                }
                else if(edit.StartsWith("Delete Node")) 
                {
                    Match match = Regex.Match(edit, "^Delete Node \\[\"(.*)\"\\] from flow chart$");
                    
                    string label = match.Groups[1].Value;
                    foreach(var nodeIndexPare in solutionGraph.GetNodes())
                    {
                        Node node = nodeIndexPare.Value;
                        if (node.GetLabel()[0].Equals(label)) solutionGraph.RemoveNode(node);
                    }
                }
            }

            // Positoins Nodes in a Grid
            var nodeDictionary = solutionGraph.GetNodes();
            float x = 0;
            float y = 0;
            int direction = 1;
            for(int i = 0; i < nodeDictionary.Count; i++)
            {
                Node node = nodeDictionary[i];
                node.Shape.x = x;
                node.Shape.y = y;

                if ((i+1) % 10 == 0) { x += widthOptimalNode; direction = 1; }
                else if((i+1) % 10 == 5) { x += widthOptimalNode; direction = -1; }

                y += direction * heightOptimalNode;

            }

            optimalGraph = solutionGraph;

            CodeGraphNodes.Clear();
            NodesInCodeGraph = codeGraph.NodeCount;
            EdgesInCodeGraph = codeGraph.EdgeCount;
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
            NodesInFlowchart = flowchartGraph.NodeCount;
            EdgesInFlowchart = flowchartGraph.EdgeCount;
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

            MccsEqualNodes.Clear();
            foreach ((Node, Node) nodePair in mcs) {
                MccsEqualNodes.Add(new($"{nodePair.Item1.LabelToString()} [{nodePair.Item1.Id}]", $"{nodePair.Item2.LabelToString()} [{nodePair.Item2.Id}]"));
            }

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
