using System;
using System.Collections.Generic;
using System.Text;

namespace LittleLearner.CFG.StateLogic
{
    public abstract class State
    {
        public FlowchartDrawer flowchartDrawer;
        public GraphicsView graphicsView;

        public State(FlowchartDrawer flowchartDrawer, GraphicsView graphicsView){ 
            this.flowchartDrawer = flowchartDrawer; 
            this.graphicsView = graphicsView;
        }

        public abstract void OnFlowchartPressed(object? sender, TouchEventArgs eventArgs);
        public abstract void OnFlowchartDragged(object? sender, TouchEventArgs eventArgs);
        public abstract void ClearEventHandler();
    }
}
