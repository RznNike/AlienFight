namespace AlienExplorer.Model
{
    /// <summary>
    /// Команда контроллеру загрузить другую модель. Вызывается логикой старой модели через делегат.
    /// </summary>
    /// <param name="parModelType">Тип модели для загрузки.</param>
    /// <param name="parLevelID">(Необязательно) ID уровня.</param>
    public delegate void dLoadAnotherModel(GameModelType parModelType, int parLevelID = 1);

    /// <summary>
    /// Закрытие приложения. Вызывается при закрытии вида или из логики модели через делегат.
    /// </summary>
    public delegate void dCloseApplication();

    /// <summary>
    /// Изменение размеров камеры модели. Вызывается видом через делегат.
    /// </summary>
    /// <param name="parSizeX">Размер камеры по горизонтали (в клетках).</param>
    /// <param name="parSizeY">Размер камеры по вертикали (в клетках).</param>
    public delegate void dSetCameraSize(float parSizeX, float parSizeY);
}
