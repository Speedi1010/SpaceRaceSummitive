using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Media;
using System.IO;

namespace SpaceRaceSummitive
{
    public partial class Form1 : Form
    {
        Rectangle rocket1 = new Rectangle(0, 153, 20, 20);
        Rectangle rocket2 = new Rectangle(0, 153, 20, 20);
        Rectangle rocket1tip = new Rectangle(55, 153, 1, 1);
        Rectangle rocket2tip = new Rectangle(55, 153, 1, 1);
        int heroSpeed = 5;

        PointF[] points1 = new PointF[3];
        PointF[] points2 = new PointF[3];
        PointF[] cone1 = new PointF[3];
        PointF[] cone2 = new PointF[3];

        List<Rectangle> astriod = new List<Rectangle>();
        List<Rectangle> reverseastriod = new List<Rectangle>();
        List<int> astriodSpeeds = new List<int>();
        List<int> astriodSizes = new List<int>();
        List<int> reverseastriodSpeeds = new List<int>();
        List<int> reverseastriodSizes = new List<int>();

        bool downArrowDown = false;
        bool upArrowDown = false;
        bool sDown = false;
        bool wDown = false;
        bool win = false;
        bool lose = false;

        int winner;
        int p1Score;
        int p2Score;
        int time = 1500;

        string gameState = "waiting";

        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush blueBrush = new SolidBrush(Color.Blue);
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush oragneBrush = new SolidBrush(Color.Orange);
        Font font = new Font("Ariel", 24, FontStyle.Bold);
        Random randGen = new Random();
        int randValue = 0;

