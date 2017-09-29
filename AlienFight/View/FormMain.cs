#define FPSMETER

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AlienFight.Model;
using AlienFight.Controller;

namespace AlienFight
{
    public partial class FormMain : Form
    {
#if FPSMETER
        private int[ ] _counter;
        private DateTime _time;
#endif
        private GameController _controller;
        private Graphics _formGraphics;
        private BufferedGraphicsContext _bufGraphicsContext;
        private BufferedGraphics _bufGraphics;

        public FormMain(GameController parController)
        {
            InitializeComponent();
#if FPSMETER
            _counter = new int[4];
            _time = DateTime.UtcNow;
#endif
            _bufGraphicsContext = BufferedGraphicsManager.Current;
            _bufGraphicsContext.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            _bufGraphics = _bufGraphicsContext.Allocate(this.CreateGraphics(), new Rectangle(0, 0, this.Width, this.Height));
            _formGraphics = this.CreateGraphics();
            _controller = parController;
            _controller.View = this;
        }

        public void ViewCanvas()
        {
            _bufGraphics.Render(_formGraphics);
#if FPSMETER
            FormMain_Paint();
#endif
        }

        public void DrawLevel(GameLevel parLevel)
        {
            _bufGraphics.Graphics.Clear(Color.White);
            foreach (GameObject levelElement in parLevel.LevelObjects)
            {
                DrawGameObject(levelElement, parLevel);
            }
            foreach (GameObject enemy in parLevel.Enemies)
            {
                DrawGameObject(enemy, parLevel);
            }
            //DrawGameObject(parLevel.Player, parLevel);
#if FPSMETER
            _bufGraphics.Graphics.DrawString($"FPS: {_counter[0] + _counter[1] + _counter[2]}", this.Font, Brushes.Black, 0, 0);
#endif
            ViewCanvas();
        }

        private void DrawGameObject(GameObject parObject, GameLevel parLevel)
        {
            if (IsVisible(parObject, parLevel))
            {
                _bufGraphics.Graphics.DrawImage(parObject.Sprites[parObject.ActiveSprite],
                    parObject.X - parLevel.CameraX,
                    this.Height - (parObject.Y + parObject.SizeY - parLevel.CameraY),
                    parObject.SizeX,
                    parObject.SizeY);
            }
        }

        private bool IsVisible(GameObject parObject, GameLevel parLevel)
        {
            double leftBound = parLevel.CameraX;
            double rightBound = parLevel.CameraX + this.Width;
            double downBound = parLevel.CameraY;
            double upBound = parLevel.CameraY + this.Height;

            return ((parObject.X < rightBound)
                    || ((parObject.X + parObject.SizeX) > leftBound))
                   && ((parObject.Y < upBound)
                    || ((parObject.Y + parObject.SizeY) > downBound));
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            Graphics oldFormGraphics = _formGraphics;
            _formGraphics = this.CreateGraphics();
            _bufGraphicsContext.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            BufferedGraphics oldGraphics = _bufGraphics;
            _bufGraphics = _bufGraphicsContext.Allocate(this.CreateGraphics(), new Rectangle(0, 0, this.Width, this.Height));

            if (oldFormGraphics != null)
            {
                oldFormGraphics.Dispose();
            }
            if (oldGraphics != null)
            {
                oldGraphics.Dispose();
            }
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            _controller.KeyDown(e);
        }

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            _controller.KeyUp(e);
        }

#if FPSMETER
        private void FormMain_Paint()
        {
            _counter[3]++;
            if ((DateTime.UtcNow - _time).TotalMilliseconds > 333)
            {
                _counter[0] = _counter[1];
                _counter[1] = _counter[2];
                _counter[2] = _counter[3];
                _counter[3] = 0;
                _time = DateTime.UtcNow;
            }
        }
#endif

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
