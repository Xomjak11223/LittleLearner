namespace LittleLearner.CFG
{
    public class GraphOperations
    {
        /// <summary>
        /// Checks if the provided point is in the provided rectangle
        /// </summary>
        /// <param name="px">x coordinate of point</param>
        /// <param name="py">y coordinate of point</param>
        /// <param name="rectStartX">x position on one end of the rectangles diagonal</param>
        /// <param name="rectStartY">y position on one end of the rectangles diagonal</param>
        /// <param name="rectEndX">x position on the opposite end of the rectangles diagonal</param>
        /// <param name="rectEndY">y position on the opposite end of the rectangles diagonal</param>
        /// <returns>true, if the point is inside of the rectangle, otherwise flase</returns>
        /// <remarks>
        /// The rectangles startpoint is allways translated to its top left corner
        /// The rectangles endpoint is allways translated to its bottom right corner
        /// </remarks>
        public static bool PointInRectangle(float px, float py, float rectStartX, float rectStartY, float rectEndX, float rectEndY)
        {
            #region Transform rectangle Startpoint to upperLeft Corner and rectangle Endpoint to lowerRight Corner and 
            if (rectStartX > rectEndX)
            {
                float temp = rectEndX;
                rectEndX = rectStartX;
                rectStartX = temp;
            }

            if(rectStartY > rectEndY)
            {
                float temp = rectEndY;
                rectEndY = rectStartY;
                rectStartY = temp;
            }
            #endregion

            return (px >= rectStartX && px <= rectEndX && py >= rectStartY && py <= rectEndY);
        }

        public static bool PointAroundLine(float px, float py, float lineStartX, float lineStartY, float lineEndX, float lineEndY, float lambda)
        {
            if (lambda < 0) { throw new ArgumentException("The lambda region around the line can not be negative"); }

            #region Transform line Startpoint to furthest left Point and line Endpoint to furthest right point
            if (lineStartX > lineEndX)
            {
                float temp = lineEndX;
                lineEndX = lineStartX;
                lineStartX = temp;

                temp = lineEndY;
                lineEndY = lineStartY;
                lineStartY = temp;
            }
            #endregion

            return PointInRectangle(px, py, lineStartX-lambda, lineStartY-lambda, lineEndX+lambda, lineEndY+lambda);
        }

        public static bool RectanglesIntersect(float r1StartX, float r1StartY, float r1EndX, float r1EndY, float r2StartX, float r2StartY, float r2EndX, float r2EndY)
        {
            #region Transforms both rectangles startPoint to be upperLeft Corner und endPoint to bo lowerRight Corner
            if (r1StartX > r1EndX)
            {
                float temp = r1EndX;
                r1EndX = r1StartX;
                r1StartX = temp;
            }

            if (r1StartY > r1EndY)
            {
                float temp = r1EndY;
                r1EndY = r1StartY;
                r1StartY = temp;
            }

            if (r2StartX > r2EndX)
            {
                float temp = r2EndX;
                r2EndX = r2StartX;
                r2StartX = temp;
            }

            if (r2StartY > r2EndY)
            {
                float temp = r2EndY;
                r2EndY = r2StartY;
                r2StartY = temp;
            }
            #endregion

            Rect rectangle1 = new Rect(r1StartX, r1StartY, Math.Abs(r1EndX - r1StartX), Math.Abs(r1EndY - r1StartY));
            Rect rectangle2 = new Rect(r2StartX, r2StartY, Math.Abs(r2EndX - r2StartX), Math.Abs(r2EndY - r2StartY));

            return rectangle1.IntersectsWith(rectangle2);
        }

        public static bool LineIntersectsRectangle(float rectangleStartX, float rectangleStartY, float rectangleEndX, float rectangleEndY, float lineStartX, float lineStartY, float lineEndX, float lineEndY)
        {
            #region transforms line startPoint to furthest left point and endPoint to be furthest right point and rectangle startPoint to be upperLeft corner and endpont to be lowerRight corner
            if (rectangleStartX > rectangleEndX)
            {
                float temp = rectangleEndX;
                rectangleEndX = rectangleStartX;
                rectangleStartX = temp;
            }

            if (rectangleStartY > rectangleEndY)
            {
                float temp = rectangleEndY;
                rectangleEndY = rectangleStartY;
                rectangleStartY = temp;
            }

            if (lineStartX > lineEndX)
            {
                float temp = lineEndX;
                lineEndX = lineStartX;
                lineStartX = temp;

                temp = lineEndY;
                lineEndY = lineStartY;
                lineStartY = temp;
            }
            #endregion

            if (PointInRectangle(lineStartX, lineStartX, rectangleStartX, rectangleStartY, rectangleEndX, rectangleEndY)
               || PointInRectangle(lineEndX, lineEndX, rectangleStartX, rectangleStartY, rectangleEndX, rectangleEndY))
            {
                return true;
            }

            // Intersects with either: leftEdge, topEdge, rightEdge or bottomEdge
            return LinesIntersect(lineStartX, lineStartY, lineEndX, lineEndY, rectangleStartX, rectangleStartY, rectangleStartX, rectangleEndY)
                   || LinesIntersect(lineStartX, lineStartY, lineEndX, lineEndY, rectangleStartX, rectangleStartY, rectangleEndX, rectangleStartY)
                   || LinesIntersect(lineStartX, lineStartY, lineEndX, lineEndY, rectangleEndX, rectangleStartY, rectangleEndX, rectangleEndY)
                   || LinesIntersect(lineStartX, lineStartY, lineEndX, lineEndY, rectangleStartX, rectangleEndY, rectangleEndX, rectangleEndY);
        }

        public static bool LinesIntersect(float l1StartX, float l1StartY, float l1EndX, float l1EndY, float l2StartX, float l2StartY, float l2EndX, float l2EndY)
        {
            #region transforms both lines startPoint to furthest left point and endPoint to be furthest right point
            if (l1StartX > l1EndX)
            {
                float temp = l1EndX;
                l1EndX = l1StartX;
                l1StartX = temp;

                temp = l1EndY;
                l1EndY = l1StartY;
                l1StartY = temp;
            }

            if (l2StartX > l2EndX)
            {
                float temp = l2EndX;
                l2EndX = l2StartX;
                l2StartX = temp;

                temp = l2EndY;
                l2EndY = l2StartY;
                l2StartY = temp;
            }

            // In case a line is a vertical line, make the point the smaller one
            if(l1StartX == l1EndX && l1StartY > l1EndY)
            {
                float temp = l1EndY;
                l1EndY = l1StartY;
                l1StartY = temp;
            }

            if (l2StartX == l2EndX && l2StartY > l2EndY)
            {
                float temp = l2EndY;
                l2EndY = l2StartY;
                l2StartY = temp;
            }
            #endregion

            #region cases in which either lines are points or horizontal / vertical lines
            // Both Lines are Points
            if (l1StartX == l1EndX && l1StartY == l1EndY && l2StartX == l2EndX && l2StartY == l2EndY) { return l1StartX == l2StartX && l1StartY == l2StartY; }

            // l1 is a vertical line
            if (l1StartX == l1EndX)
            {
                // l2 is a vertical line
                if(l2StartX == l2EndX) {
                    return l1StartX == l2StartX && (
                        PointOnLine(l1StartX, l1StartY, l2StartX, l2StartY, l2EndX, l2EndY)
                        || PointOnLine(l1StartX, l1EndY, l2StartX, l2StartY, l2EndX, l2EndY)
                        || PointOnLine(l2StartX, l2StartY, l1StartX, l1StartY, l1EndX, l1EndY)
                        || PointOnLine(l2StartX, l2EndY, l1StartX, l1StartY, l1EndX, l1EndY)
                        || (l1StartY < l2StartY && l1EndY < l2EndY)
                        || (l2StartY < l1StartY && l2EndY < l1EndY));
                }

                // l2 is a horizontal line
                if (l2StartY == l2EndY) { return l2StartX <= l1StartX && l2EndX >= l1StartX && l2StartY >= l1StartY && l2StartY <= l1EndY; }

                // l2 is neither horizontal nor vertical
                // Using the formula y = mx + b we can determin the point l2 intersects at l1StartX, then we check if that point is on l1
                // TODO check, weather the point is on the line or not
                float slope = (l2EndY - l2StartY) / (l2EndX - l2StartX);
                float y = slope * (l1StartX - l2StartX) + l2StartY;
                return (y >= l1StartY && y <= l1EndY);
            }

            // l1 is a horizontal line
            if (l1StartY == l1EndY)
            {
                // l2 is a vertical line
                if (l2StartX == l2EndX) { return l1StartX <= l2StartX && l1EndX >= l2StartX && l1StartY >= l2StartY && l1StartY <= l2EndY; }

                // l2 is a horizontal line
                if (l2StartY == l2EndY) {
                    return l1StartY == l2StartY && (
                        PointOnLine(l1StartX, l1StartY, l2StartX, l2StartY, l2EndX, l2EndY)
                        || PointOnLine(l1StartX, l1EndY, l2StartX, l2StartY, l2EndX, l2EndY)
                        || PointOnLine(l2StartX, l2StartY, l1StartX, l1StartY, l1EndX, l1EndY)
                        || PointOnLine(l2StartX, l2EndY, l1StartX, l1StartY, l1EndX, l1EndY)
                        || (l1StartX < l2StartX && l1EndX < l2EndX)
                        || (l2StartX < l1StartX && l2EndX < l1EndX));
                }

                // l2 is neither horizontal nor vertical
                // Using the formula y = mx + b we can determini the point l2 intersects at l1StartX, then we check if that point is on l1
                float slope = (l2EndY - l2StartY) / (l2EndX - l2StartX);
                float x = (l1StartY - l2StartY - slope * l2StartX) / slope;
                return (x >= l1StartX && x <= l1EndX);
            }

            // At this point l1 is neither a horizontal nor vertical Line \\

            // l2 is a horizontal line
            if (l2StartX == l2EndX)
            {
                // l1 is neither horizontal nor vertical
                // Using the formula y = mx + b we can determini the point l2 intersects at l1StartX, then we check if that point is on l1
                float slope = (l1EndY - l1StartY) / (l1EndX - l1StartX);
                float y = slope * (l2StartX - l1StartX) + l1StartY;
                return (y >= l2StartY && y <= l2EndY);
            }

            // l2 is a vertical line
            if (l2StartY == l2EndY)
            {
                // l1 is neither horizontal nor vertical
                // Using the formula y = mx + b we can determini the point l2 intersects at l1StartX, then we check if that point is on l1
                float slope = (l1EndY - l1StartY) / (l1EndX - l1StartX);
                float x = (l2StartY - l1StartY - slope * l1StartX) / slope;
                return (x >= l2StartX && x <= l2EndX);
            }
            #endregion

            // Neither line is horizontal or vertical
            double n = (double)((l2StartX - l1StartX)*(l1EndY - l1StartY) + (l1StartY - l2StartY)*(l1StartX - l1EndX)) / ((l2StartX - l2EndX)*(l1EndY - l1StartY) + (l2EndY - l2StartY)*(l1StartX - l1EndX));
            if(n <= 0 || n > 1) return false;

            double m = ((l2StartX - l1StartX) - n) / (l1EndX - l1StartX);
            return (m > 0 && m <= 1);
        }

        public static bool PointOnLine(float px, float py, float lineStartX, float lineStartY, float lineEndX, float lineEndY)
        {
            #region Transform line Startpoint to furthest left Point and line Endpoint to furthest right point
            if (lineStartX > lineEndX)
            {
                float temp = lineEndX;
                lineEndX = lineStartX;
                lineStartX = temp;

                temp = lineEndY;
                lineEndY = lineStartY;
                lineStartY = temp;
            }
            #endregion

            // Base Case: Line starts and ends on the same point its either a horizontal or vertical line
            if (lineStartX == lineEndX) { return px == lineStartX && py >= lineStartY && py <= lineEndY; }
            if(lineStartY == lineEndY) { return py == lineStartY && px >= lineStartX && px <= lineEndX; }

            double n1 = (px - lineStartX) / (lineEndX - lineStartX);
            double n2 = (py - lineStartY) / (lineEndY - lineStartY);
            return n1 == n2 && n1 >= 0 && n1 <= 1;
        }

        public static float AbsolutToRelative(float x, float offset, float zoom) { return (x - offset) * zoom; }
        public static float RelativeToAbsolut(float x, float offset, float zoom) { return (x / zoom) + offset; }
    }
}