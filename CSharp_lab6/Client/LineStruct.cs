using System.Drawing;
using System.Windows;
using Brush = System.Windows.Media.Brush;
using Point = System.Drawing.Point;

namespace Client
{
    public struct LineStruct
    {
        public Point Start { get; set; }
        public Point End { get; set; }
        public Brush Brush{ get; set; }
        public float Thickness { get; set; }
        public bool IsDrawn { get; set; }

        public LineStruct(Point start, Point end, Brush brush, float thickness)
        {
            Start = start;
            End = end;
            Brush = brush;
            Thickness = thickness;
            IsDrawn = false;
        }
    }
}