        System.Windows.Media.MediaPlayer thruster = new System.Windows.Media.MediaPlayer();
        SoundPlayer crash = new SoundPlayer(Properties.Resources.crash);
        public Form1()
        {
            InitializeComponent();
            thruster.Open(new Uri(Application.StartupPath + "/Resources/Thruster.wav"));
        }
        public void GameInitialize()
        {
            time = 1500;
            gameState = "";
            gameTimer.Enabled = true;
            lose = false;
            win = false;
            rocket1.Y = this.Height - rocket1.Height;
            rocket1.X = this.Width * 1 / 4;

            rocket1tip.X = rocket1.X + 10;
            rocket2tip.X = rocket2.X + 10;
            rocket1tip.Y = rocket1.Y + 20;
            rocket2tip.Y = rocket2.Y + 20;

            rocket2.Y = this.Height - rocket1.Height;
            rocket2.X = this.Width * 3 / 4;

            astriod.Clear();
            astriodSizes.Clear();
            astriodSpeeds.Clear();
            reverseastriod.Clear();
            reverseastriodSizes.Clear();
            reverseastriodSpeeds.Clear();

            gameState = "running";
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = true;
                    break;
                case Keys.Down:
                    downArrowDown = true;
                    break;
                case Keys.S:
                    sDown = true;
                    break;
                case Keys.Up:
                    upArrowDown = true;
                    break;
                case Keys.Space:
                    if (gameState == "waiting" || gameState == "gameover")
                    {
                        GameInitialize();
                    }
                    break;
                case Keys.Escape:
                    //if (gameState == "waiting" || gameState == "gameover")
                    //{
                    this.Close();

                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = false;
                    thruster.Stop();
                    break;
                case Keys.Down:
                    downArrowDown = false;
                    break;
                case Keys.S:
                    sDown = false;
                    break;
                case Keys.Up:
                    upArrowDown = false;
                    thruster.Stop();
                    break;
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //move hero

            if (downArrowDown == true && rocket2.Y < this.Height - rocket2.Height)
            {
                rocket2.Y += heroSpeed;
                rocket2tip.Y = rocket2.Y - 20;
            }

            if (upArrowDown == true)
            {
                rocket2.Y -= heroSpeed;
                rocket2tip.Y = rocket2.Y - 20;
                thruster.Play();
            }
            if (wDown == true)
            {
                rocket1.Y -= heroSpeed;
                rocket1tip.Y = rocket1.Y - 20;
                thruster.Play();
            }

            if (sDown == true && rocket1.Y < this.Height - rocket1.Height)
            {
                rocket1.Y += heroSpeed;
                rocket1tip.Y = rocket1.Y - 20;
            }

            //is it time to make a new ball?
            randValue = randGen.Next(1, 101);

            if (randValue < 11) // 20% of a green ball on the left
            {
                astriodSizes.Add(randGen.Next(10, 30));
                int y = randGen.Next(0, this.Height - 100);
                Rectangle newBall = new Rectangle(0, y, 20, 20);
                astriodSpeeds.Add(randGen.Next(1, 11));
                astriod.Add(newBall);
            }
            else if (randValue < 21) // 20% change of a green ball on the right
            {
                reverseastriodSizes.Add(randGen.Next(10, 30));
                int y = randGen.Next(0, this.Height - 100);
                Rectangle newAstriod = new Rectangle(this.Width, y, 20, 20);
                reverseastriodSpeeds.Add(randGen.Next(1, 11));
                reverseastriod.Add(newAstriod);
            }
            //move all the balls
            for (int i = 0; i < astriod.Count(); i++)
            {
                //get the new position of x based on speed
                int x = astriod[i].X + astriodSpeeds[i];

                //replace the rectangle in the list with updated one
                astriod[i] = new Rectangle(x, astriod[i].Y, astriodSizes[i], astriodSizes[i]);
            }
            for (int i = 0; i < reverseastriod.Count(); i++)
            {
                int reverseX = reverseastriod[i].X - reverseastriodSpeeds[i];
                reverseastriod[i] = new Rectangle(reverseX, reverseastriod[i].Y, reverseastriodSizes[i], reverseastriodSizes[i]);
            }

            //remove balls if they go beyond the play area
            for (int i = 0; i < astriod.Count(); i++)
            {
                if (astriod[i].X > this.Width - astriod[i].Width)
                {
                    astriod.RemoveAt(i);
                    astriodSpeeds.RemoveAt(i);
                    astriodSizes.RemoveAt(i);
                }
            }
            for (int i = 0; i < reverseastriod.Count(); i++)
            {
                if (reverseastriod[i].X < 0)
                {
                    reverseastriod.RemoveAt(i);
                    reverseastriodSpeeds.RemoveAt(i);
                    reverseastriodSizes.RemoveAt(i);
                }
            }

            //check for collision between ball and player
            for (int i = 0; i < astriod.Count(); i++)
            {
                if (astriod[i].IntersectsWith(rocket1) || astriod[i].IntersectsWith(rocket1tip))
                {
                    rocket1.Y = this.Height - rocket1.Height;
                    rocket1.X = this.Width * 1 / 4;
                    rocket1tip.Y = rocket1.Y + 20;
                    crash.Play();
                }
                if (astriod[i].IntersectsWith(rocket2) || astriod[i].IntersectsWith(rocket2tip))
                {
                    rocket2.Y = this.Height - rocket2.Height;
                    rocket2.X = this.Width * 3 / 4;
                    rocket2tip.Y = rocket2.Y + 20;
                    crash.Play();
                }
            }
            for (int i = 0; i < reverseastriod.Count(); i++)
            {
                if (reverseastriod[i].IntersectsWith(rocket1) || reverseastriod[i].IntersectsWith(rocket1tip))
                {
                    rocket1.Y = this.Height - rocket1.Height;
                    rocket1.X = this.Width * 1 / 4;
                    rocket1tip.Y = rocket1.X + 20;
                    crash.Play();
                }
                if (reverseastriod[i].IntersectsWith(rocket2) || reverseastriod[i].IntersectsWith(rocket1tip))
                {
                    rocket2.Y = this.Height - rocket2.Height;
                    rocket2.X = this.Width * 3 / 4;
                    rocket2tip.Y = rocket2.Y + 20;
                    crash.Play();
                }
            }
            if (rocket1.Y < 0 - rocket1.Height + 25)
            {
                p1Score++;
                rocket1.Y = this.Height - rocket1.Height;
                rocket1.X = this.Width * 1 / 4;
                player1Score.Text = p1Score.ToString();
            }
            else if (rocket2.Y < 0 - rocket2.Height + 25)
            {
                p2Score++;
                rocket2.Y = this.Height - rocket1.Height;
                rocket2.X = this.Width * 3 / 4;
                player2Score.Text = p2Score.ToString();
            }
            if (p1Score == 3)
            {
                win = true;
                winner = 1;
                gameTimer.Enabled = false;
            }
            else if (p2Score == 3)
            {
                win = true;
                winner = 2;
                gameTimer.Enabled = false;
            }
            time--;
            if (time <= 0)
            {
                if (p1Score > p2Score)
                {
                    win = true;
                    winner = 1;
                }
                else if (p1Score < p2Score)
                {
                    win = true;
                    winner = 2;
                }
                else
                {
                    lose = true;
                }
                gameTimer.Enabled = false;
            }

            points1[0] = new Point(rocket1.X, rocket1.Y + 20);
            points1[1] = new Point(rocket1.X + 10, rocket1.Y + 45);
            points1[2] = new Point(rocket1.X + 20, rocket1.Y + 20); // makes flames for rocket 1

            points2[0] = new Point(rocket2.X, rocket2.Y + 20); // makes flames for rocket 2
            points2[1] = new Point(rocket2.X + 10, rocket2.Y + 45);
            points2[2] = new Point(rocket2.X + 20, rocket2.Y + 20);

            cone1[0] = new Point(rocket1.X, rocket1.Y);
            cone1[1] = new Point(rocket1.X + 10, rocket1.Y - 20); // makes rocket 1 tip
            cone1[2] = new Point(rocket1.X + 20, rocket1.Y);

            cone2[0] = new Point(rocket2.X, rocket2.Y);
            cone2[1] = new Point(rocket2.X + 10, rocket2.Y - 20); // makes rocket 2 tip
            cone2[2] = new Point(rocket2.X + 20, rocket2.Y);
            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            timeLabel.Text = time.ToString();
            if (gameState == "waiting")
            {
                e.Graphics.DrawString("Press space to start or escape to quit", font, whiteBrush, this.Width / 3, this.Height / 2);
                timeLabel.Location = new Point(this.Width / 2, 0);
                player1Score.Location = new Point(this.Width * 1 / 4, 0);
                player2Score.Location = new Point(this.Width * 3 / 4, 0);
            }
            else
            {
                //draw hero
                if (upArrowDown == true)
                {
                    e.Graphics.FillPolygon(oragneBrush, points2);
                }
                if (wDown == true)
                {
                    e.Graphics.FillPolygon(oragneBrush, points1);
                }
                e.Graphics.FillPolygon(redBrush, cone1);
                e.Graphics.FillPolygon(blueBrush, cone2);
                e.Graphics.FillRectangle(whiteBrush, rocket1);
                e.Graphics.FillRectangle(whiteBrush, rocket2);



                //draw balls
                for (int i = 0; i < astriod.Count(); i++)
                {
                    Color randomColor = Color.FromArgb(randGen.Next(256), randGen.Next(256), randGen.Next(256));
                    SolidBrush randombrush = new SolidBrush(randomColor);
                    e.Graphics.FillEllipse(randombrush, astriod[i]);
                }
                for (int i = 0; i < reverseastriod.Count(); i++)
                {
                    Color randomColor = Color.FromArgb(randGen.Next(256), randGen.Next(256), randGen.Next(256));
                    SolidBrush randombrush = new SolidBrush(randomColor);
                    e.Graphics.FillEllipse(randombrush, reverseastriod[i]);
                }
                if (win == true)
                {
                    e.Graphics.DrawString($"Player {winner} Wins\nPress Space to play again or press the escape key to quit", font, whiteBrush, this.Width / 4, this.Height / 2);
                    gameState = "gameover";
                }
                if (lose == true)
                {
                    e.Graphics.DrawString("time is up\nPress Space to play again or press the escape key to quit", font, whiteBrush, this.Width / 4, this.Height / 2);
                    gameState = "gameover";
                }
            }
        }
    }


}
