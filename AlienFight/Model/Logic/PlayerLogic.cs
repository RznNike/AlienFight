using System;
using System.Collections.Generic;
using System.Threading;

namespace AlienFight.Model
{
    public class PlayerLogic : BaseLogic<PlayerStateMachine, PlayerStateType>
    {
        private static readonly float HORISONTAL_SPEED = MAX_SPEED / 3;
        private static readonly float JUMP_SPEED = MAX_SPEED / 1.5f;
        private static readonly int MAX_JUMPS = 2;
        protected static readonly int THREAD_SLEEP_MS = 5;

        public PlayerObject Player { get { return (PlayerObject)Object; } set { Object = value; } }
        private List<PlayerCommand> _activeCommands;

        public PlayerLogic(GameLevel parLevel) : base(parLevel)
        {
            Player = parLevel.Player;
            _activeCommands = new List<PlayerCommand>();
        }

        public void ReceiveCommand(PlayerCommand parCommand, bool parBeginCommand)
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

            while (!_stopThread)
            {
                freeSpace = FindFreeSpace();
                DateTime newTimer = DateTime.UtcNow;
                float deltaSeconds = (float)(newTimer - timer).TotalSeconds;
                timer = newTimer;
                speed = FindSpeed(speed, freeSpace, deltaSeconds, ref jumpsCount, ref jumpActive);
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
            ref bool refJumpActive)
        {
            float[ ] speed = new float[2] { 0, 0 };

            // Обработка приседания
            if ((parFreeSpace[3] < EPSILON) && _activeCommands.Contains(PlayerCommand.Down))
            {
                Player.SizeY = Player.SizeYsmall;
                return speed;
            }
            Player.SizeY = Player.SizeYstandart;

            // Обработка движений по горизонтали
            int leftCommandPosition = _activeCommands.FindIndex(element => element == PlayerCommand.Left);
            int rightCommandPosition = _activeCommands.FindIndex(element => element == PlayerCommand.Right);
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
            if (!refJumpActive && _activeCommands.Contains(PlayerCommand.Up) && (refJumpsCount < MAX_JUMPS))
            {
                refJumpsCount++;
                refJumpActive = true;
                if (parFreeSpace[1] > EPSILON)
                {
                    speed[1] = JUMP_SPEED;
                }
            }
            if (!_activeCommands.Contains(PlayerCommand.Up))
            {
                refJumpActive = false;
            }

            return speed;
        }
    }
}
