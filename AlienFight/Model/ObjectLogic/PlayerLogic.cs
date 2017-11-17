using System;
using System.Collections.Generic;
using System.Threading;

namespace AlienExplorer.Model
{
    public class PlayerLogic : BaseObjectLogic<PlayerStateMachine, PlayerStateType>
    {
        private static readonly float HORISONTAL_SPEED = MAX_SPEED / 3;
        private static readonly float JUMP_SPEED = MAX_SPEED / 1.5f;
        private static readonly int MAX_JUMPS = 2;
        private static readonly float HURT_COOLDOWN_TIME = 1f;
        private static readonly int THREAD_SLEEP_MS = 5;

        public PlayerObject Player { get { return (PlayerObject)Object; } set { Object = value; } }
        private List<ModelCommand> _activeCommands;

        public PlayerLogic(GameModel parLevel) : base(parLevel)
        {
            Player = parLevel.Player;
            _activeCommands = new List<ModelCommand>();
        }

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
        
        protected override void IterativeAction()
        {
            // left, up, right, down
            float[ ] freeSpace = new float[4] { 0, 0, 0, 0 };
            // x, y
            float[ ] speed = new float[2] { 0, 0 };
            float[ ] move = new float[2] { 0, 0 };

            int jumpsCount = 0;
            bool jumpActive = false;

            DateTime timer = DateTime.UtcNow;
            float hurtCooldown = 0;

            while (!_stopThread)
            {
                freeSpace = FindFreeSpace();
                DateTime newTimer = DateTime.UtcNow;
                float deltaSeconds = (float)(newTimer - timer).TotalSeconds;
                timer = newTimer;
                speed = FindSpeed(speed, freeSpace, deltaSeconds, ref jumpsCount, ref jumpActive, ref hurtCooldown);
                move = FindMove(speed, freeSpace, deltaSeconds);
                MoveObject(move);
                FlipObject(move);
                _stateMachine.ChangeState(Player, freeSpace, move, deltaSeconds);
                Thread.Sleep(THREAD_SLEEP_MS);
            }
        }

        private float[ ] FindSpeed(
            float[ ] parSpeed,
            float[ ] parFreeSpace,
            float parDeltaSeconds,
            ref int refJumpsCount,
            ref bool refJumpActive,
            ref float hurtCooldown)
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
            if (hurtCooldown > EPSILON)
            {
                hurtCooldown -= parDeltaSeconds;
            }
            else
            {
                if (IsEnemyAttacked())
                {
                    speed[1] = JUMP_SPEED / 1.5f;
                    hurtCooldown = HURT_COOLDOWN_TIME;
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

        private bool IsEnemyAttacked()
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
