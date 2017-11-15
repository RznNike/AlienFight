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
        private TextureBrush[] _backgroundBrushes;
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
            _backgroundBrushes = new TextureBrush[2];
            _backgroundBrushes[0] = new TextureBrush(_spritesContainer.GetBackground((GameModelType)0));
            _backgroundBrushes[1] = new TextureBrush(_spritesContainer.GetBackground((GameModelType)1));
            _cellSize = this.Width / _cellsCapacity;
            _drawingCorrection = _cellSize / 150f;
            Cursor.Hide();
        }

        public void ViewModel(GameModel parModel)
        {
            DrawBackground(parModel.Type);
            DrawLevel(parModel);
            DrawUI(parModel);
            ViewCanvas(parModel);
        }

        private void DrawBackground(GameModelType parModelType)
        {
            _bufGraphics.Graphics.FillRectangle(_backgroundBrushes[(int)parModelType], 0, 0, this.Width, this.Height);
        }

        private void DrawLevel(GameModel parModel)
        {
            float cameraX = parModel.CameraX;
            float cameraY = parModel.CameraY;
            foreach (GameObject elLevelElement in parModel.ModelObjects)
            {
                DrawGameObject(elLevelElement, parModel, cameraX, cameraY);
            }
            foreach (GameObject elEnemy in parModel.Enemies)
            {
                DrawGameObject(elEnemy, parModel, cameraX, cameraY);
            }
            if (parModel.Player != null)
            {
                DrawGameObject(parModel.Player, parModel, cameraX, cameraY);
            }
        }

        private void DrawUI(GameModel parModel)
        {
            float cameraX = parModel.CameraX;
            float cameraY = parModel.CameraY;
            foreach (GameObject elUI in parModel.UIItems)
            {
                DrawGameObject(elUI, parModel, cameraX, cameraY);
            }
        }

        private void DrawGameObject(GameObject parObject, GameModel parModel, float parCameraX, float parCameraY)
        {
            if (IsVisible(parObject, parModel))
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

        private bool IsVisible(GameObject parObject, GameModel parModel)
        {
            double leftBound = parModel.CameraX;
            double rightBound = parModel.CameraX + this.Width / _cellSize;
            double downBound = parModel.CameraY;
            double upBound = parModel.CameraY + this.Height / _cellSize;

            return ((parObject.X < rightBound)
                    || ((parObject.X + parObject.SizeX) > leftBound))
                   && ((parObject.Y < upBound)
                    || ((parObject.Y + parObject.SizeY) > downBound));
        }

        private void ViewCanvas(GameModel parModel)
        {
#if FPSMETER
            _bufGraphics.Graphics.DrawString($"FPS: {_counter[0] + _counter[1] + _counter[2]}, HP: {parModel.Player.Health}", this.Font, Brushes.White, 0, 0);
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
