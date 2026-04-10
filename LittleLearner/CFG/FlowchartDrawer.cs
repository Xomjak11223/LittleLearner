using CfgCompLib.classes;

namespace LittleLearner.CFG
{
    public class FlowchartDrawer : IDrawable
    {
        public enum PositionMarking { TOP, BOTTOM, LEFT, RIGHT, CENTER, NONE }

        public float offsetX = 0;
        public float offsetY = 0;
        public float zoom = 1;
        public static readonly int standartFontSize = 16;
        public int fontSize = 16;
        public Node? tempNode = null;
        private readonly Color Background = new Color(0, 60, 100);

        private readonly float widthLowerBound = 40;
        private readonly float heightLowerBound = 20;

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
        private bool creationWheel = false;
        public bool temporaryConnection = false;
        public bool editing = false;

        public static readonly float creationInnerRadius = 15;
        public static readonly float creationOuterRadius = 35;
        private readonly float creationWidth = 60;
        private readonly float creationHeight = 25;
        private PositionMarking creationWheelMarking = PositionMarking.NONE;

        private float creationX, creationY;
        
        public static readonly int[] startAngle = { 45, 135, 225, 315};
        public static readonly int[] endAngle = { 135, 225, 315, 45};

        public float connectionStartX, connectionStartY, connectionEndX, connectionEndY;

        public PositionMarking editingEdge1 = PositionMarking.NONE;
        public PositionMarking editingEdge2 = PositionMarking.NONE;

        public Graph? graph = null;
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Background;
            canvas.FillRectangle(dirtyRect);

