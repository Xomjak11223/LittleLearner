using CfgCompLib.classes;
using Microsoft.Maui.Graphics.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace LittleLearner.CFG
{
    public class FlowchartDrawer : IDrawable
    {
        public float offsetX = 0;
        public float offsetY = 0;
        public Node tempNode = null;
        private float zoom = 1;
        private readonly Color Background = new Color(0, 60, 100);
        public Graph graph = null;
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Background;
            canvas.FillRectangle(dirtyRect);

            if(tempNode != null)
            {
                switch (tempNode.Shape.shape)
                {
                    case Shape.Start: drawStart(canvas, tempNode.Shape.x, tempNode.Shape.y, tempNode.Shape.width, tempNode.Shape.height); break;
                    case Shape.End: drawEnd(canvas, tempNode.Shape.x, tempNode.Shape.y, tempNode.Shape.width, tempNode.Shape.height); break;
                    case Shape.Action: drawAction(canvas, "myText", tempNode.Shape.x, tempNode.Shape.y, tempNode.Shape.width, tempNode.Shape.height); break;
                    case Shape.Decision: drawDecision(canvas, "myText", tempNode.Shape.x, tempNode.Shape.y, tempNode.Shape.width, tempNode.Shape.height); break;
                }
            }

            if (graph == null) { return; }
            // Draws every single Shape
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

                //drawDecision(canvas, "myText", x, y, shape.width, shape.height);
            }

            // Draws the connection between the shapes
        }
        public void drawStart(ICanvas canvas, float startX, float startY, float width, float height)
        {
            // canvas.DrawText("Start", startX, startY, width, height);
            //IAttributedText text = new;
            // AttributedText text = new AttributedText()

            canvas.DrawRoundedRectangle(startX, startY, width, height, 4);
            //canvas.DrawText("text", startX, startY, width, height);
        }

        public void drawEnd(ICanvas canvas, float startX, float startY, float width, float height)
        {
        }

        // startX and startY defines the upper left Corner of the bounding box of the rombus
        public void drawAction(ICanvas canvas, String text, float startX, float startY, float width, float height)
        {
            
        }

        public void drawDecision(ICanvas canvas, String text, float startX, float startY, float width, float height)
        {
            float widthHalf = width / 2;
            float heightHalf = height / 2;

            PathF pathRombus = new PathF(startX, startY + heightHalf);  // Starts at left corner of rombus
            pathRombus.MoveTo(startX + widthHalf, startY);  // Moves to upper Corner of rombus
            pathRombus.MoveTo(startX + width, startY + heightHalf); // Moves to right Corner of rombus
            pathRombus.MoveTo(startX + widthHalf, startY + height); // Moves to lower Corner of rombus
            pathRombus.MoveTo(startX, startY + heightHalf); // Moves to left Corner of rombus

            canvas.DrawPath(pathRombus);
        }

        public void connectShapes(ICanvas canvas)
        {
        }

        public void zoomIn() { }
        public void zoomOut() { }

        public static void cleanCanvase(ICanvas canvas)
        {
            
        }
    }
}
