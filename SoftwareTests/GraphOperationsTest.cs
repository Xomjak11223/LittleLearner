using LittleLearner.CFG;

namespace SoftwareTests
{
    public class GraphOperationsTest
    {
        public class TestPointInRectangleFunction
        {
            [Theory]
            [InlineData(5, 5, 0, 0, 10, 10)] // Middle of rectangle
            [InlineData(7, 3, 0, 0, 10, 10)] // Random point in rectangle
            [InlineData(0, 0, 0, 0, 10, 10)] // Upper left corner of rectangle
            [InlineData(10, 10, 0, 0, 10, 10)] // Lower right corner of rectangle
            [InlineData(5, 10, 0, 0, 10, 10)] // Middle of bottom edge of rectangle
            [InlineData(10, 10, 10, 10, 10, 10)] // Rectangle is a Point
            public void TestPointInRectangleTrue(float px, float py, float rectStartX, float rectStartY, float rectEndX, float rectEndY)
            {
                bool outcome = GraphOperations.PointInRectangle(px, py, rectStartX, rectStartY, rectEndX, rectEndY);
                Assert.True(outcome);
            }

            [Theory]
            [InlineData(5, 15, 10, 10, 20, 20)] // Left of rectangle
            [InlineData(25, 15, 10, 10, 20, 20)] // Right of rectangle
            [InlineData(15, 5, 10, 10, 20, 20)] // Above rectangle
            [InlineData(15, 25, 10, 10, 20, 20)] // Below rectangle

            [InlineData(5, 5, 10, 10, 20, 20)] // Top left of rectangle
            [InlineData(25, 5, 10, 10, 20, 20)] // Top right of rectangle
            [InlineData(5, 25, 10, 10, 20, 20)] // Bottom left of rectangle
            [InlineData(25, 25, 10, 10, 20, 20)] // Bottom right of rectangle

            public void TestPointInRectangleFalse(float px, float py, float rectStartX, float rectStartY, float rectEndX, float rectEndY)
            {
                bool outcome = GraphOperations.PointInRectangle(px, py, rectStartX, rectStartY, rectEndX, rectEndY);
                Assert.False(outcome);
            }

            [Theory]
            [InlineData(15, 15, 20, 20, 10, 10)] // startPoint = bottomRight, endPoint = topLeft
            [InlineData(15, 15, 20, 10, 10, 20)] // startPoint = topRight, endPoint = bottomLeft
            [InlineData(15, 15, 10, 20, 20, 10)] // startPoint = bottomLeft, endPoint = topRight
            [InlineData(15, 10, 20, 10, 10, 10)] // startPoint = topLeft, endPoint = topRight (Rectangle is a line)
            public void TestPointInRectangleMixedInputsTrue(float px, float py, float rectStartX, float rectStartY, float rectEndX, float rectEndY)
            {
                bool outcome = GraphOperations.PointInRectangle(px, py, rectStartX, rectStartY, rectEndX, rectEndY);
                Assert.True(outcome);
            }

            [Theory]
            [InlineData(5, 5, 20, 20, 10, 10)] // Left of rectangle
            [InlineData(5, 15, 20, 10, 10, 20)] // Above rectangle
            [InlineData(25, 15, 10, 20, 20, 10)] // Above rectangle
            [InlineData(5, 10, 20, 10, 10, 10)] // Above rectangle
            public void TestPointInRectangleMixedInputsFalse(float px, float py, float rectStartX, float rectStartY, float rectEndX, float rectEndY)
            {
                bool outcome = GraphOperations.PointInRectangle(px, py, rectStartX, rectStartY, rectEndX, rectEndY);
                Assert.False(outcome);
            }

        }
        public class TranslatePointFunctions
        {
            [Theory]
            [InlineData(5, 5, 1, 0)] // No transformation, point should stay the same
            [InlineData(5, 2.5, 0.5, 0)] // Zoomed out, point should have double the distance from the origin
            [InlineData(5, 10, 2, 0)] // Zoomed in, point should have halfe the distance from the origin
            [InlineData(5, 4, 1, 1)] // Offsets the point by 1 towards the origin
            [InlineData(5, 6, 1, -1)] // Offsets the point by 1 away from the origin
            [InlineData(5, 1, 0.5, 3)] // Zoomed out and moved the point 3 towards the origin
            [InlineData(5, 12, 2, -1)] // Zoomed in and moved the point 1 away the origin
            public void TestAbsolutToRelative(float beforeTransform, float afterTransform, float zoom, float offset)
            {
                Assert.Equal(afterTransform, GraphOperations.AbsolutToRelative(beforeTransform, offset, zoom));
            }

