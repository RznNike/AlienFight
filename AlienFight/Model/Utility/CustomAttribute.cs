using System;
using System.Collections.Generic;
using System.Reflection;

namespace AlienExplorer.Model
{
    /// <summary>
    /// Пользовательский атрибут для полей в перечислениях.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class CustomAttribute : Attribute
    {
        /// <summary>
        /// Значение атрибута.
        /// </summary>
        private object _value;
        
        /// <summary>
        /// Инициализирует атрибут значением.
        /// </summary>
        /// <param name="parValue">Значение для хранения.</param>
        public CustomAttribute(object parValue)
        {
            _value = parValue;
        }
        
        /// <summary>
        /// Получение значения пользовательского атрибута по типу перечисления и строковому имени нужного поля.
        /// </summary>
        /// <param name="parEnumType">Тип перечисления.</param>
        /// <param name="parEnumValue">Имя поля.</param>
        /// <returns>Значение пользовательского атрибута.</returns>
        public static object GetValue(Type parEnumType, string parEnumValue)
        {
            IEnumerable<CustomAttributeData> attributes = parEnumType.GetField(parEnumValue).CustomAttributes;
            IEnumerator<CustomAttributeData> attributesEnumerator = attributes.GetEnumerator();
            attributesEnumerator.MoveNext();
            IList<CustomAttributeTypedArgument> arguments = attributesEnumerator.Current.ConstructorArguments;
            IEnumerator<CustomAttributeTypedArgument> argumentsEnumerator = arguments.GetEnumerator();
            argumentsEnumerator.MoveNext();
            CustomAttributeTypedArgument argument = argumentsEnumerator.Current;

            return argument.Value;
        }
    }
}
