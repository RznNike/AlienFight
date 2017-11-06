using System;
using System.Collections.Generic;
using System.Reflection;

namespace AlienFight.Model
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CustomAttribute : Attribute
    {
        private string _value;
        
		public CustomAttribute(string parValue)
        {
            _value = parValue;
        }
        
        public static string GetValue(Type parEnumType, string parValue)
        {
            IEnumerable<CustomAttributeData> attributes = parEnumType.GetField(parValue).CustomAttributes;
            IEnumerator<CustomAttributeData> attributesEnumerator = attributes.GetEnumerator();
            attributesEnumerator.MoveNext();
            IList<CustomAttributeTypedArgument> arguments = attributesEnumerator.Current.ConstructorArguments;
            IEnumerator<CustomAttributeTypedArgument> argumentsEnumerator = arguments.GetEnumerator();
            argumentsEnumerator.MoveNext();
            CustomAttributeTypedArgument argument = argumentsEnumerator.Current;
            string path = (string)argument.Value;

            return path;
        }
    }
}
