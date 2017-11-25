using System;
using System.Collections.Generic;
using System.Threading;

namespace AlienExplorer.Model
{
    /// <summary>
    /// Логика игрока.
    /// </summary>
    public class PlayerLogic : BaseObjectLogic<PlayerStateMachine, PlayerStateType>
    {
        /// <summary>
        /// Скорость перемещения по горизонтали.
        /// </summary>
        public static readonly float HORISONTAL_SPEED = MAX_SPEED / 3;
        /// <summary>
        /// Начальная скорость прыжка.
        /// </summary>
        private static readonly float JUMP_SPEED = MAX_SPEED / 1.5f;
        /// <summary>
        /// Максимальное количество прыжков не касаясь земли.
        /// </summary>
        private static readonly int MAX_JUMPS = 2;
        /// <summary>
        /// Время неуязвимости после получения урона (в секундах).
        /// </summary>
        private static readonly float HURT_COOLDOWN_TIME = 1f;
        /// <summary>
        /// Период задержки для цикла вычислений состояния объекта.
        /// </summary>
        private static readonly int THREAD_SLEEP_MS = 5;

        /// <summary>
        /// Список активных команд.
        /// </summary>
        private List<ModelCommand> _activeCommands;
        /// <summary>
        /// Целевой объект.
        /// </summary>
        public PlayerObject Player { get { return (PlayerObject)Object; } set { Object = value; } }

        /// <summary>
        /// Инициализирует логику объекта.
        /// </summary>
        /// <param name="parLevel">Уровень.</param>
        public PlayerLogic(GameModel parLevel) : base(parLevel)
        {
            Player = parLevel.Player;
            _activeCommands = new List<ModelCommand>();
        }

        /// <summary>
        /// Получение команды.
        /// </summary>
        /// <param name="parCommand">Команда.</param>
        /// <param name="parBeginCommand">Флаг начала команды (true, если начата).</param>
        public void ReceiveCommand(ModelCommand parCommand, bool parBeginCommand)
        {
            if (parBeginCommand)
            {
                if (!_activeCommands.Contains(parCommand))
                {
                    _activeCommands.Add(parCommand);
                }
            }
            else
            {
                _activeCommands.Remove(parCommand);
            }
        }

        /// <summary>
        /// Дополнительные действия при возобновлении работы потокового цикла вычислений после паузы.
        /// </summary>
        public override void Resume()
        {
            base.Resume();
            _activeCommands.Clear();
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

            int jumpsCount = 0;
            bool jumpActive = false;

            _timer = DateTime.UtcNow;
            float hurtCooldown = 0;
            
            while (!_stopThread)
            {
                _manualResetEventSlim.Wait();
                freeSpace = FindFreeSpace();
                DateTime newTimer = DateTime.UtcNow;
                float deltaSeconds = (float)(newTimer - _timer).TotalSeconds;
                _timer = newTimer;
                speed = FindSpeed(speed, freeSpace, deltaSeconds, ref jumpsCount, ref jumpActive, ref hurtCooldown);
                move = FindMove(speed, freeSpace, deltaSeconds);
                MoveObject(move);
                FlipObject(move);
                _stateMachine.ChangeState(Player, freeSpace, move, deltaSeconds);
                Thread.Sleep(THREAD_SLEEP_MS);
            }
        }

