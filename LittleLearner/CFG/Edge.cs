using CfgCompLib.classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LittleLearner.CFG
{
    public class Edge
    {
        public Node StartNode;
        public Node EndNode;
        public float startX;
        public float startY;
        public float endX;
        public float endY;
        public bool selected = false;

        public Edge(Node StartNode, Node EndNote)
        {
            this.StartNode = StartNode;
            this.EndNode = EndNote;
        }

        public float GetStartX() { return StartNode.Shape.x + (StartNode.Shape.width / 2); }
        public float GetEndX() { return EndNode.Shape.x + (EndNode.Shape.width / 2); }
        public float GetStartY() { return StartNode.Shape.y + (StartNode.Shape.height / 2); }
        public float GetEndY() { return EndNode.Shape.y + (EndNode.Shape.height / 2); }
    }
}
