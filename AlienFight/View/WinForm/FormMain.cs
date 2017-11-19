//#define FPSMETER

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Drawing.Text;
using System.Collections.Generic;
using AlienExplorer.Model;

namespace AlienExplorer.View
{
    public partial class FormMain : Form, IViewable
    {
        private static readonly float FONT_MULTIPLIER = 1f / 40;
        private static readonly int MENU_CAPACITY = 7;
        private static readonly float MENU_OFFSET_Y = 0.35f;
        private Graphics _formGraphics;
        private BufferedGraphicsContext _bufGraphicsContext;
        private BufferedGraphics _bufGraphics;
        private SpritesContainer _spritesContainer;
        private TextureBrush[] _backgroundBrushes;
        private int _cellSize;
        private int _cellsCapacity = 15;
        private float _drawingCorrection = 0;
        private PrivateFontCollection _fontCollection;
        private Font _headerFont;
#if FPSMETER
        private int[ ] _counter;
        private DateTime _time;
#endif

        public FormMain()
        {
            InitializeComponent();

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
            _fontCollection = ResourceLoader.LoadFontCollection();
            this.Font = new Font(_fontCollection.Families[0], this.Width * FONT_MULTIPLIER, FontStyle.Regular, GraphicsUnit.Point, 0);
            _headerFont = new Font(_fontCollection.Families[0], this.Width * FONT_MULTIPLIER * 2, FontStyle.Regular, GraphicsUnit.Point, 0);
#if FPSMETER
            _counter = new int[4];
            _time = DateTime.UtcNow;
#endif
        }

