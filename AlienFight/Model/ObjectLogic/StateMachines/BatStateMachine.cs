using System;

namespace AlienExplorer.Model
{
    /// <summary>
    /// Автомат состояний летучей мыши.
    /// </summary>
    public class BatStateMachine : ObjectStateMachine<BatStateType>
    {
        /// <summary>
        /// Период анимации (смены состояний объекта) (в секундах).
        /// </summary>
        private static readonly float SUBSTATE_PERIOD = 0.15f;

        /// <summary>
        /// Инициализирует автомат состояний объекта.
        /// </summary>
        public BatStateMachine()
        {
            _machineState = BatStateType.Fly;
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
            ProcessInSameState(parEnemy);
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
    }
}
