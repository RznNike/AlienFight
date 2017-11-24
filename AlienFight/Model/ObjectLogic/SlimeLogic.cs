using System;
using System.Threading;

namespace AlienExplorer.Model
{
    public class SlimeLogic : BaseObjectLogic<SlimeStateMachine, SlimeStateType>
    {
        private static readonly float HORISONTAL_SPEED = MAX_SPEED / 20;
        private static readonly int THREAD_SLEEP_MS = 25;

        public EnemyObject Enemy { get { return (EnemyObject)Object; } set { Object = value; } }

        public SlimeLogic(GameModel parLevel, EnemyObject parEnemy) : base(parLevel)
        {
            Enemy = parEnemy;
        }

        protected override void IterativeAction()
        {
            // left, up, right, down
            float[ ] freeSpace = new float[4] { 0, 0, 0, 0 };
            // x, y
            float[ ] speed = new float[2] { 0, 0 };
            float[ ] move = new float[2] { 0, 0 };
            float targetX = Enemy.LeftWalkingBoundX;

            _timer = DateTime.UtcNow;
            
            while (!_stopThread)
            {
                _manualResetEventSlim.Wait();
                freeSpace = FindFreeSpace();
                DateTime newTimer = DateTime.UtcNow;
                float deltaSeconds = (float)(newTimer - _timer).TotalSeconds;
                _timer = newTimer;
                speed = FindSpeed(speed, freeSpace, deltaSeconds, ref targetX);
                move = FindMove(speed, freeSpace, deltaSeconds);
                MoveObject(move);
                FlipObject(move);
                _stateMachine.ChangeState(Enemy, freeSpace, move, deltaSeconds);
                Thread.Sleep(THREAD_SLEEP_MS);
            }
        }

        private float[ ] FindSpeed(
            float[ ] parSpeed,
            float[ ] parFreeSpace,
            float parDeltaSeconds,
            ref float refTargetX)
        {
            float[ ] speed = new float[2] { 0, 0 };
            
            // Обработка движений по горизонтали
            if (Math.Abs(Enemy.X - refTargetX) < EPSILON)
            {
                if (Math.Abs(Enemy.LeftWalkingBoundX - refTargetX) < EPSILON)
                {
                    refTargetX = Enemy.RightWalkingBoundX;
                }
                else
                {
                    refTargetX = Enemy.LeftWalkingBoundX;
                }
            }
            else
            {
                if ((parFreeSpace[0] < EPSILON) && (Math.Abs(Enemy.LeftWalkingBoundX - refTargetX) < EPSILON))
                {
                    refTargetX = Enemy.RightWalkingBoundX;
                }
                else if((parFreeSpace[2] < EPSILON) && (Math.Abs(Enemy.RightWalkingBoundX - refTargetX) < EPSILON))
                {
                    refTargetX = Enemy.LeftWalkingBoundX;
                }
            }
            
            if (Enemy.X < refTargetX)
            {
                speed[0] = HORISONTAL_SPEED;
            }
            else
            {
                speed[0] = -HORISONTAL_SPEED;
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
            return speed;
        }
    }
}
