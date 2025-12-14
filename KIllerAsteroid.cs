using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _25FALL_DodgerGame
{
    internal class KIllerAsteroid : Asset
    {
        int Radius;
        public KIllerAsteroid(Point center, int radius)
        {
            Center = center;
            Radius = radius;
        }
        public override void Draw(PaintEventArgs e)
        {
            //e.Graphics.DrawRectangle(Pens.Gold, Rectangle);

            Rectangle = new Rectangle(Center.X, Center.Y, Radius, Radius);
            e.Graphics.DrawEllipse(Pens.Red, Rectangle);
        }


        public override bool Collision(Asset asset)
        {
            if (Rectangle.IntersectsWith(asset.Rectangle))
            {
                return true;
            }
            else return false;
        }
    }
}
