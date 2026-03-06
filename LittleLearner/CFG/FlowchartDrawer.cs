using CfgCompLib.classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LittleLearner.CFG
{
    internal class FlowchartDrawer : IDrawable
    {
        private readonly Color Background = new Color(0, 60, 100);
        public static Graph graph = null;
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if(graph == null) { return; }

            canvas.FillColor = Background;
            canvas.FillRectangle(dirtyRect);

            foreach(var nodeIndexPair in graph.GetNodes()) {
                ShapeProperties shape = nodeIndexPair.Value.shapeProperties;
             
                switch (shape.shape) {
                    case Shape.Start: drawStart(canvas, shape.x, shape.y, shape.width, shape.height); break;
                    case Shape.End: drawEnd(canvas, shape.x, shape.y, shape.width, shape.height); break;
                    case Shape.Action: drawAction(canvas, "myText", shape.x, shape.y, shape.width, shape.height); break;
                    case Shape.Decision: drawDecision(canvas, "myText", shape.x, shape.y, shape.width, shape.height); break;
                }
            }

            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 6;
            canvas.DrawLine(0, 0, dirtyRect.Center.X, dirtyRect.Center.Y);
        }

        private void drawStart(ICanvas canvas, float startX, float startY, float width, float height)
        {
            // canvas.DrawText("Start", startX, startY, width, height);
            canvas.DrawRoundedRectangle(startX, startY, width, height, 4);
        }

        private void drawEnd(ICanvas canvas, float startX, float startY, float width, float height)
        {
        }

        private void drawAction(ICanvas canvas, String text, float startX, float startY, float width, float height)
        {
        }

        private void drawDecision(ICanvas canvas, String text, float startX, float startY, float width, float height)
        {
        }

        private void connectShapes(ICanvas canvas)
        {
        }
    }
}
