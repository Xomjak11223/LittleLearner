using CfgCompLib.classes;
using System;
using System.Collections.Generic;
using System.Text;

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
        public static readonly double[] startAngle = { 0.25 * Math.PI, 0.75 * Math.PI, 1.25 * Math.PI, 1.75 * Math.PI, 0 };
        public static readonly double[] endAngle = { 0.75 * Math.PI, 1.25 * Math.PI, 1.75 * Math.PI, 0.25 * Math.PI, 2 * Math.PI };

        public AddElementState(FlowchartDrawer flowchartDrawer, GraphicsView graphicsView) : base(flowchartDrawer, graphicsView)
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
            flowchartDrawer.drawCreationWheel(FlowchartDrawer.PositionMarking.CENTER, startPositionX, startPositionY);
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
            vectorAngle = (float) (Math.Asin(height / vectorLength) % (2*Math.PI));

            if (vectorLength < FlowchartDrawer.creationInnerRadius || vectorLength > FlowchartDrawer.creationOuterRadius) { marking = FlowchartDrawer.PositionMarking.NONE; }
            else if (vectorAngle >= startAngle[0] && vectorAngle < endAngle[0]) { marking = FlowchartDrawer.PositionMarking.TOP; }
            else if (vectorAngle >= startAngle[1] && vectorAngle < endAngle[1]) { marking = FlowchartDrawer.PositionMarking.LEFT; }
            else if (vectorAngle >= startAngle[2] && vectorAngle < endAngle[2]) { marking = FlowchartDrawer.PositionMarking.BOTTOM; }
            else { marking = FlowchartDrawer.PositionMarking.RIGHT; }

            flowchartDrawer.drawCreationWheel(marking, startPositionX, startPositionY);
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
            vectorAngle = (float) (Math.Asin(height / vectorLength) % (2*Math.PI));

            if (vectorLength < FlowchartDrawer.creationInnerRadius || vectorLength > FlowchartDrawer.creationOuterRadius) { flowchartDrawer.hideCreationWheel(); return; }

            if (vectorAngle >= startAngle[0] && vectorAngle < endAngle[0]) { shape = Shape.Start; }
            else if (vectorAngle >= startAngle[1] && vectorAngle < endAngle[1]) { shape = Shape.Decision; }
            else if (vectorAngle >= startAngle[2] && vectorAngle < endAngle[2]) { shape = Shape.End; }
            else { shape = Shape.Action; }

            flowchartDrawer.createNewShape(shape, startPositionX, startPositionY);
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