        public void ViewModel(GameModel parModel)
        {
            DrawBackground(parModel.Type);
            DrawLevel(parModel);
            DrawUI(parModel);
            ViewCanvas();
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
            foreach (GameObject elDoor in parModel.Doors)
            {
                DrawGameObject(elDoor, parModel, cameraX, cameraY);
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

        private void DrawGameObject(GameObject parObject, GameModel parModel, float parCameraX, float parCameraY)
        {
            if (IsVisible(parObject, parModel))
            {
                Image sprite = _spritesContainer.GetLevelSprite(parObject, parObject.FlippedY);
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

        private void DrawUI(GameModel parModel)
        {
            if (parModel.ModelLogic.ShadowLevel)
            {
                _bufGraphics.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(180, Color.FromArgb(31, 68, 82))), 0, 0, this.Width, this.Height);
            }
            string header = parModel.ModelLogic.MenuHeader;
            if (!header.Equals(""))
            {
                int offsetX = (int)((this.Width - _bufGraphics.Graphics.MeasureString(header, _headerFont).Width) / 2);
                _bufGraphics.Graphics.DrawString(parModel.ModelLogic.MenuHeader, _headerFont, Brushes.LightBlue, offsetX, this.Height / 15f);
            }
            List<UIObject> uiList = null;
            int offset = 0;
            bool drawUpArrow = false;
            bool drawDownArrow = false;
            uiList = GetVisibleMenuRange(parModel, ref offset, ref drawUpArrow, ref drawDownArrow);
            if (drawUpArrow)
            {
                DrawUIObject(new UIObject() { Type = UIObjectType.Text, Text = "⯅" }, 0);
            }
            for (int i = 0; i < uiList.Count; i++)
            {
                Image sprite = null;
                switch (uiList[i].Type)
                {
                    case UIObjectType.Health:
                        sprite = _spritesContainer.GetUISprite(uiList[i]);
                        _bufGraphics.Graphics.DrawImage(sprite, _cellSize / 5f, this.Height - _cellSize * 0.7f, _cellSize / 2f, _cellSize / 2f);
                        _bufGraphics.Graphics.DrawString(
                            parModel.Player.Health.ToString(), this.Font, Brushes.Crimson, _cellSize / 1.5f, this.Height - _cellSize * 0.67f);
                        break;
                    case UIObjectType.Timer:
                        sprite = _spritesContainer.GetUISprite(uiList[i]);
                        TimeSpan time = ((LevelLogic)parModel.ModelLogic).LevelTimer;
                        string timeString = $"{time.Minutes:D2}:{time.Seconds:D2}.{(time.Milliseconds / 100):D1}";
                        int offsetX = (int)((this.Width - _bufGraphics.Graphics.MeasureString("00:00:0", this.Font).Width) / 2);
                        _bufGraphics.Graphics.DrawImage(sprite, offsetX - _cellSize / 2f, _cellSize / 5f, _cellSize / 2f, _cellSize / 2f);
                        _bufGraphics.Graphics.DrawString(
                            timeString, this.Font, Brushes.White, offsetX, _cellSize / 5f);
                        break;
                    default:
                        DrawUIObject(uiList[i], i + offset);
                        break;
                }
            }
            if (drawDownArrow)
            {
                DrawUIObject(new UIObject() { Type = UIObjectType.Text, Text = "⯆" }, MENU_CAPACITY - 1);
            }
        }

        private List<UIObject> GetVisibleMenuRange(
            GameModel parModel,
            ref int refOffset,
            ref bool refDrawUpArrow,
            ref bool refDrawDownArrow)
        {
            List<UIObject> result = null;

            if (parModel.UIItems.Count <= MENU_CAPACITY)
            {
                result = parModel.UIItems;
            }
            else
            {
                int selected = parModel.ModelLogic.SelectedMenuItem;
                int indexFrom = 0;
                int count = MENU_CAPACITY;

                int itemsBelow = parModel.UIItems.Count - selected - 1;
                if (itemsBelow > (MENU_CAPACITY - 1) / 2)
                {
                    refDrawDownArrow = true;
                    count--;
                }
                if (selected > (MENU_CAPACITY / 2))
                {
                    refDrawUpArrow = true;
                    count--;
                    refOffset = 1;
                    if (refDrawDownArrow)
                    {
                        indexFrom = selected + 1 - MENU_CAPACITY / 2;
                    }
                    else
                    {
                        indexFrom = selected + 2 - MENU_CAPACITY + itemsBelow;
                    }
                }
                result = parModel.UIItems.GetRange(indexFrom, count);
            }

            return result;
        }

        private void DrawUIObject(UIObject parObject, int parRowNumber)
        {
            string text = "";
            Brush brush = Brushes.White;
            if (parObject.State == 1)
            {
                brush = Brushes.GreenYellow;
            }
            if (parObject.Type == UIObjectType.Text)
            {
                text = parObject.Text;
            }
            else
            {
                text = parObject.Type.ToString().Replace('_', ' ');
            }
            int offsetX = (int)((this.Width - _bufGraphics.Graphics.MeasureString(text, this.Font).Width) / 2);
            int offsetY = (int)(this.Height * MENU_OFFSET_Y + parRowNumber * (this.Height * (1 - MENU_OFFSET_Y)) / MENU_CAPACITY);
            _bufGraphics.Graphics.DrawString(text, this.Font, brush, offsetX, offsetY);
        }

        private void ViewCanvas()
        {
#if FPSMETER
            _bufGraphics.Graphics.DrawString($"FPS: {_counter[0] + _counter[1] + _counter[2]}", this.Font, Brushes.White, 0, 0);
            FormMain_Paint();
#endif
            _bufGraphics.Render(_formGraphics);
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            _formGraphics = this.CreateGraphics();
            _bufGraphicsContext.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            _bufGraphics = _bufGraphicsContext.Allocate(this.CreateGraphics(), new Rectangle(0, 0, this.Width, this.Height));
            _cellSize = this.Width / _cellsCapacity;
            if (_fontCollection != null)
            {
                this.Font = new Font(_fontCollection.Families[0], this.Width * FONT_MULTIPLIER, FontStyle.Regular, GraphicsUnit.Point, 0);
            }

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
