using System;
using System.Linq;
using System.Threading;

namespace AlienExplorer.Model
{
    /// <summary>
    /// Логика призрака.
    /// </summary>
    public class GhostLogic : BaseObjectLogic<GhostStateMachine, GhostStateType>
    {
        /// <summary>
        /// Скорость перемещения.
        /// </summary>
        private static readonly float HORISONTAL_SPEED = PlayerLogic.HORISONTAL_SPEED;
        /// <summary>
        /// Период бездействия после атаки (в секундах).
        /// </summary>
        private static readonly float ATTACK_COOLDOWN_TIME = 1f;
        /// <summary>
        /// Период задержки для цикла вычислений состояния объекта.
        /// </summary>
        private static readonly int THREAD_SLEEP_MS = 10;
        
        /// <summary>
        /// Целевой объект.
        /// </summary>
        public EnemyObject Enemy { get { return (EnemyObject)Object; } set { Object = value; } }

        /// <summary>
        /// Инициализирует логику объекта.
        /// </summary>
        /// <param name="parLevel">Уровень.</param>
        /// <param name="parEnemy">Объект.</param>
        public GhostLogic(GameModel parLevel, EnemyObject parEnemy) : base(parLevel)
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

            _timer = DateTime.UtcNow;
            float attackCooldown = 0;
            
            while (!_stopThread)
            {
                _manualResetEventSlim.Wait();
                freeSpace = FindFreeSpace();
                DateTime newTimer = DateTime.UtcNow;
                float deltaSeconds = (float)(newTimer - _timer).TotalSeconds;
                _timer = newTimer;
                speed = FindSpeed(speed, freeSpace, deltaSeconds, ref attackCooldown);
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
        /// <param name="refAttackCooldown">Таймер бездействия после атаки.</param>
        /// <returns>Вектор скорости (X, Y).</returns>
        private float[ ] FindSpeed(
            float[ ] parSpeed,
            float[ ] parFreeSpace,
            float parDeltaSeconds,
            ref float refAttackCooldown)
        {
            float[ ] speed = new float[2] { 0, 0 };

            if (refAttackCooldown > EPSILON)
            {
                refAttackCooldown -= parDeltaSeconds;
                return speed;
            }

            if ((IsIntersected(Enemy.X, Enemy.X + Enemy.SizeX, Level.Player.X, Level.Player.X + Level.Player.SizeX))
                && (IsIntersected(Enemy.Y, Enemy.Y + Enemy.SizeY, Level.Player.Y, Level.Player.Y + Level.Player.SizeY)))
            {
                refAttackCooldown = ATTACK_COOLDOWN_TIME;
                return speed;
            }

            if (IsPlayerVisible())
            {
                float playerX = Level.Player.X;
                // Обработка движений по горизонтали
                if ((Enemy.X < playerX) && (parFreeSpace[2] > EPSILON))
                {
                    speed[0] = HORISONTAL_SPEED;
                }
                else if ((Enemy.X > playerX) && (parFreeSpace[0] > EPSILON))
                {
                    speed[0] = -HORISONTAL_SPEED;
                }
            }

            return speed;
        }

        /// <summary>
        /// Определяет, виден ли игрок призраку.
        /// </summary>
        /// <returns>True, если виден.</returns>
        private bool IsPlayerVisible()
        {
            float downBound = Enemy.Y;
            float upBound = Enemy.Y + Enemy.SizeY;
            if (IsIntersected(downBound, upBound, Level.Player.Y, Level.Player.Y + Level.Player.SizeY))
            {
                int barriersCount = (from elObject in Level.ModelObjects
                                     where IsIntersected(downBound, upBound, elObject.Y, elObject.Y + elObject.SizeY)
                                           && IsBetween(elObject.X, Enemy.X, Level.Player.X)
                                     select elObject).Count();

                return barriersCount == 0;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Проверяет, находится ли значение внутри заданного диапазона.
        /// </summary>
        /// <param name="parMiddleValue">Целевое значение.</param>
        /// <param name="parBorder1">Левая граница диапазона.</param>
        /// <param name="parBorder2">Правая граница диапазона.</param>
        /// <returns>True, если значение внутри диапазона.</returns>
        private bool IsBetween(float parMiddleValue, float parBorder1, float parBorder2)
        {
            if (parBorder1 < parBorder2)
            {
                return (parMiddleValue >= parBorder1) && (parMiddleValue <= parBorder2);
            }
            else
            {
                return (parMiddleValue >= parBorder2) && (parMiddleValue <= parBorder1);
            }
        }
    }
}
