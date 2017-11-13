using System;
using System.Threading;
using System.Linq;

namespace AlienFight.Model
{
    public abstract class BaseLogic<StateMachineType, StateType>
        : ILogic
        where StateMachineType : StateMachine<StateType>, new()
    {
        protected static readonly float EPSILON = 0.01f;
        protected static readonly float G = 15.0f;
        protected static readonly float MAX_SPEED = 9.0f;
        protected static readonly float LOOKUP_DIST = 1.5f;

        public GameLevel Level { get; set; }
        public GameObject Object { get; set; }
        protected StateMachineType _stateMachine;
        protected bool _stopThread;
        private Thread _logicThread;

        public BaseLogic(GameLevel parLevel)
        {
            Level = parLevel;
            _stateMachine = new StateMachineType();
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

        protected abstract void IterativeAction();

        protected float[ ] FindFreeSpace()
        {
            float[ ] freeSpace = new float[4] { LOOKUP_DIST, LOOKUP_DIST, LOOKUP_DIST, LOOKUP_DIST };

            float leftBound = Object.X - LOOKUP_DIST;
            float rightBound = Object.X + Object.SizeX + LOOKUP_DIST;
            float downBound = Object.Y - LOOKUP_DIST;
            float upBound = Object.Y + Object.SizeY + LOOKUP_DIST;
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

        protected void MoveObject(float[ ] parMove)
        {
            Object.X += parMove[0];
            Object.Y += parMove[1];
        }

        protected void FlipObject(float[ ] parMove)
        {
            if (Math.Abs(parMove[0]) > EPSILON)
            {
                Object.FlippedY = parMove[0] < 0;
            }
        }
    }
}
