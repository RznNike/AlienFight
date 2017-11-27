using AlienExplorer.Model;

namespace AlienExplorer.View
{
    /// <summary>
    /// Общий интерфейс для всех видов.
    /// </summary>
    public interface IViewable
    {
        /// <summary>
        /// Отображение модели.
        /// </summary>
        /// <param name="parModel">Модель.</param>
        void ViewModel(GameModel parModel);

        /// <summary>
        /// Отправка ссылки на метод установки размеров камеры в модели.
        /// </summary>
        /// <param name="parSetCameraSize">Ссылка на метод.</param>
        void SendCameraSizeDelegateSending(dSetCameraSize parSetCameraSize);

        /// <summary> 
        /// Установка размеров камеры в модели.
        /// </summary> 
        void SendCameraSizeToModel();
    }
}