            if (graph != null) 
            {
                // Draws every single Shape
                foreach(var nodeIndexPair in graph.GetNodes()) {
                    ShapeProperties shape = nodeIndexPair.Value.Shape;
                    float shapeStartX = absoluteToRelativeX(shape.x);
                    float shapeEndX = absoluteToRelativeX(shape.x + shape.width);
                    float shapeStartY = absoluteToRelativeY(shape.y);
                    float shapeEndY = absoluteToRelativeY(shape.y + shape.height);
                    float startCenterX = shapeStartX + (shape.width * zoom / 2);
                    float startCenterY = shapeStartY + (shape.height * zoom / 2);

                    bool shapeNotInDirtyRect = (shapeStartX > dirtyRect.Width) || (shapeEndX < 0) || (shapeStartY > dirtyRect.Height) || (shapeEndY < 0);

                    // Draws the connection to its succesor shape
                    // TODO Connections need to be shecked in both directions (of the start and end are out of bounds, the connection could still be visible)
                    foreach (Node child in nodeIndexPair.Value.GetSuccessors())
                    {
                        float childStartX = absoluteToRelativeX(child.Shape.x);
                        float childEndX = absoluteToRelativeX(child.Shape.x + child.Shape.width);
                        float childStartY = absoluteToRelativeY(child.Shape.y);
                        float childEndY = absoluteToRelativeY(child.Shape.y + child.Shape.height);
                        float endCenterX = childStartX + (child.Shape.width * zoom / 2);
                        float endCenterY = childStartY + (child.Shape.height * zoom / 2);

                        bool childNotInDirtyRect = (childStartX > dirtyRect.Width) || (childEndX < 0) || (childStartY > dirtyRect.Height) || (childEndY < 0);

                        if(!shapeNotInDirtyRect || !childNotInDirtyRect) { drawConnection(canvas, startCenterX, startCenterY, endCenterX, endCenterY); }
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

                // unselected circle slices
                canvas.StrokeColor = CreationWheelDefaultBorder;
                canvas.FillColor = CreationWheelDefaultArea;
                for (int i = 0; i < startAngle.Length; i++)
                {
                    if(i == skip) { continue; }
                    drawCircleSlice(canvas, creationX, creationY, creationOuterRadius, startAngle[i], endAngle[i]);
                }

                // selected circle slice
                if (skip >= 0 && skip < 4)
                {
                    canvas.StrokeColor = CreationWheelMarkedBorder;
                    canvas.FillColor = CreationWheelMarkedArea;
                    drawCircleSlice(canvas, creationX, creationY, creationOuterRadius, startAngle[skip], endAngle[skip]);
                }

                // inner circle (selected or unselected)
                if (skip == 4) 
                {
                    canvas.StrokeColor = CreationWheelMarkedBorder;
                    canvas.FillColor = CreationWheelMarkedArea;
                }
                else 
                {
                    canvas.StrokeColor = CreationWheelDefaultBorder;
                    canvas.FillColor = CreationWheelDefaultArea;
                }
                canvas.FillCircle(creationX, creationY, creationInnerRadius);
            }

            if (editing && tempNode != null)
            {
                float startX = absoluteToRelativeX(tempNode.Shape.x);
                float startY = absoluteToRelativeY(tempNode.Shape.y);
                float endX = absoluteToRelativeX(tempNode.Shape.x + tempNode.Shape.width);
                float endY = absoluteToRelativeY(tempNode.Shape.y + tempNode.Shape.height);

                canvas.StrokeColor = SelectedBorderColor;
                if (editingEdge1 == PositionMarking.TOP || editingEdge2 == PositionMarking.TOP){ canvas.DrawLine(startX, startY, endX, startY); }
                if (editingEdge1 == PositionMarking.BOTTOM || editingEdge2 == PositionMarking.BOTTOM){ canvas.DrawLine(startX, endY, endX, endY); }
                if (editingEdge1 == PositionMarking.LEFT || editingEdge2 == PositionMarking.LEFT){ canvas.DrawLine(startX, startY, startX, endY); }
                if (editingEdge1 == PositionMarking.RIGHT || editingEdge2 == PositionMarking.RIGHT){ canvas.DrawLine(endX, startY, endX, endY); }
            }

            if (temporaryConnection){ drawConnection(canvas, connectionStartX, connectionStartY, connectionEndX, connectionEndY); }
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

            canvas.FontSize = fontSize;
            canvas.DrawString(text, textBounds, HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        public void drawConnection(ICanvas canvas, float startX, float startY, float endX, float endY)
        {
            canvas.DrawLine(startX, startY, endX, startY);
            canvas.DrawLine(endX, startY, endX, endY);
        }

        public void drawCircleSlice(ICanvas canvas, float circleX, float circleY, float radius, float startAngle, float endAngle)
        {
            int sign;
            if((startAngle % 360 >= 0 && startAngle % 360 <= 90) || (startAngle % 360 >= 180 && startAngle % 360 <= 270)){ sign = -1; }
            else { sign = 1; }

            float endX = (float)(sign*Math.Cos((startAngle * Math.PI) / 180) * radius) + circleX;
            float endY = (float)(sign*Math.Sin((startAngle * Math.PI) / 180) * radius) + circleY;
            canvas.DrawLine(circleX, circleY, endX, endY);

            endX = (float)(sign*Math.Cos((endAngle * Math.PI) / 180) * radius) + circleX;
            endY = (float)(sign*Math.Sin((endAngle * Math.PI) / 180) * radius) + circleY;
            canvas.DrawLine(circleX, circleY, endX, endY);

            canvas.DrawArc(circleX - radius, circleY - radius, radius * 2, radius * 2, startAngle, endAngle, false, true);
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
                float shapeStartX = absoluteToRelativeX(shape.x);
                float shapeEndX = absoluteToRelativeX(shape.x + shape.width);
                float shapeStartY = absoluteToRelativeY(shape.y);
                float shapeEndY = absoluteToRelativeY(shape.y + shape.height);

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

        public Node? pointOnSelected(float x, float y){ return pointHitsElement(x, y, true); }
        public Node? pointOnElement(float x, float y){ return pointHitsElement(x, y, false); }
        private Node? pointHitsElement(float x, float y, bool elementHasToBeSelected)
        {
            if (graph == null) { return null; }

            foreach (var nodeIndexPair in graph.GetNodes())
            {
                ShapeProperties shape = nodeIndexPair.Value.Shape;
                float shapeStartX = absoluteToRelativeX(shape.x);
                float shapeEndX = absoluteToRelativeX(shape.x + shape.width);
                float shapeStartY = absoluteToRelativeY(shape.y);
                float shapeEndY = absoluteToRelativeY(shape.y + shape.height);

                if (x >= shapeStartX && x <= shapeEndX && y >= shapeStartY && y < shapeEndY) {
                    if (!elementHasToBeSelected) { return nodeIndexPair.Value; }
                    else if (shape.selected) { return nodeIndexPair.Value; }
                }
            }

            return null;
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
            if(graph == null) { graph = new Graph(); }

            float transformedX = (x/zoom) - (creationWidth / 2);
            float transformedY = (y/zoom) - (creationHeight / 2);

            ShapeProperties shapeProperties = new(transformedX, transformedY, creationWidth, creationHeight, shape, false);
            graph.AddNode(new Node(graph.GetNewID(), ["Filler"], null, null, shapeProperties));
        }

        public void hideCreationWheel() { creationWheel = false; }

        public void hideTemporaryConnection() { temporaryConnection = false; }
        public void createTemporaryConnection(float startX, float startY, float endX, float endY)
        {
            connectionStartX = startX;
            connectionStartY = startY;
            connectionEndX = endX;
            connectionEndY = endY;
            temporaryConnection = true;
        }

        public void setNodeSelection(int id, bool selected)
        {
            Node? node = graph.GetNode(id);
            if (node == null) { return; }

            node.Shape.selected = selected;
        }

        public void connectNodes(int parentID, int childID)
        {
            if(graph == null) { return; }

            Node? parentNode = graph.GetNode(parentID);
            Node? childNode = graph.GetNode(childID);
            if (parentNode == null) { return; }
            if (childNode == null) { return; }

            graph.AddEdge(parentNode, childNode);
        }


        public void scaleCanvas(float newZoom)
        {
            if (graph == null) { return; }

            if (newZoom >= 2) newZoom = 2;
            if (newZoom <= 0.5) newZoom = (float)0.5;

            zoom = newZoom;
            fontSize = (int) (standartFontSize * newZoom);
        }

        public void hideEditing() { editing = false; }
        public void selectEditingNode(Node node, PositionMarking p1, PositionMarking p2)
        {
            editing = true;
            tempNode = node;
            editingEdge1 = p1;
            editingEdge2 = p2;
        }

        public void scaleNode(int nodeID, PositionMarking p1, PositionMarking p2, float dx, float dy)
        {
            if(graph == null) { return; }
            
            // Checks wether position markings are opposite of each other (Which should not be allowed)
            if((p1 == PositionMarking.TOP && p2 == PositionMarking.BOTTOM) || (p1 == PositionMarking.BOTTOM && p2 == PositionMarking.TOP)) { return; }
            if ((p1 == PositionMarking.LEFT && p2 == PositionMarking.RIGHT) || (p1 == PositionMarking.RIGHT && p2 == PositionMarking.LEFT)) { return; }

            Node node = graph.GetNode(nodeID);

            if (node == null) { return; }

            if(p1 == PositionMarking.TOP || p2 == PositionMarking.TOP)
            { 
                if(node.Shape.height - dy >= heightLowerBound) { 
                    node.Shape.y += dy;
                    node.Shape.height -= dy;
                }
            }

            if (p1 == PositionMarking.BOTTOM || p2 == PositionMarking.BOTTOM)
            {
                if (node.Shape.height + dy >= heightLowerBound) { node.Shape.height += dy; }
            }

            if (p1 == PositionMarking.LEFT || p2 == PositionMarking.LEFT)
            {
                if (node.Shape.width - dx >= widthLowerBound) { 
                    node.Shape.x += dx;
                    node.Shape.width -= dx;
                }
            }

            if (p1 == PositionMarking.RIGHT || p2 == PositionMarking.RIGHT)
            {
                if (node.Shape.width + dx >= widthLowerBound) { node.Shape.width += dx; }
            }
        }

        public float absoluteToRelativeX(float x) { return (x + offsetX) * zoom; }
        public float absoluteToRelativeY(float y) { return (y + offsetY) * zoom; }
        public float relativeToAbsoluteX(float x) { return (x - offsetX) / zoom; }
        public float relativeToAbsoluteY(float y) { return (y - offsetY) / zoom; }
    }
}
