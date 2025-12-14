using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace _25FALL_DodgerGame
{
    public partial class Form1 : Form
    {
        Player player = new Player(new Point(550, 400));
        KIllerAsteroid Apollo;

        List<Asset> asteroidField = new List<Asset>();     // asteroids + triangles + Apollo
        List<Explosion> explosions = new List<Explosion>(); // NEW: animated explosions

        int Score;
        int Time;

        Point? mouseClickPosition = null;
        Rectangle laserCollisionRect = new Rectangle(-50, -50, 50, 50);
        Timer laserTimer;

        bool drawLaser = false;
        bool drawExplosion = false; // legacy “instant” hit circle, kept for effect

        int countAsteroids = 0;

        int[] movement = { -7, -5, -3, -1, 0, 1, 3, 5, 7 };
        int[] movement2 = { -7, -5, -3, -1, 1, 3, 5, 7 };

        Random random = new Random();

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            int totalAsteroids = 14; // slightly fewer asteroids
            int totalTriangles = 6; // NEW: spinning triangle enemies

            Score = 0;
            Time = 0;

            // Asteroids
            while (countAsteroids < totalAsteroids)
            {
                int x = random.Next(120, 1080);
                int y = random.Next(120, 780);
                int radius = random.Next(20, 70);

                Asteroid asteroid = new Asteroid(new Point(x, y), radius);
                asteroid.MoveX = movement[random.Next(0, movement.Length - 1)];
                if (asteroid.MoveX == 0) asteroid.MoveY = movement2[random.Next(0, movement2.Length - 1)];
                else asteroid.MoveY = movement[random.Next(0, movement.Length - 1)];

                asteroidField.Add(asteroid);
                countAsteroids++;
            }

            // Spinning triangles
            for (int i = 0; i < totalTriangles; i++)
            {
                int x = random.Next(140, 1060);
                int y = random.Next(140, 760);
                int size = random.Next(28, 56);
                float startAng = random.Next(0, 360);
                float spin = random.Next(0, 2) == 0 ? 3.5f : -3.5f;

                var tri = new SpinningTriangle(new Point(x, y), size, startAng, spin);
                tri.MoveX = movement[random.Next(0, movement.Length - 1)];
                tri.MoveY = movement[random.Next(0, movement.Length - 1)];
                asteroidField.Add(tri);
                countAsteroids++;
            }

            // Killer asteroid (red)
            Apollo = new KIllerAsteroid(new Point(600, 600), 40);
            Apollo.MoveX = 3;
            Apollo.MoveY = 3;
            asteroidField.Add(Apollo);
            countAsteroids++;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1200, 900);
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.BackColor = Color.Black;

            laserTimer = new Timer();
            laserTimer.Interval = 100; // 0.1s
            laserTimer.Tick += LaserTimer_TickEvent;

            GameLoop.Interval = 32;
            GameLoop.Start();
        }

        private void LaserTimer_TickEvent(object sender, EventArgs e)
        {
            drawExplosion = false;
            drawLaser = false;
            laserTimer.Stop();
            this.Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(100, 100, 1000, 700);
            e.Graphics.DrawRectangle(Pens.White, rectangle);

            Region clippingRegion = new Region(rectangle);
            e.Graphics.Clip = clippingRegion;

            // Player
            player.Draw(e);

            // Legacy laser visuals (kept)
            if (drawExplosion) e.Graphics.DrawEllipse(Pens.Red, laserCollisionRect);
            if (drawLaser && mouseClickPosition.HasValue)
                e.Graphics.DrawLine(Pens.Lime, player.Center, mouseClickPosition.Value);

            // Enemies
            int assetIndex = asteroidField.Count - 1;
            while (assetIndex >= 0)
            {
                var enemy = asteroidField[assetIndex];

                // Collision with player
                if (enemy.Collision(player))
                {
                    if (enemy.GetType() == typeof(KIllerAsteroid) || enemy.GetType() == typeof(SpinningTriangle) || enemy.GetType() == typeof(Asteroid))
                    {
                        // Lose a life, explosion at player
                        explosions.Add(new Explosion(player.Center));
                        player.Lives--;

                        if (player.Lives <= 0)
                        {
                            pauseGame("Game over! You are out of lives.");
                            GameLoop.Stop();
                        }
                        else
                        {
                            // Remove that enemy & respawn player to center-ish
                            asteroidField.RemoveAt(assetIndex);
                            Score -= 5;
                            player.Center = new Point(550, 400);
                        }
                    }
                }
                else
                {
                    enemy.Draw(e);
                }
                assetIndex--;
            }

            // Explosions
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Draw(e);
                if (explosions[i].Finished) explosions.RemoveAt(i);
            }

            e.Graphics.ResetClip();

            // HUD
            e.Graphics.DrawString("Score " + Score, new Font("Verdana", 24, FontStyle.Bold), Brushes.Gold, 100, 20);
            e.Graphics.DrawString("Time " + Time, new Font("Verdana", 24, FontStyle.Bold), Brushes.White, 350, 20);

            // Lives
            using (var f = new Font("Verdana", 24, FontStyle.Bold))
            {
                e.Graphics.DrawString($"Lives {player.Lives}", f, player.Lives > 1 ? Brushes.LightGreen : Brushes.OrangeRed, 550, 20);
            }
        }

        int timeCounter = 0;
        int tenSecCounter = 0;

        private void GameLoop_Tick(object sender, EventArgs e)
        {
            player.Move(100, 1100, 100, 800);

            timeCounter += 32;
            tenSecCounter += 32;

            if (timeCounter > 1000)
            {
                Time++;
                timeCounter = 0;
            }

            if (tenSecCounter > 10000)
            {
                Apollo.MoveX = random.Next(-10, 10);
                Apollo.MoveY = random.Next(-10, 10);
                tenSecCounter = 0;
            }

            foreach (Asset a in asteroidField)
            {
                a.Move(0, this.Size.Width, 0, this.Size.Height);
            }

            // Laser click “instant” removal + animated explosion
            if (drawExplosion)
            {
                for (int i = asteroidField.Count - 1; i >= 0; i--)
                {
                    if (asteroidField[i].Rectangle.IntersectsWith(laserCollisionRect))
                    {
                        explosions.Add(new Explosion(new Point(
                            asteroidField[i].Rectangle.X + asteroidField[i].Rectangle.Width / 2,
                            asteroidField[i].Rectangle.Y + asteroidField[i].Rectangle.Height / 2)));
                        asteroidField.RemoveAt(i);
                        Score += 10;
                    }
                }
            }

            // Animate explosions
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                explosions[i].Update();
                if (explosions[i].Finished) explosions.RemoveAt(i);
            }

            this.Refresh();
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Right) player.MoveX = 5;
            if (e.KeyCode == Keys.Left) player.MoveX = -5;
            if (e.KeyCode == Keys.Up) player.MoveY = -5;
            if (e.KeyCode == Keys.Down) player.MoveY = 5;

            if (e.KeyCode == Keys.Space) player.isBoosted = true; // speed boost

            if (e.KeyCode == Keys.P) pauseGame("Game is Paused.");

            if (e.KeyCode == Keys.X)
            {
                // Spawn a random asteroid
                int x = random.Next(100, 1100);
                int y = random.Next(100, 800);
                int radius = random.Next(20, 70);

                Asteroid asteroid = new Asteroid(new Point(x, y), radius);
                asteroid.MoveX = movement[random.Next(0, movement.Length - 1)];
                asteroid.MoveY = movement[random.Next(0, movement.Length - 1)];
                asteroidField.Add(asteroid);
                countAsteroids++;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            player.MoveX = 0;
            player.MoveY = 0;
            player.isBoosted = false;
        }

        private void pauseGame(string Message)
        {
            GameLoop.Stop();
            MessageBox.Show(Message, "Paused", MessageBoxButtons.OK);
            GameLoop.Start();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseClickPosition = e.Location;
            drawLaser = true;
            drawExplosion = true;

            laserCollisionRect = new Rectangle(e.X - 25, e.Y - 25, 50, 50);

            // NEW: also spawn an animated explosion at the click location
            explosions.Add(new Explosion(e.Location));

            this.Refresh();
            laserTimer.Start();
        }
    }
}
