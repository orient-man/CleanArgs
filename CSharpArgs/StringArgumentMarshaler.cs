using System.Collections.Generic;

namespace CSharpArgs
{
    public static class StringArgumentMarshaler
    {
        public static object Marshal(IEnumerator<string> currentArgument)
        {
            if (!currentArgument.MoveNext())
                throw new ArgsException(ErrorCode.MissingString);

            return currentArgument.Current;
        }
    }
}