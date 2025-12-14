using System.Drawing;
using System.Windows.Forms;

namespace _25FALL_DodgerGame
{
    internal class Player : Asset
    {
        public bool isBoosted = false;
        public int Lives { get; set; } = 3;      // NEW: 3 lives

        public Player(Point point)
        {
            Center = point;
        }

        public override void Move(int X1, int X2, int Y1, int Y2)
        {
            int speedX = isBoosted ? MoveX * 2 : MoveX;
            int speedY = isBoosted ? MoveY * 2 : MoveY;

            int newX = Center.X + speedX;
            if (newX < X1) newX = X2;
            else if (newX > X2) newX = X1;

            int newY = Center.Y + speedY;
            if (newY < Y1) newY = Y2;
            else if (newY > Y2) newY = Y1;

            Center = new Point(newX, newY);
        }

        public override void Draw(PaintEventArgs e)
        {
            // UFO proportions
            int bodyW = 70;     // oval body width
            int bodyH = 28;     // oval body height
            int domeW = 36;     // top dome width
            int domeH = 18;

            // Body rect centered around Center
            Rectangle body = new Rectangle(Center.X - bodyW / 2, Center.Y - bodyH / 2, bodyW, bodyH);
            Rectangle dome = new Rectangle(Center.X - domeW / 2, Center.Y - bodyH / 2 - domeH / 2, domeW, domeH);

            // Glow when boosted
            using (var bodyPen = new Pen(isBoosted ? Color.DeepSkyBlue : Color.Silver, 3))
            using (var domePen = new Pen(Color.LightGray, 2))
            using (var small = new Pen(Color.Gray, 1))
            {
                // Body
                e.Graphics.DrawEllipse(bodyPen, body);

                // Dome
                e.Graphics.DrawEllipse(domePen, dome);

                // Little lights under the UFO
                int bulbs = 6;
                for (int i = 0; i < bulbs; i++)
                {
                    float t = (float)i / bulbs;
                    int bx = body.Left + (int)(t * body.Width);
                    int by = body.Bottom - 4;
                    e.Graphics.DrawEllipse(small, new Rectangle(bx - 3, by, 6, 6));
                }
            }

            // Collision rectangle
            Rectangle = new Rectangle(Center.X - bodyW / 2, Center.Y - bodyH / 2 - domeH / 2, bodyW, bodyH + domeH);
            // e.Graphics.DrawRectangle(Pens.Red, Rectangle); // debug
        }

        public override bool Collision(Asset asset)
        {
            // Player collision handled in Form1 (we check enemy rectangles vs player rectangle)
            return Rectangle.IntersectsWith(asset.Rectangle);
        }
    }
}
