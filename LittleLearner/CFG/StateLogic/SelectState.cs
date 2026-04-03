using System;
using System.Collections.Generic;
using System.Text;

namespace LittleLearner.CFG.StateLogic
{
    public class SelectState : State
    {
        private float boundStartX;
        private float boundStartY;
        private float boundEndX;
        private float boundEndY;

        public SelectState(FlowchartDrawer flowchartDrawer, GraphicsView graphicsView) : base(flowchartDrawer, graphicsView)
        {
            graphicsView.StartInteraction += OnFlowchartPressed;
            graphicsView.DragInteraction += OnFlowchartDragged;
            graphicsView.EndInteraction += OnFlowchartReleased;
        }

        public override void OnFlowchartPressed(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null) { return; }
            boundStartX = eventArgs.Touches.FirstOrDefault().X;
            boundStartY = eventArgs.Touches.FirstOrDefault().Y;
        }

        public override void OnFlowchartDragged(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null) { return; }
            boundEndX = eventArgs.Touches.FirstOrDefault().X;
            boundEndY = eventArgs.Touches.FirstOrDefault().Y;

            flowchartDrawer.drawSelectionArea(boundStartX, boundStartY, boundEndX, boundEndY);
            graphicsView.Invalidate();
        }

        public void OnFlowchartReleased(object? sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null) { return; }
            boundEndX = eventArgs.Touches.FirstOrDefault().X;
            boundEndY = eventArgs.Touches.FirstOrDefault().Y;

            flowchartDrawer.hideSelectionArea();
            flowchartDrawer.selectShapesInArea(boundStartX, boundStartY, boundEndX, boundEndY);
            graphicsView.Invalidate();
        }
    }
}
