using CfgCompLib.classes;
using static LittleLearner.CFG.FlowchartDrawer;

namespace LittleLearner.CFG.StateLogic
{
    public class EditElementState : State
    {
        public static int lambda = 8;
        float startX, startY, endX, endY;
        Node? selectedNode;
        Node? relabledNode;
        PositionMarking position1, position2;
        ContentPage contentPage;
        Task resetTask = null;
        string newLabel;

        public EditElementState(FlowchartDrawer flowchartDrawer, GraphicsView graphicsView, ContentPage contentPage) : base(flowchartDrawer, graphicsView)
        {
            graphicsView.StartInteraction += OnFlowchartPressed;
            graphicsView.DragInteraction += OnFlowchartDragged;
            graphicsView.EndInteraction += OnFlowchartReleased;

            graphicsView.StartHoverInteraction += HoverInteraction;
            graphicsView.MoveHoverInteraction += HoverInteraction;
            this.contentPage = contentPage;
        }

        public override void OnFlowchartPressed(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null) { return; }

            startX = eventArgs.Touches.FirstOrDefault().X;
            startY = eventArgs.Touches.FirstOrDefault().Y;

            selectedNode = flowchartDrawer.PointOnElement(startX, startY);
            if (selectedNode == null){ return; }

            float shapeX = GraphOperations.AbsolutToRelative(selectedNode.Shape.x, flowchartDrawer.offsetX, flowchartDrawer.zoom);
            float shapeY = GraphOperations.AbsolutToRelative(selectedNode.Shape.y, flowchartDrawer.offsetY, flowchartDrawer.zoom);

            PositionMarking[] positions = PositionsMarked(startX, startY, shapeX, shapeY, selectedNode.Shape.width * flowchartDrawer.zoom, selectedNode.Shape.height * flowchartDrawer.zoom);
            if (positions[0] == PositionMarking.NONE && positions[1] == PositionMarking.NONE){
                if (relabledNode != null && selectedNode == relabledNode) { Popup(selectedNode); return; }
                if (resetTask == null || resetTask.IsCompleted) resetTask = ResetAfterTime(1);

                relabledNode = selectedNode;
                return; 
            }

            position1 = positions[0];
            position2 = positions[1];

            flowchartDrawer.SelectEditingNode(selectedNode, positions[0], positions[1]);
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
            flowchartDrawer.ScaleNode(selectedNode.Id, position1, position2, dx, dy);
            graphicsView.Invalidate();
        }

        public void OnFlowchartReleased(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null || selectedNode == null) { return; }

            endX = eventArgs.Touches.FirstOrDefault().X;
            endY = eventArgs.Touches.FirstOrDefault().Y;

            float dx = endX - startX;
            float dy = endY - startY;

            flowchartDrawer.ScaleNode(selectedNode.Id, position1, position2, dx, dy);

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

            selectedNode = flowchartDrawer.PointOnElement(startX, startY);
            if (selectedNode != null)
            {
                float shapeX = GraphOperations.AbsolutToRelative(selectedNode.Shape.x, flowchartDrawer.offsetX, flowchartDrawer.zoom);
                float shapeY = GraphOperations.AbsolutToRelative(selectedNode.Shape.y, flowchartDrawer.offsetY, flowchartDrawer.zoom);

                PositionMarking[] positions = PositionsMarked(startX, startY, shapeX, shapeY, selectedNode.Shape.width * flowchartDrawer.zoom, selectedNode.Shape.height * flowchartDrawer.zoom);
                if (positions[0] == PositionMarking.NONE && positions[1] == PositionMarking.NONE) 
                {
                    flowchartDrawer.HideEditing();
                    graphicsView.Invalidate();
                    return;
                }

                flowchartDrawer.SelectEditingNode(selectedNode, positions[0], positions[1]);
                graphicsView.Invalidate();
                return;
            }

            flowchartDrawer.HideEditing();
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

        private PositionMarking[] PositionsMarked(float x, float y, float nodeX, float nodeY, float width, float height)
        {
            if (GraphOperations.PointAroundLine(x, y, nodeX + width, nodeY, nodeX + width, nodeY, lambda))
            { return [PositionMarking.TOP, PositionMarking.RIGHT]; }

            if (GraphOperations.PointAroundLine(x, y, nodeX, nodeY, nodeX, nodeY, lambda))
            { return [PositionMarking.TOP, PositionMarking.LEFT]; }

            if (GraphOperations.PointAroundLine(x, y, nodeX + width, nodeY + height, nodeX + width, nodeY + height, lambda))
            { return [PositionMarking.BOTTOM, PositionMarking.RIGHT]; }

            if (GraphOperations.PointAroundLine(x, y, nodeX, nodeY + height, nodeX, nodeY + height, lambda))
            { return [PositionMarking.BOTTOM, PositionMarking.LEFT]; }

            if (GraphOperations.PointAroundLine(x, y, nodeX, nodeY, nodeX + width, nodeY, lambda))
            { return [PositionMarking.TOP, PositionMarking.NONE]; }

            if (GraphOperations.PointAroundLine(x, y, nodeX, nodeY + height, nodeX + width, nodeY + height, lambda))
            { return [PositionMarking.BOTTOM, PositionMarking.NONE]; }

            if (GraphOperations.PointAroundLine(x, y, nodeX, nodeY, nodeX, nodeY + height, lambda))
            { return [PositionMarking.LEFT, PositionMarking.NONE]; }

            if (GraphOperations.PointAroundLine(x, y, nodeX + width, nodeY, nodeX + width, nodeY + height, lambda))
            { return [PositionMarking.RIGHT, PositionMarking.NONE]; }

            return [PositionMarking.NONE, PositionMarking.NONE];
        }

        public async Task ResetAfterTime(int timeInSeconds) { await Task.Delay(timeInSeconds * 1000); relabledNode = null; }
        public async void Popup(Node nodeToRelable)
        {
            relabledNode = null;
            if(flowchartDrawer.graph == null) { return; }

            string userInput = await contentPage.DisplayPromptAsync("Label Umbenennen", "", "OK", "Cancel", null, -1, null, nodeToRelable.LabelToString());
            if (string.IsNullOrEmpty(userInput)) return;

            Node newNode = new Node(nodeToRelable.Id, new List<string>([userInput]), nodeToRelable.GetPredecessors(), nodeToRelable.GetSuccessors(), nodeToRelable.Shape);
            flowchartDrawer.graph.RemoveNode(nodeToRelable);
            flowchartDrawer.graph.AddNode(newNode);
            flowchartDrawer.ReplaceNodeInEdges(nodeToRelable, newNode);
            graphicsView.Invalidate();
        }
    }

}