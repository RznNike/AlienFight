using System.Threading;

namespace AlienExplorer.Model
{
    /// <summary>
    /// Общий интерфейс для логик объектов, определяет действия для управления их внутренними потоками вычислений.
    /// </summary>
    public interface ILogic
    {
        /// <summary>
        /// Запуск потокового цикла вычислений.
        /// </summary>
        /// <param name="parManualResetEventSlim">Событие для дальнейшей постановки потока на паузу.</param>
        void Start(ManualResetEventSlim parManualResetEventSlim);

        /// <summary>
        /// Остановка потокового цикла вычислений.
        /// </summary>
        void Stop();

        /// <summary>
        /// Дополнительные действия при возобновлении работы потокового цикла вычислений после паузы.
        /// </summary>
        void Resume();
    }
}
