using CfgCompLib.classes;

namespace LittleLearner.CFG.ViewModel
{
    
    public class EqualNodes
    {
        public string codeGraphNode { get; set; }
        public string flowchartNode { get; set; }

        public EqualNodes(string codeGraphNode, string flowchartNode)
        {
            this.codeGraphNode = codeGraphNode;
            this.flowchartNode = flowchartNode;
        }
    }
}
