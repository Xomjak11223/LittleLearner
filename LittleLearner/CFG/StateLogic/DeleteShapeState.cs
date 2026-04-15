using CfgCompLib.classes;

namespace LittleLearner.CFG.StateLogic
{

    public class DeleteShapeState : State
    {
        public Node? nodeLastHovered;
        public Edge? edgeLastHovered;

        public DeleteShapeState(FlowchartDrawer flowchartDrawer, GraphicsView graphicsView) : base(flowchartDrawer, graphicsView)
        {
            graphicsView.StartInteraction += OnFlowchartPressed;
            graphicsView.StartHoverInteraction += HoverInteraction;
            graphicsView.MoveHoverInteraction += HoverInteraction;
        }

        public override void OnFlowchartDragged(object? sender, TouchEventArgs eventArgs) { throw new NotImplementedException(); }

        public override void OnFlowchartPressed(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null) { return; }

            float hoverX = eventArgs.Touches.FirstOrDefault().X;
            float hoverY = eventArgs.Touches.FirstOrDefault().Y;
            Node? node = flowchartDrawer.pointOnElement(hoverX, hoverY);
            Edge? edge = flowchartDrawer.pointHitsEdge(hoverX, hoverY);

            if(node != null)
            {
                flowchartDrawer.DeleteEdgedAssociatedWithNode(node);
                flowchartDrawer.graph.RemoveNode(node);
                graphicsView.Invalidate();
                return;
            }

            if(edge != null)
            {
                flowchartDrawer.graph.RemoveEdge(edge.StartNode, edge.EndNode);
                flowchartDrawer.RemoveEdge(edge);
                graphicsView.Invalidate();
                return;
            }
        }

        public void HoverInteraction(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null) { return; }

            float hoverX = eventArgs.Touches.FirstOrDefault().X;
            float hoverY = eventArgs.Touches.FirstOrDefault().Y;
            Node? hoveredNode = flowchartDrawer.pointOnElement(hoverX, hoverY);
            Edge? hoveredEdge = flowchartDrawer.pointHitsEdge(hoverX, hoverY);

            if (hoveredNode == null)
            {
                // Cursor is NOT on a Node anymore
                nodeLastHovered?.Shape.selected = false;
                nodeLastHovered = null;

                edgeLastHovered?.selected = false;
                edgeLastHovered = null;

                if (hoveredEdge != null)
                {
                    edgeLastHovered = hoveredEdge;
                    edgeLastHovered.selected = true;
                }

                graphicsView.Invalidate();
                return;
            }

            // Cursor is on a Node
            if (nodeLastHovered != null)
            {
                if (nodeLastHovered.Equals(hoveredNode)) { return; }

                nodeLastHovered.Shape.selected = false;
                hoveredNode.Shape.selected = true;
                nodeLastHovered = hoveredNode;
            }
            else
            {
                hoveredNode.Shape.selected = true;
                nodeLastHovered = hoveredNode;

                edgeLastHovered?.selected = false;
                edgeLastHovered = null;

            }

            graphicsView.Invalidate();
        }

        public override void ClearEventHandler()
        {
            graphicsView.StartInteraction -= OnFlowchartPressed;
            graphicsView.StartHoverInteraction -= HoverInteraction;
            graphicsView.MoveHoverInteraction -= HoverInteraction;
        }
    }
}
