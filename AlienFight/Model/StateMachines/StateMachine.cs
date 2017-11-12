﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace AlienFight.Model
{
    public abstract class StateMachine<MachineStatesEnum>
    {
        protected MachineStatesEnum _machineState;
        protected Dictionary<MachineStatesEnum, List<int>> _objectStates;
        protected float _timeInState;

        public StateMachine()
        {
            _objectStates = new Dictionary<MachineStatesEnum, List<int>>();
            Type enumType = typeof(MachineStatesEnum);
            int min = Enum.GetValues(enumType).Cast<int>().Min();
            int max = Enum.GetValues(enumType).Cast<int>().Max();
            for (int i = min; i <= max; i++)
            {
                string fieldName = enumType.GetFields()[i].Name;
                ReadOnlyCollection<CustomAttributeTypedArgument> values =
                    (ReadOnlyCollection<CustomAttributeTypedArgument>)CustomAttribute.GetValue(enumType, fieldName);
                _objectStates[(MachineStatesEnum)enumType.GetFields()[i].GetValue(null)] = (from element in values
                                                                                            select (int)element.Value).ToList();
            }
        }

        public abstract void ChangeState(GameObject parGameObject, float[ ] parFreeSpace, float[ ] parMove, float parDeltaSeconds);

        public void SetState(MachineStatesEnum parState)
        {
            _machineState = parState;
        }
    }
}
