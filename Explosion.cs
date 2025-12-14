using System.Drawing;
using System.Windows.Forms;

namespace _25FALL_DodgerGame
{
    internal class Explosion
    {
        public Point Center { get; private set; }
        public int Radius { get; private set; }
        public int MaxRadius { get; private set; }
        public bool Finished => Radius >= MaxRadius;

        public Explosion(Point center, int startRadius = 4, int maxRadius = 45)
        {
            Center = center;
            Radius = startRadius;
            MaxRadius = maxRadius;
        }

        public void Update()
        {
            Radius += 4; // expand each tick
        }

        public void Draw(PaintEventArgs e)
        {
            int d = Radius * 2;
            var rect = new Rectangle(Center.X - Radius, Center.Y - Radius, d, d);
            using (var pen = new Pen(Color.Red, 2))
                e.Graphics.DrawEllipse(pen, rect);
            using (var pen2 = new Pen(Color.Orange, 1))
                e.Graphics.DrawEllipse(pen2, new Rectangle(rect.X + 6, rect.Y + 6, rect.Width - 12, rect.Height - 12));
        }
    }
}
