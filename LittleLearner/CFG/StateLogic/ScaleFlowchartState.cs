namespace LittleLearner.CFG.StateLogic
{
    public class ScaleFlowchartState : State
    {
        public float startPositionX;
        public float startPositionY;
        public float endPositionX;
        public float endPositionY;

        public ScaleFlowchartState(FlowchartDrawer flowchartDrawer, GraphicsView graphicsView) : base(flowchartDrawer, graphicsView)
        {
            graphicsView.StartInteraction += OnFlowchartPressed;
            graphicsView.DragInteraction += OnFlowchartDragged;
            graphicsView.EndInteraction += OnFlowchartReleased;
        }

        public override void OnFlowchartPressed(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null) { return; }
            startPositionX = eventArgs.Touches.FirstOrDefault().X;
            startPositionY = eventArgs.Touches.FirstOrDefault().Y;
            flowchartDrawer.DrawScalingWheel(startPositionX, startPositionY);
            graphicsView.Invalidate();
        }

        public override void OnFlowchartDragged(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null) { return; }
            endPositionX = eventArgs.Touches.FirstOrDefault().X;
            endPositionY = eventArgs.Touches.FirstOrDefault().Y;

            flowchartDrawer.ScaleCanvas(startPositionX, startPositionY, endPositionX, endPositionY);
            graphicsView.Invalidate();
        }

        public void OnFlowchartReleased(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null) { return; }
            endPositionX = eventArgs.Touches.FirstOrDefault().X;
            endPositionY = eventArgs.Touches.FirstOrDefault().Y;

            flowchartDrawer.ScaleCanvas(startPositionX, startPositionY, endPositionX, endPositionY);
            flowchartDrawer.HideScalingWheel();
            graphicsView.Invalidate();
        }

        public override void ClearEventHandler()
        {
            graphicsView.StartInteraction -= OnFlowchartPressed;
            graphicsView.DragInteraction -= OnFlowchartDragged;
            graphicsView.EndInteraction -= OnFlowchartReleased;
        }
    }
}
