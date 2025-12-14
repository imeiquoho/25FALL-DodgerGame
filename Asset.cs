using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _25FALL_DodgerGame
{
    internal abstract class Asset
    {
        public Point Center {  get; set; }
        public Rectangle Rectangle { get; set; }

        public int MoveX { get; set; }
        public int MoveY { get; set; }

        public abstract void Draw (PaintEventArgs e);

        public abstract bool Collision(Asset asset);

        public virtual void Move(int X1, int X2, int Y1, int Y2)
        {
            int newX = Center.X + MoveX;

            if (newX < X1) newX = X2;
            else if (newX > X2) newX = X1;

            int newY = Center.Y + MoveY;
            
            if (newY < Y1) newY = Y2;
            else if (newY > Y2) newY = Y1;


            Center = new Point (newX, newY);

        }
       

    }
}
