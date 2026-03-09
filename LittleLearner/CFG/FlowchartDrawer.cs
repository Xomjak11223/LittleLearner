using CfgCompLib.classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LittleLearner.CFG
{
    internal class FlowchartDrawer : IDrawable
    {
        public static float offsetX = 0;
        public static float offsetY = 0;
        private float zoom = 1;
        private readonly Color Background = new Color(0, 60, 100);
        public static Graph graph = null;
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {

            canvas.FillColor = Background;
            canvas.FillRectangle(dirtyRect);

            if(graph == null) { return; }
            foreach(var nodeIndexPair in graph.GetNodes()) {
                ShapeProperties shape = nodeIndexPair.Value.Shape;
                float x = shape.x + offsetX;
                float y = shape.y + offsetY;

                // Check if shape is out of drawable area
                if (x > dirtyRect.Width) { continue; }
                if (x + shape.width < 0) { continue; }

                if (y > dirtyRect.Height) { continue; }
                if (y + shape.height < 0) { continue; }

                switch (shape.shape) {
                    case Shape.Start: drawStart(canvas, x, y, shape.width, shape.height); break;
                    case Shape.End: drawEnd(canvas, x, y, shape.width, shape.height); break;
                    case Shape.Action: drawAction(canvas, "myText", x, y, shape.width, shape.height); break;
                    case Shape.Decision: drawDecision(canvas, "myText", x, y, shape.width, shape.height); break;
                }
            }
        }
        public void drawStart(ICanvas canvas, float startX, float startY, float width, float height)
        {
            // canvas.DrawText("Start", startX, startY, width, height);
            canvas.DrawRoundedRectangle(startX, startY, width, height, 4);
        }

        public void drawEnd(ICanvas canvas, float startX, float startY, float width, float height)
        {
        }

        public void drawAction(ICanvas canvas, String text, float startX, float startY, float width, float height)
        {
        }

        public void drawDecision(ICanvas canvas, String text, float startX, float startY, float width, float height)
        {
        }

        public void connectShapes(ICanvas canvas)
        {
        }

        public void zoomIn() { }
        public void zoomOut() { }

        public static void cleanCanvase()
        {

        }
    }
}
