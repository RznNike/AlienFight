using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace AlienFight.Model
{
    public class PlayerLogic
    {
        private static readonly float EPSILON = 0.01f;
        private static readonly float G = 15.0f;
        private static readonly float MAX_SPEED = 9.0f;
        private static readonly float HORISONTAL_SPEED = MAX_SPEED / 3;
        private static readonly float JUMP_SPEED = MAX_SPEED / 1.5f;
        private static readonly int MAX_JUMPS = 2;
        private static readonly float LOOKUP_DIST = 1.5f;

        public GameLevel Level { get; set; }
        public PlayerObject Player { get; set; }
        private PlayerStateMachine _stateMachine;
        private HashSet<PlayerCommand> _activeCommands;
        private Thread _logicThread;
        private bool _stopThread;

        public PlayerLogic(GameLevel parLevel)
        {
            Level = parLevel;
            Player = parLevel.Player;
            _stateMachine = new PlayerStateMachine();
            _activeCommands = new HashSet<PlayerCommand>();
        }

        public void ReceiveCommand(PlayerCommand parCommand, bool parBeginCommand)
        {
            if (parBeginCommand)
            {
                _activeCommands.Add(parCommand);
                if (parCommand == PlayerCommand.Left)
                {
                    _activeCommands.Remove(PlayerCommand.Right);
                }
                else if (parCommand == PlayerCommand.Right)
                {
                    _activeCommands.Remove(PlayerCommand.Left);
                }
            }
            else
            {
                _activeCommands.Remove(parCommand);
            }
        }

        public void Start()
        {
            _stopThread = false;
            _logicThread = new Thread(IterativeAction)
            {
                IsBackground = true
            };
            _logicThread.Start();
        }

        public void Stop()
        {
            _stopThread = true;
            if (_logicThread != null)
            {
                _logicThread.Resume();
            }
        }

        public void Pause()
        {
            if (_logicThread != null)
            {
                _logicThread.Suspend();
            }
        }

        public void Resume()
        {
            if (_logicThread != null)
            {
                _logicThread.Resume();
            }
        }

        private void IterativeAction()
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
                MovePlayer(move);
                FlipPayer(move);
                _stateMachine.ChangeState(Player, freeSpace, move, deltaSeconds);
                Thread.Sleep(5);
            }
        }

        private float[ ] FindFreeSpace()
        {
            float[ ] freeSpace = new float[4] { LOOKUP_DIST, LOOKUP_DIST, LOOKUP_DIST, LOOKUP_DIST };

            float leftBound = Player.X - LOOKUP_DIST;
            float rightBound = Player.X + Player.SizeX + LOOKUP_DIST;
            float downBound = Player.Y - LOOKUP_DIST;
            float upBound = Player.Y + Player.SizeY + LOOKUP_DIST;
            LevelObject[ ] objects = (from elObject in Level.LevelObjects
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

        private bool IsIntersected(float parMin1, float parMax1, float parMin2, float parMax2)
        {
            return ((parMax2 >= parMin1) && (parMax2 <= parMax1))
                || ((parMax2 > parMax1) && (parMin2 <= parMax1));
        }

        private float[ ] FindDistances(LevelObject parObject)
        {
            float[ ] distances = new float[4] { LOOKUP_DIST, LOOKUP_DIST, LOOKUP_DIST, LOOKUP_DIST };
            if (IsIntersected(Player.X, Player.X + Player.SizeX, parObject.X, parObject.X + parObject.SizeX))
            {
                // Игрок выше объекта
                if (Player.Y - parObject.Y > EPSILON)
                {
                    distances[3] = Player.Y - (parObject.Y + parObject.SizeY);
                }
                // Игрок ниже объекта
                else
                {
                    distances[1] = parObject.Y - (Player.Y + Player.SizeY);
                }
            }
            else if (IsIntersected(Player.Y, Player.Y + Player.SizeY, parObject.Y, parObject.Y + parObject.SizeY))
            {
                // Игрок правее объекта
                if (Player.X - parObject.X > EPSILON)
                {
                    distances[0] = Player.X - (parObject.X + parObject.SizeX);
                }
                // Игрок левее объекта
                else
                {
                    distances[2] = parObject.X - (Player.X + Player.SizeX);
                }
            }

            return distances;
        }

        private float[ ] FindSpeed(
            float[ ] parSpeed,
            float[ ] parFreeSpace,
            float parDeltaSeconds,
            ref int refJumpsCount,
            ref bool refJumpActive)
        {
            float[ ] speed = new float[2] { 0, 0 };

            // Обработка движений по горизонтали
            if (_activeCommands.Contains(PlayerCommand.Left) && (parFreeSpace[0] > EPSILON))
            {
                speed[0] = -HORISONTAL_SPEED;
            }
            else if (_activeCommands.Contains(PlayerCommand.Right) && (parFreeSpace[2] > EPSILON))
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

        private float[ ] FindMove(float[ ] parSpeed, float[ ] parFreeSpace, float parDeltaSeconds)
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

        private void MovePlayer(float[ ] parMove)
        {
            Player.X += parMove[0];
            Player.Y += parMove[1];
        }

        private void FlipPayer(float[ ] parMove)
        {
            if (Math.Abs(parMove[0]) > EPSILON)
            {
                Player.FlippedY = parMove[0] < 0;
            }
        }
    }
}
