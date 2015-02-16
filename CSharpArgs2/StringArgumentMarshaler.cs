using System.Collections.Generic;

namespace CSharpArgs2
{
    public static class StringArgumentMarshaler
    {
        public static dynamic Marshal(IEnumerator<string> currentArgument)
        {
            if (!currentArgument.MoveNext())
                throw new ArgsException(ErrorCode.MissingString);

            return currentArgument.Current;
        }
    }
}