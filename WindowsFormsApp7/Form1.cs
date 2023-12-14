using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp7
{
    public partial class Form1 : Form
    {
        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        private Direction direction;

        private const int gridSize = 20;
        private const int initialSpeed = 100; // v milisekundách
        private const int accelerationInterval = 10;
        private const int accelerationRate = 15;

        private List<Point> had;
        private List<Point> salaty;
        private Random random;
        private int score;
        private int speed;
        private bool running;
        private bool paused = false;

        public Form1()
        {
            InitializeComponent();

            had = new List<Point>();
            salaty = new List<Point>();
            random = new Random();
            timer1.Interval = initialSpeed;
            timer1.Tick += timer1_Tick;


            InitializeGame();
        }
        private void InitializeGame()
        {
            had.Clear();
            salaty.Clear();
            score = 0;
            speed = initialSpeed;
            running = false;

            for (int i = 0; i < 3; i++)
            {
                had.Add(new Point(i, 0));
            }

            GenerateSalaty();
            UpdateSalatyLabels();

            direction = Direction.Left; // Nastavení počátečního směru pohybu hada

            Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void GenerateSalaty()
        {
            if (salaty.Count == 0) // Pokud je to na začátku hry
            {
                for (int i = 0; i < 5; i++)
                {
                    Point newSalat;
                    do
                    {
                        newSalat = new Point(random.Next(ClientSize.Width / gridSize), random.Next(ClientSize.Height / gridSize));
                    } while (had.Contains(newSalat) || salaty.Contains(newSalat));

                    salaty.Add(newSalat);
                }
            }
            else // Pokud už hra běží a had sežral salát
            {
                Point newSalat;
                do
                {
                    newSalat = new Point(random.Next(ClientSize.Width / gridSize), random.Next(ClientSize.Height / gridSize));
                } while (had.Contains(newSalat) || salaty.Contains(newSalat));

                salaty.Add(newSalat);
            }
        }

        private void UpdateSalatyLabels()
        {
            label1.Text = $"Saláty: {score}/50";
        }


        private void MoveHad()
        {
            Point head = had[0];
            Point newHead;

            switch (direction)
            {
                case Direction.Up:
                    newHead = new Point(head.X, (head.Y - 1 + ClientSize.Height / gridSize) % (ClientSize.Height / gridSize));
                    break;
                case Direction.Down:
                    newHead = new Point(head.X, (head.Y + 1) % (ClientSize.Height / gridSize));
                    break;
                case Direction.Left:
                    newHead = new Point((head.X - 1 + ClientSize.Width / gridSize) % (ClientSize.Width / gridSize), head.Y);
                    break;
                case Direction.Right:
                    newHead = new Point((head.X + 1) % (ClientSize.Width / gridSize), head.Y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            had.Insert(0, newHead);

            if (salaty.Contains(newHead))
            {
                score++;
                UpdateSalatyLabels();
                salaty.Remove(newHead); // Odeber salát, který byl sežrán
                GenerateSalaty();

                if (score % accelerationInterval == 0)
                {
                    speed = (int)(speed * (1.0 - accelerationRate / 100.0));
                    timer1.Interval = speed;
                }

                // Přidáme nový segment hada (vygeneruje se nový had) místo generování nového hada
                had.Add(had.Last());
            }
            else
            {
                had.RemoveAt(had.Count - 1);
            }
        }

        private void CheckCollision()
        {
            Point head = had[0];

            if (had.IndexOf(head, 1) != -1 || head.X < 0 || head.X >= ClientSize.Width / gridSize ||
                head.Y < 0 || head.Y >= ClientSize.Height / gridSize)
            {
                EndGame();
            }
        }

        private void EndGame()
        {
            timer1.Stop();
            MessageBox.Show($"Gratulujeme! Konec hry. Sežral jsi {score} salátů.", "Konec hry");
            InitializeGame();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    if (direction != Direction.Down)
                        direction = Direction.Up;
                    break;
                case Keys.S:
                    if (direction != Direction.Up)
                        direction = Direction.Down;
                    break;
                case Keys.A:
                    if (direction != Direction.Right)
                        direction = Direction.Left;
                    break;
                case Keys.D:
                    if (direction != Direction.Left)
                        direction = Direction.Right;
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            MoveHad();
            CheckCollision();
            Invalidate();
            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            foreach (Point segment in had)
            {
                g.FillRectangle(Brushes.Green, segment.X * gridSize, segment.Y * gridSize, gridSize, gridSize);
            }

            foreach (Point salat in salaty)
            {
                g.FillEllipse(Brushes.Red, salat.X * gridSize, salat.Y * gridSize, gridSize, gridSize);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!running)
            {
                if (paused)
                {
                    timer1.Start();
                    paused = false;

                }
                else
                {
                    InitializeGame();
                    timer1.Start();
                    running = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (running)
            {
                if (paused)
                {
                    timer1.Start();
                    paused = false;
                }
                else
                {
                    timer1.Stop();
                    paused = true;

                }
            }
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }
    }
}
