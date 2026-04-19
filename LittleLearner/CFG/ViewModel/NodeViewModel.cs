using CfgCompLib.classes;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace LittleLearner.CFG.ViewModel
{
    public class NodeViewModel : Node
    {
        public string IngoingNodesString { get; set; }
        public string OutgoingNodesString { get; set; }
        public string Title { get; set; }
        public NodeViewModel(int id, List<string> label = null, List<Node> predecessors = null, List<Node> successors = null, ShapeProperties shape = null) : base(id, label, predecessors, successors, shape)
        { }

        public NodeViewModel(Node node) : base(node.Id, node.GetLabel(), node.GetPredecessors(), node.GetSuccessors(), node.Shape)
        { }
    }
}
