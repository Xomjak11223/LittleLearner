using CfgCompLib.classes;

namespace LittleLearner.CFG
{
    public class FlowchartDrawer : IDrawable
    {
        public enum PositionMarking { TOP, BOTTOM, LEFT, RIGHT, CENTER, NONE }

        public float offsetX = 0;
        public float offsetY = 0;
        public float zoom = 1;
        public Node? tempNode = null;
        private readonly Color Background = new Color(0, 60, 100);

        private readonly Color SelectedBorderColor = new Color(0, 0, 255);
        private readonly Color SelectedFillColor = new Color(0, 0, 100);

        private readonly Color SelectoinBoxBorderColor = new Color(255, 0, 0);
        private readonly Color SelectoinBoxFillColor = new Color(100, 0, 0);

        private readonly Color DefaultBorderColor = new Color(0, 0, 0);
        private readonly Color DefaultFillColor = new Color(255, 255, 255);

        private readonly Color CreationWheelMarkedBorder = new Color(0, 0, 255);
        private readonly Color CreationWheelMarkedArea = new Color(0, 0, 100);
        private readonly Color CreationWheelDefaultBorder = new Color(0, 0, 0);
        private readonly Color CreationWheelDefaultArea = new Color(252, 252, 252);

        private bool drawSelectionBox = false;

        public static readonly float creationInnerRadius = 30;
        public static readonly float creationOuterRadius = 60;
        private readonly float creationWidth = 60;
        private readonly float creationHeight = 25;
        private PositionMarking creationWheelMarking = PositionMarking.NONE;
        private bool creationWheel = false;
        private float creationX, creationY;
        // Order: Top(45°, 135°), Left(135°, 225°), Bottom(225°, 315°), Right(315°, 45°), FullCircle(0°, 360°)
        public static readonly int[] startAngle = { 45, 135, 225, 315, 0 };
        public static readonly int[] endAngle = { 135, 225, 315, 45, 360 };
        float[] radia = { creationOuterRadius, creationOuterRadius, creationOuterRadius, creationOuterRadius, creationInnerRadius };

        public Graph? graph = null;
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Background;
            canvas.FillRectangle(dirtyRect);

            if (graph == null) { return; }
            // Draws every single Shape
            foreach(var nodeIndexPair in graph.GetNodes()) {
                ShapeProperties shape = nodeIndexPair.Value.Shape;
                float shapeStartX = zoom*(shape.x + offsetX);
                float shapeEndX = zoom * (shape.x + shape.width + offsetX);
                float shapeStartY = zoom * (shape.y + offsetY);
                float shapeEndY = zoom * (shape.y + shape.height + offsetY);

                bool shapeNotInDirtyRect = (shapeStartX > dirtyRect.Width) || (shapeEndX < 0) || (shapeStartY > dirtyRect.Height) || (shapeEndY < 0);

                // Draws the connection to its succesor shape
                // TODO Connections need to be shecked in both directions (of the start and end are out of bounds, the connection could still be visible)
                foreach (Node child in nodeIndexPair.Value.GetSuccessors())
                {
                    float childStartX = zoom * (child.Shape.x + offsetX);
                    float childEndX = zoom * (child.Shape.x + child.Shape.width + offsetX);
                    float childStartY = zoom * (child.Shape.y + offsetY);
                    float childEndY = zoom * (child.Shape.y + child.Shape.height + offsetY);

                    bool childNotInDirtyRect = (childStartX > dirtyRect.Width) || (childEndX < 0) || (childStartY > dirtyRect.Height) || (childEndY < 0);

                    if(!shapeNotInDirtyRect || !childNotInDirtyRect) { connectShapes(canvas, shapeStartX, shapeStartY, shape.width * zoom, shape.height * zoom, childStartX, childStartY, child.Shape.width * zoom, child.Shape.height * zoom); }
                }

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
                    case Shape.Start: drawStart(canvas, shapeStartX, shapeStartY, shape.width * zoom, shape.height * zoom); break;
                    case Shape.End: drawEnd(canvas, shapeStartX, shapeStartY, shape.width * zoom, shape.height * zoom); break;
                    case Shape.Action: drawAction(canvas, label, shapeStartX, shapeStartY, shape.width * zoom, shape.height * zoom); break;
                    case Shape.Decision: drawDecision(canvas, label, shapeStartX, shapeStartY, shape.width * zoom, shape.height * zoom); break;
                }
            }

            // Draws the selection Box if needed
            // Does not need to be translated, uses absolute koordinates
            // ERROR when drawing selection box around shape it sometimes does not draw
            if (drawSelectionBox && tempNode != null)
            {
                float x = tempNode.Shape.x;
                float y = tempNode.Shape.y;

                canvas.StrokeColor = SelectoinBoxBorderColor;
                canvas.FillColor = SelectoinBoxFillColor;
                canvas.DrawRectangle(x, y, tempNode.Shape.width, tempNode.Shape.height);
            }

