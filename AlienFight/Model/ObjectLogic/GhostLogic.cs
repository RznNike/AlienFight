using System;
using System.Linq;
using System.Threading;

namespace AlienFight.Model
{
    public class GhostLogic : BaseObjectLogic<GhostStateMachine, GhostStateType>
    {
        private static readonly float HORISONTAL_SPEED = MAX_SPEED / 5;
        private static readonly int THREAD_SLEEP_MS = 10;
        private static readonly float ATTACK_COOLDOWN_TIME = 1f;

        public EnemyObject Enemy { get { return (EnemyObject)Object; } set { Object = value; } }

        public GhostLogic(GameModel parLevel, EnemyObject parEnemy) : base(parLevel)
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

            DateTime timer = DateTime.UtcNow;
            float attackCooldown = 0;

            while (!_stopThread)
            {
                freeSpace = FindFreeSpace();
                DateTime newTimer = DateTime.UtcNow;
                float deltaSeconds = (float)(newTimer - timer).TotalSeconds;
                timer = newTimer;
                speed = FindSpeed(speed, freeSpace, deltaSeconds, ref attackCooldown);
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
            ref float parAttackCooldown)
        {
            float[ ] speed = new float[2] { 0, 0 };

            if (parAttackCooldown > EPSILON)
            {
                parAttackCooldown -= parDeltaSeconds;
                return speed;
            }

            if ((IsIntersected(Enemy.X, Enemy.X + Enemy.SizeX, Level.Player.X, Level.Player.X + Level.Player.SizeX))
                && (IsIntersected(Enemy.Y, Enemy.Y + Enemy.SizeY, Level.Player.Y, Level.Player.Y + Level.Player.SizeY)))
            {
                parAttackCooldown = ATTACK_COOLDOWN_TIME;
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
