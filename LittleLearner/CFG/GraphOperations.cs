using System;
using System.Collections.Generic;
using System.Text;

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
            if(rectStartX > rectEndX)
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

            return (px >= rectStartX && px <= rectEndX && py >= rectStartY && py <= rectEndY);
        }

        public static bool PointAroundLine(float px, float py, float lineStartX, float lineStartY, float lineEndX, float lineEndY, float lambda)
        {
            if (lambda < 0) { throw new ArgumentException("The lambda region around the line can not be negative"); }

            if (lineStartX > lineEndX)
            {
                float temp = lineEndX;
                lineEndX = lineStartX;
                lineStartX = temp;
            }

            if (lineStartY > lineEndY)
            {
                float temp = lineEndY;
                lineEndY = lineStartY;
                lineStartY = temp;
            }

            return PointInRectangle(px, py, lineStartX-lambda, lineStartY-lambda, lineEndX+lambda, lineEndY+lambda);
        }

        public static float AbsolutToRelative(float x, float offset, float zoom) { return (x - offset) * zoom; }
        public static float RelativeToAbsolut(float x, float offset, float zoom) { return (x / zoom) + offset; }
    }
}
