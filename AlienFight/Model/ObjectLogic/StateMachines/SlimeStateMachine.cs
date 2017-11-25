using System;

namespace AlienExplorer.Model
{
    /// <summary>
    /// Автомат состояний слизня.
    /// </summary>
    public class SlimeStateMachine : ObjectStateMachine<SlimeStateType>
    {
        /// <summary>
        /// Период анимации (смены состояний объекта) (в секундах).
        /// </summary>
        private static readonly float SUBSTATE_PERIOD = 0.35f;

        /// <summary>
        /// Инициализирует автомат состояний объекта.
        /// </summary>
        public SlimeStateMachine()
        {
            _machineState = SlimeStateType.Stand;
            _timeInState = 0;
        }

        /// <summary>
        /// Изменение состояния автомата и состояния объекта на основе входных данных.
        /// </summary>
        /// <param name="parEnemy">Целевой объект.</param>
        /// <param name="parFreeSpace">Массив свободных расстояний вокруг объекта (слева, сверху, справа, снизу).</param>
        /// <param name="parMove">Вектор перемещения (X, Y).</param>
        /// <param name="parDeltaSeconds">Время, прошедшее с предыдущего шага (в секундах).</param>
        public override void ChangeState(GameObject parEnemy, float[ ] parFreeSpace, float[ ] parMove, float parDeltaSeconds)
        {
            _timeInState += parDeltaSeconds;
            SlimeStateType possibleState = FindPossibleState(parEnemy, parFreeSpace, parMove);
            if (_machineState == possibleState)
            {
                ProcessInSameState(parEnemy);
            }
            else
            {
                ProcessInNewState(parEnemy, possibleState);
            }
        }

        /// <summary>
        /// Нахождение возможного следующего состояния автомата.
        /// </summary>
        /// <param name="parEnemy">Целевой объект.</param>
        /// <param name="parFreeSpace">Массив свободных расстояний вокруг объекта (слева, сверху, справа, снизу).</param>
        /// <param name="parMove">Вектор перемещения (X, Y).</param>
        /// <returns>Возможное состояние.</returns>
        private SlimeStateType FindPossibleState(GameObject parEnemy, float[ ] parFreeSpace, float[ ] parMove)
        {
            if (parFreeSpace[3] > EPSILON)
            {
                return SlimeStateType.Stand;
            }
            else
            {
                if (Math.Abs(parMove[0]) > EPSILON)
                {
                    return SlimeStateType.Walk;
                }
                else
                {
                    return SlimeStateType.Stand;
                }
            }
        }

        /// <summary>
        /// Изменение состояния объекта (анимация).
        /// </summary>
        /// <param name="parEnemy">Целевой объект.</param>
        private void ProcessInSameState(GameObject parEnemy)
        {
            if (_timeInState > SUBSTATE_PERIOD)
            {
                int multiplicity = (int)Math.Floor(_timeInState / SUBSTATE_PERIOD);
                _timeInState -= SUBSTATE_PERIOD * multiplicity;

                int oldSubState = parEnemy.State;
                int oldSubStatePosition = _objectStates[_machineState].FindIndex(element => element == oldSubState);
                int newSubStatePosition = 0;
                if ((oldSubStatePosition >= 0) && (oldSubStatePosition < _objectStates[_machineState].Count - 1))
                {
                    newSubStatePosition = oldSubStatePosition + 1;
                }
                int newSubState = _objectStates[_machineState][newSubStatePosition];
                parEnemy.State = newSubState;
            }
        }

        /// <summary>
        /// Изменение состояний автомата и объекта.
        /// </summary>
        /// <param name="parEnemy">Целевой объект.</param>
        /// <param name="parState">Новое состояние автомата.</param>
        private void ProcessInNewState(GameObject parEnemy, SlimeStateType parState)
        {
            _timeInState = 0;
            _machineState = parState;
            parEnemy.State = _objectStates[parState][0];
        }
    }
}
