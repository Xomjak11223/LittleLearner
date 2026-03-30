using CfgCompLib.classes;
using Microsoft.Maui.Graphics.Text;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

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
                bool shapeNotInDirtyRect = (x > dirtyRect.Width) || (x + shape.width < 0) || (y > dirtyRect.Height) || (y + shape.height < 0);

                // Draws the connection to its succesor shape
                foreach (Node child in nodeIndexPair.Value.GetSuccessors())
                {
                    float childX = child.Shape.x + offsetX;
                    float childY = child.Shape.y + offsetY;
                    bool childNotInDirtyRect = (childX > dirtyRect.Width) || (childX + shape.width < 0) || (childY > dirtyRect.Height) || (childY + shape.height < 0);

                    if(!shapeNotInDirtyRect || !childNotInDirtyRect) { connectShapes(canvas, x, y, childX, childY); }
                }

                // Check if shape is out of drawable area
                if (shapeNotInDirtyRect) { continue; }

                var labels = nodeIndexPair.Value.GetLabel();
                string label = "NO LABEL";
                if (labels != null)
                {
                    if(labels.Count != 0) label = labels[0];
                }

                // Draws the Flow Graph Shape
                switch (shape.shape) {
                    case Shape.Start: drawStart(canvas, x, y, shape.width, shape.height); break;
                    case Shape.End: drawEnd(canvas, x, y, shape.width, shape.height); break;
                    case Shape.Action: drawAction(canvas, label, x, y, shape.width, shape.height); break;
                    case Shape.Decision: drawDecision(canvas, label, x, y, shape.width, shape.height); break;
                }
            }

            // Draws the connection between the shapes
        }
        public void drawStart(ICanvas canvas, float startX, float startY, float width, float height)
        {
            canvas.DrawRoundedRectangle(startX, startY, width, height, 4);
            drawText(canvas, "Start", startX, startY, width, height);
        }

        public void drawEnd(ICanvas canvas, float startX, float startY, float width, float height)
        {
            canvas.DrawRoundedRectangle(startX, startY, width, height, 4);
            drawText(canvas, "End", startX, startY, width, height);
        }

        // startX and startY defines the upper left Corner of the bounding box of the rombus
        public void drawAction(ICanvas canvas, String text, float startX, float startY, float width, float height)
        {
            canvas.DrawRectangle(startX, startY, width, height);
            drawText(canvas, text, startX, startY, width, height);
        }

        public void drawDecision(ICanvas canvas, String text, float startX, float startY, float width, float height)
        {
            float widthHalf = width / 2;
            float heightHalf = height / 2;

            // Draws the Decision field
            PathF pathRombus = new PathF(startX, startY + heightHalf);  // Starts at left corner of rombus
            pathRombus.LineTo(startX + widthHalf, startY);  // Moves to top Corner
            pathRombus.LineTo(startX + width, startY + heightHalf); // Moves to right Corner
            pathRombus.LineTo(startX + widthHalf, startY + height); // Moves to bottom Corner
            pathRombus.LineTo(startX, startY + heightHalf); // Moves to left Corner
            pathRombus.Close();

            canvas.DrawPath(pathRombus);
            drawText(canvas, text, startX, startY, width, height);
        }

        public void drawText(ICanvas canvas, String text, float startX, float startY, float width, float height)
        {
            RectF textBounds = new RectF(startX, startY, width, height);

            canvas.FontSize = 16;
            canvas.DrawString(text, textBounds, HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        public void connectShapes(ICanvas canvas, float startX, float startY, float endX, float endY)
        {
            canvas.DrawLine(startX, startY, endX, endY);
        }

        public void zoomIn() { }
        public void zoomOut() { }

        public static void cleanCanvase(ICanvas canvas)
        {
            
        }
    }
}
