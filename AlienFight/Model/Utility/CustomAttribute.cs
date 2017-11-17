using System;
using System.Collections.Generic;
using System.Reflection;

namespace AlienExplorer.Model
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CustomAttribute : Attribute
    {
        private object _value;
        
		public CustomAttribute(object parValue)
        {
            _value = parValue;
        }
        
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
