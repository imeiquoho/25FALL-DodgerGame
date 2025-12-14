using System;
using System.Drawing;
using System.Windows.Forms;

namespace _25FALL_DodgerGame
{
    internal class SpinningTriangle : Asset
    {
        private int size;
        private float angle;              // degrees
        private float angularSpeed;       // degrees per tick

        public SpinningTriangle(Point center, int size, float startAngleDeg, float angularSpeedDegPerTick)
        {
            Center = center;
            this.size = size;
            angle = startAngleDeg;
            angularSpeed = angularSpeedDegPerTick;
        }

        public override void Draw(PaintEventArgs e)
        {
            // Three points forming an equilateral-ish triangle, rotated by angle
            PointF[] pts = GetRotatedTrianglePoints(Center, size, angle);

            using (var pen = new Pen(Color.OrangeRed, 2))
            {
                e.Graphics.DrawPolygon(pen, pts);
            }

            // Collision rectangle as bounding box of triangle
            int minX = (int)Math.Min(pts[0].X, Math.Min(pts[1].X, pts[2].X));
            int maxX = (int)Math.Max(pts[0].X, Math.Max(pts[1].X, pts[2].X));
            int minY = (int)Math.Min(pts[0].Y, Math.Min(pts[1].Y, pts[2].Y));
            int maxY = (int)Math.Max(pts[0].Y, Math.Max(pts[1].Y, pts[2].Y));
            Rectangle = new Rectangle(minX, minY, Math.Max(1, maxX - minX), Math.Max(1, maxY - minY));

            // Spin
            angle += angularSpeed;
            if (angle >= 360) angle -= 360;
            if (angle < 0) angle += 360;
        }

        public override bool Collision(Asset asset)
        {
            return Rectangle.IntersectsWith(asset.Rectangle);
        }

        private static PointF[] GetRotatedTrianglePoints(Point center, int s, float deg)
        {
            // Base triangle (pointing up) around center
            float h = s;                 // height
            float halfBase = s * 0.6f;   // base half-width

            PointF p1 = new PointF(center.X, center.Y - h * 0.6f);     // top
            PointF p2 = new PointF(center.X - halfBase, center.Y + h * 0.4f); // bottom-left
            PointF p3 = new PointF(center.X + halfBase, center.Y + h * 0.4f); // bottom-right

            // Rotate around center
            float rad = (float)(Math.PI * deg / 180.0);
            float cos = (float)Math.Cos(rad);
            float sin = (float)Math.Sin(rad);

            PointF r1 = RotatePoint(p1, center, cos, sin);
            PointF r2 = RotatePoint(p2, center, cos, sin);
            PointF r3 = RotatePoint(p3, center, cos, sin);

            return new[] { r1, r2, r3 };
        }

        private static PointF RotatePoint(PointF p, Point c, float cos, float sin)
        {
            float dx = p.X - c.X;
            float dy = p.Y - c.Y;
            return new PointF(c.X + dx * cos - dy * sin, c.Y + dx * sin + dy * cos);
        }
    }
}
