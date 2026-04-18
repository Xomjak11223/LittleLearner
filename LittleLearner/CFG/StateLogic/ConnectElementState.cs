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
            nodeSelected = flowchartDrawer.PointOnElement(startX, startY);

            if(nodeSelected == null) { return; }
            flowchartDrawer.SetNodeSelection(nodeSelected.Id, true);
            startX = GraphOperations.AbsolutToRelative(nodeSelected.Shape.x + (nodeSelected.Shape.width / 2), flowchartDrawer.offsetX, flowchartDrawer.zoom);
            startY = GraphOperations.AbsolutToRelative(nodeSelected.Shape.y + (nodeSelected.Shape.height / 2), flowchartDrawer.offsetY, flowchartDrawer.zoom);
        }

        public override void OnFlowchartDragged(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null || nodeSelected == null) { return; }

            if(nodeLastHovered != null && nodeLastHovered.Id != nodeSelected.Id)
            {
                endX = GraphOperations.AbsolutToRelative(nodeLastHovered.Shape.x + (nodeLastHovered.Shape.width / 2), flowchartDrawer.offsetX, flowchartDrawer.zoom);
                endY = GraphOperations.AbsolutToRelative(nodeLastHovered.Shape.y + (nodeLastHovered.Shape.height / 2), flowchartDrawer.offsetY, flowchartDrawer.zoom);
            }
            else
            {
                endX = eventArgs.Touches.FirstOrDefault().X;
                endY = eventArgs.Touches.FirstOrDefault().Y;
            }

            flowchartDrawer.CreateTemporaryConnection(startX, startY, endX, endY);
            graphicsView.Invalidate();
        }

        public void OnFlowchartReleased(object? sender, TouchEventArgs eventArgs)
        {
            flowchartDrawer.HideTemporaryConnection();

            if (nodeSelected != null) { flowchartDrawer.SetNodeSelection(nodeSelected.Id, false); }
            if (flowchartDrawer.graph == null || nodeSelected == null || nodeLastHovered == null)
            {
                graphicsView.Invalidate();

                return;
            }

            flowchartDrawer.ConnectNodes(nodeSelected.Id, nodeLastHovered.Id);
            nodeLastHovered = null;
            nodeSelected = null;
            graphicsView.Invalidate();
        }

        public void HoverInteraction(object? sender, TouchEventArgs eventArgs)
        {
            if(flowchartDrawer.graph == null) { return; }

            float hoverX = eventArgs.Touches.FirstOrDefault().X;
            float hoverY = eventArgs.Touches.FirstOrDefault().Y;
            Node? hoveredNode = flowchartDrawer.PointOnElement(hoverX, hoverY);

            if(hoveredNode == null) {

                if (nodeLastHovered == null){ return; }
                if (nodeSelected == null || nodeLastHovered.Id != nodeSelected.Id) { flowchartDrawer.SetNodeSelection(nodeLastHovered.Id, false); }

                nodeLastHovered = null;
                graphicsView.Invalidate();
                return;
            }

            nodeLastHovered = hoveredNode;
            flowchartDrawer.SetNodeSelection(nodeLastHovered.Id, true);
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
