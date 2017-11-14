#define FPSMETER

using System;
using System.Drawing;
using System.Windows.Forms;

using AlienFight.Model;
using System.Threading;

namespace AlienFight.View
{
    public partial class FormMain : Form, IViewable
    {
#if FPSMETER
        private int[ ] _counter;
        private DateTime _time;
#endif
        private Graphics _formGraphics;
        private BufferedGraphicsContext _bufGraphicsContext;
        private BufferedGraphics _bufGraphics;
        private SpritesContainer _spritesContainer;
        private TextureBrush _backgroundBrush;
        private int _cellSize;
        private int _cellsCapacity = 15;
        private float _drawingCorrection = 0;

        public FormMain()
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
            _spritesContainer = ResourceLoader.LoadSprites();
            _backgroundBrush = new TextureBrush(_spritesContainer.Background);
            _cellSize = this.Width / _cellsCapacity;
            _drawingCorrection = _cellSize / 150f;
            Cursor.Hide();
        }

        public void ViewModel(GameModel parLevel)
        {
            DrawBackground();
            float cameraX = parLevel.CameraX;
            float cameraY = parLevel.CameraY;
            foreach (GameObject levelElement in parLevel.ModelObjects)
            {
                DrawGameObject(levelElement, parLevel, cameraX, cameraY);
            }
            foreach (GameObject enemy in parLevel.Enemies)
            {
                DrawGameObject(enemy, parLevel, cameraX, cameraY);
            }
            if (parLevel.Player != null)
            {
                DrawGameObject(parLevel.Player, parLevel, cameraX, cameraY);
            }
            ViewCanvas(parLevel);
        }

        private void DrawBackground()
        {
            _bufGraphics.Graphics.FillRectangle(_backgroundBrush, 0, 0, this.Width, this.Height);
        }

        private void DrawGameObject(GameObject parObject, GameModel parLevel, float parCameraX, float parCameraY)
        {
            if (IsVisible(parObject, parLevel))
            {
                Image sprite = _spritesContainer.GetSprite(parObject, parObject.FlippedY);
                _bufGraphics.Graphics.DrawImage(
                    sprite,
                    parObject.X * _cellSize - parCameraX * _cellSize - _drawingCorrection,
                    this.Height - (parObject.Y * _cellSize + parObject.SizeY * _cellSize - parCameraY * _cellSize) - _drawingCorrection,
                    parObject.SizeX * _cellSize + _drawingCorrection * 2,
                    parObject.SizeY * _cellSize + _drawingCorrection * 2);
            }
        }

        private bool IsVisible(GameObject parObject, GameModel parLevel)
        {
            double leftBound = parLevel.CameraX;
            double rightBound = parLevel.CameraX + this.Width / _cellSize;
            double downBound = parLevel.CameraY;
            double upBound = parLevel.CameraY + this.Height / _cellSize;

            return ((parObject.X < rightBound)
                    || ((parObject.X + parObject.SizeX) > leftBound))
                   && ((parObject.Y < upBound)
                    || ((parObject.Y + parObject.SizeY) > downBound));
        }

        private void ViewCanvas(GameModel parLevel)
        {
#if FPSMETER
            _bufGraphics.Graphics.DrawString($"FPS: {_counter[0] + _counter[1] + _counter[2]}, HP: {parLevel.Player.Health}", this.Font, Brushes.White, 0, 0);
#endif
            _bufGraphics.Render(_formGraphics);
#if FPSMETER
            FormMain_Paint();
#endif
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            _formGraphics = this.CreateGraphics();
            _bufGraphicsContext.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            _bufGraphics = _bufGraphicsContext.Allocate(this.CreateGraphics(), new Rectangle(0, 0, this.Width, this.Height));
            _cellSize = this.Width / _cellsCapacity;

            Thread delayedGC = new Thread(GCcollectWithDelay);
            delayedGC.Start(500);
        }

        private void GCcollectWithDelay(object parData)
        {
            Thread.Sleep((int)parData);
            GC.Collect();
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