            // Draws the Node that the User wants to currently create
            // TODO may need to be translated
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

            // Draws the Selection Wheel when the user wants to create a new Shape
            if (creationWheel)
            {
                int skip;

                switch (creationWheelMarking)
                {
                    case PositionMarking.TOP: skip = 0; break;
                    case PositionMarking.LEFT: skip = 1; break;
                    case PositionMarking.BOTTOM: skip = 2; break;
                    case PositionMarking.RIGHT: skip = 3; break;
                    case PositionMarking.CENTER: skip = 4; break;
                    default: skip = -1; break;
                }

                canvas.StrokeColor = CreationWheelDefaultBorder;
                canvas.FillColor = CreationWheelDefaultArea;
                for (int i = 0; i < startAngle.Length; i++)
                {
                    if(i == skip) { continue; }
                    canvas.DrawArc(creationX, creationY, radia[i], radia[i], startAngle[i], endAngle[i], false, true);
                }

                if (skip != -1)
                {
                    canvas.StrokeColor = CreationWheelMarkedBorder;
                    canvas.FillColor = CreationWheelMarkedArea;
                    canvas.DrawArc(creationX, creationY, radia[skip], radia[skip], startAngle[skip], endAngle[skip], false, true);
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

        public void drawCircleSlice(ICanvas canvas, float circleX, float circleY, float radius, float startAngle, float endAngle)
        {
            canvas.DrawArc(circleX, circleY, radius, radius, startAngle, endAngle, false, true);
            //canvas.DrawLine();
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

                // Adding offset to every shape
                float shapeStartX = zoom * (shape.x + offsetX);
                float shapeEndX = zoom * (shape.x + shape.width + offsetX);
                float shapeStartY = zoom * (shape.y + offsetY);
                float shapeEndY = zoom * (shape.y + shape.height + offsetY);

                // Case 1: Rectangle is partially in selection
                if (startX <= shapeEndX && endX >= shapeStartX && startY <= shapeEndY && endY >= shapeStartY)
                {
                    nodeIndexPair.Value.Shape.selected = true;
                    continue;
                }

                // Case 2: Rectangle is completely inside selection
                if(shapeStartX >= startX && shapeEndX <= endX && shapeStartY >= startY && shapeEndY <= endY)
                {
                    nodeIndexPair.Value.Shape.selected = true;
                    continue;
                }

                // Case 3: Rectangle surrounds selection
                if (startX >= shapeStartX && endX <= shapeEndX && startY >= shapeStartY && endY <= shapeEndY)
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
            tempNode = new Node(0, null, null, null, new ShapeProperties(startX, startY, width, height, Shape.Action, false));
            drawSelectionBox = true;
        }

        public void hideSelectionArea() { this.drawSelectionBox = false; tempNode = null; }

        public bool pointOnSelected(float x, float y)
        {
            if(graph == null) { return false; }

            foreach(var nodeIndexPair in graph.GetNodes())
            {
                ShapeProperties shape = nodeIndexPair.Value.Shape;
                float shapeStartX = zoom * (shape.x + offsetX);
                float shapeEndX = zoom * (shape.x + shape.width + offsetX);
                float shapeStartY = zoom * (shape.y + offsetY);
                float shapeEndY = zoom * (shape.y + shape.height + offsetY);

                if (x >= shapeStartX && x <= shapeEndX && y >= shapeStartY && y < shapeEndY) { return true; }
            }

            return false;
        }

        public void moveSelected(float dx, float dy)
        {
            foreach (var nodeIndexPair in graph.GetNodes())
            {
                ShapeProperties shape = nodeIndexPair.Value.Shape;
                if (shape.selected) 
                {
                    nodeIndexPair.Value.Shape.x += dx / zoom;
                    nodeIndexPair.Value.Shape.y += dy / zoom;
                }
            }
        }

        public void drawCreationWheel(PositionMarking markedArea, float x, float y)
        {
            creationWheel = true;
            creationX = x;
            creationY = y;
            creationWheelMarking = markedArea;
        }

        public void createNewShape(Shape shape, float x, float y)
        {
            if(graph == null) { return; }

            hideCreationWheel();
            //ShapeProperties shapeProperties = new(x - (creationWidth/2), y - (creationHeight/2), creationWidth, creationHeight, shape);
            //Node newShape = new(0, null, null, null, shapeProperties);
        }

        public void hideCreationWheel() { creationWheel = false; }
    }
}
