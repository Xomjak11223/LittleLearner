using CfgCompLib.classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LittleLearner.CFG
{
    public class NodeForViewModel : Node
    {
        public string IngoingNodesString;
        public string OutgoingNodesString;
        public string Title;
        public NodeForViewModel(int id, List<string> label = null, List<Node> predecessors = null, List<Node> successors = null, ShapeProperties shape = null) : base(id, label, predecessors, successors, shape)
        { }
    }
}
