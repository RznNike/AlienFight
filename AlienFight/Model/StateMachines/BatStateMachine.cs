using System;

namespace AlienExplorer.Model
{
    public class BatStateMachine : StateMachine<BatStateType>
    {
        private static readonly float SUBSTATE_PERIOD = 0.15f;

        public BatStateMachine()
        {
            _machineState = BatStateType.Fly;
            _timeInState = 0;
        }

        public override void ChangeState(GameObject parEnemy, float[ ] parFreeSpace, float[ ] parMove, float parDeltaSeconds)
        {
            _timeInState += parDeltaSeconds;
            ProcessInSameState(parEnemy);
        }

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
