using System;
using System.Threading;
using System.Linq;

namespace AlienExplorer.Model
{
    /// <summary>
    /// Абстрактный базовый класс для логик объектов.
    /// </summary>
    /// <typeparam name="StateMachineType">Тип автомата состояний.</typeparam>
    /// <typeparam name="StateType">Тип состояний автомата.</typeparam>
    public abstract class BaseObjectLogic<StateMachineType, StateType>
        : ILogic
        where StateMachineType : ObjectStateMachine<StateType>, new()
    {
        /// <summary>
        /// Погрешность вычислений.
        /// </summary>
        protected static readonly float EPSILON = 0.01f;
        /// <summary>
        /// Максимальная скорость перемещения (клеток/сек.).
        /// </summary>
        public static readonly float MAX_SPEED = 9.0f;
        /// <summary>
        /// Ускорение свободного падения (клеток/сек.^2).
        /// </summary>
        protected static readonly float G = 15.0f;
        /// <summary>
        /// Дистанция для анализа объектов (в клетках).
        /// </summary>
        protected static readonly float LOOKUP_DIST = 1.5f;

        /// <summary>
        /// Автомат состояний.
        /// </summary>
        protected StateMachineType _stateMachine;
        /// <summary>
        /// Флаг остановки потокового цикла логики.
        /// </summary>
        protected bool _stopThread;
        /// <summary>
        /// Таймер для нахождения дельты времени в потоковом цикле логики.
        /// </summary>
        protected DateTime _timer;
        /// <summary>
        /// Событие для блокировки потока во время паузы.
        /// </summary>
        protected ManualResetEventSlim _manualResetEventSlim;

        /// <summary>
        /// Уровень.
        /// </summary>
        public GameModel Level { get; set; }
        /// <summary>
        /// Целевой объект логики.
        /// </summary>
        public GameObject Object { get; set; }

        /// <summary>
        /// Инициализирует логику объекта.
        /// </summary>
        /// <param name="parLevel">Уровень.</param>
        public BaseObjectLogic(GameModel parLevel)
        {
            Level = parLevel;
            _stateMachine = new StateMachineType();
        }

        /// <summary>
        /// Запуск потокового цикла вычислений.
        /// </summary>
        /// <param name="parManualResetEventSlim">Событие для дальнейшей постановки потока на паузу.</param>
        public void Start(ManualResetEventSlim parManualResetEventSlim)
        {
            _manualResetEventSlim = parManualResetEventSlim;
            _stopThread = false;
            Thread logicThread = new Thread(IterativeAction)
            {
                IsBackground = true
            };
            logicThread.Start();
        }

        /// <summary>
        /// Остановка потокового цикла вычислений.
        /// </summary>
        public void Stop()
        {
            _stopThread = true;
        }

        /// <summary>
        /// Дополнительные действия при возобновлении работы потокового цикла вычислений после паузы.
        /// </summary>
        public virtual void Resume()
        {
            _timer = DateTime.UtcNow;
        }

        /// <summary>
        /// Потоковый цикл вычислений.
        /// </summary>
        protected abstract void IterativeAction();

        /// <summary>
        /// Нахождение свободного пространства вокруг целевого объекта.
        /// </summary>
        /// <returns>Массив свободных расстояний вокруг объекта (слева, сверху, справа, снизу).</returns>
        protected float[ ] FindFreeSpace()
        {
            float[ ] freeSpace = new float[4] { LOOKUP_DIST, LOOKUP_DIST, LOOKUP_DIST, LOOKUP_DIST };

            float leftBound = Object.X - LOOKUP_DIST;
            float rightBound = Object.X + Object.SizeX + LOOKUP_DIST;
            float downBound = Object.Y - LOOKUP_DIST;
            float upBound = Object.Y + Object.SizeY + LOOKUP_DIST;
            LevelObject[ ] objects = (from elObject in Level.ModelObjects
                                      where IsIntersected(leftBound, rightBound, elObject.X, elObject.X + elObject.SizeX)
                                            || IsIntersected(downBound, upBound, elObject.Y, elObject.Y + elObject.SizeY)
                                      select elObject).ToArray();
            foreach (LevelObject elObject in objects)
            {
                float[ ] distances = FindDistances(elObject);
                for (int i = 0; i < 4; i++)
                {
                    if (distances[i] < freeSpace[i])
                    {
                        freeSpace[i] = distances[i];
                    }
                }
            }

            return freeSpace;
        }

        /// <summary>
        /// Проверка пересечения двух отрезков.
        /// </summary>
        /// <param name="parMin1">Левая граница отрезка 1.</param>
        /// <param name="parMax1">Правая граница отрезка 1.</param>
        /// <param name="parMin2">Левая граница отрезка 2.</param>
        /// <param name="parMax2">Правая граница отрезка 2.</param>
        /// <returns>True, если отрезки пересекаются.</returns>
        protected bool IsIntersected(float parMin1, float parMax1, float parMin2, float parMax2)
        {
            return ((parMax2 >= parMin1) && (parMax2 <= parMax1))
                || ((parMax2 > parMax1) && (parMin2 <= parMax1));
        }

        /// <summary>
        /// Нахождение расстояний между целевым объектом и данным.
        /// </summary>
        /// <param name="parObject">Объект, с которым производится сравнение.</param>
        /// <returns>Массив свободных расстояний вокруг целевого объекта (слева, сверху, справа, снизу).</returns>
        private float[ ] FindDistances(LevelObject parObject)
        {
            float[ ] distances = new float[4] { LOOKUP_DIST, LOOKUP_DIST, LOOKUP_DIST, LOOKUP_DIST };
            if (IsIntersected(Object.X, Object.X + Object.SizeX, parObject.X, parObject.X + parObject.SizeX))
            {
                // Наблюдатель выше объекта
                if (Object.Y - parObject.Y > EPSILON)
                {
                    distances[3] = Object.Y - (parObject.Y + parObject.SizeY);
                }
                // Наблюдатель ниже объекта
                else
                {
                    distances[1] = parObject.Y - (Object.Y + Object.SizeY);
                }
            }
            else if (IsIntersected(Object.Y, Object.Y + Object.SizeY, parObject.Y, parObject.Y + parObject.SizeY))
            {
                // Наблюдатель правее объекта
                if (Object.X - parObject.X > EPSILON)
                {
                    distances[0] = Object.X - (parObject.X + parObject.SizeX);
                }
                // Наблюдатель левее объекта
                else
                {
                    distances[2] = parObject.X - (Object.X + Object.SizeX);
                }
            }

            return distances;
        }

        /// <summary>
        /// Нахождение вектора перемещения.
        /// </summary>
        /// <param name="parSpeed">Вектор скорости (X, Y).</param>
        /// <param name="parFreeSpace">Массив свободных расстояний вокруг объекта (слева, сверху, справа, снизу).</param>
        /// <param name="parDeltaSeconds">Время, прошедшее с предыдущего шага (в секундах).</param>
        /// <returns>Вектор перемещения (X, Y).</returns>
        protected float[ ] FindMove(float[ ] parSpeed, float[ ] parFreeSpace, float parDeltaSeconds)
        {
            float[ ] move = new float[2];
            move[0] = parSpeed[0] * parDeltaSeconds;
            move[1] = parSpeed[1] * parDeltaSeconds;

            if (move[0] > EPSILON)
            {
                if ((parFreeSpace[2] - move[0]) < EPSILON)
                {
                    move[0] = parFreeSpace[2] - EPSILON / 2;
                }
            }
            else
            {
                if ((parFreeSpace[0] + move[0]) < EPSILON)
                {
                    move[0] = -(parFreeSpace[0] - EPSILON / 2);
                }
            }

            if (move[1] > EPSILON)
            {
                if ((parFreeSpace[1] - move[1]) < EPSILON)
                {
                    move[1] = parFreeSpace[1] - EPSILON / 2;
                }
            }
            else
            {
                if ((parFreeSpace[3] + move[1]) < EPSILON)
                {
                    move[1] = -(parFreeSpace[3] - EPSILON / 2);
                }
            }

            return move;
        }

        /// <summary>
        /// Перемещение объекта.
        /// </summary>
        /// <param name="parMove">Вектор перемещения (X, Y).</param>
        protected void MoveObject(float[ ] parMove)
        {
            Object.X += parMove[0];
            Object.Y += parMove[1];
        }

        /// <summary>
        /// Отражение объекта по вертикали (при необходимости).
        /// </summary>
        /// <param name="parMove">Вектор перемещения (X, Y).</param>
        protected void FlipObject(float[ ] parMove)
        {
            if (Math.Abs(parMove[0]) > EPSILON)
            {
                Object.FlippedY = parMove[0] < 0;
            }
        }
    }
}
