using System;
using System.Collections.Generic;

namespace CSharpArgs
{
    public class StringArrayArgumentMarshaler : IArgumentMarshaler
    {
        private readonly List<string> strings = new List<string>();

        public void Set(Iterator<string> currentArgument)
        {
            try
            {
                strings.Add(currentArgument.Next());
            }
            catch (NoSuchElementException)
            {
                throw new ArgsException(ErrorCode.MissingString);
            }
        }

        public static String[] GetValue(IArgumentMarshaler am)
        {
            var marshaler = am as StringArrayArgumentMarshaler;
            return marshaler != null ? marshaler.strings.ToArray() : new String[0];
        }
    }
}