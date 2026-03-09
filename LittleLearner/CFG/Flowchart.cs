using System;
using System.Collections.Generic;
using System.Text;

namespace LittleLearner.CFG
{
    internal class Flowchart
    {
        // TODO Flowchart mittels json / xml objkt initialisieren
        public Flowchart() { }
    }

    class FlowchartNode
    {
        public String Text { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float width { get; set; }
        public float height { get; set; }

        public List<FlowchartNode> Children { get; set; }
        public FlowchartNode(String text)
        {
            this.Text = text;
            this.Children = new List<FlowchartNode>();
        }
    }


}
