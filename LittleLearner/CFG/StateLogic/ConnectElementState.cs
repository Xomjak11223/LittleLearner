using CfgCompLib.classes;

namespace LittleLearner.CFG.StateLogic
{
    public class ConnectElementState : State
    {
        public float startX, startY, endX, endY;
        public Node? nodeSelected = null;
        public Node? nodeLastHovered = null;

        public ConnectElementState(FlowchartDrawer flowchartDrawer, GraphicsView graphicsView) : base(flowchartDrawer, graphicsView) 
        {
            graphicsView.StartInteraction += OnFlowchartPressed;
            graphicsView.DragInteraction += OnFlowchartDragged;
            graphicsView.EndInteraction += OnFlowchartReleased;

            graphicsView.StartHoverInteraction += HoverInteraction;
            graphicsView.MoveHoverInteraction += HoverInteraction;
        }

        public override void OnFlowchartPressed(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null) { return; }

            startX = eventArgs.Touches.FirstOrDefault().X;
            startY = eventArgs.Touches.FirstOrDefault().Y;
            nodeSelected = flowchartDrawer.pointOnElement(startX, startY);

            if(nodeSelected == null) { return; }
            flowchartDrawer.setNodeSelection(nodeSelected.Id, true);
            startX = flowchartDrawer.absoluteToRelativeX(nodeSelected.Shape.x + (nodeSelected.Shape.width / 2));
            startY = flowchartDrawer.absoluteToRelativeY(nodeSelected.Shape.y + (nodeSelected.Shape.height / 2));
        }

        public override void OnFlowchartDragged(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null || nodeSelected == null) { return; }

            if(nodeLastHovered != null && nodeLastHovered.Id != nodeSelected.Id)
            {
                endX = flowchartDrawer.absoluteToRelativeX(nodeLastHovered.Shape.x + (nodeLastHovered.Shape.width / 2));
                endY = flowchartDrawer.absoluteToRelativeY(nodeLastHovered.Shape.y + (nodeLastHovered.Shape.height / 2));
            }
            else
            {
                endX = eventArgs.Touches.FirstOrDefault().X;
                endY = eventArgs.Touches.FirstOrDefault().Y;
            }

            flowchartDrawer.createTemporaryConnection(startX, startY, endX, endY);
            graphicsView.Invalidate();
        }

        public void OnFlowchartReleased(object? sender, TouchEventArgs eventArgs)
        {
            flowchartDrawer.hideTemporaryConnection();

            if (nodeSelected != null) { flowchartDrawer.setNodeSelection(nodeSelected.Id, false); }
            if (flowchartDrawer.graph == null || nodeSelected == null || nodeLastHovered == null)
            {
                graphicsView.Invalidate();

                return;
            }

            flowchartDrawer.connectNodes(nodeSelected.Id, nodeLastHovered.Id);
            nodeLastHovered = null;
            nodeSelected = null;
            graphicsView.Invalidate();
        }

        public void HoverInteraction(object? sender, TouchEventArgs eventArgs)
        {
            if(flowchartDrawer.graph == null) { return; }

            float hoverX = eventArgs.Touches.FirstOrDefault().X;
            float hoverY = eventArgs.Touches.FirstOrDefault().Y;
            Node? hoveredNode = flowchartDrawer.pointOnElement(hoverX, hoverY);

            if(hoveredNode == null) {

                if (nodeLastHovered == null){ return; }
                if (nodeSelected == null || nodeLastHovered.Id != nodeSelected.Id) { flowchartDrawer.setNodeSelection(nodeLastHovered.Id, false); }

                nodeLastHovered = null;
                graphicsView.Invalidate();
                return;
            }

            nodeLastHovered = hoveredNode;
            flowchartDrawer.setNodeSelection(nodeLastHovered.Id, true);
            graphicsView.Invalidate();
        }

        public override void ClearEventHandler()
        {
            graphicsView.StartInteraction -= OnFlowchartPressed;
            graphicsView.DragInteraction -= OnFlowchartDragged;
            graphicsView.EndInteraction -= OnFlowchartReleased;

            graphicsView.StartHoverInteraction -= HoverInteraction;
            graphicsView.MoveHoverInteraction -= HoverInteraction;
        }
    }
}
