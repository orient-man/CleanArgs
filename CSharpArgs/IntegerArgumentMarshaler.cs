using System;
using System.Collections.Generic;

namespace CSharpArgs
{
    public class IntegerArgumentMarshaler : IArgumentMarshaler
    {
        private int intValue;

        public void Set(IEnumerator<string> currentArgument)
        {
            if (!currentArgument.MoveNext())
                throw new ArgsException(ErrorCode.MissingInteger);

            try
            {
                intValue = Int32.Parse(currentArgument.Current);
            }
            catch (FormatException)
            {
                throw new ArgsException(
                    ErrorCode.InvalidInteger,
                    currentArgument.Current);
            }
        }

        public static int GetValue(IArgumentMarshaler am)
        {
            var marshaler = am as IntegerArgumentMarshaler;
            return marshaler != null ? marshaler.intValue : 0;
        }
    }
}