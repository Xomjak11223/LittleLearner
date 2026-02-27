using System;
using System.Collections.Generic;
using System.Text;

namespace LittleLearner.CFG
{
    internal class FlowchartDrawer : IDrawable
    {
        private readonly Color Background = new Color(0, 60, 100);
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {

            canvas.FillColor = Background;
            canvas.FillRectangle(dirtyRect);

            drawStartOrEnd(canvas);

            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 6;
            canvas.DrawLine(0, 0, dirtyRect.Center.X, dirtyRect.Center.Y);

            drawStartOrEnd(canvas);
        }

        private void drawStartOrEnd(ICanvas canvas)
        {

        }

        private void drawProcess(ICanvas canvas, String text, float startX, float startY, float width, float height)
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
