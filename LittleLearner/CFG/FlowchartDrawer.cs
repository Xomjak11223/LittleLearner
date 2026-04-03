using CfgCompLib.classes;
using Microsoft.Maui.Graphics.Text;
using System;
using System.Collections.Generic;
using System.Text;
using static Antlr4.Runtime.Atn.SemanticContext;
using static System.Net.Mime.MediaTypeNames;

namespace LittleLearner.CFG
{
    public class FlowchartDrawer : IDrawable
    {
        private bool drawSelectionBox = false;
        public float offsetX = 0;
        public float offsetY = 0;
        public Node? tempNode = null;
        private float zoom = 1;
        private readonly Color Background = new Color(0, 60, 100);

        private readonly Color SelectedBorderColor = new Color(0, 0, 255);
        private readonly Color SelectedFillColor = new Color(0, 0, 100);

        private readonly Color SelectoinBoxBorderColor = new Color(255, 0, 0);
        private readonly Color SelectoinBoxFillColor = new Color(100, 0, 0);

        private readonly Color DefaultBorderColor = new Color(0, 0, 0);
        private readonly Color DefaultFillColor = new Color(255, 255, 255);

        public Graph graph = null;
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Background;
            canvas.FillRectangle(dirtyRect);

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

                    if(!shapeNotInDirtyRect || !childNotInDirtyRect) { connectShapes(canvas, x, y, shape.width, shape.height, childX, childY, child.Shape.width, child.Shape.height); }
                }

                // Check if shape is out of drawable area
                if (shapeNotInDirtyRect) { continue; }

                var labels = nodeIndexPair.Value.GetLabel();
                string label = "NO LABEL";
                if (labels != null)
                {
                    if(labels.Count != 0) label = labels[0];
                }

                if (shape.selected) 
                {
                    canvas.StrokeColor = SelectedBorderColor;
                    canvas.FillColor = SelectedFillColor;
                }
                else 
                {
                    canvas.StrokeColor = DefaultBorderColor;
                    canvas.FillColor = DefaultFillColor;
                }

                // Draws the Flow Graph Shape
                switch (shape.shape) {
                    case Shape.Start: drawStart(canvas, x, y, shape.width, shape.height); break;
                    case Shape.End: drawEnd(canvas, x, y, shape.width, shape.height); break;
                    case Shape.Action: drawAction(canvas, label, x, y, shape.width, shape.height); break;
                    case Shape.Decision: drawDecision(canvas, label, x, y, shape.width, shape.height); break;
                }
            }

            // Draws the selection Box if needed
            if (drawSelectionBox && tempNode != null)
            {
                float x = tempNode.Shape.x + offsetX;
                float y = tempNode.Shape.y + offsetY;

                canvas.StrokeColor = SelectoinBoxBorderColor;
                canvas.FillColor = SelectoinBoxFillColor;
                canvas.DrawRectangle(x, y, tempNode.Shape.width, tempNode.Shape.height);
            }

            // Draws the Node that the User wants to currently create
            if (false && tempNode != null)
            {
                switch (tempNode.Shape.shape)
                {
                    case Shape.Start: drawStart(canvas, tempNode.Shape.x, tempNode.Shape.y, tempNode.Shape.width, tempNode.Shape.height); break;
                    case Shape.End: drawEnd(canvas, tempNode.Shape.x, tempNode.Shape.y, tempNode.Shape.width, tempNode.Shape.height); break;
                    case Shape.Action: drawAction(canvas, "myText", tempNode.Shape.x, tempNode.Shape.y, tempNode.Shape.width, tempNode.Shape.height); break;
                    case Shape.Decision: drawDecision(canvas, "myText", tempNode.Shape.x, tempNode.Shape.y, tempNode.Shape.width, tempNode.Shape.height); break;
                }
            }
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

        public void connectShapes(ICanvas canvas, float startX, float startY, float startWidth, float startHeight, float endX, float endY, float endWidth, float endHeight)
        {
            float startCenterX = startX + (startWidth / 2);
            float startCenterY = startY + (startHeight / 2);
            float endCenterX = endX + (endWidth / 2);
            float endCenterY = endY + (endHeight / 2);

            canvas.DrawLine(startCenterX, startCenterY, endCenterX, startCenterY);
            canvas.DrawLine(endCenterX, startCenterY, endCenterX, endCenterY);
        }

        public void zoomIn() { }
        public void zoomOut() { }

        public static void cleanCanvase(ICanvas canvas)
        {
            
        }

        public void selectShapesInArea(float startX, float startY, float endX, float endY)
        {
            if (graph == null) return;

            // Translates selection, such that
            // (startX, startY) is the upper left corner and
            // (endX, endY) is the lower right corner
            if(startX > endX)
            {
                float temp = startX;
                startX = endX;
                endX = temp;
            }

            if (startY > endY)
            {
                float temp = startY;
                startY = endY;
                endY = temp;
            }

            foreach (var nodeIndexPair in graph.GetNodes())
            {
                ShapeProperties shape = nodeIndexPair.Value.Shape;

                // Case 1: Rectangle is partially in selection
                if (startX <= (shape.x + shape.width) && endX >= shape.x && startY <= (shape.y + shape.height) && endY >= shape.y)
                {
                    nodeIndexPair.Value.Shape.selected = true;
                    continue;
                }

                // Case 2: Rectangle is completely inside selection
                if(shape.x >= startX && (shape.x + shape.width) <= endX && shape.y >= startY && (shape.y + shape.height) <= endY)
                {
                    nodeIndexPair.Value.Shape.selected = true;
                    continue;
                }

                // Case 3: Rectangle surrounds selection
                if (startX >= shape.x && endX <= (shape.x + shape.width) && startY >= shape.y && endY <= (shape.y + shape.height))
                {
                    nodeIndexPair.Value.Shape.selected = true;
                    continue;
                }

                nodeIndexPair.Value.Shape.selected = false;
            }
        }

        public void drawSelectionArea(float startX, float startY, float endX, float endY)
        {
            float width = (endX == startX) ? ((float) 0.1) : (endX - startX);
            float height = (endY == startY) ? ((float)0.1) : (endY - startY);

            // Shape as a property is irrelevant, chose Shape.Action as filler
            tempNode = new Node(0, null, null, null, new ShapeProperties(startX, startY, width, height, Shape.Action));
            drawSelectionBox = true;
        }

        public void hideSelectionArea() { this.drawSelectionBox = false; tempNode = null; }
    }
}
