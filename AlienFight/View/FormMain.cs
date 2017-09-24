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
        private GameController _controller;
        private Bitmap _canvas;
        private Graphics _graphics;

        public FormMain(GameController parController)
        {
            InitializeComponent();
            _controller = parController;
            _controller.View = this;
            _canvas = new Bitmap(this.Width, this.Height);
            _graphics = Graphics.FromImage(_canvas);
        }

        public void ViewCanvas()
        {
            _canvas.RotateFlip(RotateFlipType.RotateNoneFlipX);
            this.BackgroundImage = new Bitmap(_canvas);
        }

        public void DrawLevel(GameLevel parLevel)
        {
            _graphics.Clear(Color.White);
            foreach (GameObject levelElement in parLevel.LevelObjects)
            {
                DrawGameObject(levelElement, parLevel);
            }
            foreach (GameObject enemy in parLevel.Enemies)
            {
                DrawGameObject(enemy, parLevel);
            }
            DrawGameObject(parLevel.Player, parLevel);
            ViewCanvas();
        }

        private void DrawGameObject(GameObject parObject, GameLevel parLevel)
        {
            if (IsVisible(parObject, parLevel))
            {
                _graphics.DrawImage(parObject.Sprites[parObject.ActiveSprite], parObject.X, parObject.Y);
            }
        }

        private bool IsVisible(GameObject parObject, GameLevel parLevel)
        {
            double leftBound = parLevel.CameraX - parLevel.SizeX / 2;
            double rightBound = parLevel.CameraX + parLevel.SizeX / 2;
            double upBound = parLevel.CameraY - parLevel.SizeY / 2;
            double downBound = parLevel.CameraY + parLevel.SizeY / 2;

            return ((parObject.X < rightBound)
                    || ((parObject.X + parObject.SizeX) > leftBound))
                   && ((parObject.Y < upBound)
                    || ((parObject.Y + parObject.SizeY) > downBound));
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            _canvas = new Bitmap(this.Width, this.Height);
            _graphics = Graphics.FromImage(_canvas);
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            _controller.KeyDown(e);
        }

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            _controller.KeyUp(e);
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
