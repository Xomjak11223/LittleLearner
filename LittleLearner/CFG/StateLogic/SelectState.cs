using CfgCompLib.classes;

namespace LittleLearner.CFG.StateLogic
{
    public class SelectState : State
    {
        private float boundStartX;
        private float boundStartY;
        private float boundEndX;
        private float boundEndY;
        private bool moveSelected;
        public Node? nodeLastHovered = null;

        public SelectState(FlowchartDrawer flowchartDrawer, GraphicsView graphicsView) : base(flowchartDrawer, graphicsView)
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

            boundStartX = eventArgs.Touches.FirstOrDefault().X;
            boundStartY = eventArgs.Touches.FirstOrDefault().Y;

            graphicsView.StartHoverInteraction -= HoverInteraction;
            graphicsView.MoveHoverInteraction -= HoverInteraction;

            if (nodeLastHovered != null) { nodeLastHovered.Shape.selected = true; }
            moveSelected = flowchartDrawer.pointOnSelected(boundStartX, boundStartY) != null;
        }

        public override void OnFlowchartDragged(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null) { return; }

            boundEndX = eventArgs.Touches.FirstOrDefault().X;
            boundEndY = eventArgs.Touches.FirstOrDefault().Y;

            if (moveSelected)
            {
                //TODO shapes can be moved around but after moving, every shape, exept for the one the mouse is hovering, will be deselected
                flowchartDrawer.moveSelected(boundEndX - boundStartX, boundEndY - boundStartY);
                boundStartX = boundEndX;
                boundStartY = boundEndY;
            }
            else
            {
                flowchartDrawer.drawSelectionArea(boundStartX, boundStartY, boundEndX, boundEndY);
            }
            graphicsView.Invalidate();
        }

        public void OnFlowchartReleased(object? sender, TouchEventArgs eventArgs)
        {
            if(flowchartDrawer.graph == null) { return; }

            boundEndX = eventArgs.Touches.FirstOrDefault().X;
            boundEndY = eventArgs.Touches.FirstOrDefault().Y;

            flowchartDrawer.hideSelectionArea();
            flowchartDrawer.selectShapesInArea(boundStartX, boundStartY, boundEndX, boundEndY);

            if (moveSelected || flowchartDrawer.nodesSelected() == 0)
            {
                nodeLastHovered = flowchartDrawer.pointOnElement(boundEndX, boundEndY);
                graphicsView.StartHoverInteraction += HoverInteraction;
                graphicsView.MoveHoverInteraction += HoverInteraction;
            }

            graphicsView.Invalidate();

        }

        public void HoverInteraction(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null) { return; }

            float hoverX = eventArgs.Touches.FirstOrDefault().X;
            float hoverY = eventArgs.Touches.FirstOrDefault().Y;
            Node? hoveredNode = flowchartDrawer.pointOnElement(hoverX, hoverY);

            if (hoveredNode == null)
            {
                if (nodeLastHovered != null)
                {
                    flowchartDrawer.setNodeSelection(nodeLastHovered.Id, false);
                    nodeLastHovered = null;
                    graphicsView.Invalidate();
                }

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
