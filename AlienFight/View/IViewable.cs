using AlienExplorer.Model;

namespace AlienExplorer.View
{
    /// <summary>
    /// Общий интерфейс для всех видов.
    /// </summary>
    public interface IViewable
    {
        /// <summary>
        /// Ссылка на метод модели по установке размеров камеры.
        /// </summary>
        dSetCameraSize SetCameraSize { get; set; }

        /// <summary>
        /// Установка размеров камеры в модели.
        /// </summary>
        void SendCameraSizeToModel();
        /// <summary>
        /// Отображение модели.
        /// </summary>
        /// <param name="parModel">Модель.</param>
        void ViewModel(GameModel parModel);
    }
}
