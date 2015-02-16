using System;
using System.Collections.Generic;

namespace CSharpArgs2
{
    public static class IntArgumentMarshaler
    {
        public static dynamic Marshal(IEnumerator<string> currentArgument)
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
                    errorParameter: currentArgument.Current);
            }
        }
    }
}