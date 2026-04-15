using CfgCompLib.classes;
using static LittleLearner.CFG.FlowchartDrawer;

namespace LittleLearner.CFG.StateLogic
{
    public class EditElementState : State
    {
        public static int lambda = 8;
        float startX, startY, endX, endY;
        Node? selectedNode;
        PositionMarking position1, position2;

        public EditElementState(FlowchartDrawer flowchartDrawer, GraphicsView graphicsView) : base(flowchartDrawer, graphicsView)
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

            selectedNode = flowchartDrawer.pointOnElement(startX, startY);
            if (selectedNode == null){ return; }

            float shapeX = flowchartDrawer.absoluteToRelativeX(selectedNode.Shape.x);
            float shapeY = flowchartDrawer.absoluteToRelativeY(selectedNode.Shape.y);

            PositionMarking[] positions = PositionsMarked(startX, startY, shapeX, shapeY, selectedNode.Shape.width * flowchartDrawer.zoom, selectedNode.Shape.height * flowchartDrawer.zoom);
            if (positions[0] == PositionMarking.NONE && positions[1] == PositionMarking.NONE){ return; }

            position1 = positions[0];
            position2 = positions[1];

            flowchartDrawer.selectEditingNode(selectedNode, positions[0], positions[1]);
            graphicsView.StartHoverInteraction -= HoverInteraction;
            graphicsView.MoveHoverInteraction -= HoverInteraction;
            graphicsView.Invalidate();
        }

        public override void OnFlowchartDragged(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null || selectedNode == null) { return; }

            endX = eventArgs.Touches.FirstOrDefault().X;
            endY = eventArgs.Touches.FirstOrDefault().Y;

            float dx = (endX - startX) / flowchartDrawer.zoom;
            float dy = (endY - startY) / flowchartDrawer.zoom;

            startX = endX;
            startY = endY;

            // Logik zum skallieren 
            flowchartDrawer.scaleNode(selectedNode.Id, position1, position2, dx, dy);
            graphicsView.Invalidate();
        }

        public void OnFlowchartReleased(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null || selectedNode == null) { return; }

            endX = eventArgs.Touches.FirstOrDefault().X;
            endY = eventArgs.Touches.FirstOrDefault().Y;

            float dx = endX - startX;
            float dy = endY - startY;

            flowchartDrawer.scaleNode(selectedNode.Id, position1, position2, dx, dy);

            selectedNode = null;
            graphicsView.StartHoverInteraction += HoverInteraction;
            graphicsView.MoveHoverInteraction += HoverInteraction;
            graphicsView.Invalidate();
        }

        public void HoverInteraction(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null) { return; }

            startX = eventArgs.Touches.FirstOrDefault().X;
            startY = eventArgs.Touches.FirstOrDefault().Y;

            selectedNode = flowchartDrawer.pointOnElement(startX, startY);
            if (selectedNode != null)
            {
                float shapeX = flowchartDrawer.absoluteToRelativeX(selectedNode.Shape.x);
                float shapeY = flowchartDrawer.absoluteToRelativeY(selectedNode.Shape.y);

                PositionMarking[] positions = PositionsMarked(startX, startY, shapeX, shapeY, selectedNode.Shape.width * flowchartDrawer.zoom, selectedNode.Shape.height * flowchartDrawer.zoom);
                if (positions[0] == PositionMarking.NONE && positions[1] == PositionMarking.NONE) 
                {
                    flowchartDrawer.hideEditing();
                    graphicsView.Invalidate();
                    return;
                }

                flowchartDrawer.selectEditingNode(selectedNode, positions[0], positions[1]);
                graphicsView.Invalidate();
                return;
            }

            flowchartDrawer.hideEditing();
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

        private bool pointInLambdaArea(float pointX, float pointY, float lineStartX, float lineStartY, float lineEndX, float lineEndY, float lambda)
        {
            return (pointX <= (lineEndX + lambda) &&
                    pointX >= (lineStartX - lambda) &&
                    pointY <= (lineEndY + lambda) &&
                    pointY >= (lineStartY - lambda));
        }

        private PositionMarking[] PositionsMarked(float x, float y, float nodeX, float nodeY, float width, float height)
        {
            if (pointInLambdaArea(x, y, nodeX + width, nodeY, nodeX + width, nodeY, lambda))
            { return [PositionMarking.TOP, PositionMarking.RIGHT]; }

            if (pointInLambdaArea(x, y, nodeX, nodeY, nodeX, nodeY, lambda))
            { return [PositionMarking.TOP, PositionMarking.LEFT]; }

            if (pointInLambdaArea(x, y, nodeX + width, nodeY + height, nodeX + width, nodeY + height, lambda))
            { return [PositionMarking.BOTTOM, PositionMarking.RIGHT]; }

            if (pointInLambdaArea(x, y, nodeX, nodeY + height, nodeX, nodeY + height, lambda))
            { return [PositionMarking.BOTTOM, PositionMarking.LEFT]; }

            if (pointInLambdaArea(x, y, nodeX, nodeY, nodeX + width, nodeY, lambda))
            { return [PositionMarking.TOP, PositionMarking.NONE]; }

            if (pointInLambdaArea(x, y, nodeX, nodeY + height, nodeX + width, nodeY + height, lambda))
            { return [PositionMarking.BOTTOM, PositionMarking.NONE]; }

            if (pointInLambdaArea(x, y, nodeX, nodeY, nodeX, nodeY + height, lambda))
            { return [PositionMarking.LEFT, PositionMarking.NONE]; }

            if (pointInLambdaArea(x, y, nodeX + width, nodeY, nodeX + width, nodeY + height, lambda))
            { return [PositionMarking.RIGHT, PositionMarking.NONE]; }

            return [PositionMarking.NONE, PositionMarking.NONE];
        }
    }
}