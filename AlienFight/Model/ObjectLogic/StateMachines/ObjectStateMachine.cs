using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace AlienExplorer.Model
{
    /// <summary>
    /// Абстрактный базовый класс автомата состояний объекта уровня.
    /// </summary>
    /// <typeparam name="MachineStatesEnum">Тип состояний объекта.</typeparam>
    public abstract class ObjectStateMachine<MachineStatesEnum>
    {
        /// <summary>
        /// Погрешность вычислений.
        /// </summary>
        protected static readonly float EPSILON = 0.01f;

        /// <summary>
        /// Состояние автомата.
        /// </summary>
        protected MachineStatesEnum _machineState;
        /// <summary>
        /// Словарь: состояние автомата - список состояний объекта.
        /// </summary>
        protected Dictionary<MachineStatesEnum, List<int>> _objectStates;
        /// <summary>
        /// Время в текущем состоянии в секундах.
        /// </summary>
        protected float _timeInState;

        /// <summary>
        /// Инициализирует автомат состояний объекта.
        /// </summary>
        public ObjectStateMachine()
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

        /// <summary>
        /// Изменение состояния автомата и состояния объекта на основе входных данных.
        /// </summary>
        /// <param name="parGameObject">Целевой объект.</param>
        /// <param name="parFreeSpace">Массив свободных расстояний вокруг объекта (слева, сверху, справа, снизу).</param>
        /// <param name="parMove">Вектор перемещения (X, Y).</param>
        /// <param name="parDeltaSeconds">Время, прошедшее с предыдущего шага (в секундах).</param>
        public abstract void ChangeState(GameObject parGameObject, float[ ] parFreeSpace, float[ ] parMove, float parDeltaSeconds);

        /// <summary>
        /// Установка нужного состояния автомата.
        /// </summary>
        /// <param name="parState">Целевое состояние.</param>
        public void SetState(MachineStatesEnum parState)
        {
            _machineState = parState;
        }
    }
}