            [Theory]
            [InlineData(5, 5, 1, 0)] // No transformation, point should stay the same
            [InlineData(2.5, 5, 0.5, 0)] // Zoomed out, point should have double the distance from the origin
            [InlineData(10, 5, 2, 0)] // Zoomed in, point should have halfe the distance from the origin
            [InlineData(4, 5, 1, 1)] // Offsets the point by 1 towards the origin
            [InlineData(6, 5, 1, -1)] // Offsets the point by 1 away from the origin
            [InlineData(1, 5, 0.5, 3)] // Zoomed out and moved the point 3 towards the origin
            [InlineData(12, 5, 2, -1)] // Zoomed in and moved the point 1 away the origin
            public void TestRelativeToAbsolut(float beforeTransform, float afterTransform, float zoom, float offset)
            {
                Assert.Equal(afterTransform, GraphOperations.RelativeToAbsolut(beforeTransform, offset, zoom));
            }
        }
        public class PointAroundLineFunction
        {
            [Theory]
            // Horizontal line
            [InlineData(15, 10, 10, 10, 20, 10, 2)]
            [InlineData(8, 10, 10, 10, 20, 10, 2)]
            [InlineData(21, 8, 10, 10, 20, 10, 2)]
            [InlineData(13, 12, 10, 10, 20, 10, 2)]

            // Vertical Line
            [InlineData(10, 8, 10, 10, 10, 20, 2)]
            [InlineData(11, 13, 10, 10, 10, 20, 2)]
            [InlineData(8, 20, 10, 10, 10, 20, 2)]
            [InlineData(12, 22, 10, 10, 10, 20, 2)]

            // Any Line
            [InlineData(8, 8, 10, 10, 20, 20, 2)] // Upper left corner
            [InlineData(8, 12, 10, 10, 20, 20, 2)] // Lower left corner
            [InlineData(22, 18, 10, 10, 20, 20, 2)] // Upper right corner
            [InlineData(22, 22, 10, 10, 20, 20, 2)] // Lower right corner
            public void PointAroundLineTrue(float px, float py, float lineStartX, float lineStartY, float lineEndX, float lineEndY, float lambda)
            {
                bool outcome = GraphOperations.PointAroundLine(px, py, lineStartX, lineStartY, lineEndX, lineEndY, lambda);
                Assert.True(outcome);
            }

            [Theory]
            // Horizontal line
            [InlineData(15, 7, 10, 10, 20, 10, 2)]
            [InlineData(7, 10, 10, 10, 20, 10, 2)]
            [InlineData(23, 8, 10, 10, 20, 10, 2)]
            [InlineData(13, 13, 10, 10, 20, 10, 2)]

            // Vertical Line
            [InlineData(10, 7, 10, 10, 10, 20, 2)]
            [InlineData(13, 13, 10, 10, 10, 20, 2)]
            [InlineData(7, 20, 10, 10, 10, 20, 2)]
            [InlineData(12, 23, 10, 10, 10, 20, 2)]

            // Any Line
            [InlineData(7, 7, 10, 10, 20, 20, 2)]
            [InlineData(7, 13, 10, 10, 20, 20, 2)]
            [InlineData(23, 17, 10, 10, 20, 20, 2)]
            [InlineData(23, 23, 10, 10, 20, 20, 2)]
            public void PointAroundLineFalse(float px, float py, float lineStartX, float lineStartY, float lineEndX, float lineEndY, float lambda)
            {
                bool outcome = GraphOperations.PointAroundLine(px, py, lineStartX, lineStartY, lineEndX, lineEndY, lambda);
                Assert.False(outcome);
            }

            [Fact]
            public void PointAroundLineNegativeLambda()
            {
                Assert.Throws<ArgumentException>(() => GraphOperations.PointAroundLine(15, 10, 10, 10, 20, 10, -1));
            }
        }
        public class PointOnLine
        {
            [Theory]
            [InlineData(10, 10, 10, 10, 20, 20)]
            [InlineData(20, 20, 10, 10, 20, 20)]
            [InlineData(15, 15, 10, 10, 20, 20)]

