using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using AlienExplorer.Model;

namespace AlienExplorer.View
{
    /// <summary>
    /// Вид - форма Windows.
    /// </summary>
    public partial class FormMain : Form, IViewable
    {
        /// <summary>
        /// Объект графики формы.
        /// </summary>
        private Graphics _formGraphics;
        /// <summary>
        /// Объект буферизированного создателя кадров.
        /// </summary>
        private BufferedWinFormDrawer _bufferedDrawer;
        
        /// <summary>
        /// Инициализирует форму начальными значениями.
        /// </summary>
        public FormMain()
        {
            InitializeComponent();

            Cursor.Hide();
            _formGraphics = this.CreateGraphics();
            _bufferedDrawer = new BufferedWinFormDrawer(_formGraphics, this.Width, this.Height);
        }

        /// <summary>
        /// Отправка ссылки на метод установки размеров камеры в модели.
        /// </summary>
        /// <param name="parSetCameraSize">Ссылка на метод.</param>
        public void SendCameraSizeDelegateSending(dSetCameraSize parSetCameraSize)
        {
            _bufferedDrawer.SetCameraSize += parSetCameraSize;
        }

        /// <summary> 
        /// Установка размеров камеры в модели.
        /// </summary> 
        public void SendCameraSizeToModel()
        {
            _bufferedDrawer?.FormSizeChanged(_formGraphics, this.Width, this.Height);
        }

        /// <summary>
        /// Отображение модели.
        /// </summary>
        /// <param name="parModel">Модель.</param>
        public void ViewModel(GameModel parModel)
        {
            Invoke((MethodInvoker)(() => _bufferedDrawer.DrawFrameToForm(parModel, _formGraphics)));
        }

        /// <summary>
        /// Обработчик события изменения размеров формы.
        /// </summary>
        /// <param name="sender">Отправитель события.</param>
        /// <param name="e">Аргументы события.</param>
        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            _formGraphics = this.CreateGraphics();
            _bufferedDrawer?.FormSizeChanged(_formGraphics, this.Width, this.Height);

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
