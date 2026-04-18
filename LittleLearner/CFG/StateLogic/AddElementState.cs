using CfgCompLib.classes;

namespace LittleLearner.CFG.StateLogic
{
    public class AddElementState : State
    {
        public float startPositionX;
        public float endPositionX;
        public float startPositionY;
        public float endPositionY;
        public float vectorLength;
        public float vectorAngle;
        public static readonly double radiance_45 = 0.25 * Math.PI;
        public static readonly double radiance_90 = 0.5 * Math.PI;

        public AddElementState(FlowchartDrawer flowchartDrawer, GraphicsView graphicsView) : base(flowchartDrawer, graphicsView)
        {
            graphicsView.StartInteraction += OnFlowchartPressed;
            graphicsView.DragInteraction += OnFlowchartDragged;
            graphicsView.EndInteraction += OnFlowchartReleased;
        }

        public override void OnFlowchartPressed(object? sender, TouchEventArgs eventArgs)
        {
            startPositionX = eventArgs.Touches.FirstOrDefault().X;
            startPositionY = eventArgs.Touches.FirstOrDefault().Y;
            flowchartDrawer.DrawCreationWheel(FlowchartDrawer.PositionMarking.CENTER, startPositionX, startPositionY);
            graphicsView.Invalidate();
        }

        public override void OnFlowchartDragged(object? sender, TouchEventArgs eventArgs)
        {
            endPositionX = eventArgs.Touches.FirstOrDefault().X;
            endPositionY = eventArgs.Touches.FirstOrDefault().Y;

            float width = endPositionX - startPositionX;
            float height = endPositionY - startPositionY;
            FlowchartDrawer.PositionMarking marking;

            vectorLength = (float) Math.Sqrt((width * width) + (height * height));
            vectorAngle = Math.Abs((float) (Math.Asin(height / vectorLength) % (2*Math.PI)));

            if (vectorLength < FlowchartDrawer.creationInnerRadius) { marking = FlowchartDrawer.PositionMarking.CENTER; }
            else if(vectorLength > FlowchartDrawer.creationOuterRadius) { marking = FlowchartDrawer.PositionMarking.NONE; }
            else if (((vectorAngle >= radiance_45 && vectorAngle <= radiance_90) || (startPositionX == endPositionX)) && endPositionY < startPositionY) { marking = FlowchartDrawer.PositionMarking.TOP; }
            else if (((vectorAngle >= radiance_45 && vectorAngle <= radiance_90) || (startPositionX == endPositionX)) && endPositionY > startPositionY) { marking = FlowchartDrawer.PositionMarking.BOTTOM; }
            else if (vectorAngle >= 0 && vectorAngle < radiance_45 && endPositionX < startPositionX) { marking = FlowchartDrawer.PositionMarking.LEFT; }
            else { marking = FlowchartDrawer.PositionMarking.RIGHT; }

            flowchartDrawer.DrawCreationWheel(marking, startPositionX, startPositionY);
            graphicsView.Invalidate();
        }

        public void OnFlowchartReleased(object? sender, TouchEventArgs eventArgs)
        {
            endPositionX = eventArgs.Touches.FirstOrDefault().X;
            endPositionY = eventArgs.Touches.FirstOrDefault().Y;

            Shape shape;
            float width = endPositionX - startPositionX;
            float height = endPositionY - startPositionY;

            vectorLength = (float) Math.Sqrt((width * width) + (height * height));
            vectorAngle = (float) Math.Abs((Math.Asin(height / vectorLength) % (2*Math.PI)));

            if (vectorLength < FlowchartDrawer.creationInnerRadius || vectorLength > FlowchartDrawer.creationOuterRadius) { 
                flowchartDrawer.HideCreationWheel();
                graphicsView.Invalidate();
                return;
            }

            if (((vectorAngle >= radiance_45 && vectorAngle <= radiance_90) || (startPositionX == endPositionX)) && endPositionY < startPositionY) { shape = Shape.Start; }
            else if (((vectorAngle >= radiance_45 && vectorAngle <= radiance_90) || (startPositionX == endPositionX)) && endPositionY > startPositionY) { shape = Shape.End; }
            else if (vectorAngle >= 0 && vectorAngle < radiance_45 && endPositionX < startPositionX) { shape = Shape.Decision; }
            else { shape = Shape.Action; }

            flowchartDrawer.CreateNewShape(shape, startPositionX, startPositionY);
            flowchartDrawer.HideCreationWheel();
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
