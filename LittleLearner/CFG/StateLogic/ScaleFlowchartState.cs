using System;
using System.Collections.Generic;
using System.Text;

namespace LittleLearner.CFG.StateLogic
{
    public class ScaleFlowchartState : State
    {
        public float startPositionX;
        public float endPositionX;
        public float scalingFactor;

        public ScaleFlowchartState(FlowchartDrawer flowchartDrawer, GraphicsView graphicsView) : base(flowchartDrawer, graphicsView)
        {
            graphicsView.StartInteraction += OnFlowchartPressed;
            graphicsView.DragInteraction += OnFlowchartDragged;
        }

        public override void OnFlowchartPressed(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null) { return; }
            startPositionX = eventArgs.Touches.FirstOrDefault().X;
            scalingFactor = flowchartDrawer.zoom;
            graphicsView.Invalidate();
        }

        public override void OnFlowchartDragged(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null) { return; }
            endPositionX = eventArgs.Touches.FirstOrDefault().X;
            scalingFactor = scalingFactor + endPositionX - startPositionX;

            if(scalingFactor >= 2) scalingFactor = 2;
            if (scalingFactor <= 0.5) scalingFactor = (float) 0.5;

            flowchartDrawer.zoom = scalingFactor;
            graphicsView.Invalidate();
        }

        public override void ClearEventHandler()
        {
            graphicsView.StartInteraction -= OnFlowchartPressed;
            graphicsView.DragInteraction -= OnFlowchartDragged;
        }
    }
}
