using System.Drawing;

namespace Client
{
    public struct MyPoint
    {
        public Color Color { get; set; }
        public Point Point { get; set; }

        public MyPoint(Color color, Point point)
        {
            Color = color;
            Point = point;
        }
    }
}
