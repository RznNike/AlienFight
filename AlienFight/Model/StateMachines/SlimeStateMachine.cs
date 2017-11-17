using System;

namespace AlienExplorer.Model
{
    public class SlimeStateMachine : StateMachine<SlimeStateType>
    {
        private static readonly float SUBSTATE_PERIOD = 0.35f;

        public SlimeStateMachine()
        {
            _machineState = SlimeStateType.Stand;
            _timeInState = 0;
        }

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

        private void ProcessInNewState(GameObject parEnemy, SlimeStateType parState)
        {
            _timeInState = 0;
            _machineState = parState;
            parEnemy.State = _objectStates[parState][0];
        }
    }
}
