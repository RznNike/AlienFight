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
    /// <summary>
    /// Вид - форма Windows.
    /// </summary>
    public partial class FormMain : Form, IViewable
    {
        /// <summary>
        /// Максимальная емкость экрана по ширине (в клетках).
        /// </summary>
        private static readonly int CELLS_CAPACITY_MAX = 15;
        /// <summary>
        /// Множитель размера шрифта относительно ширины экрана.
        /// </summary>
        private static readonly float FONT_MULTIPLIER = 1f / 40;
        /// <summary>
        /// Емкость меню (в строках).
        /// </summary>
        private static readonly int MENU_CAPACITY = 7;
        /// <summary>
        /// Смещение меню от верха экрана (доля высоты экрана).
        /// </summary>
        private static readonly float MENU_OFFSET_Y = 0.35f;
        /// <summary>
        /// Объект графики формы.
        /// </summary>
        private Graphics _formGraphics;
        /// <summary>
        /// Контекст буферизированной графики.
        /// </summary>
        private BufferedGraphicsContext _bufGraphicsContext;
        /// <summary>
        /// Объект буферизированной графики.
        /// </summary>
        private BufferedGraphics _bufGraphics;
        /// <summary>
        /// Хранилище спрайтов.
        /// </summary>
        private SpritesContainer _spritesContainer;
        /// <summary>
        /// Текстурные кисти для заливки фона.
        /// </summary>
        private TextureBrush[] _backgroundBrushes;
        /// <summary>
        /// Размер клетки в пикселях.
        /// </summary>
        private int _cellSize;
        /// <summary>
        /// Текущая емкость экрана по ширине (в клетках).
        /// </summary>
        private int _cellsCapacity = CELLS_CAPACITY_MAX;
        /// <summary>
        /// Коррекция размера клетки (в большую сторону) для устранения промежутков между клетками (в пикселях).
        /// </summary>
        private float _drawingCorrection;
        /// <summary>
        /// Коллекция шрифтов.
        /// </summary>
        private PrivateFontCollection _fontCollection;
        /// <summary>
        /// Шрифт заголовка.
        /// </summary>
        private Font _headerFont;
#if FPSMETER
        /// <summary>
        /// Массив счетчиков FPS.
        /// </summary>
        private int[ ] _counter;
        /// <summary>
        /// Таймер для отсчета FPS.
        /// </summary>
        private DateTime _time;
#endif

        /// <summary>
        /// Ссылка на метод модели по установке размеров камеры.
        /// </summary>
        public dSetCameraSize SetCameraSize { get; set; }

        /// <summary>
        /// Инициализирует форму начальными значениями.
        /// </summary>
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
            _cellsCapacity = CELLS_CAPACITY_MAX;
            FindCellSize();
            Cursor.Hide();
            _fontCollection = ResourceLoader.LoadFontCollection();
            this.Font = new Font(_fontCollection.Families[0], this.Width * FONT_MULTIPLIER, FontStyle.Regular, GraphicsUnit.Point, 0);
            _headerFont = new Font(_fontCollection.Families[0], this.Width * FONT_MULTIPLIER * 2, FontStyle.Regular, GraphicsUnit.Point, 0);
            SendCameraSizeToModel();
#if FPSMETER
            _counter = new int[4];
            _time = DateTime.UtcNow;
#endif
        }

        /// <summary>
        /// Вычисление емкости экрана в клетках и размера клетки.
        /// </summary>
        /// <param name="parModel">Модель.</param>
        private void FindCellsCapacityAndSize(GameModel parModel)
        {
            int newCellsCapacity;
            if (((parModel.SizeX - 1) > CELLS_CAPACITY_MAX) || (parModel.SizeX < 2))
            {
                newCellsCapacity = CELLS_CAPACITY_MAX;
            }
            else
            {
                newCellsCapacity = (parModel.SizeX - 1);
            }
            if (newCellsCapacity != _cellsCapacity)
            {
                _cellsCapacity = newCellsCapacity;
                FindCellSize();
            }
        }

        /// <summary>
        /// Вычисление размера клетки.
        /// </summary>
        private void FindCellSize()
        {
            _cellSize = this.Width / _cellsCapacity;
            _drawingCorrection = _cellSize / 100f;
            SendCameraSizeToModel();
        }

        /// <summary>
        /// Установка размеров камеры в модели.
        /// </summary>
        public void SendCameraSizeToModel()
        {
            SetCameraSize?.Invoke(this.Width * 1.0f / _cellSize, this.Height * 1.0f / _cellSize);
        }

        /// <summary>
        /// Отображение модели.
        /// </summary>
        /// <param name="parModel">Модель.</param>
        public void ViewModel(GameModel parModel)
        {
            FindCellsCapacityAndSize(parModel);

            DrawBackground(parModel.Type);
            DrawLevel(parModel);
            DrawUI(parModel);
            ViewCanvas();
        }

        /// <summary>
        /// Отрисовка фона.
        /// </summary>
        /// <param name="parModelType">Тип модели.</param>
        private void DrawBackground(GameModelType parModelType)
        {
            _bufGraphics.Graphics.FillRectangle(_backgroundBrushes[(int)parModelType], 0, 0, this.Width, this.Height);
        }

        /// <summary>
        /// Отрисовка модели.
        /// </summary>
        /// <param name="parModel">Модель.</param>
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

        /// <summary>
        /// Отрисовка объекта модели.
        /// </summary>
        /// <param name="parObject">Объект.</param>
        /// <param name="parModel">Модель.</param>
        /// <param name="parCameraX">Координата X камеры.</param>
        /// <param name="parCameraY">Координата Y камеры.</param>
        private void DrawGameObject(GameObject parObject, GameModel parModel, float parCameraX, float parCameraY)
        {
            if (IsVisible(parObject, parModel))
            {
                Image sprite = _spritesContainer.GetLevelSprite(parObject);
                _bufGraphics.Graphics.DrawImage(
                    sprite,
                    parObject.X * _cellSize - parCameraX * _cellSize - _drawingCorrection,
                    this.Height - (parObject.Y * _cellSize + parObject.SizeY * _cellSize - parCameraY * _cellSize) - _drawingCorrection,
                    parObject.SizeX * _cellSize + _drawingCorrection * 2,
                    parObject.SizeY * _cellSize + _drawingCorrection * 2);
            }
        }

        /// <summary>
        /// Определение, виден ли объект на экране.
        /// </summary>
        /// <param name="parObject">Объект.</param>
        /// <param name="parModel">Модель.</param>
        /// <returns>True, если виден.</returns>
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

        /// <summary>
        /// Отрисовка интерфейса.
        /// </summary>
        /// <param name="parModel"></param>
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
                _bufGraphics.Graphics.DrawString(header, _headerFont, Brushes.LightBlue, offsetX, this.Height / 15f);
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

        /// <summary>
        /// Получение видимого диапазона элементов меню.
        /// </summary>
        /// <param name="parModel">Модель.</param>
        /// <param name="refOffset">Смещение по строкам на экране.</param>
        /// <param name="refDrawUpArrow">Флаг отрисовки кнопки "вверх".</param>
        /// <param name="refDrawDownArrow">Флаг отрисовки кнопки "вниз".</param>
        /// <returns>Список видимых элементов меню.</returns>
        private List<UIObject> GetVisibleMenuRange(
            GameModel parModel,
            ref int refOffset,
            ref bool refDrawUpArrow,
            ref bool refDrawDownArrow)
        {
            List<UIObject> result = null;
            refOffset = 0;

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

        /// <summary>
        /// Отрисовка элемента меню.
        /// </summary>
        /// <param name="parObject">Элемент меню.</param>
        /// <param name="parRowNumber">Номер строки для вывода.</param>
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

        /// <summary>
        /// Вывод кадра из буфера на форму.
        /// </summary>
        private void ViewCanvas()
        {
#if FPSMETER
            _bufGraphics.Graphics.DrawString($"FPS: {_counter[0] + _counter[1] + _counter[2]}", this.Font, Brushes.White, 0, 0);
            FormMain_Paint();
#endif
            _bufGraphics.Render(_formGraphics);
        }

        /// <summary>
        /// Обработчик события изменения размеров формы.
        /// </summary>
        /// <param name="sender">Отправитель события.</param>
        /// <param name="e">Аргументы события.</param>
        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            _formGraphics = this.CreateGraphics();
            _bufGraphicsContext.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
            _bufGraphics = _bufGraphicsContext.Allocate(this.CreateGraphics(), new Rectangle(0, 0, this.Width, this.Height));
            FindCellSize();
            if (_fontCollection != null)
            {
                this.Font = new Font(_fontCollection.Families[0], this.Width * FONT_MULTIPLIER, FontStyle.Regular, GraphicsUnit.Point, 0);
            }
            SendCameraSizeToModel();

            Thread delayedGC = new Thread(GCcollectWithDelay);
            delayedGC.Start(500);
        }

        /// <summary>
        /// Сборка мусора с задержкой.
        /// </summary>
        /// <param name="parData">Задержка (в миллисекундах).</param>
        private void GCcollectWithDelay(object parData)
        {
            Thread.Sleep((int)parData);
            GC.Collect();
        }

#if FPSMETER
        /// <summary>
        /// Реакция на запуск отрисовки готового кадра на форме.
        /// </summary>
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

        /// <summary>
        /// Обработчик события закрытия формы.
        /// </summary>
        /// <param name="sender">Отправитель события.</param>
        /// <param name="e">Аргументы события.</param>
        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {   
            Environment.Exit(0);
        }
    }
}
