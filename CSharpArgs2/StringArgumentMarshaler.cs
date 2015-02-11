using System.Collections.Generic;

namespace CSharpArgs2
{
    public class StringArgumentMarshaler : IArgumentMarshaler
    {
        public dynamic Marshal(IEnumerator<string> currentArgument)
        {
            if (!currentArgument.MoveNext())
                throw new ArgsException(ErrorCode.MissingString);

            return currentArgument.Current;
        }
    }
}