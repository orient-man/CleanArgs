using System;
using System.Collections.Generic;

namespace CSharpArgs
{
    public static class IntegerArgumentMarshaler
    {
        public static object Marshal(IEnumerator<string> currentArgument)
        {
            if (!currentArgument.MoveNext())
                throw new ArgsException(ErrorCode.MissingInteger);

            try
            {
                return Int32.Parse(currentArgument.Current);
            }
            catch (FormatException)
            {
                throw new ArgsException(
                    ErrorCode.InvalidInteger,
                    currentArgument.Current);
            }
        }
    }
}