using System;

namespace AlienExplorer.Model
{
    public class GhostStateMachine : ObjectStateMachine<GhostStateType>
    {
        private static readonly float SUBSTATE_PERIOD = float.PositiveInfinity; // нет анимации

        public GhostStateMachine()
        {
            _machineState = GhostStateType.Stand;
            _timeInState = 0;
        }

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

        private void ProcessInNewState(GameObject parEnemy, GhostStateType parState)
        {
            _timeInState = 0;
            _machineState = parState;
            parEnemy.State = _objectStates[parState][0];
        }
    }
}
