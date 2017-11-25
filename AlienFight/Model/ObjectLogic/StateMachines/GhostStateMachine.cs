using System;

namespace AlienExplorer.Model
{
    /// <summary>
    /// Автомат состояний призрака.
    /// </summary>
    public class GhostStateMachine : ObjectStateMachine<GhostStateType>
    {
        /// <summary>
        /// Период анимации (смены состояний объекта) (в секундах).
        /// </summary>
        private static readonly float SUBSTATE_PERIOD = float.PositiveInfinity; // нет анимации

        /// <summary>
        /// Инициализирует автомат состояний объекта.
        /// </summary>
        public GhostStateMachine()
        {
            _machineState = GhostStateType.Stand;
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
            GhostStateType possibleState = FindPossibleState(parEnemy, parMove);
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
        /// <param name="parMove">Вектор перемещения (X, Y).</param>
        /// <returns>Возможное состояние.</returns>
        private GhostStateType FindPossibleState(GameObject parEnemy, float[ ] parMove)
        {
            if (Math.Abs(parMove[0]) > EPSILON)
            {
                return GhostStateType.Attack;
            }
            else
            {
                return GhostStateType.Stand;
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
        private void ProcessInNewState(GameObject parEnemy, GhostStateType parState)
        {
            _timeInState = 0;
            _machineState = parState;
            parEnemy.State = _objectStates[parState][0];
        }
    }
}