        /// <summary>
        /// Нахождение вектора скорости.
        /// </summary>
        /// <param name="parSpeed">Вектор скорости (X, Y) на предыдущем шаге.</param>
        /// <param name="parFreeSpace">Массив свободных расстояний вокруг объекта (слева, сверху, справа, снизу).</param>
        /// <param name="parDeltaSeconds">Время, прошедшее с предыдущего шага (в секундах).</param>
        /// <param name="refJumpsCount">Количество уже совершенных прыжков до касания земли.</param>
        /// <param name="refJumpActive">Флаг активности прыжка в данный момент.</param>
        /// <param name="refHurtCooldown">Таймер неуязвимости после получения урона.</param>
        /// <returns>Вектор скорости (X, Y).</returns>
        private float[ ] FindSpeed(
            float[ ] parSpeed,
            float[ ] parFreeSpace,
            float parDeltaSeconds,
            ref int refJumpsCount,
            ref bool refJumpActive,
            ref float refHurtCooldown)
        {
            float[ ] speed = new float[2] { 0, 0 };

            // Обработка приседания
            if ((parFreeSpace[3] < EPSILON) && _activeCommands.Contains(ModelCommand.Down))
            {
                Player.SizeY = Player.SizeYsmall;
                return speed;
            }
            Player.SizeY = Player.SizeYstandart;

            // Обработка движений по горизонтали
            int leftCommandPosition = _activeCommands.FindIndex(element => element == ModelCommand.Left);
            int rightCommandPosition = _activeCommands.FindIndex(element => element == ModelCommand.Right);
            if ((leftCommandPosition >= 0) && (leftCommandPosition >rightCommandPosition) && (parFreeSpace[0] > EPSILON))
            {
                speed[0] = -HORISONTAL_SPEED;
            }
            else if ((rightCommandPosition >= 0) && (rightCommandPosition > leftCommandPosition) && (parFreeSpace[2] > EPSILON))
            {
                speed[0] = HORISONTAL_SPEED;
            }

            // Обработка движений по вертикали
            if (((parSpeed[1] > EPSILON) && (parFreeSpace[1] > EPSILON))
                || (parFreeSpace[3] > EPSILON))
            {
                speed[1] = parSpeed[1] - G * parDeltaSeconds;
                if (speed[1] < -MAX_SPEED)
                {
                    speed[1] = -MAX_SPEED;
                }
            }
            if ((parSpeed[1] > EPSILON) && (parFreeSpace[1] < EPSILON))
            {
                speed[1] = 0;
            }

            // Обработка получения урона
            if (refHurtCooldown > EPSILON)
            {
                refHurtCooldown -= parDeltaSeconds;
            }
            else
            {
                if (IsEnemyAttacks())
                {
                    speed[1] = JUMP_SPEED / 1.5f;
                    refHurtCooldown = HURT_COOLDOWN_TIME;
                }
            }

            // Сброс счетчика прыжков, если игрок на земле
            if (parFreeSpace[3] < EPSILON)
            {
                refJumpsCount = 0;
            }

            // Счетчик прыжков >= 1, если игрок в воздухе
            if ((parFreeSpace[3] > EPSILON) & (refJumpsCount == 0))
            {
                refJumpsCount = 1;
            }

            // Обработка попытки прыжка
            if (!refJumpActive && _activeCommands.Contains(ModelCommand.Up) && (refJumpsCount < MAX_JUMPS))
            {
                refJumpsCount++;
                refJumpActive = true;
                if (parFreeSpace[1] > EPSILON)
                {
                    speed[1] = JUMP_SPEED;
                }
            }
            if (!_activeCommands.Contains(ModelCommand.Up))
            {
                refJumpActive = false;
            }

            return speed;
        }

        /// <summary>
        /// Проверка наличия атаки от врага.
        /// </summary>
        /// <returns>True, если атака произошла.</returns>
        private bool IsEnemyAttacks()
        {
            foreach (EnemyObject elEnemy in Level.Enemies)
            {
                if (IsIntersected(Player.X, Player.X + Player.SizeX, elEnemy.X, elEnemy.X + elEnemy.SizeX)
                    && IsIntersected(Player.Y, Player.Y + Player.SizeY, elEnemy.Y, elEnemy.Y + elEnemy.SizeY))
                {
                    Player.Health -= elEnemy.Damage;
                    if (Player.Health < 0)
                    {
                        Player.Health = 0;
                    }
                    _stateMachine.SetState(PlayerStateType.Hurt);
                    return true;
                }
            }
            return false;
        }
    }
}
