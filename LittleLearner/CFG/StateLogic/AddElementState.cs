using System;
using System.Collections.Generic;
using System.Text;

namespace LittleLearner.CFG.StateLogic
{
    public class AddElementState : State
    {

        public AddElementState(FlowchartDrawer flowchartDrawer, GraphicsView graphicsView) : base(flowchartDrawer, graphicsView)
        {
            graphicsView.StartInteraction += OnFlowchartPressed;
            graphicsView.DragInteraction += OnFlowchartDragged;
        }

        public override void OnFlowchartPressed(object? sender, TouchEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        public override void OnFlowchartDragged(object? sender, TouchEventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        public override void ClearEventHandler()
        {
            throw new NotImplementedException();
        }
    }
}
