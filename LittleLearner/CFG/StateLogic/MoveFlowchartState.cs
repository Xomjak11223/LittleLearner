using System;
using System.Collections.Generic;
using System.Text;

namespace LittleLearner.CFG.StateLogic
{
    public class MoveFlowchartState : State
    {
        private float GraphicsViewOffsetY = 0;
        private float GraphicsViewOffsetX = 0;

        private float UserTouchX = 0;
        private float UserTouchY = 0;

        public MoveFlowchartState(FlowchartDrawer flowchartDrawer, GraphicsView graphicsView) : base(flowchartDrawer, graphicsView) {}

        public override void OnFlowchartDragged(object sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null) { return; }

            float dx = eventArgs.Touches.FirstOrDefault().X - UserTouchX;
            float dy = eventArgs.Touches.FirstOrDefault().Y - UserTouchY;

            flowchartDrawer.offsetX = GraphicsViewOffsetX + dx;
            flowchartDrawer.offsetY = GraphicsViewOffsetY + dy;
            graphicsView.Invalidate();
        }

        public override void OnFlowchartPressed(object sender, TouchEventArgs eventArgs)
        {
            if (flowchartDrawer.graph == null) { return; }

            UserTouchX = eventArgs.Touches.FirstOrDefault().X;
            UserTouchY = eventArgs.Touches.FirstOrDefault().Y;

            GraphicsViewOffsetX = flowchartDrawer.offsetX;
            GraphicsViewOffsetY = flowchartDrawer.offsetY;
        }
    }
}