            [InlineData(10, 10, 10, 10, 20, 10)]
            [InlineData(20, 10, 10, 10, 20, 10)]
            [InlineData(15, 10, 10, 10, 20, 10)]

            [InlineData(10, 10, 10, 10, 10, 20)]
            [InlineData(10, 20, 10, 10, 10, 20)]
            [InlineData(10, 15, 10, 10, 10, 20)]

            [InlineData(10, 10, 10, 10, 10, 10)]
            public void PointOnLineTrue(float px, float py, float lineStartX, float lineStartY, float lineEndX, float lineEndY)
            {
                Assert.True(GraphOperations.PointOnLine(px, py, lineStartX, lineStartY, lineEndX, lineEndY));
            }

            [Theory]
            [InlineData(9, 10, 10, 10, 20, 20)]
            [InlineData(10, 9, 10, 10, 20, 20)]
            [InlineData(21, 20, 10, 10, 20, 20)]
            [InlineData(20, 21, 10, 10, 20, 20)]
            [InlineData(15, 12, 10, 10, 20, 20)]
            [InlineData(13, 7, 10, 10, 20, 20)]

            [InlineData(15, 9, 10, 10, 20, 10)]
            [InlineData(15, 11, 10, 10, 20, 10)]
            [InlineData(9, 10, 10, 10, 20, 10)]
            [InlineData(21, 10, 10, 10, 20, 10)]

            [InlineData(5, 15, 10, 10, 10, 20)]
            [InlineData(15, 15, 10, 10, 10, 20)]
            [InlineData(10, 9, 10, 10, 10, 20)]
            [InlineData(10, 21, 10, 10, 10, 20)]

            [InlineData(10, 10, 20, 20, 20, 20)]
            public void PointOnLineFalse(float px, float py, float lineStartX, float lineStartY, float lineEndX, float lineEndY)
            {
                Assert.False(GraphOperations.PointOnLine(px, py, lineStartX, lineStartY, lineEndX, lineEndY));
            }
        }
        public class LinesIntersect
        {
            [Theory]
            // Both horizontal lines
            [InlineData(10, 10, 20, 10, 10, 10, 20, 10)]
            [InlineData(10, 10, 20, 10, 15, 10, 17, 10)]
            [InlineData(10, 10, 20, 10, 5, 10, 15, 10)]
            [InlineData(10, 10, 20, 10, 15, 10, 25, 10)]
            [InlineData(20, 10, 10, 10, 15, 10, 25, 10)]
            [InlineData(10, 10, 20, 10, 5, 10, 25, 10)]
            [InlineData(10, 10, 20, 10, 25, 10, 15, 10)]

            // Both Vertical lines
            [InlineData(10, 10, 10, 20, 10, 10, 10, 20)]
            [InlineData(10, 10, 10, 20, 10, 15, 10, 17)]
            [InlineData(10, 10, 10, 20, 10, 5, 10, 15)]
            [InlineData(10, 10, 10, 20, 10, 15, 10, 25)]
            [InlineData(10, 10, 10, 20, 10, 25, 10, 15)]
            [InlineData(10, 10, 10, 20, 10, 5, 10, 25)]
            [InlineData(10, 10, 10, 20, 10, 25, 10, 5)]

            // One Horizontal one Vertical
            [InlineData(10, 10, 20, 10, 15, 5, 15, 17)]
            [InlineData(17, 8, 25, 8, 19, 3, 19, 10)]
            [InlineData(15, 5, 15, 17, 10, 10, 20, 10)]
            [InlineData(20, 10, 10, 10, 15, 17, 15, 5)]

            // Tilted Lines

            // Points
            [InlineData(10, 10, 10, 10, 10, 10, 10, 10)]
            public void LinesIntersectTrue(float l1StartX, float l1StartY, float l1EdX, float l1EndY, float l2StartX, float l2StartY, float l2EdX, float l2EndY)
            {
                Assert.True(GraphOperations.LinesIntersect(l1StartX, l1StartY, l1EdX, l1EndY, l2StartX, l2StartY, l2EdX, l2EndY));
            }

            public void LinesIntersectFalse(float l1StartX, float l1StartY, float l2StartX, float l2StartY)
            {

            }
        }

        public class LineIntersectsRectangle
        {

        }
        public class RectanglesIntersect
        {

        }
    }
}
