using System;

namespace AlienExplorer.Model
{
    /// <summary>
    /// Автомат состояний игрока.
    /// </summary>
    public class PlayerStateMachine : ObjectStateMachine<PlayerStateType>
    {
        /// <summary>
        /// Период анимации (смены состояний объекта) (в секундах).
        /// </summary>
        private static readonly float SUBSTATE_PERIOD = 0.05f;
        /// <summary>
        /// Множитель периода состояния повреждения (после получения урона).
        /// </summary>
        private static readonly int HURT_PERIOD_MULT = 10;

        /// <summary>
        /// Инициализирует автомат состояний объекта.
        /// </summary>
        public PlayerStateMachine()
        {
            _machineState = PlayerStateType.Stand;
            _timeInState = 0;
        }

        /// <summary>
        /// Изменение состояния автомата и состояния объекта на основе входных данных.
        /// </summary>
        /// <param name="parPlayer">Целевой объект.</param>
        /// <param name="parFreeSpace">Массив свободных расстояний вокруг объекта (слева, сверху, справа, снизу).</param>
        /// <param name="parMove">Вектор перемещения (X, Y).</param>
        /// <param name="parDeltaSeconds">Время, прошедшее с предыдущего шага (в секундах).</param>
        public override void ChangeState(GameObject parPlayer, float[ ] parFreeSpace, float[ ] parMove, float parDeltaSeconds)
        {
            _timeInState += parDeltaSeconds;
            PlayerStateType possibleState = FindPossibleState((PlayerObject)parPlayer, parFreeSpace, parMove);
            if (_machineState == possibleState)
            {
                ProcessInSameState(parPlayer);
            }
            else
            {
                ProcessInNewState(parPlayer, possibleState);
            }
        }

        /// <summary>
        /// Нахождение возможного следующего состояния автомата.
        /// </summary>
        /// <param name="parPlayer">Целевой объект.</param>
        /// <param name="parFreeSpace">Массив свободных расстояний вокруг объекта (слева, сверху, справа, снизу).</param>
        /// <param name="parMove">Вектор перемещения (X, Y).</param>
        /// <returns>Возможное состояние.</returns>
        private PlayerStateType FindPossibleState(PlayerObject parPlayer, float[ ] parFreeSpace, float[ ] parMove)
        {
            if (parFreeSpace[3] > EPSILON)
            {
                return PlayerStateType.Jump;
            }
            else
            {
                if (parPlayer.SizeY > parPlayer.SizeYsmall)
                {
                    if (Math.Abs(parMove[0]) > EPSILON)
                    {
                        return PlayerStateType.Walk;
                    }
                    else
                    {
                        return PlayerStateType.Stand;
                    }
                }
                else
                {
                    return PlayerStateType.Duck;
                }
            }
        }

        /// <summary>
        /// Изменение состояния объекта (анимация).
        /// </summary>
        /// <param name="parPlayer">Целевой объект.</param>
        private void ProcessInSameState(GameObject parPlayer)
        {
            if (_timeInState >= SUBSTATE_PERIOD)
            {
                int multiplicity = (int)Math.Floor(_timeInState / SUBSTATE_PERIOD);
                _timeInState -= SUBSTATE_PERIOD * multiplicity;

                int oldSubState = parPlayer.State;
                int oldSubStatePosition = _objectStates[_machineState].FindIndex(element => element == oldSubState);
                int newSubStatePosition = 0;
                if ((oldSubStatePosition >= 0) && (oldSubStatePosition < _objectStates[_machineState].Count - 1))
                {
                    newSubStatePosition = oldSubStatePosition + 1;
                }
                int newSubState = _objectStates[_machineState][newSubStatePosition];
                parPlayer.State = newSubState;
            }
        }

        /// <summary>
        /// Изменение состояний автомата и объекта.
        /// </summary>
        /// <param name="parPlayer">Целевой объект.</param>
        /// <param name="parState">Новое состояние автомата.</param>
        private void ProcessInNewState(GameObject parPlayer, PlayerStateType parState)
        {
            if ((_machineState != PlayerStateType.Hurt)
                || (_timeInState >= SUBSTATE_PERIOD * HURT_PERIOD_MULT))
            {
                _timeInState = 0;
                _machineState = parState;
                parPlayer.State = _objectStates[parState][0];
            }
            else
            {
                parPlayer.State = _objectStates[PlayerStateType.Hurt][0];
            }
        }
    }
}
