using System;
using System.Threading;

namespace AlienExplorer.Model
{
    /// <summary>
    /// Логика летучей мыши.
    /// </summary>
    public class BatLogic : BaseObjectLogic<BatStateMachine, BatStateType>
    {
        /// <summary>
        /// Скорость перемещения.
        /// </summary>
        private static readonly float MOVE_SPEED = MAX_SPEED / 10;
        /// <summary>
        /// Период задержки для цикла вычислений состояния объекта.
        /// </summary>
        private static readonly int THREAD_SLEEP_MS = 15;

        /// <summary>
        /// Целевой объект.
        /// </summary>
        public EnemyObject Enemy { get { return (EnemyObject)Object; } set { Object = value; } }

        /// <summary>
        /// Инициализирует логику объекта.
        /// </summary>
        /// <param name="parLevel">Уровень.</param>
        /// <param name="parEnemy">Объект.</param>
        public BatLogic(GameModel parLevel, EnemyObject parEnemy) : base(parLevel)
        {
            Enemy = parEnemy;
        }

        /// <summary>
        /// Потоковый цикл вычислений.
        /// </summary>
        protected override void IterativeAction()
        {
            // left, up, right, down
            float[ ] freeSpace = new float[4] { 0, 0, 0, 0 };
            // x, y
            float[ ] speed = new float[2] { 0, 0 };
            float[ ] move = new float[2] { 0, 0 };
            float targetX = Enemy.LeftWalkingBoundX;
            float targetY = Enemy.LeftWalkingBoundY;

            _timer = DateTime.UtcNow;
            
            while (!_stopThread)
            {
                _manualResetEventSlim.Wait();
                freeSpace = FindFreeSpace();
                DateTime newTimer = DateTime.UtcNow;
                float deltaSeconds = (float)(newTimer - _timer).TotalSeconds;
                _timer = newTimer;
                speed = FindSpeed(speed, freeSpace, deltaSeconds, ref targetX, ref targetY);
                move = FindMove(speed, freeSpace, deltaSeconds);
                MoveObject(move);
                FlipObject(move);
                _stateMachine.ChangeState(Enemy, freeSpace, move, deltaSeconds);
                Thread.Sleep(THREAD_SLEEP_MS);
            }
        }

        /// <summary>
        /// Нахождение вектора скорости.
        /// </summary>
        /// <param name="parSpeed">Вектор скорости (X, Y) на предыдущем шаге.</param>
        /// <param name="parFreeSpace">Массив свободных расстояний вокруг объекта (слева, сверху, справа, снизу).</param>
        /// <param name="parDeltaSeconds">Время, прошедшее с предыдущего шага (в секундах).</param>
        /// <param name="refTargetX">Целевая точка (координата X).</param>
        /// <param name="refTargetY">Целевая точка (координата Y).</param>
        /// <returns>Вектор скорости (X, Y).</returns>
        private float[ ] FindSpeed(
            float[ ] parSpeed,
            float[ ] parFreeSpace,
            float parDeltaSeconds,
            ref float refTargetX,
            ref float refTargetY)
        {
            float[ ] speed = new float[2] { 0, 0 };

            float dX = Math.Abs(Enemy.X - refTargetX);
            float dY = Math.Abs(Enemy.Y - refTargetY);
            float speedMultiplier;
            float path = (float)Math.Sqrt(dX * dX + dY * dY);
            if (path > MOVE_SPEED * parDeltaSeconds)
            {
                speedMultiplier = MOVE_SPEED / path;
            }
            else
            {
                speedMultiplier = EPSILON * 2 / path;
            }

            // Обработка движений
            if ((dX < EPSILON) && (dY < EPSILON))
            {
                if ((Math.Abs(Enemy.LeftWalkingBoundX - refTargetX) < EPSILON) && (Math.Abs(Enemy.LeftWalkingBoundY - refTargetY) < EPSILON))
                {
                    refTargetX = Enemy.RightWalkingBoundX;
                    refTargetY = Enemy.RightWalkingBoundY;
                }
                else
                {
                    refTargetX = Enemy.LeftWalkingBoundX;
                    refTargetY = Enemy.LeftWalkingBoundY;
                }
            }

            // Нахождение скорости
            if (dX > EPSILON)
            {
                if (Enemy.X < refTargetX)
                {
                    speed[0] = dX * speedMultiplier;
                }
                else
                {
                    speed[0] = -dX * speedMultiplier;
                }
            }

            if (dY > EPSILON)
            {
                if (Enemy.Y < refTargetY)
                {
                    speed[1] = dY * speedMultiplier;
                }
                else
                {
                    speed[1] = -dY * speedMultiplier;
                }
            }

            return speed;
        }
    }
}
